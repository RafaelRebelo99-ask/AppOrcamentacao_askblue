using System.Globalization;
using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class NotificacaoService
    {
        private const int MaximoListado = 20;
        private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-PT");

        private readonly AppDbContext db;

        public NotificacaoService(AppDbContext db)
        {
            this.db = db;
        }

        // ===== Leitura: cada utilizador só vê as suas =====

        public Notificacao[] Listar(Guid idUtilizador) =>
            db.Notificacoes
                .AsNoTracking()
                .Where(n => n.IdUtilizador == idUtilizador)
                .OrderByDescending(n => n.DataCriacao)
                .Take(MaximoListado)
                .ToArray();

        public int ContarNaoLidas(Guid idUtilizador) =>
            db.Notificacoes.Count(n => n.IdUtilizador == idUtilizador && !n.Lida);

        // Devolve false se não existir (ou for de outro utilizador)
        public bool MarcarComoLida(Guid id, Guid idUtilizador)
        {
            var notificacao = db.Notificacoes
                .FirstOrDefault(n => n.Id == id && n.IdUtilizador == idUtilizador);

            if (notificacao == null)
                return false;

            notificacao.MarcarComoLida();
            db.SaveChanges();

            return true;
        }

        public void MarcarTodasComoLidas(Guid idUtilizador)
        {
            var naoLidas = db.Notificacoes
                .Where(n => n.IdUtilizador == idUtilizador && !n.Lida)
                .ToList();

            foreach (var notificacao in naoLidas)
                notificacao.MarcarComoLida();

            db.SaveChanges();
        }

        // ===== Criação =====
        // Estes métodos NÃO gravam: quem os chama faz o SaveChanges juntamente com a mudança de estado.
        // Assim nunca fica uma coisa sem a outra.

        // Oportunidade registada ou com novo estado -> Cliente
        public void OportunidadeAlterada(Oportunidade oportunidade, Guid idAutor)
        {
            string titulo = oportunidade.Titulo;

            string texto = oportunidade.Estado switch
            {
                EstadoOportunidade.Registada => $"A oportunidade \"{titulo}\" foi registada.",
                EstadoOportunidade.EmAnalise => $"A oportunidade \"{titulo}\" está em análise.",
                EstadoOportunidade.EmPreparacao => $"A proposta para \"{titulo}\" está em preparação.",
                EstadoOportunidade.Perdida => $"A oportunidade \"{titulo}\" foi encerrada.",
                EstadoOportunidade.Cancelada => $"A oportunidade \"{titulo}\" foi cancelada.",
                _ => throw new ArgumentException(
                    "Este estado da oportunidade não gera notificação.")
            };

            Adicionar([oportunidade.IdCliente], idAutor, texto, DestinoNotificacao.Oportunidades);
        }

        // Proposta enviada -> Cliente
        public void PropostaEnviada(Orcamento orcamento, Oportunidade oportunidade, Guid idAutor)
        {
            string valor = orcamento.ValorTotal.ToString("C2", Pt);

            Adicionar([oportunidade.IdCliente], idAutor,
                $"Recebeu uma proposta de {valor} para \"{oportunidade.Titulo}\".",
                DestinoNotificacao.Orcamentos);
        }

        // Proposta aceite -> criador, responsável e administradores.
        // Os criadores das outras propostas são avisados de que foram recusadas.
        public void PropostaAceite(
            Orcamento orcamento,
            Oportunidade oportunidade,
            IEnumerable<Orcamento> recusadas,
            Guid idCliente)
        {
            var interessados = Interessados(orcamento, oportunidade);

            Adicionar(interessados, idCliente,
                $"{NomeDe(idCliente)} aceitou a proposta {Numero(orcamento)} para \"{oportunidade.Titulo}\".",
                DestinoNotificacao.Orcamentos);

            // Quem já recebeu a de cima não precisa desta
            foreach (var recusada in recusadas.Where(r => !interessados.Contains(r.IdCriador)))
            {
                Adicionar([recusada.IdCriador], idCliente,
                    $"A proposta {Numero(recusada)} para \"{oportunidade.Titulo}\" foi recusada: o cliente aceitou outra.",
                    DestinoNotificacao.Orcamentos);
            }
        }

        // Proposta recusada -> criador, responsável e administradores
        public void PropostaRecusada(Orcamento orcamento, Oportunidade oportunidade, Guid idCliente)
        {
            Adicionar(Interessados(orcamento, oportunidade), idCliente,
                $"{NomeDe(idCliente)} recusou a proposta {Numero(orcamento)} para \"{oportunidade.Titulo}\".",
                DestinoNotificacao.Orcamentos);
        }

        // ===== Auxiliares =====

        // HashSet: ninguém é avisado duas vezes (ex.: um administrador que também é o criador)
        private HashSet<Guid> Interessados(Orcamento orcamento, Oportunidade oportunidade)
        {
            var ids = db.Utilizadores
                .Where(u => u.Role == Role.Administrador && u.Ativo)
                .Select(u => u.Id)
                .ToHashSet();

            ids.Add(orcamento.IdCriador);
            ids.Add(oportunidade.IdResponsavel);

            return ids;
        }

        private string NomeDe(Guid idUtilizador) =>
            db.Utilizadores
                .Where(u => u.Id == idUtilizador)
                .Select(u => u.Nome)
                .FirstOrDefault() ?? "O cliente";

        // O mesmo número que aparece no cartão e no PDF
        private static string Numero(Orcamento orcamento) =>
            orcamento.Id.ToString("N")[..8].ToUpperInvariant();

        // Quem faz a ação não recebe notificação dela
        private void Adicionar(
            IEnumerable<Guid> destinatarios,
            Guid idAutor,
            string texto,
            DestinoNotificacao destino)
        {
            foreach (var id in destinatarios.Where(id => id != idAutor))
                db.Notificacoes.Add(new Notificacao(id, texto, destino));
        }
    }
}
