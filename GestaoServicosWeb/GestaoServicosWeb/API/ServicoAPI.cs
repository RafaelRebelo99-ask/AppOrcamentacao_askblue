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
            var grupo = app.MapGroup("/api/servicos")
                .WithTags("Serviços")
                .RequireAuthorization();

            // 1. GET /api/servicos → listar com filtros opcionais (estado, categoria)
            grupo.MapGet("/", (EstadoServico? estado, Guid? idCategoria, ServicoService service) =>
            {
                return Results.Ok(service.Listar(estado, idCategoria).Select(ParaDto));
            });

            // 2. GET /api/servicos/disponiveis → listar serviços abertos a propostas (PorAprovar)
            grupo.MapGet("/disponiveis", (Guid? idCategoria, ServicoService service) =>
            {
                return Results.Ok(service.ListarDisponiveis(idCategoria).Select(ParaDto));
            });

            // 3. GET /api/servicos/{id} → ver um serviço por Id
            grupo.MapGet("/{id:guid}", (Guid id, ServicoService service) =>
            {
                var s = service.Buscar(id);

                return s == null
                    ? Results.NotFound()
                    : Results.Ok(ParaDto(s));
            });

            // 4. GET /api/servicos/cliente/{clienteId} → ver serviços de um determinado cliente
            grupo.MapGet("/cliente/{clienteId:guid}", (Guid clienteId, ServicoService service) =>
            {
                return Results.Ok(service.ListarPorCliente(clienteId).Select(ParaDto));
            });

            // 5. POST /api/servicos → criar novo serviço
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

            // 6. PUT /api/servicos/{id} → editar serviço (apenas enquanto PorAprovar)
            grupo.MapPut("/{id:guid}", (Guid id, AtualizarServicoDto dto, ServicoService service) =>
            {
                try
                {
                    var s = service.Atualizar(id, dto);
                    return s == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(s));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // 7. PUT /api/servicos/{id}/cancelar → cancelar serviço
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

            // 8. PUT /api/servicos/{id}/concluir → concluir serviço (apenas se EmCurso)
            grupo.MapPut("/{id:guid}/concluir", (Guid id, ServicoService service) =>
            {
                try
                {
                    return service.Concluir(id)
                        ? Results.Ok(new { mensagem = "Serviço concluído com sucesso." })
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
