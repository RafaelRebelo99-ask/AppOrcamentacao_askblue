using GestaoServicosWeb.Contratos;
using GestaoServicosWeb.Dados;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{

    public class UtilizadorService
    {
        private readonly AppDbContext db;
        private readonly PasswordHasher<Utilizador> hasher = new();

        public UtilizadorService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Utilizador> Listar(Role? role = null)
        {
            var query = db.Utilizadores.AsNoTracking();

            if (role != null)
                query = query.Where(u => u.Role == role);

            return query.ToList();
        }

        public Utilizador? Buscar(Guid id) =>
            db.Utilizadores.FirstOrDefault(u => u.Id == id);

        public Utilizador? BuscarPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            // Os emails são guardados sempre em minúsculas (ver Utilizador.Email)
            string emailNormalizado = email.Trim().ToLower();

            return db.Utilizadores.FirstOrDefault(u => u.Email == emailNormalizado);
        }

        public Utilizador Criar(CriarUtilizadorDto dto)
        {
            ValidarPassword(dto.Password);

            if (EmailEmUso(dto.Email))
                throw new InvalidOperationException("Já existe um utilizador com esse email.");

            string hash = hasher.HashPassword(null!, dto.Password);

            // O construtor valida nome, email, NIF e telefone
            var utilizador = new Utilizador(dto.Nome, dto.Email, hash, dto.Role);

            db.Utilizadores.Add(utilizador);
            db.SaveChanges();
            return utilizador;
        }

        // Um colaborador fica sempre associado à AskBlue
        public Utilizador CriarColaborador(CriarColaboradorDto dto)
        {
            ValidarPassword(dto.Password);

            if (EmailEmUso(dto.Email))
                throw new InvalidOperationException("Já existe um utilizador com esse email.");

            var empresa = db.Empresas.SingleOrDefault(e => e.Nome == "AskBlue" && e.Ativa)
                ?? throw new InvalidOperationException("A empresa AskBlue não existe ou está inativa.");

            string hash = hasher.HashPassword(null!, dto.Password);

            var colaborador = new Utilizador(dto.Nome, dto.Email, hash, Role.Colaborador, empresa);

            db.Utilizadores.Add(colaborador);
            db.SaveChanges();
            return colaborador;
        }

        // Devolve null se o utilizador não existir
        public Utilizador? Atualizar(Guid id, AtualizarUtilizadorDto dto)
        {
            var utilizador = db.Utilizadores.FirstOrDefault(u => u.Id == id);
            if (utilizador == null)
                return null;

            if (EmailEmUso(dto.Email, ignorarId: id))
                throw new InvalidOperationException("Esse email já está a ser usado.");

            // Os setters validam. Se algum falhar, a exceção sai antes do SaveChanges
            // e nada é gravado na base de dados.
            utilizador.Nome = dto.Nome;
            utilizador.Email = dto.Email;

            db.SaveChanges();
            return utilizador;
        }

        // Devolve false se o utilizador não existir
        public bool AlterarPassword(Guid id, AlterarPasswordDto dto)
        {
            var utilizador = db.Utilizadores.FirstOrDefault(u => u.Id == id);
            if (utilizador == null)
                return false;

            var resultado = hasher.VerifyHashedPassword(utilizador, utilizador.PasswordHash, dto.PasswordAtual);
            if (resultado == PasswordVerificationResult.Failed)
                throw new ArgumentException("A password atual está errada.");

            ValidarPassword(dto.PasswordNova);

            utilizador.AlterarPassword(hasher.HashPassword(utilizador, dto.PasswordNova));
            db.SaveChanges();
            return true;
        }

        // Devolve false se o utilizador não existir
        public bool Apagar(Guid id)
        {
            var utilizador = db.Utilizadores.FirstOrDefault(u => u.Id == id);
            if (utilizador == null)
                return false;

            if (utilizador.Role == Role.Administrador)
                throw new InvalidOperationException("Não é possível apagar um administrador.");

            if (db.Oportunidades.Any(o => o.IdCliente == id || o.IdResponsavel == id) ||
                db.Orcamentos.Any(o => o.IdCriador == id))
                throw new InvalidOperationException("Não é possível apagar um utilizador com oportunidades ou orçamentos associados.");

            db.Utilizadores.Remove(utilizador);
            db.SaveChanges();
            return true;
        }

        // ===== Auxiliares =====

        private static void ValidarPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new ArgumentException("A password deve ter pelo menos 8 caracteres.");
        }

        private bool EmailEmUso(string? email, Guid? ignorarId = null)
        {
            string normalizado = email?.Trim().ToLower() ?? "";
            return db.Utilizadores.Any(u => u.Email == normalizado && u.Id != ignorarId);
        }
    }
}
