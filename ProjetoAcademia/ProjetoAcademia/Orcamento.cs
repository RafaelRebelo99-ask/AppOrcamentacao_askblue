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

        private decimal ValorTotal { set; get; }
        private decimal ValorSemIVA { set; get; }
        private int TaxaIVA { set; get; }
        private DateTime DataValidade { set; get; }
        private TimeSpan DuracaoDias { set; get; }
    }
}
