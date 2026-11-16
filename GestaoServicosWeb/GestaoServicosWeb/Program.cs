using System.Text.Json.Serialization;
using GestaoServicosWeb.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Converte Enums (como EstadoServico) para strings legíveis em vez de números inteiros
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Redireciona a raiz para a interface do Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));

    // Swagger UI visual interativo compatível com o endpoint /openapi/v1.json do .NET
    app.MapGet("/swagger", () => Results.Content("""
        <!DOCTYPE html>
        <html lang="pt">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1" />
            <title>Swagger UI - GestaoServicosWeb</title>
            <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
            <link rel="icon" type="image/png" href="https://unpkg.com/swagger-ui-dist@5/favicon-32x32.png" sizes="32x32" />
        </head>
        <body>
            <div id="swagger-ui"></div>
            <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
            <script>
                window.onload = () => {
                    SwaggerUIBundle({
                        url: '/openapi/v1.json',
                        dom_id: '#swagger-ui',
                        deepLinking: true,
                        displayRequestDuration: true
                    });
                };
            </script>
        </body>
        </html>
        """, "text/html"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapServicoEndpoints();

app.Run();
