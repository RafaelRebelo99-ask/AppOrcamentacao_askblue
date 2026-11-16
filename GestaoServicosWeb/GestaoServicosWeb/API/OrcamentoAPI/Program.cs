using GestaoServicosWeb.API.OrcamentoAPI;
using GestaoServicosWeb;

var builder = WebApplication.CreateBuilder(args);

        // Permitem gerar a descrição dos endpoints e mostrar o Swagger UI.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // Uma instância compartilhada mantém os dados enquanto esta aplicação roda.
        builder.Services.AddSingleton<OrcamentoService>();

        var app = builder.Build();

        // Documentação interativa apenas no ambiente de desenvolvimento.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/orcamentos", (OrcamentoService service) =>
        Results.Ok(service.Listar()))
        .Produces<Orcamento[]>(StatusCodes.Status200OK);

        app.MapGet("/orcamentos/{id:guid}", IResult(Guid id, OrcamentoService service) =>
        {
            var orcamento = service.Buscar(id);
            return orcamento is null ? Results.NotFound() : Results.Ok(orcamento);
        })
        .Produces<Orcamento>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/orcamentos", IResult (CriarOrcamentoRequest pedido, OrcamentoService service) =>
        {
            try
            {
                var orcamento = service.Criar(pedido.idPrestador, pedido.idServico, pedido.valorSemIVA,
                                                pedido.taxaIVA, pedido.dataValidade, pedido.duracaoDias);
                return Results.Created($"/orcamentos/{orcamento.Id}", orcamento);
            }
            catch (ArgumentException erro)
            {
                return Results.BadRequest(new { erro = erro.Message });
            }
        })
        .Accepts<CriarOrcamentoRequest>("application/json")
        .Produces<Orcamento>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.Run();

