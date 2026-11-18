using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.API
{
    // ===== Endpoints =====
    // Só tratam do HTTP: recebem o pedido, chamam o UtilizadorService e escolhem a resposta.
    public static class UtilizadorAPI
    {
        public static void MapUtilizadorAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/utilizadores")
                .WithTags("Utilizadores")
                .RequireAuthorization();

            // 1. GET /api/utilizadores?role=Prestador → listar (filtro opcional)
            grupo.MapGet("/", (Role? role, UtilizadorService service) =>
            {
                return Results.Ok(service.Listar(role).Select(ParaDto));
            });

            // 2. GET /api/utilizadores/{id} → ver um utilizador
            grupo.MapGet("/{id:guid}", (Guid id, UtilizadorService service) =>
            {
                var u = service.Buscar(id);

                return u == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(u));
            });

            // 3. POST /api/utilizadores → registar
            grupo.MapPost("/", (CriarUtilizadorDto dto, UtilizadorService service) =>
            {
                try
                {
                    var u = service.Criar(dto);
                    return Results.Created($"/api/utilizadores/{u.Id}", ParaDto(u));
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            })
            .AllowAnonymous()
            .WithName("RegistarUtilizador");

            // 4. PUT /api/utilizadores/{id} → atualizar perfil
            grupo.MapPut("/{id:guid}", (Guid id, AtualizarUtilizadorDto dto, UtilizadorService service) =>
            {
                try
                {
                    var u = service.Atualizar(id, dto);

                    return u == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(u));
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            });

            // 5. PUT /api/utilizadores/{id}/password → alterar password
            grupo.MapPut("/{id:guid}/password", (Guid id, AlterarPasswordDto dto, UtilizadorService service) =>
            {
                try
                {
                    return service.AlterarPassword(id, dto)
                        ? Results.NoContent()
                        : Results.NotFound();
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
            });

            // 6. DELETE /api/utilizadores/{id} → apagar
            grupo.MapDelete("/{id:guid}", (Guid id, UtilizadorService service) =>
            {
                try
                {
                    return service.Apagar(id)
                        ? Results.NoContent()
                        : Results.NotFound();
                }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            });
        }

        // Converte a entidade em DTO (nunca expõe o PasswordHash)
        private static UtilizadorDto ParaDto(Utilizador u) =>
            new(u.Id, u.Nome, u.Email, u.NIF, u.Telefone, u.Role);
    }
}
