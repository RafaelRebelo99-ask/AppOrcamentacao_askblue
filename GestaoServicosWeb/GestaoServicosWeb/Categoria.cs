namespace GestaoServicosWeb
{
    public class Categoria
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;

        public Categoria(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome?.Trim()))
                throw new ArgumentException("Nome de categoria inválido.", nameof(nome));

            Id = Guid.NewGuid();
            Nome = nome.Trim();
        }

        public Categoria(Guid id, string nome)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id de categoria inválido.", nameof(id));

            if (string.IsNullOrWhiteSpace(nome?.Trim()))
                throw new ArgumentException("Nome de categoria inválido.", nameof(nome));

            Id = id;
            Nome = nome.Trim();
        }
    }
}
