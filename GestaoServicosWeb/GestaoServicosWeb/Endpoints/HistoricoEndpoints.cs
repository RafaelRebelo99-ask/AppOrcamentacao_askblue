using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.API
{
    // Histórico de atividade da plataforma — acesso exclusivo para Administradores
    public static class HistoricoAPI
    {
        public static void MapHistoricoAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/historico")
                .WithTags("Histórico")
                .RequireAuthorization(Politicas.Administrador);

            // GET /api/historico?categoria=Oportunidades
            grupo.MapGet("/", (CategoriaHistorico? categoria, HistoricoService service) =>
            {
                return Results.Ok(
                    service.Listar(categoria).Select(HistoricoDto.De));
            });
        }
    }
}
