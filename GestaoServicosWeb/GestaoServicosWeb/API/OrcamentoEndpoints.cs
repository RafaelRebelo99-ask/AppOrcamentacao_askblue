using System.Security.Claims;
using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.API;

public static class OrcamentoEndpoints
{
    public static void MapOrcamentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orcamentos")
            .WithTags("Orçamentos")
            .RequireAuthorization();

        // ---------------- Leitura ----------------

        // Prestador: os meus orçamentos
        group.MapGet("/", (ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(
                service.ListarDoPrestador(IdDe(user)).Select(OrcamentoResponse.De))));

        // Cliente: orçamentos recebidos para um serviço seu
        group.MapGet("/servico/{servicoId:guid}", (Guid servicoId, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(
                service.ListarDoServico(servicoId, IdDe(user)).Select(OrcamentoResponse.De))));

        // Prestador dono, ou cliente dono do serviço (se não for rascunho)
        group.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Obter(id, IdDe(user))))));

        // ---------------- Prestador ----------------

        // O prestador vem do token, não do corpo do pedido
        group.MapPost("/", (OrcamentoDtos pedido, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() =>
            {
                var orcamento = service.Criar(IdDe(user), pedido.IdServico, pedido.descricao, pedido.ValorSemIVA,
                                              pedido.TaxaIVA, pedido.DataValidade, pedido.DataInicio, pedido.DuracaoDias);
                return TypedResults.Created($"/api/orcamentos/{orcamento.Id}", OrcamentoResponse.De(orcamento));
            }));

        group.MapPut("/{id:guid}", (Guid id, EditarOrcamentoRequest pedido, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(
                service.Editar(id, IdDe(user), pedido.descricao, pedido.ValorSemIVA,
                               pedido.TaxaIVA, pedido.DataValidade, pedido.DataInicio, pedido.DuracaoDias)))));

        group.MapPost("/{id:guid}/enviar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Enviar(id, IdDe(user))))));

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() =>
            {
                service.Eliminar(id, IdDe(user));
                return TypedResults.NoContent();
            }));

        // ---------------- Cliente ----------------

        group.MapPost("/{id:guid}/aceitar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Aceitar(id, IdDe(user))))));

        group.MapPost("/{id:guid}/recusar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Recusar(id, IdDe(user))))));
    }

    // O utilizador vem sempre do token, nunca do corpo do pedido
    private static Guid IdDe(ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return Guid.TryParse(valor, out var id)
            ? id
            : throw new UnauthorizedAccessException("Utilizador não identificado.");
    }

    // Traduz as exceções do service para códigos HTTP num só sítio
    private static IResult Executar(Func<IResult> acao)
    {
        try
        {
            return acao();
        }
        catch (NaoEncontradoException e)
        {
            return TypedResults.NotFound(new { erro = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return TypedResults.Json(new { erro = e.Message }, statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.Conflict(new { erro = e.Message });
        }
        catch (ArgumentException e)
        {
            return TypedResults.BadRequest(new { erro = e.Message });
        }
        catch (DbUpdateException) // inclui DbUpdateConcurrencyException
        {
            return TypedResults.Conflict(new { erro = "O pedido entrou em conflito com outra alteração. Tenta novamente." });
        }
    }
}