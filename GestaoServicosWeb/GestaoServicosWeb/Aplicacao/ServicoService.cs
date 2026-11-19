using GestaoServicosWeb.Contratos;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    // Regras de acesso:
    //   - Cliente: cria serviços e só vê / edita / cancela os seus
    //   - Prestador: só vê serviços abertos (PorAprovar e com prazo de propostas por passar)
    //                ou serviços onde já fez um orçamento
    //   - Prestador com orçamento aceite: conclui o serviço
    //   - Empresa: vê, edita, cancela, conclui e apaga tudo
    // Quando alguém tenta mexer no que não é seu, é lançada UnauthorizedAccessException (o endpoint devolve 403).
    public class ServicoService
    {
        private readonly AppDbContext db;

        public ServicoService(AppDbContext db)
        {
            this.db = db;
        }

        // Todos os serviços, com filtros opcionais (para a Empresa)
        public List<Servico> Listar(EstadoServico? estado = null, Guid? idCategoria = null)
        {
            var query = db.Servicos.AsNoTracking();

            if (estado.HasValue)
                query = query.Where(s => s.Estado == estado.Value);

            if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                query = query.Where(s => s.IdCategoria == idCategoria.Value);

            return query.ToList();
        }

        // Serviços abertos a propostas (para os prestadores)
        public List<Servico> ListarAbertos(Guid? idCategoria = null)
        {
            var agora = DateTime.Now;
            var query = db.Servicos.AsNoTracking()
                .Where(s => s.Estado == EstadoServico.PorAprovar && s.PrazoPropostas > agora);

            if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                query = query.Where(s => s.IdCategoria == idCategoria.Value);

            return query.ToList();
        }

        public List<Servico> ListarPorCliente(Guid clienteId, EstadoServico? estado = null, Guid? idCategoria = null)
        {
            var query = db.Servicos.AsNoTracking().Where(s => s.IdCliente == clienteId);

            if (estado.HasValue)
                query = query.Where(s => s.Estado == estado.Value);

            if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                query = query.Where(s => s.IdCategoria == idCategoria.Value);

            return query.ToList();
        }

        // Devolve null se não existir; lança UnauthorizedAccessException se o utilizador não o puder ver
        public Servico? Buscar(Guid id, Guid idUtilizador, Role role)
        {
            var servico = db.Servicos.AsNoTracking().FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return null;

            bool podeVer = role switch
            {
                Role.Empresa => true,
                Role.Cliente => servico.IdCliente == idUtilizador,
                Role.Prestador => EstaAberto(servico)
                                  || db.Orcamentos.Any(o => o.IdServico == id && o.IdPrestador == idUtilizador),
                _ => false
            };

            if (!podeVer)
                throw new UnauthorizedAccessException("Não tem acesso a este serviço.");

            return servico;
        }

        // O cliente vem do token (idCliente), não do body do pedido
        public Servico Criar(Guid idCliente, CriarServicoDto dto)
        {
            // O construtor do Servico valida datas no futuro, strings vazias e Guids válidos
            var servico = new Servico(
                idCliente,
                dto.IdCategoria,
                dto.Titulo,
                dto.Descricao,
                dto.PrazoPropostas,
                dto.DataDesejada
            );

            if (servico.Titulo.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            // As chaves estrangeiras têm de apontar para registos que existam
            if (!db.Utilizadores.Any(u => u.Id == idCliente))
                throw new ArgumentException("O cliente indicado não existe.");

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException("A categoria indicada não existe.");

            db.Servicos.Add(servico);
            db.SaveChanges();
            return servico;
        }

        // Devolve o serviço atualizado, ou null se não existir. Só o cliente dono ou a Empresa.
        // A entidade só deixa editar enquanto o serviço estiver PorAprovar.
        public Servico? Atualizar(Guid id, AtualizarServicoDto dto, Guid idUtilizador, bool eEmpresa)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return null;

            if (!eEmpresa && servico.IdCliente != idUtilizador)
                throw new UnauthorizedAccessException("Só o cliente que criou o serviço o pode editar.");

            if (dto.Titulo.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException("A categoria indicada não existe.");

            servico.Atualizar(dto.IdCategoria, dto.Titulo, dto.Descricao, dto.PrazoPropostas, dto.DataDesejada);
            db.SaveChanges();
            return servico;
        }

        // Devolve false se o serviço não existir. Só o cliente dono ou a Empresa podem cancelar.
        public bool Cancelar(Guid id, Guid idUtilizador, bool eEmpresa)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return false;

            if (!eEmpresa && servico.IdCliente != idUtilizador)
                throw new UnauthorizedAccessException("Só o cliente que criou o serviço o pode cancelar.");

            servico.Cancelar();
            db.SaveChanges();
            return true;
        }

        // EmCurso → Concluido. Só o prestador cujo orçamento foi aceite, ou a Empresa.
        public bool Concluir(Guid id, Guid idUtilizador, bool eEmpresa)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return false;

            bool eOPrestadorAceite = db.Orcamentos.Any(o => o.IdServico == id
                                                           && o.IdPrestador == idUtilizador
                                                           && o.Estado == EstadoOrcamento.Aceite);

            if (!eEmpresa && !eOPrestadorAceite)
                throw new UnauthorizedAccessException("Só o prestador com o orçamento aceite pode concluir o serviço.");

            servico.Concluir();
            db.SaveChanges();
            return true;
        }

        // Só Empresa (verificado no endpoint). Apaga também os orçamentos desse serviço.
        public bool Apagar(Guid id)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return false;

            var orcamentos = db.Orcamentos.Where(o => o.IdServico == id);
            db.Orcamentos.RemoveRange(orcamentos);
            db.Servicos.Remove(servico);
            db.SaveChanges();
            return true;
        }

        private static bool EstaAberto(Servico s) =>
            s.Estado == EstadoServico.PorAprovar && s.PrazoPropostas > DateTime.Now;
    }
}
