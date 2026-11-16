namespace GestaoServicosWeb.API.OrcamentoAPI
{
    public record CriarOrcamentoRequest(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias);

}