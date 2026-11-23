
namespace GestaoServicosWeb
{
    /*
     removi o NIF e telefone. Como o negócio vai funcionar a partir da empresa, ela é que vai ter os dados fiscais.
    Esta classe fica só como um uitlizador do sistema (trabalhador).
     */
    public class Utilizador
    {
        private string _nome = "";
        private string _email = "";

        // qual empresa a que o utilzador pertence (neste caso vai ser a askblue)
        public Guid? EmpresaId { get; private set; }

        public Empresa? Empresa { get; private set; }

        public Guid Id { get; private set; }

        public string Nome
        {
            get => _nome;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O nome é obrigatório.");

                string nome = value.Trim();

                if (nome.Length < 2 || nome.Length > 100)
                    throw new ArgumentException(
                        "O nome deve ter entre 2 e 100 caracteres.");

                _nome = nome;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O email é obrigatório.");

                string email = value.Trim().ToLowerInvariant();

                int arroba = email.IndexOf('@');

                if (arroba <= 0 || email.LastIndexOf('.') <= arroba)
                    throw new ArgumentException("Email inválido.");

                _email = email;
            }
        }

        public string PasswordHash { get; private set; } = "";

        public Role Role { get; private set; }

        public bool Ativo { get; private set; } = true;

        private Utilizador() { }

        public Utilizador(
    string nome,
    string email,
    string passwordHash,
    Role role,
    Empresa? empresa = null)
        {
            Id = Guid.NewGuid();

            Nome = nome;
            Email = email;
            AlterarPassword(passwordHash);

            if (!Enum.IsDefined(typeof(Role), role))
                throw new ArgumentException("Role inválido.");

            if (role == Role.Cliente && empresa != null)
                throw new ArgumentException(
                    "Um cliente externo não pode pertencer à empresa.");

            if (role != Role.Cliente && empresa == null)
                throw new ArgumentException(
                    "Um colaborador deve estar associado à empresa.");

            Role = role;
            Empresa = empresa;
            EmpresaId = empresa?.Id;
            Ativo = true;
        }

        public void AlterarPassword(string novoHash)
        {
            if (string.IsNullOrWhiteSpace(novoHash))
                throw new ArgumentException("A password é obrigatória.");

            PasswordHash = novoHash;
        }

        public void AlterarNome(string nome)
        {
            Nome = nome;
        }

        public void AlterarEmail(string email)
        {
            Email = email;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }
    }

    public enum Role
    {
        Colaborador, // trabalha na empresa
        Administrador, //trabalha na empresa e é admin
        Cliente // externo
    }
}
