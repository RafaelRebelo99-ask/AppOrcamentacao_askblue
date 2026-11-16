using ProjetoAcademia;

namespace GestaoServicosWeb.API
{
    public static class ServicoAPI
    {
        // Lista em memória para guardar os serviços temporariamente
        private static readonly List<Servico> _servicos = new();

        // Método de extensão que regista todas as rotas (Minimal API) de Serviços
        public static void MapServicoEndpoints(this IEndpointRouteBuilder app)
        {
            var grupo = app.MapGroup("/api/servicos").WithTags("Serviços");

            // 1. Criar novo serviço: POST /api/servicos
            grupo.MapPost("/", (CriarServicoDto dto) =>
            {
                try
                {
                    var servico = new Servico(
                        dto.IdCliente,
                        dto.IdCategoria,
                        dto.Titulo,
                        dto.Descricao,
                        dto.PrazoPropostas,
                        dto.DataDesejada
                    );

                    _servicos.Add(servico);

                    return Results.Created($"/api/servicos/{servico.Id}", servico);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { erro = ex.Message });
                }
            });

            // 2. Listar todos os serviços: GET /api/servicos
            grupo.MapGet("/", () => Results.Ok(_servicos));

            // 3. Obter um serviço por Id: GET /api/servicos/{id}
            grupo.MapGet("/{id:guid}", (Guid id) =>
            {
                var servico = _servicos.FirstOrDefault(s => s.Id == id);
                return servico is not null 
                    ? Results.Ok(servico) 
                    : Results.NotFound(new { mensagem = "Serviço não encontrado." });
            });

            // 4. Cancelar um serviço: PUT /api/servicos/{id}/cancelar
            grupo.MapPut("/{id:guid}/cancelar", (Guid id) =>
            {
                var servico = _servicos.FirstOrDefault(s => s.Id == id);
                if (servico is null)
                {
                    return Results.NotFound(new { mensagem = "Serviço não encontrado." });
                }

                try
                {
                    servico.Cancelar();
                    return Results.Ok(new { mensagem = "Serviço cancelado com sucesso.", servico });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { erro = ex.Message });
                }
            });
        }
    }

    // DTO simples com os dados recebidos para criar o serviço
    public class CriarServicoDto
    {
        public Guid IdCliente { get; set; }
        public Guid IdCategoria { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime PrazoPropostas { get; set; }
        public DateTime DataDesejada { get; set; }
    }
}
