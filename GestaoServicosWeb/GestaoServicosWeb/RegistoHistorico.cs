namespace GestaoServicosWeb
{
    public enum CategoriaHistorico
    {
        Autenticacao,
        Oportunidades,
        Orcamentos,
        Utilizadores,
        Categorias
    }

    // Registo de auditoria para ações dos utilizadores na plataforma (visível apenas para Admin)
    public class RegistoHistorico
    {
        public Guid Id { get; private set; }
        public Guid? IdUtilizador { get; private set; }
        public string NomeUtilizador { get; private set; } = string.Empty;
        public CategoriaHistorico Categoria { get; private set; }
        public string Acao { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public DateTime DataHora { get; private set; }

        private RegistoHistorico() { }

        public RegistoHistorico(
            Guid? idUtilizador,
            string nomeUtilizador,
            CategoriaHistorico categoria,
            string acao,
            string descricao)
        {
            if (string.IsNullOrWhiteSpace(nomeUtilizador))
                throw new ArgumentException("O nome do utilizador é obrigatório.");

            if (string.IsNullOrWhiteSpace(acao))
                throw new ArgumentException("A ação é obrigatória.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição é obrigatória.");

            Id = Guid.NewGuid();
            IdUtilizador = idUtilizador;
            NomeUtilizador = nomeUtilizador.Trim();
            Categoria = categoria;
            Acao = acao.Trim();
            Descricao = descricao.Trim();
            DataHora = DateTime.UtcNow;
        }
    }
}
