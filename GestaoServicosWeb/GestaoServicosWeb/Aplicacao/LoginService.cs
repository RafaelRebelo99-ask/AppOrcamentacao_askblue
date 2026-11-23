using GestaoServicosWeb.Contratos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GestaoServicosWeb.Aplicacao
{
    public class LoginService
    {
        private readonly UtilizadorService utilizadorService;
        private readonly JwtSettings jwtSettings;
        private readonly PasswordHasher<Utilizador> hasher = new();

        public LoginService(UtilizadorService utilizadorService, JwtSettings jwtSettings)
        {
            this.utilizadorService = utilizadorService;
            this.jwtSettings = jwtSettings;
        }

        public LoginResponse? Autenticar(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                Console.WriteLine("LOGIN: Email ou password vazios.");
                return null;
            }

            var utilizador = utilizadorService.BuscarPorEmail(dto.Email);

            if (utilizador == null)
            {
                Console.WriteLine($"LOGIN: Utilizador não encontrado: {dto.Email}");
                return null;
            }

            Console.WriteLine($"LOGIN: Utilizador encontrado: {utilizador.Email}");
            Console.WriteLine($"LOGIN: Role: {utilizador.Role}");
            Console.WriteLine($"LOGIN: Ativo: {utilizador.Ativo}");
            Console.WriteLine($"LOGIN: Hash length: {utilizador.PasswordHash?.Length}");

            var resultado = hasher.VerifyHashedPassword(
                utilizador,
                utilizador.PasswordHash,
                dto.Password);

            Console.WriteLine($"LOGIN: Resultado password: {resultado}");

            if (resultado == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("LOGIN: Password incorreta.");
                return null;
            }

            if (resultado == PasswordVerificationResult.Failed)
                return null;

            var agora = DateTime.UtcNow;
            var expiracao = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
                new Claim(ClaimTypes.Name, utilizador.Nome),
                new Claim(ClaimTypes.Email, utilizador.Email),
                new Claim(ClaimTypes.Role, utilizador.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtSettings.GetValidatedSigningKey()),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                notBefore: agora,
                expires: expiracao.UtcDateTime,
                signingCredentials: credentials);

            return new LoginResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                "Bearer",
                expiracao,
                new UtilizadorDto(utilizador.Id, utilizador.Nome, utilizador.Email,
                    utilizador.Role));
        }
    }
}