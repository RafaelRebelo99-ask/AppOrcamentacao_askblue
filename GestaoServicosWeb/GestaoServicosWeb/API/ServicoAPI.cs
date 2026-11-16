using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.API
{
    // ===== Endpoints do Serviço =====
    // Só tratam do HTTP: recebem o pedido, chamam o ServicoService e escolhem a resposta.
    public static class ServicoAPI
    {
        public static void MapServicoAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/servicos").WithTags("Serviços");

            // 1. GET /api/servicos → listar com filtros opcionais (estado, categoria)
            grupo.MapGet("/", (EstadoServico? estado, Guid? idCategoria, ServicoService service) =>
            {
                return Results.Ok(service.Listar(estado, idCategoria).Select(ParaDto));
            });

            // 2. GET /api/servicos/{id} → ver um serviço por Id
            grupo.MapGet("/{id:guid}", (Guid id, ServicoService service) =>
            {
                var s = service.Buscar(id);

                return s == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(s));
            });

            // 3. GET /api/servicos/cliente/{clienteId} → ver serviços de um determinado cliente
            grupo.MapGet("/cliente/{clienteId:guid}", (Guid clienteId, ServicoService service) =>
            {
                return Results.Ok(service.ListarPorCliente(clienteId).Select(ParaDto));
            });

            // 4. POST /api/servicos → criar novo serviço
            grupo.MapPost("/", (CriarServicoDto dto, ServicoService service) =>
            {
                try
                {
                    var s = service.Criar(dto);
                    return Results.Created($"/api/servicos/{s.Id}", ParaDto(s));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // 5. PUT /api/servicos/{id}/cancelar → cancelar serviço
            grupo.MapPut("/{id:guid}/cancelar", (Guid id, ServicoService service) =>
            {
                try
                {
                    return service.Cancelar(id)
                        ? Results.Ok(new { mensagem = "Serviço cancelado com sucesso." })
                        : Results.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
        }

        // Converte a entidade de domínio no DTO de resposta
        private static ServicoDto ParaDto(Servico s) =>
            new(s.Id, s.IdCliente, s.IdCategoria, s.Titulo, s.Descricao, s.Estado, s.CriadoEm, s.PrazoPropostas, s.DataDesejada);
    }
}
