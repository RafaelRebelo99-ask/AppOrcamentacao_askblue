namespace GestaoServicosWeb.Contratos;

public record OrcamentoDtos(Guid IdPrestador, Guid IdServico, decimal ValorSemIVA,
    decimal TaxaIVA, DateTime DataValidade, int DuracaoDias);

public record OrcamentoResponse(Guid Id, Guid IdPrestador, Guid IdServico, decimal ValorSemIVA,
    decimal TaxaIVA, decimal ValorTotal, DateTime DataValidade, int DuracaoDias)
{
    public static OrcamentoResponse De(Orcamento o) =>
        new(o.Id, o.IdPrestador, o.IdServico, o.ValorSemIVA, o.TaxaIVA,
            o.ValorTotal, o.DataValidade, o.DuracaoDias);
}

public record EditarOrcamentoRequest(Guid IdPrestador, decimal ValorSemIVA,
    decimal TaxaIVA, DateTime DataValidade, int DuracaoDias);