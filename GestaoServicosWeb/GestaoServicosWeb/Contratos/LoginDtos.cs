namespace GestaoServicosWeb.Contratos
{
    /// <summary>Credenciais submetidas para iniciar sessão.</summary>
    public sealed record LoginDto(string Email, string Password);

    /// <summary>Token JWT e informação básica do utilizador autenticado.</summary>
    public sealed record LoginResponse(
        string AccessToken,
        string TokenType,
        DateTimeOffset ExpiresAtUtc,
        UtilizadorDto Utilizador);
}