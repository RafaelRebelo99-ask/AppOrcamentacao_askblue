using Microsoft.AspNetCore.Identity;

namespace GestaoServicosWeb.Dados
{
    // ===== Cria as contas de administrador (Empresa) ao arrancar a aplicação =====
    // Os dados vêm da configuração (user-secrets em desenvolvimento, variáveis de ambiente em produção),
    // numa lista "Admins" (0, 1, 2, ...):
    //   dotnet user-secrets set "Admins:0:Nome" "Administrador"
    //   dotnet user-secrets set "Admins:0:Email" "admin@askblue.pt"
    //   dotnet user-secrets set "Admins:0:Password" "..."
    //   dotnet user-secrets set "Admins:0:Nif" "123456789"
    // Cada admin só é criado se ainda não existir um utilizador com esse email.
    public static class AdminSeeder
    {
        private sealed record AdminConfig(string? Nome, string? Email, string? Password, string? Nif);

        public static void CriarAdminsSeNecessario(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AdminSeeder));

            var admins = config.GetSection("Admins").Get<List<AdminConfig>>() ?? [];

            if (admins.Count == 0)
            {
                if (!db.Utilizadores.Any(u => u.Role == Role.Empresa))
                    logger.LogWarning("Não existe nenhum admin e a secção 'Admins' não está configurada. Ver comentário em AdminSeeder.cs.");
                return;
            }

            var hasher = new PasswordHasher<Utilizador>();

            foreach (var a in admins)
            {
                if (string.IsNullOrWhiteSpace(a.Email) || string.IsNullOrWhiteSpace(a.Password) || string.IsNullOrWhiteSpace(a.Nif))
                {
                    logger.LogWarning("Admin ignorado: faltam Email, Password ou Nif na configuração.");
                    continue;
                }

                string email = a.Email.Trim().ToLower();
                if (db.Utilizadores.Any(u => u.Email == email))
                    continue;

                if (a.Password.Length < 8)
                    throw new InvalidOperationException($"A password do admin {email} deve ter pelo menos 8 caracteres.");

                string hash = hasher.HashPassword(null!, a.Password);
                var admin = new Utilizador(a.Nome ?? "Administrador", email, hash, a.Nif, null, Role.Empresa);

                db.Utilizadores.Add(admin);
                logger.LogInformation("Conta de admin criada ({Email}).", admin.Email);
            }

            db.SaveChanges();
        }
    }
}
