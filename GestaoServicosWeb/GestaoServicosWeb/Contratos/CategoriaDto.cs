namespace GestaoServicosWeb.Contratos
{

    // Entrada: criar uma nova categoria
    public record CriarCategoriaDto(string Nome);

    // Saída: formato devolvido pela API
    public record CategoriaDto(Guid Id, string Nome);
}
