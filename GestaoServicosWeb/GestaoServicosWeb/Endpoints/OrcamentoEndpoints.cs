
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

        // leitura

        // Colaborador: propostas que criou
        group.MapGet("/", (
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    service.ListarDoCriador(IdDe(user))
                        .Select(OrcamentoResponse.De))))
            .RequireAuthorization(p =>
                p.RequireRole(nameof(Role.Colaborador)));

        // Administrador: todas as propostas
        group.MapGet("/todos", (
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    service.ListarTodos(IdDe(user))
                        .Select(OrcamentoResponse.De))))
            .RequireAuthorization(p =>
                p.RequireRole(nameof(Role.Administrador)));

        // Cliente: propostas enviadas para uma das suas oportunidades
        group.MapGet("/oportunidade/{oportunidadeId:guid}", (
            Guid oportunidadeId,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    service.ListarDaOportunidade(
                        oportunidadeId,
                        IdDe(user))
                    .Select(OrcamentoResponse.De))))
            .RequireAuthorization(p =>
                p.RequireRole(nameof(Role.Cliente)));

        // Obter uma proposta individual
        group.MapGet("/{id:guid}", (
            Guid id,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    OrcamentoResponse.De(
                        service.Obter(id, IdDe(user))))));

        // Colaborador

        // Criar proposta
        group.MapPost("/", (
            OrcamentoDto pedido,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
            {
                var orcamento = service.Criar(
                    IdDe(user),
                    pedido.IdOportunidade,
                    pedido.Descricao,
                    pedido.ValorSemIVA,
                    pedido.TaxaIVA,
                    pedido.DataValidade,
                    pedido.DataInicio,
                    pedido.DuracaoDias);

                return TypedResults.Created(
                    $"/api/orcamentos/{orcamento.Id}",
                    OrcamentoResponse.De(orcamento));
            }))
            .RequireAuthorization(p =>
                p.RequireRole(
                    nameof(Role.Colaborador),
                    nameof(Role.Administrador)));

        // Editar proposta
        group.MapPut("/{id:guid}", (
            Guid id,
            EditarOrcamentoRequest pedido,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    OrcamentoResponse.De(
                        service.Editar(
                            id,
                            IdDe(user),
                            pedido.Descricao,
                            pedido.ValorSemIVA,
                            pedido.TaxaIVA,
                            pedido.DataValidade,
                            pedido.DataInicio,
                            pedido.DuracaoDias)))))
            .RequireAuthorization(p =>
                p.RequireRole(
                    nameof(Role.Colaborador),
                    nameof(Role.Administrador)));

        // Enviar proposta ao cliente
        group.MapPost("/{id:guid}/enviar", (
            Guid id,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    OrcamentoResponse.De(
                        service.Enviar(id, IdDe(user)))))
            )
            .RequireAuthorization(p =>
                p.RequireRole(
                    nameof(Role.Colaborador),
                    nameof(Role.Administrador)));

        // Eliminar apenas rascunhos
        group.MapDelete("/{id:guid}", (
            Guid id,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
            {
                service.Eliminar(id, IdDe(user));
                return TypedResults.NoContent();
            }))
            .RequireAuthorization(p =>
                p.RequireRole(
                    nameof(Role.Colaborador),
                    nameof(Role.Administrador)));

        // cliente

        // Aceitar proposta
        group.MapPost("/{id:guid}/aceitar", (
            Guid id,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    OrcamentoResponse.De(
                        service.Aceitar(id, IdDe(user)))))
            )
            .RequireAuthorization(p =>
                p.RequireRole(nameof(Role.Cliente)));

        // Recusar proposta
        group.MapPost("/{id:guid}/recusar", (
            Guid id,
            ClaimsPrincipal user,
            OrcamentoService service) =>
            Executar(() =>
                TypedResults.Ok(
                    OrcamentoResponse.De(
                        service.Recusar(id, IdDe(user)))))
            )
            .RequireAuthorization(p =>
                p.RequireRole(nameof(Role.Cliente)));
    }

    // auxiliares

    // O utilizador vem sempre do token JWT.
    private static Guid IdDe(ClaimsPrincipal user)
    {
        var valor =
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        return Guid.TryParse(valor, out var id)
            ? id
            : throw new UnauthorizedAccessException(
                "Utilizador não identificado.");
    }

    // Tradução centralizada das exceções para HTTP.
    private static IResult Executar(Func<IResult> acao)
    {
        try
        {
            return acao();
        }
        catch (NaoEncontradoException e)
        {
            return TypedResults.NotFound(
                new { erro = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return TypedResults.Json(
                new { erro = e.Message },
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.Conflict(
                new { erro = e.Message });
        }
        catch (ArgumentException e)
        {
            return TypedResults.BadRequest(
                new { erro = e.Message });
        }
        catch (DbUpdateException)
        {
            return TypedResults.Conflict(
                new
                {
                    erro = "O pedido entrou em conflito com outra alteração. Tenta novamente."
                });
        }
    }
}
