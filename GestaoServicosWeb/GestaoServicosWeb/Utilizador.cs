namespace ProjetoAcademia
{
    class Utilizador
    {
        // ===== Campos privados (onde os valores ficam realmente guardados) =====
        private string _nome = "";
        private string _email = "";
        private string _nif = "";
        private string? _telefone;

        // ===== Propriedades =====

        public Guid Id { get; }   // só get: definido no construtor, nunca muda

        public string Nome
        {
            get { return _nome; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O nome é obrigatório.");

                string nome = value.Trim();

                if (nome.Length < 2 || nome.Length > 100)
                    throw new ArgumentException("O nome deve ter entre 2 e 100 caracteres.");

                _nome = nome;
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O email é obrigatório.");

                string email = value.Trim().ToLower();

                int arroba = email.IndexOf('@');
                if (arroba <= 0 || email.LastIndexOf('.') < arroba)
                    throw new ArgumentException("Email inválido.");

                _email = email;
            }
        }

        public string NIF
        {
            get { return _nif; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O NIF é obrigatório.");

                string nif = value.Trim();

                if (nif.Length != 9 || !nif.All(char.IsDigit))
                    throw new ArgumentException("O NIF deve ter 9 dígitos.");

                if (!NifValido(nif))
                    throw new ArgumentException("NIF inválido.");

                _nif = nif;
            }
        }

        public string? Telefone   // opcional
        {
            get { return _telefone; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _telefone = null;
                    return;
                }

                string tel = value.Replace(" ", "");

                if (tel.Length != 9 || !tel.All(char.IsDigit))
                    throw new ArgumentException("O telefone deve ter 9 dígitos.");

                if (tel[0] != '9' && tel[0] != '2' && tel[0] != '3')
                    throw new ArgumentException("O telefone deve começar por 9, 2 ou 3.");

                _telefone = tel;
            }
        }

        public string PasswordHash { get; private set; } = "";

        public Role Role { get; private set; }

        // ===== Construtor =====

        public Utilizador(string nome, string email, string passwordHash,
                          string nif, string? telefone, Role role)
        {
            Id = Guid.NewGuid();

            // Atribuir através das propriedades faz correr as validações
            Nome = nome;
            Email = email;
            NIF = nif;
            Telefone = telefone;
            AlterarPassword(passwordHash);

            if (!Enum.IsDefined(typeof(Role), role))
                throw new ArgumentException("Role inválido.");
            Role = role;
        }

        // ===== Métodos =====

        public void AlterarPassword(string novoHash)
        {
            if (string.IsNullOrWhiteSpace(novoHash))
                throw new ArgumentException("A password é obrigatória.");

            PasswordHash = novoHash;
        }

        // Algoritmo do dígito de controlo do NIF português
        private static bool NifValido(string nif)
        {
            int soma = 0;
            for (int i = 0; i < 8; i++)
                soma += (nif[i] - '0') * (9 - i);

            int resto = soma % 11;
            int controlo = resto < 2 ? 0 : 11 - resto;

            return controlo == (nif[8] - '0');
        }
    }

    enum Role
    {
        Cliente,
        Prestador,
        Empresa // admin
    }
}
