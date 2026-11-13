namespace ProjetoAcademia
{
    class Orcamento
    {
        private Guid Id { set; get; }

        //FK
        private Guid IdPrestador {  get; set; }
        private Utilizador Prestador { get; set; }

        //FK
        private Guid IdServico { get; set; }
        private Servico Servico { get; set; }

        private decimal ValorTotal
        {
            get
            {
                return Math.Round(ValorSemIVA * (1 + TaxaIVA / 100m), 2);
            }
        }

        private decimal ValorSemIVA { set; get; }
        private decimal TaxaIVA { set; get; }
        private DateTime DataValidade { set; get; }
        private int DuracaoDias { set; get; }

        public Orcamento(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            if (idPrestador == Guid.Empty) throw new ArgumentNullException("Prestador inválido: " + nameof(idPrestador));

            if (idServico == Guid.Empty) throw new ArgumentNullException("Serviço inválido: " + nameof(idServico));

            if (valorSemIVA <= 0)
                throw new ArgumentOutOfRangeException(nameof(valorSemIVA), "O valor tem de ser positivo.");

            if (taxaIVA < 0 || taxaIVA > 100)
                throw new ArgumentOutOfRangeException(nameof(taxaIVA), "Taxa de IVA inválida.");

            if (dataValidade.Date <= DateTime.UtcNow.Date)
                throw new ArgumentOutOfRangeException(nameof(dataValidade), "A validade tem de ser no futuro.");

            if (duracaoDias is <= 0)
                throw new ArgumentOutOfRangeException(nameof(duracaoDias), "A duração tem de ser positiva.");

            this.IdPrestador = idPrestador;
            this.IdServico = idServico;
            this.ValorSemIVA = valorSemIVA;
            this.TaxaIVA = taxaIVA;
            this.DataValidade = dataValidade;
            this.DuracaoDias = duracaoDias;

        }
    }
}
