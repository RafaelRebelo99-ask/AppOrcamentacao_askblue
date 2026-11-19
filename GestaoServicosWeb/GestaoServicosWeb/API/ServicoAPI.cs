using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using System.Security.Claims;

namespace GestaoServicosWeb.API
{
    // ===== Endpoints do Serviço =====
    // Só tratam do HTTP: recebem o pedido, chamam o ServicoService e escolhem a resposta.
    //
    // Permissões:
    //   - Criar: só Cliente (o dono é quem fez login)
    //   - Listar / ver: Cliente vê os seus, Prestador vê os abertos, Empresa vê todos
    //   - Editar / cancelar: o cliente dono, ou Empresa
    //   - Concluir: o prestador cujo orçamento foi aceite, ou Empresa
    //   - Apagar: só Empresa
    public static class ServicoAPI
    {
        public static void MapServicoAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/servicos")
                .WithTags("Serviços")
                .RequireAuthorization();

            // 1. GET /api/servicos → listar (o resultado depende do role de quem pede)
            grupo.MapGet("/", (EstadoServico? estado, Guid? idCategoria, ClaimsPrincipal user, ServicoService service) =>
            {
                var servicos = user.ObterRole() switch
                {
                    Role.Empresa => service.Listar(estado, idCategoria),
                    Role.Cliente => service.ListarPorCliente(user.ObterId(), estado, idCategoria),
                    // O prestador só vê serviços abertos, por isso o filtro de estado não se aplica
                    _ => service.ListarAbertos(idCategoria)
                };

                return Results.Ok(servicos.Select(ParaDto));
            });

            // 2. GET /api/servicos/disponiveis → serviços abertos a propostas — Prestador (e Empresa)
            grupo.MapGet("/disponiveis", (Guid? idCategoria, ServicoService service) =>
            {
                return Results.Ok(service.ListarAbertos(idCategoria).Select(ParaDto));
            })
            .RequireAuthorization(Politicas.Prestador);

            // 3. GET /api/servicos/{id} → ver um serviço por Id
            grupo.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, ServicoService service) =>
            {
                try
                {
                    var s = service.Buscar(id, user.ObterId(), user.ObterRole());

                    return s == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(s));
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
            });

            // 4. GET /api/servicos/cliente/{clienteId} → serviços de um cliente — o próprio, ou Empresa
            grupo.MapGet("/cliente/{clienteId:guid}", (Guid clienteId, ClaimsPrincipal user, ServicoService service) =>
            {
                if (user.ObterId() != clienteId && !user.EEmpresa())
                    return Results.Forbid();

                return Results.Ok(service.ListarPorCliente(clienteId).Select(ParaDto));
            });

            // 5. POST /api/servicos → criar novo serviço — só Cliente
            grupo.MapPost("/", (CriarServicoDto dto, ClaimsPrincipal user, ServicoService service) =>
            {
                try
                {
                    var s = service.Criar(user.ObterId(), dto);
                    return Results.Created($"/api/servicos/{s.Id}", ParaDto(s));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization(p => p.RequireRole(nameof(Role.Cliente)));

            // 6. PUT /api/servicos/{id} → editar serviço (apenas enquanto PorAprovar) — o cliente dono, ou Empresa
            grupo.MapPut("/{id:guid}", (Guid id, AtualizarServicoDto dto, ClaimsPrincipal user, ServicoService service) =>
            {
                try
                {
                    var s = service.Atualizar(id, dto, user.ObterId(), user.EEmpresa());
                    return s == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(s));
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization(Politicas.Cliente);

            // 7. PUT /api/servicos/{id}/cancelar → cancelar serviço — o cliente dono, ou Empresa
            grupo.MapPut("/{id:guid}/cancelar", (Guid id, ClaimsPrincipal user, ServicoService service) =>
            {
                try
                {
                    return service.Cancelar(id, user.ObterId(), user.EEmpresa())
                        ? Results.Ok(new { mensagem = "Serviço cancelado com sucesso." })
                        : Results.NotFound();
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization(Politicas.Cliente);

            // 8. PUT /api/servicos/{id}/concluir → EmCurso → Concluido — o prestador do orçamento aceite, ou Empresa
            grupo.MapPut("/{id:guid}/concluir", (Guid id, ClaimsPrincipal user, ServicoService service) =>
            {
                try
                {
                    return service.Concluir(id, user.ObterId(), user.EEmpresa())
                        ? Results.Ok(new { mensagem = "Serviço concluído com sucesso." })
                        : Results.NotFound();
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization(Politicas.Prestador);

            // 9. DELETE /api/servicos/{id} → apagar serviço e os seus orçamentos — só Empresa
            grupo.MapDelete("/{id:guid}", (Guid id, ServicoService service) =>
            {
                return service.Apagar(id)
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .RequireAuthorization(Politicas.Empresa);
        }

        // Converte a entidade de domínio no DTO de resposta
        private static ServicoDto ParaDto(Servico s) =>
            new(s.Id, s.IdCliente, s.IdCategoria, s.Titulo, s.Descricao, s.Estado, s.CriadoEm, s.PrazoPropostas, s.DataDesejada);
    }
}
