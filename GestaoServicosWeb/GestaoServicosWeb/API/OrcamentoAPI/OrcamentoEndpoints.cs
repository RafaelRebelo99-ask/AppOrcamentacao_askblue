using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoServicosWeb.API.OrcamentoAPI;

public static class OrcamentoEndpoints
{
    public static void MapOrcamentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orcamentos").WithTags("Orçamentos");

        group.MapGet("/", (OrcamentoService service) =>
            TypedResults.Ok(service.Listar().Select(OrcamentoResponse.De)));

        group.MapGet("/{id:guid}", Results<Ok<OrcamentoResponse>, NotFound> (Guid id, OrcamentoService service) =>
        {
            var orcamento = service.Buscar(id);
            return orcamento is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(OrcamentoResponse.De(orcamento));
        });

        group.MapPost("/", Results<Created<OrcamentoResponse>, BadRequest<object>> (
            CriarOrcamentoRequest pedido, OrcamentoService service) =>
        {
            try
            {
                var orcamento = service.Criar(pedido.IdPrestador, pedido.IdServico, pedido.ValorSemIVA,
                                              pedido.TaxaIVA, pedido.DataValidade, pedido.DuracaoDias);
                return TypedResults.Created($"/orcamentos/{orcamento.Id}", OrcamentoResponse.De(orcamento));
            }
            catch (ArgumentException erro)
            {
                return TypedResults.BadRequest<object>(new { erro = erro.Message });
            }
        });
    }
}