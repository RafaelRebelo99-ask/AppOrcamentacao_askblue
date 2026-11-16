using GestaoServicosWeb.API;
using GestaoServicosWeb.Aplicacao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Uma única instância partilhada: mantém os dados enquanto a aplicação corre
builder.Services.AddSingleton<UtilizadorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapUtilizadorAPI();

app.Run();
