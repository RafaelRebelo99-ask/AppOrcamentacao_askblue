using GestaoServicosWeb.Contratos;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class CategoriaService
    {
        // As categorias iniciais são criadas pela migração (ver AppDbContext.HasData)
        private readonly AppDbContext db;

        public CategoriaService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Categoria> Listar() =>
            db.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToList();

        public Categoria? Buscar(Guid id) =>
            db.Categorias.FirstOrDefault(c => c.Id == id);

        public Categoria Criar(CriarCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("O nome da categoria é obrigatório.");

            string nomeLimpo = dto.Nome.Trim();

            if (nomeLimpo.Length > 100)
                throw new ArgumentException("O nome da categoria não pode ter mais de 100 caracteres.");

            // A collation do SQL Server não distingue maiúsculas de minúsculas
            if (db.Categorias.Any(c => c.Nome == nomeLimpo))
                throw new InvalidOperationException("Já existe uma categoria com esse nome.");

            var novaCategoria = new Categoria(nomeLimpo);
            db.Categorias.Add(novaCategoria);
            db.SaveChanges();
            return novaCategoria;
        }

        public bool Apagar(Guid id)
        {
            var categoria = db.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return false;

            if (db.Oportunidades.Any(s => s.IdCategoria == id))
                throw new InvalidOperationException("Não é possível apagar uma categoria com serviços associados.");

            db.Categorias.Remove(categoria);
            db.SaveChanges();
            return true;
        }
    }
}
