using GestaoServicosWeb;
using GestaoServicosWeb.API;
using GestaoServicosWeb.Aplicacao;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;
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

// Base de dados SQL Server. A connection string fica nos user-secrets (não vai para o Git):
//   dotnet user-secrets set "ConnectionStrings:Default" "Server=127.0.0.1,14330;Database=GestaoServicos;User Id=sa;Password=...;TrustServerCertificate=True"
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("A connection string 'Default' é obrigatória (ver comentário no Program.cs).");
builder.Services.AddDbContext<AppDbContext>(opcoes => opcoes.UseSqlServer(connectionString));

// Scoped: uma instância por pedido HTTP, tal como o AppDbContext
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<UtilizadorService>();
builder.Services.AddScoped<NotificacaoService>();
builder.Services.AddScoped<OportunidadeService>();
builder.Services.AddScoped<OrcamentoService>();
builder.Services.AddScoped<CategoriaService>();
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
// Uma policy por role. Usar nos endpoints com .RequireAuthorization(Politicas.Cliente), etc.
// O admin entra em todas, porque pode fazer tudo.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Politicas.Cliente, p =>
        p.RequireRole(nameof(Role.Cliente), nameof(Role.Administrador)))
    .AddPolicy(Politicas.Colaborador, p =>
        p.RequireRole(nameof(Role.Colaborador), nameof(Role.Administrador)))
    .AddPolicy(Politicas.Administrador, p =>
        p.RequireRole(nameof(Role.Administrador)));

var app = builder.Build();

// Aplica as migrações em falta (cria a base de dados e as tabelas se ainda não existirem).
// Usa-se Migrate() e não EnsureCreated(): o EnsureCreated cria as tabelas sem o histórico de
// migrações, e a partir daí o "dotnet ef database update" deixa de funcionar.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Cria as contas de admin que ainda não existirem (dados nos user-secrets, secção "Admins")
AdminSeeder.CriarAdminsSeNecessario(app.Services);

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
app.MapOportunidadeAPI();
app.MapOrcamentoEndpoints();
app.MapCategoriaAPI();
app.MapNotificacaoAPI();

app.Run();
