namespace GestaoServicosWeb.Contratos
{
    // ===== DTOs do Serviço: o que entra e sai da API =====

    // Entrada: dados para criar um novo pedido de serviço
    public record CriarServicoDto(
        Guid IdCliente,
        Guid IdCategoria,
        string Titulo,
        string Descricao,
        DateTime PrazoPropostas,
        DateTime DataDesejada
    );

    // Saída: formato devolvido pela API
    public record ServicoDto(
        Guid Id,
        Guid IdCliente,
        Guid IdCategoria,
        string Titulo,
        string Descricao,
        EstadoServico Estado,
        DateTime CriadoEm,
        DateTime PrazoPropostas,
        DateTime DataDesejada
    );
}
