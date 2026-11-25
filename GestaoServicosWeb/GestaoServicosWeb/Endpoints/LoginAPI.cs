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
                (LoginDto dto, LoginService service, HistoricoService historico) =>
            {
                var resposta = service.Autenticar(dto);

                if (resposta is null)
                    return TypedResults.Unauthorized();

                historico.Registar(
                    resposta.Utilizador.Id,
                    resposta.Utilizador.Nome,
                    CategoriaHistorico.Autenticacao,
                    "Login",
                    $"{resposta.Utilizador.Nome} iniciou sessão.");

                return TypedResults.Ok(resposta);
            })
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Autentica um utilizador e devolve um token JWT.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}