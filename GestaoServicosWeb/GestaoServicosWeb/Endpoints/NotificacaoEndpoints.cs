using System.Security.Claims;
using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.API
{
    // Cada utilizador só vê e altera as suas notificações (o id vem do token)
    public static class NotificacaoAPI
    {
        public static void MapNotificacaoAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/notificacoes")
                .WithTags("Notificações")
                .RequireAuthorization();

            // GET /api/notificacoes → as mais recentes e o total de não lidas
            grupo.MapGet("/", (ClaimsPrincipal user, NotificacaoService service) =>
            {
                var id = user.ObterId();

                return Results.Ok(new ListaNotificacoesDto(
                    service.ContarNaoLidas(id),
                    service.Listar(id).Select(NotificacaoDto.De)));
            });

            // PUT /api/notificacoes/{id}/lida
            grupo.MapPut("/{id:guid}/lida", (Guid id, ClaimsPrincipal user, NotificacaoService service) =>
                service.MarcarComoLida(id, user.ObterId())
                    ? Results.NoContent()
                    : Results.NotFound());

            // PUT /api/notificacoes/lidas → marcar todas
            grupo.MapPut("/lidas", (ClaimsPrincipal user, NotificacaoService service) =>
            {
                service.MarcarTodasComoLidas(user.ObterId());
                return Results.NoContent();
            });
        }
    }
}
