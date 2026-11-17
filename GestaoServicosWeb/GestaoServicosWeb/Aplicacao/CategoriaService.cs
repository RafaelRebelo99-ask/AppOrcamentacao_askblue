using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.Aplicacao
{
    public class CategoriaService
    {
        // Armazenamento em memória com categorias iniciais para testes imediatos
        private readonly List<Categoria> categorias = new()
        {
            new Categoria(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Pintura e Remodelação"),
            new Categoria(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Eletricidade"),
            new Categoria(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Canalização"),
            new Categoria(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Carpintaria"),
            new Categoria(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Limpeza e Manutenção")
        };

        private readonly object bloqueio = new();

        public List<Categoria> Listar()
        {
            lock (bloqueio)
            {
                return categorias.ToList();
            }
        }

        public Categoria? Buscar(Guid id)
        {
            lock (bloqueio)
            {
                return categorias.FirstOrDefault(c => c.Id == id);
            }
        }

        public Categoria Criar(CriarCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("O nome da categoria é obrigatório.");

            string nomeLimpo = dto.Nome.Trim();

            lock (bloqueio)
            {
                if (categorias.Any(c => c.Nome.Equals(nomeLimpo, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Já existe uma categoria com esse nome.");

                var novaCategoria = new Categoria(nomeLimpo);
                categorias.Add(novaCategoria);
                return novaCategoria;
            }
        }

        public bool Apagar(Guid id)
        {
            lock (bloqueio)
            {
                return categorias.RemoveAll(c => c.Id == id) > 0;
            }
        }
    }
}
