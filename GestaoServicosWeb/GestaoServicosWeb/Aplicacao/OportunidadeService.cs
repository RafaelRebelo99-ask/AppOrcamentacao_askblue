
using GestaoServicosWeb.Contratos;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class OportunidadeService
    {
        private readonly AppDbContext db;
        private readonly NotificacaoService notificacoes;

        public OportunidadeService(AppDbContext db, NotificacaoService notificacoes)
        {
            this.db = db;
            this.notificacoes = notificacoes;
        }

        // Listar todas as oportunidades: colaboradores e administradores.
        public List<Oportunidade> Listar(
            Guid idUtilizador,
            EstadoOportunidade? estado = null,
            Guid? idCategoria = null)
        {
            ExigirColaborador(idUtilizador);

            var query = db.Oportunidades.AsNoTracking();

            if (estado.HasValue)
                query = query.Where(o => o.Estado == estado.Value);

            if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                query = query.Where(o => o.IdCategoria == idCategoria.Value);

            return query
                .OrderByDescending(o => o.DataCriacao)
                .ToList();
        }

        // Listar as oportunidades do próprio cliente.
        public List<Oportunidade> ListarPorCliente(
            Guid idCliente,
            EstadoOportunidade? estado = null)
        {
            var cliente = ObterUtilizadorAtivo(idCliente);

            if (cliente.Role != Role.Cliente)
                throw new UnauthorizedAccessException(
                    "Apenas clientes podem consultar as suas oportunidades.");

            var query = db.Oportunidades
                .AsNoTracking()
                .Where(o => o.IdCliente == idCliente);

            if (estado.HasValue)
                query = query.Where(o => o.Estado == estado.Value);

            return query
                .OrderByDescending(o => o.DataCriacao)
                .ToList();
        }

        // Consultar uma oportunidade com controlo de acesso.
        public Oportunidade? Buscar(
            Guid id,
            Guid idUtilizador)
        {
            var utilizador = ObterUtilizadorAtivo(idUtilizador);

            var oportunidade = db.Oportunidades
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == id);

            if (oportunidade == null)
                return null;

            bool podeVer = utilizador.Role switch
            {
                Role.Cliente =>
                    oportunidade.IdCliente == idUtilizador,

                Role.Colaborador or Role.Administrador =>
                    EhColaboradorAskBlue(utilizador),

                _ => false
            };

            if (!podeVer)
                throw new UnauthorizedAccessException(
                    "Não tem acesso a esta oportunidade.");

            return oportunidade;
        }

        // Criar oportunidade.
        // O responsável é sempre o utilizador autenticado.
        public Oportunidade Criar(
            Guid idResponsavel,
            CriarOportunidadeDto dto)
        {
            ExigirColaborador(idResponsavel);

            var cliente = ObterUtilizadorAtivo(dto.IdCliente);

            if (cliente.Role != Role.Cliente)
                throw new ArgumentException(
                    "O utilizador indicado não é um cliente.");

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException(
                    "A categoria indicada não existe.");

            var oportunidade = new Oportunidade(
                dto.IdCliente,
                dto.IdCategoria,
                idResponsavel,
                dto.Titulo,
                dto.Descricao,
                dto.PrazoProposta,
                dto.DataPretendida
            );

            db.Oportunidades.Add(oportunidade);
            notificacoes.OportunidadeAlterada(oportunidade, idResponsavel);
            db.SaveChanges();

            return oportunidade;
        }

        // Atualizar oportunidade.
        // O responsável atual é mantido.
        public Oportunidade? Atualizar(
            Guid id,
            AtualizarOportunidadeDto dto,
            Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var oportunidade = db.Oportunidades
                .FirstOrDefault(o => o.Id == id);

            if (oportunidade == null)
                return null;

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException(
                    "A categoria indicada não existe.");

            // Confirma que o responsável atual continua a ser um
            // colaborador ativo associado à AskBlue.
            ExigirColaborador(oportunidade.IdResponsavel);

            oportunidade.Atualizar(
                dto.IdCategoria,
                oportunidade.IdResponsavel,
                dto.Titulo,
                dto.Descricao,
                dto.PrazoProposta,
                dto.DataPretendida
            );

            db.SaveChanges();

            return oportunidade;
        }

        // Cancelar oportunidade.
        public bool Cancelar(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var oportunidade = db.Oportunidades
                .FirstOrDefault(o => o.Id == id);

            if (oportunidade == null)
                return false;

            oportunidade.Cancelar();
            notificacoes.OportunidadeAlterada(oportunidade, idUtilizador);
            db.SaveChanges();

            return true;
        }

        // Iniciar análise.
        public bool IniciarAnalise(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var oportunidade = ObterOportunidade(id);

            if (oportunidade == null)
                return false;

            oportunidade.IniciarAnalise();
            notificacoes.OportunidadeAlterada(oportunidade, idUtilizador);
            db.SaveChanges();

            return true;
        }

        // Iniciar preparação da proposta.
        public bool IniciarPreparacao(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var oportunidade = ObterOportunidade(id);

            if (oportunidade == null)
                return false;

            oportunidade.IniciarPreparacao();
            notificacoes.OportunidadeAlterada(oportunidade, idUtilizador);
            db.SaveChanges();

            return true;
        }

        // Marcar oportunidade como perdida.
        public bool MarcarComoPerdida(Guid id, Guid idUtilizador)
        {
            ExigirColaborador(idUtilizador);

            var oportunidade = ObterOportunidade(id);

            if (oportunidade == null)
                return false;

            oportunidade.MarcarComoPerdida();
            notificacoes.OportunidadeAlterada(oportunidade, idUtilizador);
            db.SaveChanges();

            return true;
        }

        private Oportunidade? ObterOportunidade(Guid id)
        {
            return db.Oportunidades
                .FirstOrDefault(o => o.Id == id);
        }

        private Utilizador ObterUtilizadorAtivo(Guid id)
        {
            var utilizador = db.Utilizadores
                .FirstOrDefault(u => u.Id == id && u.Ativo);

            if (utilizador == null)
                throw new UnauthorizedAccessException(
                    "Utilizador inexistente ou inativo.");

            return utilizador;
        }

        private void ExigirColaborador(Guid idUtilizador)
        {
            var utilizador = ObterUtilizadorAtivo(idUtilizador);

            if (utilizador.Role is not (
                Role.Colaborador or Role.Administrador))
            {
                throw new UnauthorizedAccessException(
                    "Apenas colaboradores da AskBlue podem realizar esta operação.");
            }

            if (!EhColaboradorAskBlue(utilizador))
                throw new UnauthorizedAccessException(
                    "O utilizador não está associado a uma empresa AskBlue ativa.");
        }

        private bool EhColaboradorAskBlue(Utilizador utilizador)
        {
            return utilizador.EmpresaId.HasValue &&
                db.Empresas.Any(e =>
                    e.Id == utilizador.EmpresaId.Value &&
                    e.Ativa);
        }
    }
}
