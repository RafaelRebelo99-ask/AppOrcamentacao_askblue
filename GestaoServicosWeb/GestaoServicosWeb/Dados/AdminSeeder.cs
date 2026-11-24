using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Dados
{

    /**
     * dotnet user-secrets set "Admins:0:Nome" "Administrador"
dotnet user-secrets set "Admins:0:Email" "admin@askblue.pt"
dotnet user-secrets set "Admins:0:Password" "your-development-password"
    */
    public static class AdminSeeder
    {
        private sealed record AdminConfig(
            string? Nome,
            string? Email,
            string? Password);

        public static void CriarAdminsSeNecessario(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(AdminSeeder));

            var admins = config.GetSection("Admins").Get<List<AdminConfig>>() ?? [];

            if (admins.Count == 0)
            {
                logger.LogWarning("A secção Admins não está configurada.");
                return;
            }

            var empresa = db.Empresas.SingleOrDefault(e => e.Nome == "AskBlue");

            if (empresa == null || !empresa.Ativa)
                throw new InvalidOperationException(
                    "A empresa AskBlue não existe ou está inativa.");

            var hasher = new PasswordHasher<Utilizador>();

            foreach (var a in admins)
            {
                if (string.IsNullOrWhiteSpace(a.Email) ||
                    string.IsNullOrWhiteSpace(a.Password))
                {
                    logger.LogWarning("Admin ignorado: faltam Email ou Password.");
                    continue;
                }

                var email = a.Email.Trim().ToLowerInvariant();

                if (db.Utilizadores.Any(u => u.Email == email))
                    continue;

                if (a.Password.Length < 8)
                    throw new InvalidOperationException(
                        $"A password do admin {email} deve ter pelo menos 8 caracteres.");

                var hash = hasher.HashPassword(null!, a.Password);

                var admin = new Utilizador(
                    a.Nome ?? "Administrador",
                    email,
                    hash,
                    Role.Administrador,
                    empresa);

                db.Utilizadores.Add(admin);

                logger.LogInformation(
                    "Conta de administrador criada: {Email}", email);
            }

            db.SaveChanges();
        }
    }
}