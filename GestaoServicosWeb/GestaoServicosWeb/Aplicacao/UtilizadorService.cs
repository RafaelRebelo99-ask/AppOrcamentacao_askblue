using GestaoServicosWeb.Contratos;
using Microsoft.AspNetCore.Identity;

namespace GestaoServicosWeb.Aplicacao
{

    public class UtilizadorService
    {
        // Armazenamento temporário: não é base de dados. Perde-se ao reiniciar a API.
        private readonly List<Utilizador> utilizadores = new();
        private readonly PasswordHasher<Utilizador> hasher = new();

        // Vários pedidos podem chegar ao mesmo tempo; o lock evita que mexam na lista em simultâneo.
        private readonly object bloqueio = new();

        public List<Utilizador> Listar(Role? role = null)
        {
            lock (bloqueio)
            {
                return role == null
                    ? utilizadores.ToList()
                    : utilizadores.Where(u => u.Role == role).ToList();
            }
        }

        public Utilizador? Buscar(Guid id)
        {
            lock (bloqueio)
            {
                return utilizadores.FirstOrDefault(u => u.Id == id);
            }
        }

        public Utilizador Criar(CriarUtilizadorDto dto)
        {
            ValidarPassword(dto.Password);

            if (dto.Role == Role.Empresa)
                throw new ArgumentException("Não é possível registar-se como Empresa.");

            lock (bloqueio)
            {
                if (EmailEmUso(dto.Email))
                    throw new InvalidOperationException("Já existe um utilizador com esse email.");

                if (utilizadores.Any(u => u.NIF == dto.Nif?.Trim()))
                    throw new InvalidOperationException("Já existe um utilizador com esse NIF.");

                string hash = hasher.HashPassword(null!, dto.Password);

                // O construtor valida nome, email, NIF e telefone
                var utilizador = new Utilizador(dto.Nome, dto.Email, hash, dto.Nif, dto.Telefone, dto.Role);

                utilizadores.Add(utilizador);
                return utilizador;
            }
        }

        // Devolve null se o utilizador não existir
        public Utilizador? Atualizar(Guid id, AtualizarUtilizadorDto dto)
        {
            lock (bloqueio)
            {
                var utilizador = utilizadores.FirstOrDefault(u => u.Id == id);
                if (utilizador == null)
                    return null;

                if (EmailEmUso(dto.Email, ignorarId: id))
                    throw new InvalidOperationException("Esse email já está a ser usado.");

                // Guardar os valores antigos: se um campo falhar, os outros não ficam alterados a meio
                var (nomeAntigo, emailAntigo, telefoneAntigo) =
                    (utilizador.Nome, utilizador.Email, utilizador.Telefone);

                try
                {
                    utilizador.Nome = dto.Nome;           // os setters validam
                    utilizador.Email = dto.Email;
                    utilizador.Telefone = dto.Telefone;
                }
                catch (ArgumentException)
                {
                    utilizador.Nome = nomeAntigo;
                    utilizador.Email = emailAntigo;
                    utilizador.Telefone = telefoneAntigo;
                    throw;
                }

                return utilizador;
            }
        }

        // Devolve false se o utilizador não existir
        public bool AlterarPassword(Guid id, AlterarPasswordDto dto)
        {
            lock (bloqueio)
            {
                var utilizador = utilizadores.FirstOrDefault(u => u.Id == id);
                if (utilizador == null)
                    return false;

                var resultado = hasher.VerifyHashedPassword(utilizador, utilizador.PasswordHash, dto.PasswordAtual);
                if (resultado == PasswordVerificationResult.Failed)
                    throw new ArgumentException("A password atual está errada.");

                ValidarPassword(dto.PasswordNova);

                utilizador.AlterarPassword(hasher.HashPassword(utilizador, dto.PasswordNova));
                return true;
            }
        }

        // Devolve false se o utilizador não existir
        public bool Apagar(Guid id)
        {
            lock (bloqueio)
            {
                return utilizadores.RemoveAll(u => u.Id == id) > 0;
            }
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
            return utilizadores.Any(u => u.Email == normalizado && u.Id != ignorarId);
        }
    }
}
