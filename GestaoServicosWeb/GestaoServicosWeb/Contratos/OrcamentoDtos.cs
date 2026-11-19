namespace GestaoServicosWeb.Contratos;

// O prestador não vem no corpo: é o utilizador autenticado (token)
public record OrcamentoDtos(Guid IdServico, string descricao, decimal ValorSemIVA,
    decimal TaxaIVA, DateTime DataValidade, DateTime DataInicio, int DuracaoDias);

public record OrcamentoResponse(Guid Id, Guid IdPrestador, Guid IdServico, string descricao, decimal ValorSemIVA,
    decimal TaxaIVA, decimal ValorTotal, DateTime DataValidade, DateTime DataInicio, DateTime DataFim,
    int DuracaoDias, EstadoOrcamento Estado, bool Expirado)
{
    public static OrcamentoResponse De(Orcamento o) =>
        new(o.Id, o.IdPrestador, o.IdServico, o.Descricao ?? string.Empty, o.ValorSemIVA, o.TaxaIVA,
            o.ValorTotal, o.DataValidade, o.DataInicio, o.DataFim, o.DuracaoDias, o.Estado, o.Expirado);
}

public record EditarOrcamentoRequest(string descricao, decimal ValorSemIVA,
    decimal TaxaIVA, DateTime DataValidade, DateTime DataInicio, int DuracaoDias);
