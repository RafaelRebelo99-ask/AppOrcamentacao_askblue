
namespace GestaoServicosWeb.Contratos
{
    // Dados necessários para criar uma oportunidade.
    // O responsável é o colaborador autenticado.
    public record CriarOportunidadeDto(
        Guid IdCliente,
        Guid IdCategoria,
        string Titulo,
        string Descricao,
        DateTime? PrazoProposta,
        DateTime? DataPretendida
    );

    // Dados que podem ser alterados numa oportunidade.
    public record AtualizarOportunidadeDto(
        Guid IdCategoria,
        string Titulo,
        string Descricao,
        DateTime? PrazoProposta,
        DateTime? DataPretendida
    );

    // Dados devolvidos pela API.
    public record OportunidadeDto(
        Guid Id,
        Guid IdCliente,
        Guid IdCategoria,
        Guid IdResponsavel,
        string Titulo,
        string Descricao,
        EstadoOportunidade Estado,
        DateTime DataCriacao,
        DateTime? PrazoProposta,
        DateTime? DataPretendida
    );
}
