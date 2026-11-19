using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    // 404: não existe, ou existe mas não é do utilizador (não revelamos qual dos casos)
    public class NaoEncontradoException : Exception
    {
        public NaoEncontradoException(string mensagem) : base(mensagem) { }
    }

    /*
     Convenção de exceções (traduzidas para HTTP nos endpoints):
       NaoEncontradoException       -> 404
       UnauthorizedAccessException  -> 403 (papel errado)
       InvalidOperationException    -> 409 (transição de estado inválida, conflito)
       ArgumentException            -> 400 (dados inválidos)
    */
    public class OrcamentoService
    {
        private readonly AppDbContext db;

        public OrcamentoService(AppDbContext db)
        {
            this.db = db;
        }

        // ------------------------------------------------------------------
        // Leitura
        // ------------------------------------------------------------------

        // Ecrã "Os meus orçamentos" (prestador)
        public Orcamento[] ListarDoPrestador(Guid idPrestador) =>
            db.Orcamentos.AsNoTracking()
                .Include(o => o.Servico)
                .Where(o => o.IdPrestador == idPrestador)
                .OrderBy(o => o.DataValidade)
                .ToArray();

        // Ecrã de comparação (cliente): nunca inclui rascunhos
        public Orcamento[] ListarDoServico(Guid idServico, Guid idCliente)
        {
            var servico = db.Servicos.AsNoTracking().FirstOrDefault(s => s.Id == idServico);
            if (servico is null || servico.IdCliente != idCliente)
                throw new NaoEncontradoException("Serviço não encontrado.");

            return db.Orcamentos.AsNoTracking()
                .Where(o => o.IdServico == idServico && o.Estado != EstadoOrcamento.Rascunho)
                .OrderBy(o => o.ValorSemIVA * (1 + o.TaxaIVA / 100m))
                .ToArray();
        }

        // Visível ao prestador dono e, se não for rascunho, ao cliente dono do serviço
        public Orcamento Obter(Guid id, Guid idUtilizador)
        {
            var o = db.Orcamentos.AsNoTracking().Include(x => x.Servico).FirstOrDefault(x => x.Id == id);

            var visivel = o is not null &&
                (o.IdPrestador == idUtilizador ||
                 (o.Servico!.IdCliente == idUtilizador && o.Estado != EstadoOrcamento.Rascunho));

            return visivel ? o! : throw new NaoEncontradoException("Orçamento não encontrado.");
        }

        // ------------------------------------------------------------------
        // Prestador
        // ------------------------------------------------------------------

        public Orcamento Criar(Guid idPrestador, Guid idServico, string descricao, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, DateTime dataInicio, int duracaoDias)
        {
            var prestador = db.Utilizadores.AsNoTracking().FirstOrDefault(u => u.Id == idPrestador)
                ?? throw new NaoEncontradoException("O prestador indicado não existe.");

            if (prestador.Role != Role.Prestador)
                throw new UnauthorizedAccessException("Apenas prestadores podem enviar orçamentos.");

            var servico = db.Servicos.AsNoTracking().FirstOrDefault(s => s.Id == idServico)
                ?? throw new NaoEncontradoException("O serviço indicado não existe.");

            ExigirServicoAberto(servico);

            var jaTemAtivo = db.Orcamentos.Any(o =>
                o.IdServico == idServico && o.IdPrestador == idPrestador &&
                (o.Estado == EstadoOrcamento.Rascunho ||
                 o.Estado == EstadoOrcamento.Enviado ||
                 o.Estado == EstadoOrcamento.Aceite));
            if (jaTemAtivo)
                throw new InvalidOperationException("Já tens um orçamento ativo para este serviço.");

            var orcamento = new Orcamento(idPrestador, idServico, descricao, valorSemIVA, taxaIVA, dataValidade, dataInicio, duracaoDias);

            db.Orcamentos.Add(orcamento);
            db.SaveChanges();
            return orcamento;
        }

        public Orcamento Editar(Guid id, Guid idPrestador, string descricao, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, DateTime dataInicio, int duracaoDias)
        {
            var orcamento = ObterDoPrestador(id, idPrestador);

            orcamento.Atualizar(descricao, valorSemIVA, taxaIVA, dataValidade, dataInicio, duracaoDias); // só rascunhos
            db.SaveChanges();
            return orcamento;
        }

        // Rascunho -> Enviado (fica congelado)
        public Orcamento Enviar(Guid id, Guid idPrestador)
        {
            var orcamento = ObterDoPrestador(id, idPrestador);

            var servico = db.Servicos.AsNoTracking().First(s => s.Id == orcamento.IdServico);
            ExigirServicoAberto(servico);

            orcamento.Enviar(); // valida estado Rascunho e validade futura
            db.SaveChanges();
            return orcamento;
        }

        // Só rascunhos
        public void Eliminar(Guid id, Guid idPrestador)
        {
            var orcamento = ObterDoPrestador(id, idPrestador);

            if (orcamento.Estado != EstadoOrcamento.Rascunho)
                throw new InvalidOperationException("Só é possível eliminar rascunhos.");

            db.Orcamentos.Remove(orcamento);
            db.SaveChanges();
        }

        // ------------------------------------------------------------------
        // Cliente
        // ------------------------------------------------------------------

        // Tudo ou nada: aceitar um orçamento recusa os restantes e coloca o serviço em curso
        public Orcamento Aceitar(Guid id, Guid idCliente)
        {
            using var transacao = db.Database.BeginTransaction();

            var (orcamento, servico) = ObterDoCliente(id, idCliente);

            if (servico.Estado != EstadoServico.PorAprovar)
                throw new InvalidOperationException("O serviço já não está aberto.");

            orcamento.Aceitar(); // exige Enviado e não expirado

            var outros = db.Orcamentos
                .Where(o => o.IdServico == servico.Id &&
                            o.Id != orcamento.Id &&
                            o.Estado == EstadoOrcamento.Enviado)
                .ToList();
            foreach (var outro in outros)
                outro.Recusar();

            servico.Aprovar(); // PorAprovar -> EmCurso

            db.SaveChanges();   // um aceite concorrente falha aqui (RowVersion / índice único)
            transacao.Commit();
            return orcamento;
        }

        public Orcamento Recusar(Guid id, Guid idCliente)
        {
            var (orcamento, _) = ObterDoCliente(id, idCliente);

            orcamento.Recusar(); // exige Enviado
            db.SaveChanges();
            return orcamento;
        }

        // ------------------------------------------------------------------
        // Auxiliares
        // ------------------------------------------------------------------

        private Orcamento ObterDoPrestador(Guid id, Guid idPrestador) =>
            db.Orcamentos.FirstOrDefault(o => o.Id == id && o.IdPrestador == idPrestador)
            ?? throw new NaoEncontradoException("Orçamento não encontrado.");

        // O cliente só vê (e decide sobre) orçamentos não-rascunho dos seus serviços
        private (Orcamento orcamento, Servico servico) ObterDoCliente(Guid id, Guid idCliente)
        {
            var orcamento = db.Orcamentos.FirstOrDefault(o => o.Id == id);
            var servico = orcamento is null
                ? null
                : db.Servicos.FirstOrDefault(s => s.Id == orcamento.IdServico);

            if (orcamento is null || servico is null ||
                servico.IdCliente != idCliente ||
                orcamento.Estado == EstadoOrcamento.Rascunho)
                throw new NaoEncontradoException("Orçamento não encontrado.");

            return (orcamento, servico);
        }

        private static void ExigirServicoAberto(Servico servico)
        {
            if (servico.Estado != EstadoServico.PorAprovar)
                throw new InvalidOperationException("O serviço já não aceita orçamentos.");

            if (servico.PrazoPropostas.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("O prazo para propostas terminou.");
        }
    }
}