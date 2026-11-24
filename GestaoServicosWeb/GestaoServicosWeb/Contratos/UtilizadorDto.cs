
namespace GestaoServicosWeb.Contratos
{
    // ===== DTOs do Utilizador: o que entra e sai da API =====

    // Entrada: registar um utilizador (a password vem em texto; o hash é feito no servidor)
    public record CriarUtilizadorDto(string Nome, string Email, string Password,
                                      Role Role);

    // Entrada: atualizar o perfil (NIF, Role e password não se alteram por aqui)
    public record AtualizarUtilizadorDto(string Nome, string Email);

    // Entrada: alterar a password (pede a atual para confirmar)
    public record AlterarPasswordDto(string PasswordAtual, string PasswordNova);

    // Saída: o que a API devolve (sem PasswordHash)
    public record UtilizadorDto(Guid Id, string Nome, string Email,
                                 Role Role);

    // Saída: o mínimo para escolher um cliente numa lista
    public record ClienteResumoDto(Guid Id, string Nome);
}
