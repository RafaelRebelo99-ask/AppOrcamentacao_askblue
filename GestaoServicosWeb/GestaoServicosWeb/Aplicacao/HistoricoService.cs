using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class HistoricoService
    {
        private readonly AppDbContext db;

        public HistoricoService(AppDbContext db)
        {
            this.db = db;
        }

        // Lista os registos mais recentes (com filtro opcional por categoria)
        public List<RegistoHistorico> Listar(CategoriaHistorico? categoria = null, int limite = 200)
        {
            var query = db.Historico.AsNoTracking();

            if (categoria.HasValue)
                query = query.Where(h => h.Categoria == categoria.Value);

            return query
                .OrderByDescending(h => h.DataHora)
                .Take(limite)
                .ToList();
        }

        // Grava um registo de atividade sem nunca deixar um erro de log quebrar a ação principal
        public void Registar(
            Guid? idUtilizador,
            string nomeUtilizador,
            CategoriaHistorico categoria,
            string acao,
            string descricao)
        {
            try
            {
                var registo = new RegistoHistorico(
                    idUtilizador,
                    Cortar(nomeUtilizador, 100),
                    categoria,
                    Cortar(acao, 80),
                    Cortar(descricao, 500));

                db.Historico.Add(registo);
                db.SaveChanges();
            }
            catch
            {
                // Ignora falhas de log para não interromper operações da plataforma
            }
        }

        private static string Cortar(string texto, int max)
        {
            var limpo = texto.Trim();
            return limpo.Length <= max ? limpo : limpo[..max];
        }
    }
}
