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

        // Prestador: os meus orçamentos. Empresa: todos.
        group.MapGet("/", (ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(
                (user.EEmpresa() ? service.ListarTodos() : service.ListarDoPrestador(IdDe(user)))
                    .Select(OrcamentoResponse.De))))
        .RequireAuthorization(Politicas.Prestador);

        // Cliente: orçamentos recebidos para um serviço seu. Empresa: de qualquer serviço.
        group.MapGet("/servico/{servicoId:guid}", (Guid servicoId, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(
                service.ListarDoServico(servicoId, IdDe(user), user.EEmpresa()).Select(OrcamentoResponse.De))))
        .RequireAuthorization(Politicas.Cliente);

        // Prestador dono, cliente dono do serviço (se não for rascunho), ou Empresa
        group.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Obter(id, IdDe(user), user.EEmpresa())))));

        // ---------------- Prestador ----------------

        // O prestador vem do token, não do corpo do pedido
        group.MapPost("/", (OrcamentoDtos pedido, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() =>
            {
                var orcamento = service.Criar(IdDe(user), pedido.IdServico, pedido.descricao, pedido.ValorSemIVA,
                                              pedido.TaxaIVA, pedido.DataValidade, pedido.DataInicio, pedido.DuracaoDias);
                return TypedResults.Created($"/api/orcamentos/{orcamento.Id}", OrcamentoResponse.De(orcamento));
            }))
        .RequireAuthorization(p => p.RequireRole(nameof(Role.Prestador)));

        group.MapPut("/{id:guid}", (Guid id, EditarOrcamentoRequest pedido, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(
                service.Editar(id, IdDe(user), pedido.descricao, pedido.ValorSemIVA,
                               pedido.TaxaIVA, pedido.DataValidade, pedido.DataInicio, pedido.DuracaoDias)))))
        .RequireAuthorization(p => p.RequireRole(nameof(Role.Prestador)));

        group.MapPost("/{id:guid}/enviar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Enviar(id, IdDe(user))))))
        .RequireAuthorization(p => p.RequireRole(nameof(Role.Prestador)));

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() =>
            {
                service.Eliminar(id, IdDe(user), user.EEmpresa());
                return TypedResults.NoContent();
            }))
        .RequireAuthorization(Politicas.Prestador); // Prestador (os seus rascunhos) ou Empresa (qualquer um)

        // ---------------- Cliente ----------------

        group.MapPost("/{id:guid}/aceitar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Aceitar(id, IdDe(user))))))
        .RequireAuthorization(p => p.RequireRole(nameof(Role.Cliente)));

        group.MapPost("/{id:guid}/recusar", (Guid id, ClaimsPrincipal user, OrcamentoService service) =>
            Executar(() => TypedResults.Ok(OrcamentoResponse.De(service.Recusar(id, IdDe(user))))))
        .RequireAuthorization(p => p.RequireRole(nameof(Role.Cliente)));
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