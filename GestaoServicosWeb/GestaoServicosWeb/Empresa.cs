namespace GestaoServicosWeb;

public class Empresa
{
    public Guid Id { get; private set; }

    private string _nome = string.Empty;
    public string Nome => _nome;

    private string _nif = string.Empty;
    public string NIF => _nif;

    private string _email = string.Empty;
    public string Email => _email;

    private string? _telefone;
    public string? Telefone => _telefone;

    public DateTime DataCriacao { get; private set; }

    public bool Ativa { get; private set; }

    private Empresa() { }

    public Empresa(
        string nome,
        string nif,
        string email,
        string? telefone = null)
    {
        Id = Guid.NewGuid();

        AlterarNome(nome);
        AlterarNif(nif);
        AlterarEmail(email);
        AlterarTelefone(telefone);

        DataCriacao = DateTime.UtcNow;
        Ativa = true;
    }

    public void AlterarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da empresa é obrigatório.");

        nome = nome.Trim();

        if (nome.Length > 150)
            throw new ArgumentException("O nome não pode exceder 150 caracteres.");

        _nome = nome;
    }

    public void AlterarNif(string nif)
    {
        if (string.IsNullOrWhiteSpace(nif))
            throw new ArgumentException("O NIF é obrigatório.");

        nif = nif.Trim();

        if (!NifValido(nif))
            throw new ArgumentException("O NIF introduzido é inválido.");

        _nif = nif;
    }

    public void AlterarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email é obrigatório.");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 254 || !email.Contains('@'))
            throw new ArgumentException("O email indicado é inválido.");

        _email = email;
    }

    public void AlterarTelefone(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            _telefone = null;
            return;
        }

        telefone = telefone.Trim();
        _telefone = telefone;
    }

    public void Ativar()
    {
        Ativa = true;
    }

    public void Desativar()
    {
        Ativa = false;
    }

    private static bool NifValido(string nif)
    {
        int soma = 0;
        for (int i = 0; i < 8; i++)
            soma += (nif[i] - '0') * (9 - i);

        int resto = soma % 11;
        int controlo = resto < 2 ? 0 : 11 - resto;

        return controlo == (nif[8] - '0');
    }
}