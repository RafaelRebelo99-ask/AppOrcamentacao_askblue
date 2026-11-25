namespace GestaoServicosWeb.Contratos
{
    public record HistoricoDto(
        Guid Id,
        Guid? IdUtilizador,
        string NomeUtilizador,
        CategoriaHistorico Categoria,
        string Acao,
        string Descricao,
        DateTime DataHora)
    {
        public static HistoricoDto De(RegistoHistorico r) =>
            new(
                r.Id,
                r.IdUtilizador,
                r.NomeUtilizador,
                r.Categoria,
                r.Acao,
                r.Descricao,
                r.DataHora);
    }
}
