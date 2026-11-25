namespace GestaoServicosWeb.Contratos
{
    public record NotificacaoDto(
        Guid Id,
        string Texto,
        DestinoNotificacao Destino,
        bool Lida,
        DateTime DataCriacao
    )
    {
        // A base de dados guarda a hora em UTC mas não o indica; assim segue com "Z" no JSON
        // e o browser mostra a hora certa.
        public static NotificacaoDto De(Notificacao n) =>
            new(n.Id, n.Texto, n.Destino, n.Lida,
                DateTime.SpecifyKind(n.DataCriacao, DateTimeKind.Utc));
    }

    // O total de não lidas pode ser maior do que o número de itens devolvidos
    public record ListaNotificacoesDto(
        int NaoLidas,
        IEnumerable<NotificacaoDto> Itens
    );
}
