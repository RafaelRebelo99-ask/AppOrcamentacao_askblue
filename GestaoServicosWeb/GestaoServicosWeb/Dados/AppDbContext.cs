using GestaoServicosWeb;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Dados
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options)
    {
        public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
        public DbSet<Empresa> Empresas => Set<Empresa>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Oportunidade> Oportunidades => Set<Oportunidade>();
        public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
        public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Empresa =====
            modelBuilder.Entity<Empresa>(e =>
            {
                e.ToTable("Empresas");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();

                e.Property(x => x.Nome)
                    .HasMaxLength(150)
                    .IsRequired();

                e.Property(x => x.NIF)
                    .HasField("_nif")
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasMaxLength(20)
                    .IsRequired();

                e.HasIndex(x => x.NIF).IsUnique();

                e.Property(x => x.Email)
                    .HasMaxLength(254)
                    .IsRequired();

                e.Property(x => x.Telefone)
                    .HasMaxLength(30);

                e.Property(x => x.Ativa).IsRequired();
                e.Property(x => x.DataCriacao).IsRequired();

                e.HasData(
                    new
                    {
                        Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        Nome = "AskBlue",
                        NIF = "123456789",
                        Email = "empresa@askblue.com",
                        Telefone = (string?)null,
                        DataCriacao = new DateTime(2026, 1, 1),
                        Ativa = true
                    });
            });

            // ===== Utilizador =====
            modelBuilder.Entity<Utilizador>(u =>
            {
                u.ToTable("Utilizadores");
                u.HasKey(x => x.Id);
                u.Property(x => x.Id).ValueGeneratedNever();

                u.Property(x => x.Nome)
                    .HasMaxLength(100)
                    .IsRequired();

                u.Property(x => x.Email)
                    .HasMaxLength(254)
                    .IsRequired();

                u.HasIndex(x => x.Email).IsUnique();

                u.Property(x => x.PasswordHash)
                    .IsRequired();

                u.Property(x => x.Role)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                u.Property(x => x.Ativo).IsRequired();

                u.HasOne(x => x.Empresa)
                    .WithMany()
                    .HasForeignKey(x => x.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Categoria =====
            modelBuilder.Entity<Categoria>(c =>
            {
                c.ToTable("Categorias");
                c.HasKey(x => x.Id);
                c.Property(x => x.Id).ValueGeneratedNever();

                c.Property(x => x.Nome)
                    .HasMaxLength(100)
                    .IsRequired();

                c.HasIndex(x => x.Nome).IsUnique();

                c.HasData(
                    new
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Nome = "Monitorização e Suporte Aplicacional"
                    },
                    new
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Nome = "Quality Assurance"
                    },
                    new
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Nome = "Desenvolvimento Aplicacional"
                    },
                    new
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Nome = "Design de Serviços"
                    }
                );
            });

            // ===== Oportunidade =====
            modelBuilder.Entity<Oportunidade>(o =>
            {
                o.ToTable("Oportunidades");
                o.HasKey(x => x.Id);
                o.Property(x => x.Id).ValueGeneratedNever();

                o.Property(x => x.Titulo)
                    .HasMaxLength(200)
                    .IsRequired();

                o.Property(x => x.Descricao).IsRequired();

                o.Property(x => x.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                o.HasOne(x => x.Cliente)
                    .WithMany()
                    .HasForeignKey(x => x.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(x => x.Categoria)
                    .WithMany()
                    .HasForeignKey(x => x.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(x => x.Responsavel)
                    .WithMany()
                    .HasForeignKey(x => x.IdResponsavel)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasIndex(x => x.IdCliente);
            });

            // ===== Orcamento =====
            modelBuilder.Entity<Orcamento>(o =>
            {
                o.ToTable("Orcamentos");
                o.HasKey(x => x.Id);
                o.Property(x => x.Id).ValueGeneratedNever();

                o.Property(x => x.ValorSemIVA)
                    .HasPrecision(18, 2);

                o.Property(x => x.TaxaIVA)
                    .HasPrecision(5, 2);

                o.Property(x => x.Descricao)
                    .HasMaxLength(4000)
                    .IsRequired();

                o.Property(x => x.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                o.HasOne(x => x.Criador)
                    .WithMany()
                    .HasForeignKey(x => x.IdCriador)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(x => x.Oportunidade)
                    .WithMany()
                    .HasForeignKey(x => x.IdOportunidade)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasIndex(x => x.IdOportunidade)
                    .IsUnique()
                    .HasFilter("[Estado] = 'Aceite'");

                // propriedades derivadas
                o.Ignore(x => x.ValorTotal);
                o.Ignore(x => x.ValorIVA);
                o.Ignore(x => x.Expirado);
                o.Ignore(x => x.DataFim);
            });

            // ===== Notificacao =====
            modelBuilder.Entity<Notificacao>(n =>
            {
                n.ToTable("Notificacoes");
                n.HasKey(x => x.Id);
                n.Property(x => x.Id).ValueGeneratedNever();

                n.Property(x => x.Texto)
                    .HasMaxLength(500)
                    .IsRequired();

                n.Property(x => x.Destino)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                // Apagar um utilizador apaga as notificações dele
                n.HasOne(x => x.Utilizador)
                    .WithMany()
                    .HasForeignKey(x => x.IdUtilizador)
                    .OnDelete(DeleteBehavior.Cascade);

                n.HasIndex(x => new { x.IdUtilizador, x.DataCriacao });
            });
        }
    }
}
