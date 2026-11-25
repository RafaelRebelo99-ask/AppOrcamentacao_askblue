
using GestaoServicosWeb;
using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Contratos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestaoServicosWeb.API
{
    public static class OportunidadeAPI
    {
        public static void MapOportunidadeAPI(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/oportunidades")
                .WithTags("Oportunidades")
                .RequireAuthorization();

            // GET /api/oportunidades
            // Colaboradores e administradores veem todas.
            // Clientes veem apenas as suas.
            grupo.MapGet("/", (
                EstadoOportunidade? estado,
                Guid? idCategoria,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                var idUtilizador = user.ObterId();

                return Executar(() =>
                {
                    var oportunidades = user.ObterRole() switch
                    {
                        Role.Colaborador or Role.Administrador =>
                            service.Listar(idUtilizador, estado, idCategoria),

                        Role.Cliente =>
                            service.ListarPorCliente(idUtilizador, estado),

                        _ => throw new UnauthorizedAccessException(
                            "Não tem permissão para consultar oportunidades.")
                    };

                    return Results.Ok(oportunidades.Select(ParaDto));
                });
            });

            // GET /api/oportunidades/{id}
            grupo.MapGet("/{id:guid}", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                {
                    var oportunidade = service.Buscar(id, user.ObterId());

                    return oportunidade == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(oportunidade));
                });
            });

            // POST /api/oportunidades
            // Apenas colaboradores e administradores.
            grupo.MapPost("/", (
                CriarOportunidadeDto dto,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                {
                    var oportunidade = service.Criar(
                        user.ObterId(),
                        dto);

                    return Results.Created(
                        $"/api/oportunidades/{oportunidade.Id}",
                        ParaDto(oportunidade));
                });
            })
            .RequireAuthorization(Politicas.Colaborador);

            // PUT /api/oportunidades/{id}
            grupo.MapPut("/{id:guid}", (
                Guid id,
                AtualizarOportunidadeDto dto,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                {
                    var oportunidade = service.Atualizar(
                        id,
                        dto,
                        user.ObterId());

                    return oportunidade == null
                        ? Results.NotFound()
                        : Results.Ok(ParaDto(oportunidade));
                });
            })
            .RequireAuthorization(Politicas.Colaborador);

            // PUT /api/oportunidades/{id}/iniciar-analise
            grupo.MapPut("/{id:guid}/iniciar-analise", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                    service.IniciarAnalise(id, user.ObterId())
                        ? Results.Ok(new
                        {
                            mensagem = "Análise iniciada com sucesso."
                        })
                        : Results.NotFound());
            })
            .RequireAuthorization(Politicas.Colaborador);

            // PUT /api/oportunidades/{id}/iniciar-preparacao
            grupo.MapPut("/{id:guid}/iniciar-preparacao", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                    service.IniciarPreparacao(id, user.ObterId())
                        ? Results.Ok(new
                        {
                            mensagem = "Preparação da proposta iniciada com sucesso."
                        })
                        : Results.NotFound());
            })
            .RequireAuthorization(Politicas.Colaborador);

            // PUT /api/oportunidades/{id}/perdida
            grupo.MapPut("/{id:guid}/perdida", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                    service.MarcarComoPerdida(id, user.ObterId())
                        ? Results.Ok(new
                        {
                            mensagem = "Oportunidade marcada como perdida."
                        })
                        : Results.NotFound());
            })
            .RequireAuthorization(Politicas.Colaborador);

            // PUT /api/oportunidades/{id}/cancelar
            grupo.MapPut("/{id:guid}/cancelar", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service) =>
            {
                return Executar(() =>
                    service.Cancelar(id, user.ObterId())
                        ? Results.Ok(new
                        {
                            mensagem = "Oportunidade cancelada com sucesso."
                        })
                        : Results.NotFound());
            })
            .RequireAuthorization(Politicas.Colaborador);

            // POST /api/oportunidades/{id}/documentos
            // Guarda um ou mais ficheiros na pasta Uploads/oportunidades/{id}
            grupo.MapPost("/{id:guid}/documentos", async (
                Guid id,
                HttpRequest request,
                ClaimsPrincipal user,
                OportunidadeService service,
                IWebHostEnvironment env) =>
            {
                Oportunidade? oportunidade;
                try
                {
                    oportunidade = service.Buscar(id, user.ObterId());
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }

                if (oportunidade == null)
                    return Results.NotFound("Oportunidade não encontrada.");

                if (!request.HasFormContentType)
                    return Results.BadRequest("Envie os ficheiros em formato multipart/form-data.");

                var form = await request.ReadFormAsync();
                if (form.Files.Count == 0)
                    return Results.BadRequest("Nenhum ficheiro enviado.");

                const long maxTamanhoBytes = 10 * 1024 * 1024; // 10 MB por ficheiro
                var pasta = ObterPastaDocumentos(env, id);
                Directory.CreateDirectory(pasta);

                var guardados = new List<object>();

                foreach (var ficheiro in form.Files)
                {
                    if (ficheiro.Length == 0)
                        continue;

                    if (ficheiro.Length > maxTamanhoBytes)
                        return Results.BadRequest(
                            $"O ficheiro '{ficheiro.FileName}' excede o limite de 10 MB.");

                    var nomeSeguro = Path.GetFileName(ficheiro.FileName);
                    if (string.IsNullOrWhiteSpace(nomeSeguro))
                        continue;

                    var caminho = Path.Combine(pasta, nomeSeguro);
                    if (File.Exists(caminho))
                    {
                        nomeSeguro = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{nomeSeguro}";
                        caminho = Path.Combine(pasta, nomeSeguro);
                    }

                    await using var stream = File.Create(caminho);
                    await ficheiro.CopyToAsync(stream);

                    guardados.Add(new { nome = nomeSeguro, tamanho = ficheiro.Length });
                }

                return Results.Ok(guardados);
            })
            .RequireAuthorization(Politicas.Colaborador)
            .DisableAntiforgery();

            // GET /api/oportunidades/{id}/documentos
            // Lista os documentos associados a uma oportunidade (com controlo de acesso)
            grupo.MapGet("/{id:guid}/documentos", (
                Guid id,
                ClaimsPrincipal user,
                OportunidadeService service,
                IWebHostEnvironment env) =>
            {
                return Executar(() =>
                {
                    var oportunidade = service.Buscar(id, user.ObterId());
                    if (oportunidade == null)
                        return Results.NotFound();

                    var pasta = ObterPastaDocumentos(env, id);
                    if (!Directory.Exists(pasta))
                        return Results.Ok(Array.Empty<object>());

                    var ficheiros = new DirectoryInfo(pasta)
                        .GetFiles()
                        .OrderBy(f => f.Name)
                        .Select(f => new { nome = f.Name, tamanho = f.Length })
                        .ToList();

                    return Results.Ok(ficheiros);
                });
            });

            // GET /api/oportunidades/{id}/documentos/{nome}
            // Faz download de um documento específico da oportunidade
            grupo.MapGet("/{id:guid}/documentos/{nome}", (
                Guid id,
                string nome,
                ClaimsPrincipal user,
                OportunidadeService service,
                IWebHostEnvironment env) =>
            {
                return Executar(() =>
                {
                    var oportunidade = service.Buscar(id, user.ObterId());
                    if (oportunidade == null)
                        return Results.NotFound();

                    var nomeSeguro = Path.GetFileName(nome);
                    var caminho = Path.Combine(ObterPastaDocumentos(env, id), nomeSeguro);

                    if (!File.Exists(caminho))
                        return Results.NotFound("Documento não encontrado.");

                    return Results.File(caminho, "application/octet-stream", nomeSeguro);
                });
            });
        }

        private static string ObterPastaDocumentos(IWebHostEnvironment env, Guid oportunidadeId) =>
            Path.Combine(env.ContentRootPath, "Uploads", "oportunidades", oportunidadeId.ToString());

        private static OportunidadeDto ParaDto(Oportunidade o) =>
            new(
                o.Id,
                o.IdCliente,
                o.IdCategoria,
                o.IdResponsavel,
                o.Titulo,
                o.Descricao,
                o.Estado,
                o.DataCriacao,
                o.PrazoProposta,
                o.DataPretendida
            );

        private static IResult Executar(Func<IResult> acao)
        {
            try
            {
                return acao();
            }
            catch (NaoEncontradoException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Results.Conflict(
                    "Não foi possível guardar os dados. Verifique as relações e os valores enviados.");
            }
        }
    }
}
