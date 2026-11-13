namespace ProjetoAcademia
{
    class Orcamento
    {
        private Guid _id;
        private decimal ValorTotal { set; get; }
        private decimal ValorSemIVA { set; get; }
        private int TaxaIVA { set; get; }
    }
}
