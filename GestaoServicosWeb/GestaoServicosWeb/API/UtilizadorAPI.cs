using Microsoft.AspNetCore.Identity;
using ProjetoAcademia;   // enquanto as entidades tiverem o namespace antigo

namespace GestaoServicosWeb.API
{
    // ===== DTOs: o que entra e sai da API =====
    public record CriarUtilizadorDto(string Nome, string Email, string Password,
                                     string Nif, string? Telefone, Role Role);

    public record AtualizarUtilizadorDto(string Nome, string Email, string? Telefone);

    public record AlterarPasswordDto(string PasswordAtual, string PasswordNova);

    public record UtilizadorDto(Guid Id, string Nome, string Email,
                                string Nif, string? Telefone, Role Role);   // sem password

    // ===== Endpoints =====
    public static class UtilizadorAPI
    {
        // Lista temporária (substitui a BD por agora)
        private static readonly List<Utilizador> utilizadores = new();
        private static readonly PasswordHasher<Utilizador> hasher = new();

        public static void MapUtilizadorAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/utilizadores").WithTags("Utilizadores");

            // 1. GET /api/utilizadores?role=Prestador → listar (filtro opcional)
            grupo.MapGet("/", (Role? role) =>
            {
                var lista = role == null
                    ? utilizadores
                    : utilizadores.Where(u => u.Role == role).ToList();

                return Results.Ok(lista.Select(ParaDto));
            });

            // 2. GET /api/utilizadores/{id} → ver um utilizador
            grupo.MapGet("/{id:guid}", (Guid id) =>
            {
                var u = utilizadores.FirstOrDefault(x => x.Id == id);

                return u == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(u));
            });

            // 3. POST /api/utilizadores → registar
            grupo.MapPost("/", (CriarUtilizadorDto dto) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
                    return Results.BadRequest("A password deve ter pelo menos 8 caracteres.");

                if (dto.Role == Role.Empresa)
                    return Results.BadRequest("Não é possível registar-se como Empresa.");

                if (utilizadores.Any(u => u.Email == dto.Email?.Trim().ToLower()))
                    return Results.Conflict("Já existe um utilizador com esse email.");

                if (utilizadores.Any(u => u.NIF == dto.Nif?.Trim()))
                    return Results.Conflict("Já existe um utilizador com esse NIF.");

                try
                {
                    string hash = hasher.HashPassword(null!, dto.Password);
                    var u = new Utilizador(dto.Nome, dto.Email, hash, dto.Nif, dto.Telefone, dto.Role);

                    utilizadores.Add(u);
                    return Results.Created($"/api/utilizadores/{u.Id}", ParaDto(u));
                }
                catch (ArgumentException ex)          // validações da classe Utilizador
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // 4. PUT /api/utilizadores/{id} → atualizar perfil
            grupo.MapPut("/{id:guid}", (Guid id, AtualizarUtilizadorDto dto) =>
            {
                var u = utilizadores.FirstOrDefault(x => x.Id == id);
                if (u == null)
                    return Results.NotFound();

                if (utilizadores.Any(x => x.Id != id && x.Email == dto.Email?.Trim().ToLower()))
                    return Results.Conflict("Esse email já está a ser usado.");

                try
                {
                    u.Nome = dto.Nome;                // os setters validam
                    u.Email = dto.Email;
                    u.Telefone = dto.Telefone;
                    return Results.Ok(ParaDto(u));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // 5. PUT /api/utilizadores/{id}/password → alterar password
            grupo.MapPut("/{id:guid}/password", (Guid id, AlterarPasswordDto dto) =>
            {
                var u = utilizadores.FirstOrDefault(x => x.Id == id);
                if (u == null)
                    return Results.NotFound();

                var resultado = hasher.VerifyHashedPassword(u, u.PasswordHash, dto.PasswordAtual);
                if (resultado == PasswordVerificationResult.Failed)
                    return Results.BadRequest("A password atual está errada.");

                if (string.IsNullOrWhiteSpace(dto.PasswordNova) || dto.PasswordNova.Length < 8)
                    return Results.BadRequest("A nova password deve ter pelo menos 8 caracteres.");

                u.AlterarPassword(hasher.HashPassword(u, dto.PasswordNova));
                return Results.NoContent();
            });

            // 6. DELETE /api/utilizadores/{id} → apagar
            grupo.MapDelete("/{id:guid}", (Guid id) =>
            {
                var u = utilizadores.FirstOrDefault(x => x.Id == id);
                if (u == null)
                    return Results.NotFound();

                utilizadores.Remove(u);
                return Results.NoContent();
            });
        }

        // Converte a entidade em DTO (nunca expõe o PasswordHash)
        private static UtilizadorDto ParaDto(Utilizador u) =>
            new(u.Id, u.Nome, u.Email, u.NIF, u.Telefone, u.Role);
    }
}
