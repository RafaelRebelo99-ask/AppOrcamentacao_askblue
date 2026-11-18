using GestaoServicosWeb.API;
using GestaoServicosWeb.Aplicacao;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("A secção de configuração Jwt é obrigatória.");
var signingKey = jwtSettings.GetValidatedSigningKey();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(opcoes =>
{
    opcoes.AddDocumentTransformer((documento, _, _) =>
    {
        var componentes = documento.Components ??= new OpenApiComponents();
        var esquemas = componentes.SecuritySchemes
            ?? new Dictionary<string, IOpenApiSecurityScheme>();
        esquemas["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Introduza o token JWT devolvido pelo endpoint de login."
        };
        componentes.SecuritySchemes = esquemas;

        documento.Security ??= [];
        documento.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", documento)] = []
        });

        foreach (var path in documento.Paths.Values)
        {
            if (path.Operations is null)
                continue;

            foreach (var operacao in path.Operations.Values)
            {
                if (operacao.OperationId is "Login" or "RegistarUtilizador")
                    operacao.Security = [];
            }
        }

        return Task.CompletedTask;
    });
});

// Uma única instância partilhada: mantém os dados enquanto a aplicação corre
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<UtilizadorService>();
builder.Services.AddSingleton<ServicoService>();
builder.Services.AddSingleton<OrcamentoService>();
builder.Services.AddSingleton<CategoriaService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapLoginAPI();
app.MapUtilizadorAPI();
app.MapServicoAPI();
app.MapOrcamentoEndpoints();
app.MapCategoriaAPI();

app.Run();
