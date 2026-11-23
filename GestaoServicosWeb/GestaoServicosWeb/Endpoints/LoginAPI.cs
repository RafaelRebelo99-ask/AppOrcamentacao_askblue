using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoServicosWeb.API
{
    public static class LoginAPI
    {
        public static void MapLoginAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/auth").WithTags("Autenticação");

            grupo.MapPost("/login", Results<Ok<LoginResponse>, UnauthorizedHttpResult>
                (LoginDto dto, LoginService service) =>
            {
                var resposta = service.Autenticar(dto);

                return resposta is null
                    ? TypedResults.Unauthorized()
                    : TypedResults.Ok(resposta);
            })
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Autentica um utilizador e devolve um token JWT.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}