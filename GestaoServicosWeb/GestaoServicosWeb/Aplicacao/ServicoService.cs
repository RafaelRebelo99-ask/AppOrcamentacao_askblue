using GestaoServicosWeb.Contratos;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class ServicoService
    {
        private readonly AppDbContext db;

        public ServicoService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Servico> Listar(EstadoServico? estado = null, Guid? idCategoria = null)
        {
            var query = db.Servicos.AsNoTracking();

            if (estado.HasValue)
                query = query.Where(s => s.Estado == estado.Value);

            if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                query = query.Where(s => s.IdCategoria == idCategoria.Value);

            return query.ToList();
        }

        public Servico? Buscar(Guid id) =>
            db.Servicos.FirstOrDefault(s => s.Id == id);

        public List<Servico> ListarPorCliente(Guid clienteId) =>
            db.Servicos.AsNoTracking().Where(s => s.IdCliente == clienteId).ToList();

        public Servico Criar(CriarServicoDto dto)
        {
            // O construtor do Servico valida datas no futuro, strings vazias e Guids válidos
            var servico = new Servico(
                dto.IdCliente,
                dto.IdCategoria,
                dto.Titulo,
                dto.Descricao,
                dto.PrazoPropostas,
                dto.DataDesejada
            );

            if (servico.Titulo.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            // As chaves estrangeiras têm de apontar para registos que existam
            if (!db.Utilizadores.Any(u => u.Id == dto.IdCliente))
                throw new ArgumentException("O cliente indicado não existe.");

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException("A categoria indicada não existe.");

            db.Servicos.Add(servico);
            db.SaveChanges();
            return servico;
        }

        // Devolve o serviço atualizado, ou null se não existir
        public Servico? Atualizar(Guid id, AtualizarServicoDto dto)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return null;

            if (dto.Titulo.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            if (!db.Categorias.Any(c => c.Id == dto.IdCategoria))
                throw new ArgumentException("A categoria indicada não existe.");

            servico.Atualizar(dto.IdCategoria, dto.Titulo, dto.Descricao, dto.PrazoPropostas, dto.DataDesejada);
            db.SaveChanges();
            return servico;
        }

        // Devolve false se o serviço não existir
        public bool Cancelar(Guid id)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return false;

            servico.Cancelar();
            db.SaveChanges();
            return true;
        }

        // Devolve false se o serviço não existir
        public bool Concluir(Guid id)
        {
            var servico = db.Servicos.FirstOrDefault(s => s.Id == id);
            if (servico == null)
                return false;

            servico.Concluir();
            db.SaveChanges();
            return true;
        }

        // Lista serviços abertos a propostas (PorAprovar), com filtro opcional por categoria
        public List<Servico> ListarDisponiveis(Guid? idCategoria = null) =>
            Listar(EstadoServico.PorAprovar, idCategoria);
    }
}
