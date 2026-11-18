using System.Text;

namespace GestaoServicosWeb.Aplicacao
{
    public sealed class JwtSettings
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string SigningKey { get; init; } = string.Empty;
        public int ExpirationMinutes { get; init; } = 60;

        public byte[] GetValidatedSigningKey()
        {
            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException("A configuração Jwt:Issuer é obrigatória.");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException("A configuração Jwt:Audience é obrigatória.");

            if (Encoding.UTF8.GetByteCount(SigningKey) < 32)
                throw new InvalidOperationException("Jwt:SigningKey deve ter pelo menos 32 bytes.");

            if (ExpirationMinutes <= 0)
                throw new InvalidOperationException("Jwt:ExpirationMinutes deve ser maior que zero.");

            return Encoding.UTF8.GetBytes(SigningKey);
        }
    }
}
