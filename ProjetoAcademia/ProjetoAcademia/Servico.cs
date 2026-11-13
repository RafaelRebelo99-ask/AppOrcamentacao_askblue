namespace ProjetoAcademia
{
    public enum Estado
    {
        PorAprovar,
        EmCurso,
        Cancelado
    }

    public class Servico
    {
        private Guid Id {  get; set; }

        //FK
        private Guid IdCliente { get; set; }
        private Utilizador Cliente { get; set; }

        //FK
        private Guid IdCategoria { get; set; }
        private Categoria Categoria { get; set; }

        private string Titulo { get; set; }
        private string Descricao {  get; set; }
        private Estado Estado { get; set; }
        private DateTime CriadoEm { get; set; }
        private DateTime PrazoPropostas {  get; set; }
        private DateTime DataDesejada { get; set; }

        public Servico(Utilizador cliente, Categoria categoria, string titulo, string descricao, DateTime prazoPropostas, DateTime dataDesejada)
        {
            // Validar Cliente
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente), "O cliente é obrigatório.");

            if (cliente.Role != Role.Cliente)
                throw new ArgumentException("Apenas utilizadores com o perfil de Cliente podem criar serviços.");

            // Validar Categoria
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria), "A categoria é obrigatória.");

            // Validar Textos
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título não pode estar vazio.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição não pode estar vazia.", nameof(descricao));

            // Validar Datas
            if (prazoPropostas <= DateTime.Now)
                throw new ArgumentException("O prazo de propostas deve ser uma data futura.", nameof(prazoPropostas));
            if (dataDesejada < prazoPropostas)
                throw new ArgumentException("A data desejada para o serviço deve ser posterior ao prazo de propostas.", nameof(dataDesejada));

            // Atribuição de valores
            Id = Guid.NewGuid();
            Cliente = cliente;
            IdCliente = cliente.Id;
            Categoria = categoria;
            // IdCategoria = categoria.Id; // quando Id for public em Categoria
            Titulo = titulo.Trim();
            Descricao = descricao.Trim();
            PrazoPropostas = prazoPropostas;
            DataDesejada = dataDesejada;
            CriadoEm = DateTime.Now;
            Estado = Estado.PorAprovar;
        }
        
        // Regras simples de mudança de estado
        public void Aprovar()
        {
            if (Estado != Estado.PorAprovar)
                throw new InvalidOperationException("Apenas serviços por aprovar podem ser colocados em curso.");
            Estado = Estado.EmCurso;
        }
        public void Cancelar()
        {
            if (Estado == Estado.Cancelado)
                throw new InvalidOperationException("O serviço já se encontra cancelado.");
            Estado = Estado.Cancelado;
        }
    }

    }

    
}
}
