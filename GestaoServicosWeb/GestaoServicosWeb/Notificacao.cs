namespace GestaoServicosWeb
{
    // Página que se abre ao clicar na notificação
    public enum DestinoNotificacao
    {
        Oportunidades,
        Orcamentos
    }

    // Aviso para um utilizador sobre algo que aconteceu numa oportunidade ou num orçamento
    public class Notificacao
    {
        public Guid Id { get; private set; }

        // Quem recebe
        public Guid IdUtilizador { get; private set; }
        internal Utilizador? Utilizador { get; private set; }

        public string Texto { get; private set; } = string.Empty;
        public DestinoNotificacao Destino { get; private set; }

        public bool Lida { get; private set; }
        public DateTime DataCriacao { get; private set; }

        private Notificacao() { }

        public Notificacao(Guid idUtilizador, string texto, DestinoNotificacao destino)
        {
            if (idUtilizador == Guid.Empty)
                throw new ArgumentException("O destinatário é obrigatório.");

            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("O texto da notificação é obrigatório.");

            Id = Guid.NewGuid();
            IdUtilizador = idUtilizador;
            Texto = texto.Trim();
            Destino = destino;
            DataCriacao = DateTime.UtcNow;
        }

        public void MarcarComoLida()
        {
            Lida = true;
        }
    }
}
