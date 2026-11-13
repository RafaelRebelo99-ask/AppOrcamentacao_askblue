namespace ProjetoAcademia
{
    enum Estado
    {
        PorAprovar,
        EmCurso,
        Cancelado
    }

    class Servico
    {
        private Guid Id {  get; set; }

        private Guid IdCliente { get; set; }
        private Utilizador Cliente { get; set; }

        private Guid IdCategoria { get; set; }
        private Categoria Categoria { get; set; }

        private string Titulo { get; set; }
        private string Descricao {  get; set; }
        private Estado Estado { get; set; }
        private DateTime CriadoEm { get; set; }
        private DateTime PrazoPropostas {  get; set; }
        private DateTime DataDesejada { get; set; }
        private Categoria Categoria { get; set; }

    }
}
