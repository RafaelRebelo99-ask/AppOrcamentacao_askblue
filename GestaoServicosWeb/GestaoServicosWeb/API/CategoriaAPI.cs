using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.API
{
    // ===== Endpoints de Categoria =====
    // Só tratam do HTTP: recebem o pedido, chamam o CategoriaService e escolhem a resposta.
    public static class CategoriaAPI
    {
        public static void MapCategoriaAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/categorias").WithTags("Categorias");

            // 1. GET /api/categorias → listar todas as categorias
            grupo.MapGet("/", (CategoriaService service) =>
            {
                return Results.Ok(service.Listar().Select(ParaDto));
            });

            // 2. GET /api/categorias/{id} → ver uma categoria por Id
            grupo.MapGet("/{id:guid}", (Guid id, CategoriaService service) =>
            {
                var c = service.Buscar(id);

                return c == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(c));
            });

            // 3. POST /api/categorias → criar nova categoria
            grupo.MapPost("/", (CriarCategoriaDto dto, CategoriaService service) =>
            {
                try
                {
                    var c = service.Criar(dto);
                    return Results.Created($"/api/categorias/{c.Id}", ParaDto(c));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
            });

            // 4. DELETE /api/categorias/{id} → apagar categoria
            grupo.MapDelete("/{id:guid}", (Guid id, CategoriaService service) =>
            {
                return service.Apagar(id)
                    ? Results.NoContent()
                    : Results.NotFound();
            });
        }

        // Converte a entidade de domínio no DTO de resposta
        private static CategoriaDto ParaDto(Categoria c) =>
            new(c.Id, c.Nome);
    }
}
