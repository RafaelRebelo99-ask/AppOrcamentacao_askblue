
namespace GestaoServicosWeb.Contratos
{
    // ===== DTOs do Utilizador: o que entra e sai da API =====

    // Entrada: registar um utilizador (a password vem em texto; o hash é feito no servidor)
    public record CriarUtilizadorDto(string Nome, string Email, string Password,
                                     string Nif, string? Telefone, Role Role);

    // Entrada: atualizar o perfil (NIF, Role e password não se alteram por aqui)
    public record AtualizarUtilizadorDto(string Nome, string Email, string? Telefone);

    // Entrada: alterar a password (pede a atual para confirmar)
    public record AlterarPasswordDto(string PasswordAtual, string PasswordNova);

    // Saída: o que a API devolve (sem PasswordHash)
    public record UtilizadorDto(Guid Id, string Nome, string Email,
                                string Nif, string? Telefone, Role Role);
}
