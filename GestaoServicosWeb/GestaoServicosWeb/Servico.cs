namespace ProjetoAcademia
{
    public enum EstadoServico
    {
        PorAprovar,
        EmCurso,
        Cancelado,
        Concluido
    }

    public class Servico
    {
        public Guid Id { get; private set; }

        // FK
        public Guid IdCliente { get; private set; }
        internal Utilizador? Cliente { get; private set; }

        // FK
        public Guid IdCategoria { get; private set; }
        internal Categoria? Categoria { get; private set; }

        public string Titulo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public EstadoServico Estado { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime PrazoPropostas { get; private set; }
        public DateTime DataDesejada { get; private set; }

        public Servico() { }

        // Construtor principal para criação por IDs (usado nas APIs/DTOs)
        public Servico(Guid idCliente, Guid idCategoria, string titulo, string descricao, DateTime prazoPropostas, DateTime dataDesejada)
        {
            if (idCliente == Guid.Empty)
                throw new ArgumentException("O cliente é obrigatório.", nameof(idCliente));

            if (idCategoria == Guid.Empty)
                throw new ArgumentException("A categoria é obrigatória.", nameof(idCategoria));

            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título não pode estar vazio.", nameof(titulo));

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição não pode estar vazia.", nameof(descricao));

            if (prazoPropostas <= DateTime.Now)
                throw new ArgumentException("O prazo de propostas deve ser uma data futura.", nameof(prazoPropostas));

            if (dataDesejada < prazoPropostas)
                throw new ArgumentException("A data desejada para o serviço deve ser posterior ao prazo de propostas.", nameof(dataDesejada));

            Id = Guid.NewGuid();
            IdCliente = idCliente;
            IdCategoria = idCategoria;
            Titulo = titulo.Trim();
            Descricao = descricao.Trim();
            PrazoPropostas = prazoPropostas;
            DataDesejada = dataDesejada;
            CriadoEm = DateTime.Now;
            Estado = EstadoServico.PorAprovar;
        }

        // Construtor completo com entidades
        internal Servico(Utilizador cliente, Categoria categoria, string titulo, string descricao, DateTime prazoPropostas, DateTime dataDesejada)
            : this(
                cliente?.Id ?? throw new ArgumentNullException(nameof(cliente), "O cliente é obrigatório."),
                categoria?.Id ?? throw new ArgumentNullException(nameof(categoria), "A categoria é obrigatória."),
                titulo, descricao, prazoPropostas, dataDesejada)
        {
            if (cliente.Role != Role.Cliente)
                throw new ArgumentException("Apenas utilizadores com o perfil de Cliente podem criar serviços.");

            Cliente = cliente;
            Categoria = categoria;
        }
        
        // Métodos de mudança de estado
        public void Aprovar()
        {
            if (Estado != EstadoServico.PorAprovar)
                throw new InvalidOperationException("Apenas serviços por aprovar podem ser colocados em curso.");
            Estado = EstadoServico.EmCurso;
        }

        public void Cancelar()
        {
            if (Estado == EstadoServico.Cancelado)
                throw new InvalidOperationException("O serviço já se encontra cancelado.");
            if (Estado == EstadoServico.Concluido)
                throw new InvalidOperationException("Não é possível cancelar um serviço já concluído.");
            Estado = EstadoServico.Cancelado;
        }

        public void Concluir()
        {
            if (Estado != EstadoServico.EmCurso)
                throw new InvalidOperationException("Apenas serviços em curso podem ser concluídos.");
            Estado = EstadoServico.Concluido;
        }
    }

    

    
}

