namespace ProjetoAcademia
{
    enum EstadoOrcamento
    {
        Aceite,
        Recusado
    }

    class Orcamento
    {
        public Guid Id { get; private set; }

        // FK
        public Guid IdPrestador { get; private set; }
        public Utilizador Prestador { get; private set; }

        // FK
        public Guid IdServico { get; private set; }
        public Servico Servico { get; private set; }

        public decimal ValorSemIVA { get; private set; }
        public decimal TaxaIVA { get; private set; }
        public DateTime DataValidade { get; private set; }
        public int DuracaoDias { get; private set; }

        private decimal ValorTotal
        {
            get
            {
                return Math.Round(ValorSemIVA * (1 + TaxaIVA / 100m), 2);
            }
        }

        public Orcamento(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            if (idPrestador == Guid.Empty) 
                throw new ArgumentException("Prestador inválido.", nameof(idPrestador));

            if (idServico == Guid.Empty) 
                throw new ArgumentException("Serviço inválido.", nameof(idServico));

            if (valorSemIVA <= 0)
                throw new ArgumentOutOfRangeException(nameof(valorSemIVA), "O valor tem de ser positivo.");

            if (taxaIVA < 0 || taxaIVA > 100)
                throw new ArgumentOutOfRangeException(nameof(taxaIVA), "Taxa de IVA inválida.");

            if (dataValidade.Date <= DateTime.Now.Date)
                throw new ArgumentOutOfRangeException(nameof(dataValidade), "A validade tem de ser no futuro.");

            if (duracaoDias <= 0)
                throw new ArgumentOutOfRangeException(nameof(duracaoDias), "A duração tem de ser positiva.");

            this.Id = Guid.NewGuid();

            this.IdPrestador = idPrestador;
            this.IdServico = idServico;
            this.ValorSemIVA = valorSemIVA;
            this.TaxaIVA = taxaIVA;
            this.DataValidade = dataValidade;
            this.DuracaoDias = duracaoDias;

        }
    }
}
