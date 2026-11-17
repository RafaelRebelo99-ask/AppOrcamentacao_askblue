using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoServicosWeb.API;

public static class OrcamentoEndpoints
{
    public static void MapOrcamentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orcamentos").WithTags("Orçamentos");

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
            OrcamentoDtos pedido, OrcamentoService service) =>
        {
            try
            {
                var orcamento = service.Criar(pedido.IdPrestador, pedido.IdServico, pedido.ValorSemIVA,
                                              pedido.TaxaIVA, pedido.DataValidade, pedido.DuracaoDias);
                return TypedResults.Created($"/api/orcamentos/{orcamento.Id}", OrcamentoResponse.De(orcamento));
            }
            catch (ArgumentException erro)
            {
                return TypedResults.BadRequest<object>(new { erro = erro.Message });
            }
        });

        group.MapPut("/{id:guid}", Results<Ok<OrcamentoResponse>, NotFound, ForbidHttpResult, BadRequest<object>> (
            Guid id, EditarOrcamentoRequest pedido, OrcamentoService service) =>
        {
            try
            {
                var orcamento = service.Editar(id, pedido.IdPrestador, pedido.ValorSemIVA,
                                               pedido.TaxaIVA, pedido.DataValidade, pedido.DuracaoDias);
                return orcamento is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(OrcamentoResponse.De(orcamento));
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
            catch (ArgumentException erro)
            {
                return TypedResults.BadRequest<object>(new { erro = erro.Message });
            }
        });
    }
}