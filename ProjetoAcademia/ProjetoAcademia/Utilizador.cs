namespace ProjetoAcademia
{
    class Utilizador
    {
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Nif { get; set; }
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