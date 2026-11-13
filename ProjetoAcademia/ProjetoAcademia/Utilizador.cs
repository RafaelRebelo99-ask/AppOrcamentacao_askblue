namespace ProjetoAcademia
{
    class Utilizador
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string NIF { get; set; }
        public string Telefone { get; set; }
        public Role Role { get; set; }

    }

    enum Role
    {
        Cliente,
        Prestador,
        Empresa // admin
    }
}

