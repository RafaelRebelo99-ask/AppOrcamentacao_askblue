using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Dados
{
    // Ligação entre as entidades e a base de dados SQL Server (via Entity Framework Core)
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Servico> Servicos => Set<Servico>();
        public DbSet<Orcamento> Orcamentos => Set<Orcamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== Utilizador =====
            modelBuilder.Entity<Utilizador>(u =>
            {
                u.ToTable("Utilizadores");
                u.HasKey(x => x.Id);
                u.Property(x => x.Id).ValueGeneratedNever();

                // Ao ler da BD, o EF escreve diretamente nos campos privados (não volta a correr as validações)
                u.Property(x => x.Nome).HasField("_nome").HasMaxLength(100).IsRequired();
                u.Property(x => x.Email).HasField("_email").HasMaxLength(256).IsRequired();
                u.Property(x => x.NIF).HasField("_nif").HasMaxLength(9).IsFixedLength().IsRequired();
                u.Property(x => x.Telefone).HasField("_telefone").HasMaxLength(9);
                u.Property(x => x.PasswordHash).IsRequired();
                u.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);

                u.HasIndex(x => x.Email).IsUnique();
                u.HasIndex(x => x.NIF).IsUnique();
            });

            // ===== Categoria =====
            modelBuilder.Entity<Categoria>(c =>
            {
                c.ToTable("Categorias");
                c.HasKey(x => x.Id);
                c.Property(x => x.Id).ValueGeneratedNever();
                c.Property(x => x.Nome).HasMaxLength(100).IsRequired();
                c.HasIndex(x => x.Nome).IsUnique();

                // Categorias iniciais (as mesmas que existiam em memória)
                c.HasData(
                    new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nome = "Pintura e Remodelação" },
                    new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Nome = "Eletricidade" },
                    new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nome = "Canalização" },
                    new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Nome = "Carpintaria" },
                    new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Nome = "Limpeza e Manutenção" });
            });

            // ===== Servico =====
            modelBuilder.Entity<Servico>(s =>
            {
                s.ToTable("Servicos");
                s.HasKey(x => x.Id);
                s.Property(x => x.Id).ValueGeneratedNever();
                s.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
                s.Property(x => x.Descricao).IsRequired();
                s.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);

                // Restrict: não se pode apagar um cliente/categoria que tenha serviços
                s.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);
                s.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Orcamento =====
            modelBuilder.Entity<Orcamento>(o =>
            {
                o.ToTable("Orcamentos");
                o.HasKey(x => x.Id);
                o.Property(x => x.Id).ValueGeneratedNever();
                o.Property(x => x.ValorSemIVA).HasPrecision(18, 2);
                o.Property(x => x.TaxaIVA).HasPrecision(5, 2);
                o.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);

                // Calculados em C#, não são guardados
                o.Ignore(x => x.ValorTotal);
                o.Ignore(x => x.Expirado);

                o.HasOne(x => x.Prestador).WithMany().HasForeignKey(x => x.IdPrestador)
                    .OnDelete(DeleteBehavior.Restrict);
                o.HasOne(x => x.Servico).WithMany().HasForeignKey(x => x.IdServico)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
