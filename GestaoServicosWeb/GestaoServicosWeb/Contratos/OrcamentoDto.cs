
namespace GestaoServicosWeb.Contratos;

// O criador é identificado pelo utilizador autenticado.
// Não deve ser enviado no corpo do pedido.
public record OrcamentoDto(
    Guid IdOportunidade,
    string Descricao,
    decimal ValorSemIVA,
    decimal TaxaIVA,
    DateTime DataValidade,
    DateTime DataInicio,
    int DuracaoDias
);

public record OrcamentoResponse(
    Guid Id,
    Guid IdCriador,
    Guid IdOportunidade,
    string Descricao,
    decimal ValorSemIVA,
    decimal TaxaIVA,
    decimal ValorTotal,
    DateTime DataCriacao,
    DateTime? DataEnvio,
    DateTime? DataDecisao,
    DateTime DataValidade,
    DateTime DataInicio,
    DateTime DataFim,
    int DuracaoDias,
    EstadoOrcamento Estado,
    bool Expirado
)
{
    public static OrcamentoResponse De(Orcamento o) =>
        new(
            o.Id,
            o.IdCriador,
            o.IdOportunidade,
            o.Descricao,
            o.ValorSemIVA,
            o.TaxaIVA,
            o.ValorTotal,
            o.DataCriacao,
            o.DataEnvio,
            o.DataDecisao,
            o.DataValidade,
            o.DataInicio,
            o.DataFim,
            o.DuracaoDias,
            o.Estado,
            o.Expirado
        );
}

public record EditarOrcamentoRequest(
    string Descricao,
    decimal ValorSemIVA,
    decimal TaxaIVA,
    DateTime DataValidade,
    DateTime DataInicio,
    int DuracaoDias
);
