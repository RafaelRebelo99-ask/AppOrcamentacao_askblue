using GestaoServicosWeb.API;
using GestaoServicosWeb.API.OrcamentoAPI;
using GestaoServicosWeb.Aplicacao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Uma única instância partilhada: mantém os dados enquanto a aplicação corre
builder.Services.AddSingleton<UtilizadorService>();
builder.Services.AddSingleton<ServicoService>();
builder.Services.AddSingleton<OrcamentoService>();
builder.Services.AddSingleton<CategoriaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Gera a descrição da API em /openapi/v1.json
    app.MapOpenApi();

    // Página do Swagger em /swagger, que lê essa descrição
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "GestaoServicosWeb v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapUtilizadorAPI();
app.MapServicoAPI();
app.MapOrcamentoEndpoints();
app.MapCategoriaAPI();

app.Run();
