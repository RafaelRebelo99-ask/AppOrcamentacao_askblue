using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using System.Security.Claims;

namespace GestaoServicosWeb.API
{
    // ===== Endpoints =====
    // Só tratam do HTTP: recebem o pedido, chamam o UtilizadorService e escolhem a resposta.
    //
    // Permissões:
    //   - Registar: público (só Cliente ou Prestador; a Empresa é criada pelo AdminSeeder)
    //   - Listar todos / apagar: só Empresa (um admin não pode apagar a própria conta)
    //   - Ver / editar perfil: o próprio, ou Empresa
    //   - Alterar password: só o próprio (pede a password atual)
    public static class UtilizadorAPI
    {
        public static void MapUtilizadorAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/utilizadores")
                .WithTags("Utilizadores")
                .RequireAuthorization();

            // 1. GET /api/utilizadores?role=Prestador → listar (filtro opcional) — só Empresa
            grupo.MapGet("/", (Role? role, UtilizadorService service) =>
            {
                return Results.Ok(service.Listar(role).Select(ParaDto));
            })
            .RequireAuthorization(Politicas.Administrador);

            // 2. GET /api/utilizadores/eu → o perfil de quem fez login
            grupo.MapGet("/eu", (ClaimsPrincipal user, UtilizadorService service) =>
            {
                var u = service.Buscar(user.ObterId());

                // Pode acontecer se a conta foi apagada depois de o token ser emitido
                return u == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(u));
            });

            // GET /api/utilizadores/clientes → id e nome dos clientes ativos — Colaborador e Administrador
            // Serve para escolher o cliente ao criar uma oportunidade, sem expor os outros dados.
            grupo.MapGet("/clientes", (UtilizadorService service) =>
            {
                return Results.Ok(service.Listar(Role.Cliente)
                    .Where(u => u.Ativo)
                    .OrderBy(u => u.Nome)
                    .Select(u => new ClienteResumoDto(u.Id, u.Nome)));
            })
            .RequireAuthorization(Politicas.Colaborador);

            // 3. GET /api/utilizadores/{id} → ver um utilizador — o próprio, ou Empresa
            grupo.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, UtilizadorService service) =>
            {
                if (!EOProprioOuEmpresa(user, id))
                    return Results.Forbid();

                var u = service.Buscar(id);

                return u == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(u));
            });

            // 4. POST /api/utilizadores → registar (público)
            grupo.MapPost("/", (CriarUtilizadorDto dto, ClaimsPrincipal user, UtilizadorService service, HistoricoService historico) =>
            {
                try
                {
                    var u = service.Criar(dto);

                    var autenticado = user.Identity?.IsAuthenticated == true;
                    var autorId = autenticado ? user.ObterId() : u.Id;
                    var autorNome = autenticado ? user.ObterNome() : u.Nome;
                    var descricao = autenticado
                        ? $"{autorNome} criou a conta de {u.Role} \"{u.Nome}\" ({u.Email})."
                        : $"{u.Nome} registou-se na plataforma ({u.Email}).";

                    historico.Registar(
                        autorId,
                        autorNome,
                        CategoriaHistorico.Utilizadores,
                        "Criar utilizador",
                        descricao);

                    return Results.Created($"/api/utilizadores/{u.Id}", ParaDto(u));
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            })
            .AllowAnonymous()
            .WithName("RegistarUtilizador");

            // POST /api/utilizadores/colaboradores → criar um colaborador da AskBlue — só Administrador
            grupo.MapPost("/colaboradores", (CriarColaboradorDto dto, ClaimsPrincipal user, UtilizadorService service, HistoricoService historico) =>
            {
                try
                {
                    var u = service.CriarColaborador(dto);

                    var nome = user.ObterNome();
                    historico.Registar(
                        user.ObterId(),
                        nome,
                        CategoriaHistorico.Utilizadores,
                        "Criar colaborador",
                        $"{nome} criou o colaborador \"{u.Nome}\" ({u.Email}).");

                    return Results.Created($"/api/utilizadores/{u.Id}", ParaDto(u));
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            })
            .RequireAuthorization(Politicas.Administrador);

            // 5. PUT /api/utilizadores/{id} → atualizar perfil — o próprio, ou Empresa
            grupo.MapPut("/{id:guid}", (Guid id, AtualizarUtilizadorDto dto, ClaimsPrincipal user, UtilizadorService service, HistoricoService historico) =>
            {
                if (!EOProprioOuEmpresa(user, id))
                    return Results.Forbid();

                try
                {
                    var u = service.Atualizar(id, dto);

                    if (u == null)
                        return Results.NotFound();

                    var nome = user.ObterNome();
                    historico.Registar(
                        user.ObterId(),
                        nome,
                        CategoriaHistorico.Utilizadores,
                        "Editar utilizador",
                        $"{nome} atualizou os dados de \"{u.Nome}\" ({u.Email}).");

                    return Results.Ok(ParaDto(u));
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            });

            // 6. PUT /api/utilizadores/{id}/password → alterar password — só o próprio
            grupo.MapPut("/{id:guid}/password", (Guid id, AlterarPasswordDto dto, ClaimsPrincipal user, UtilizadorService service, HistoricoService historico) =>
            {
                if (user.ObterId() != id)
                    return Results.Forbid();

                try
                {
                    if (!service.AlterarPassword(id, dto))
                        return Results.NotFound();

                    var nome = user.ObterNome();
                    historico.Registar(
                        user.ObterId(),
                        nome,
                        CategoriaHistorico.Utilizadores,
                        "Alterar password",
                        $"{nome} alterou a palavra-passe.");

                    return Results.NoContent();
                }
                catch (ArgumentException ex)          { return Results.BadRequest(ex.Message); }
            });

            // 7. DELETE /api/utilizadores/{id} → apagar — só Empresa, e nunca a própria conta
            grupo.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, UtilizadorService service, HistoricoService historico) =>
            {
                if (user.ObterId() == id)
                    return Results.BadRequest("Não pode apagar a sua própria conta.");

                try
                {
                    var alvo = service.Buscar(id);

                    if (!service.Apagar(id))
                        return Results.NotFound();

                    var nome = user.ObterNome();
                    var alvoTexto = alvo != null ? $"\"{alvo.Nome}\" ({alvo.Email})" : id.ToString()[..8];
                    historico.Registar(
                        user.ObterId(),
                        nome,
                        CategoriaHistorico.Utilizadores,
                        "Apagar utilizador",
                        $"{nome} apagou a conta de {alvoTexto}.");

                    return Results.NoContent();
                }
                catch (InvalidOperationException ex)  { return Results.Conflict(ex.Message); }
            })
            .RequireAuthorization(Politicas.Administrador);
        }

        private static bool EOProprioOuEmpresa(ClaimsPrincipal user, Guid id) =>
            user.ObterId() == id || user.EAdmin();

        // Converte a entidade em DTO (nunca expõe o PasswordHash)
        private static UtilizadorDto ParaDto(Utilizador u) =>
            new(u.Id, u.Nome, u.Email, u.Role);
    }
}
