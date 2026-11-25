using System.Security.Claims;

namespace GestaoServicosWeb.API
{
    // ===== Nomes das policies de autorização =====
    // Usar estas constantes em vez de strings soltas evita erros de escrita.
    public static class Politicas
    {
        public const string Cliente = nameof(Role.Cliente);
        public const string Colaborador = nameof(Role.Colaborador);
        public const string Administrador = nameof(Role.Administrador);
    }

    // ===== Ajudas para ler o utilizador autenticado a partir do token JWT =====
    public static class ClaimsPrincipalExtensions
    {
        // Id do utilizador que fez o pedido (claim NameIdentifier, posta no token pelo LoginService)
        public static Guid ObterId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(valor, out var id)
                ? id
                : throw new InvalidOperationException("O token não tem um Id de utilizador válido.");
        }

        // Role do utilizador que fez o pedido (claim Role, posta no token pelo LoginService)
        public static Role ObterRole(this ClaimsPrincipal user) =>
            Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), out var role)
                ? role
                : throw new InvalidOperationException("O token não tem um role válido.");

        public static bool EAdmin(this ClaimsPrincipal user) =>
            user.IsInRole(nameof(Role.Administrador));

        public static string ObterNome(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.Name) ?? "Utilizador";
    }
}
