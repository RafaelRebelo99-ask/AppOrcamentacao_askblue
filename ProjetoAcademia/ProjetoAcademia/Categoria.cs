namespace ProjetoAcademia
{
    class Categoria
    {
        private Guid Id { get; set; }
        private string Nome { get; set; }

        public Categoria(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome.Trim()))
                throw new ArgumentException(nameof(nome), "Nome de categoria inválido.");

            this.Nome = nome;
        }
    }

}
