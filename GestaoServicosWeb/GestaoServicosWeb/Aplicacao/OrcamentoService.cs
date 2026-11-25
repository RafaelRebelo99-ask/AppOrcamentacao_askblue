
using GestaoServicosWeb.Dados;
using GestaoServicosWeb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace GestaoServicosWeb.Aplicacao
{
    public class NaoEncontradoException : Exception
    {
        public NaoEncontradoException(string mensagem) : base(mensagem) { }
    }

    public class OrcamentoService
    {
        private readonly AppDbContext db;
        private readonly NotificacaoService notificacoes;

        public OrcamentoService(AppDbContext db, NotificacaoService notificacoes)
        {
            this.db = db;
            this.notificacoes = notificacoes;
        }

        // leitura
        public Orcamento[] ListarDoCriador(Guid idCriador)
        {
            ExigirColaborador(idCriador);

            return db.Orcamentos
                .AsNoTracking()
                .Where(o => o.IdCriador == idCriador)
                .OrderByDescending(o => o.DataCriacao)
                .ToArray();
        }

        // Todas as propostas (administrador)
        public Orcamento[] ListarTodos(Guid idUtilizador)
        {
            ExigirAdministrador(idUtilizador);

            return db.Orcamentos
                .AsNoTracking()
                .OrderByDescending(o => o.DataCriacao)
                .ToArray();
        }

        // Todas as propostas enviadas ao cliente, de todas as suas oportunidades
        public Orcamento[] ListarRecebidos(Guid idCliente)
        {
            var utilizador = ObterUtilizador(idCliente);

            if (utilizador.Role != Role.Cliente)
                throw new UnauthorizedAccessException(
                    "Apenas clientes recebem propostas.");

            return db.Orcamentos
                .AsNoTracking()
                .Where(o =>
                    o.Oportunidade!.IdCliente == idCliente &&
                    o.Estado != EstadoOrcamento.Rascunho)
                .OrderByDescending(o => o.DataEnvio)
                .ToArray();
        }

        // Propostas de uma oportunidade (cliente)
        public Orcamento[] ListarDaOportunidade(
            Guid idOportunidade,
            Guid idCliente)
        {
            var oportunidade = db.Oportunidades
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == idOportunidade);

            if (oportunidade is null ||
                oportunidade.IdCliente != idCliente)
            {
                throw new NaoEncontradoException(
                    "Oportunidade não encontrada.");
            }

            return db.Orcamentos
                .AsNoTracking()
                .Where(o =>
                    o.IdOportunidade == idOportunidade &&
                    o.Estado != EstadoOrcamento.Rascunho)
                .OrderBy(o => o.ValorSemIVA)
                .ToArray();
        }

        // Colaborador/admin vê propostas internas.
        // Cliente só vê propostas enviadas das suas oportunidades.
        public Orcamento Obter(Guid id, Guid idUtilizador)
        {
            var orcamento = db.Orcamentos
                .AsNoTracking()
                .Include(o => o.Oportunidade)
                .FirstOrDefault(o => o.Id == id);

            if (orcamento is null)
                throw new NaoEncontradoException(
                    "Orçamento não encontrado.");

            var utilizador = ObterUtilizador(idUtilizador);

            bool eColaborador =
                utilizador.Role is Role.Colaborador or Role.Administrador
                && utilizador.EmpresaId.HasValue;

            if (eColaborador)
                return orcamento;

            bool eCliente =
                utilizador.Role == Role.Cliente &&
                orcamento.IdOportunidade != Guid.Empty &&
                orcamento.Oportunidade?.IdCliente == idUtilizador &&
                orcamento.Estado != EstadoOrcamento.Rascunho;

            if (eCliente)
                return orcamento;

            throw new NaoEncontradoException(
                "Orçamento não encontrado.");
        }

        // PDF da proposta. Pode descarregar quem a pode ver (regras do Obter).
        public (string nome, byte[] conteudo) GerarPdf(Guid id, Guid idUtilizador)
        {
            var orcamento = Obter(id, idUtilizador);

            var oportunidade = db.Oportunidades
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Include(o => o.Categoria)
                .First(o => o.Id == orcamento.IdOportunidade);

            var empresa = db.Utilizadores
                .AsNoTracking()
                .Where(u => u.Id == orcamento.IdCriador)
                .Select(u => u.Empresa)
                .FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "O criador da proposta não está associado a uma empresa.");

            var numero = orcamento.Id.ToString("N")[..8].ToUpperInvariant();

            var pdf = PropostaPdf.Gerar(new DadosPropostaPdf(
                Numero: numero,
                Estado: orcamento.Estado.ToString(),
                Rascunho: orcamento.Estado == EstadoOrcamento.Rascunho,
                Expirada: orcamento.Expirado,
                EmpresaNome: empresa.Nome,
                EmpresaNif: empresa.NIF,
                EmpresaEmail: empresa.Email,
                EmpresaTelefone: empresa.Telefone,
                ClienteNome: oportunidade.Cliente!.Nome,
                ClienteEmail: oportunidade.Cliente.Email,
                Oportunidade: oportunidade.Titulo,
                Categoria: oportunidade.Categoria!.Nome,
                Descricao: orcamento.Descricao,
                ValorSemIVA: orcamento.ValorSemIVA,
                TaxaIVA: orcamento.TaxaIVA,
                ValorIVA: orcamento.ValorIVA,
                ValorTotal: orcamento.ValorTotal,
                Data: orcamento.DataEnvio ?? orcamento.DataCriacao,
                DataInicio: orcamento.DataInicio,
                DataFim: orcamento.DataFim,
                DuracaoDias: orcamento.DuracaoDias,
                DataValidade: orcamento.DataValidade));

            return ($"Proposta-{numero}.pdf", pdf);
        }

        // criação e edição

        public Orcamento Criar(
            Guid idCriador,
            Guid idOportunidade,
            string descricao,
            decimal valorSemIVA,
            decimal taxaIVA,
            DateTime dataValidade,
            DateTime dataInicio,
            int duracaoDias)
        {
            ExigirColaborador(idCriador);

            var oportunidade = ObterOportunidade(idOportunidade);

            ExigirOportunidadeAberta(oportunidade);

            var orcamento = new Orcamento(
                idOportunidade,
                idCriador,
                descricao,
                valorSemIVA,
                taxaIVA,
                dataValidade,
                dataInicio,
                duracaoDias);

            db.Orcamentos.Add(orcamento);
            db.SaveChanges();

            return orcamento;
        }

        public Orcamento Editar(
            Guid id,
            Guid idUtilizador,
            string descricao,
            decimal valorSemIVA,
            decimal taxaIVA,
            DateTime dataValidade,
            DateTime dataInicio,
            int duracaoDias)
        {
            ExigirColaborador(idUtilizador);

            var orcamento = ObterDoCriadorOuAdministrador(
                id,
                idUtilizador);

            var oportunidade = ObterOportunidade(
                orcamento.IdOportunidade);

            ExigirOportunidadeAberta(oportunidade);

            orcamento.Atualizar(
                descricao,
                valorSemIVA,
                taxaIVA,
                dataValidade,
                dataInicio,
                duracaoDias);

            db.SaveChanges();

            return orcamento;
        }

        // envio

        public Orcamento Enviar(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var orcamento = ObterDoCriadorOuAdministrador(
                id,
                idUtilizador);

            var oportunidade = ObterOportunidade(
                orcamento.IdOportunidade);

            ExigirOportunidadeAberta(oportunidade);

            orcamento.Enviar();

            // Atualiza o estado comercial da oportunidade.
            oportunidade.RegistarPropostaEnviada();

            notificacoes.PropostaEnviada(orcamento, oportunidade, idUtilizador);

            db.SaveChanges();

            return orcamento;
        }

        // eliminar

        // Apenas rascunhos podem ser eliminados.
        // O administrador pode eliminar rascunhos de qualquer colaborador.
        public void Eliminar(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var orcamento = ObterDoCriadorOuAdministrador(
                id,
                idUtilizador);

            if (orcamento.Estado != EstadoOrcamento.Rascunho)
                throw new InvalidOperationException(
                    "Só é possível eliminar propostas em rascunho.");

            db.Orcamentos.Remove(orcamento);
            db.SaveChanges();
        }

        // decis~eos do cliente

        public Orcamento Aceitar(Guid id, Guid idCliente)
        {
            // Serializable ajuda a impedir duas aceitações simultâneas.
            using var transacao = db.Database.BeginTransaction(
                IsolationLevel.Serializable);

            var (orcamento, oportunidade) =
                ObterDoCliente(id, idCliente);

            orcamento.Aceitar();

            // Todas as outras propostas enviadas são recusadas.
            // Rascunhos ficam intactos.
            var outrasPropostas = db.Orcamentos
                .Where(o =>
                    o.IdOportunidade == oportunidade.Id &&
                    o.Id != orcamento.Id &&
                    o.Estado == EstadoOrcamento.Enviado)
                .ToList();

            foreach (var outra in outrasPropostas)
            {
                outra.RecusarPorAceitacaoDeOutraProposta();
            }

            oportunidade.MarcarComoGanha();

            notificacoes.PropostaAceite(orcamento, oportunidade, outrasPropostas, idCliente);

            db.SaveChanges();
            transacao.Commit();

            return orcamento;
        }

        public Orcamento Recusar(Guid id, Guid idCliente)
        {
            var (orcamento, oportunidade) = ObterDoCliente(id, idCliente);

            orcamento.Recusar();

            notificacoes.PropostaRecusada(orcamento, oportunidade, idCliente);

            db.SaveChanges();

            return orcamento;
        }

        // aux
        private Utilizador ObterUtilizador(Guid id)
        {
            return db.Utilizadores
                .FirstOrDefault(u => u.Id == id && u.Ativo)
                ?? throw new NaoEncontradoException(
                    "Utilizador não encontrado.");
        }

        private void ExigirColaborador(Guid id)
        {
            var utilizador = ObterUtilizador(id);

            if (utilizador.Role is not (
                Role.Colaborador or Role.Administrador) ||
                !utilizador.EmpresaId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Apenas colaboradores da AskBlue podem realizar esta operação.");
            }
        }

        private void ExigirAdministrador(Guid id)
        {
            var utilizador = ObterUtilizador(id);

            if (utilizador.Role != Role.Administrador ||
                !utilizador.EmpresaId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Apenas administradores podem realizar esta operação.");
            }
        }

        private Orcamento ObterDoCriadorOuAdministrador(
            Guid id,
            Guid idUtilizador)
        {
            var orcamento = db.Orcamentos
                .FirstOrDefault(o => o.Id == id);

            if (orcamento is null)
                throw new NaoEncontradoException(
                    "Orçamento não encontrado.");

            var utilizador = ObterUtilizador(idUtilizador);

            bool eAdministrador =
                utilizador.Role == Role.Administrador &&
                utilizador.EmpresaId.HasValue;

            if (!eAdministrador && orcamento.IdCriador != idUtilizador)
            {
                throw new NaoEncontradoException(
                    "Orçamento não encontrado.");
            }

            return orcamento;
        }

        private (Orcamento orcamento, Oportunidade oportunidade)
            ObterDoCliente(Guid id, Guid idCliente)
        {
            var orcamento = db.Orcamentos
                .FirstOrDefault(o => o.Id == id);

            if (orcamento is null ||
                orcamento.Estado == EstadoOrcamento.Rascunho)
            {
                throw new NaoEncontradoException(
                    "Orçamento não encontrado.");
            }

            var utilizador = ObterUtilizador(idCliente);

            if (utilizador.Role != Role.Cliente)
                throw new UnauthorizedAccessException(
                    "Apenas clientes podem decidir sobre propostas.");

            var oportunidade = ObterOportunidade(
                orcamento.IdOportunidade);

            if (oportunidade.IdCliente != idCliente)
                throw new NaoEncontradoException(
                    "Orçamento não encontrado.");

            return (orcamento, oportunidade);
        }

        private Oportunidade ObterOportunidade(Guid id)
        {
            return db.Oportunidades
                .FirstOrDefault(o => o.Id == id)
                ?? throw new NaoEncontradoException(
                    "Oportunidade não encontrada.");
        }

        private static void ExigirOportunidadeAberta(
            Oportunidade oportunidade)
        {
            if (oportunidade.Estado is
                EstadoOportunidade.Ganha or
                EstadoOportunidade.Perdida or
                EstadoOportunidade.Cancelada)
            {
                throw new InvalidOperationException(
                    "A oportunidade já não está disponível para propostas.");
            }

            if (oportunidade.PrazoProposta.HasValue &&
                oportunidade.PrazoProposta.Value.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    "O prazo para apresentação de propostas terminou.");
            }
        }
    }
}
