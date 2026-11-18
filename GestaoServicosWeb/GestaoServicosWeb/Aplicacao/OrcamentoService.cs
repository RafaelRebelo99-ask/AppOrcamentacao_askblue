using GestaoServicosWeb.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoServicosWeb.Aplicacao
{
    public class OrcamentoService
    {
        private readonly AppDbContext db;

        public OrcamentoService(AppDbContext db)
        {
            this.db = db;
        }

        public Orcamento Criar(Guid idPrestador, Guid idServico, string descricao, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            var orcamento = new Orcamento(idPrestador, idServico, descricao, valorSemIVA, taxaIVA, dataValidade, duracaoDias);

            // As chaves estrangeiras têm de apontar para registos que existam
            if (!db.Utilizadores.Any(u => u.Id == idPrestador))
                throw new ArgumentException("O prestador indicado não existe.");

            if (!db.Servicos.Any(s => s.Id == idServico))
                throw new ArgumentException("O serviço indicado não existe.");

            db.Orcamentos.Add(orcamento);
            db.SaveChanges();
            return orcamento;
        }

        public Orcamento[] Listar() => db.Orcamentos.AsNoTracking().ToArray();

        public Orcamento? Buscar(Guid id) =>
            db.Orcamentos.FirstOrDefault(o => o.Id == id);

        public Orcamento? Editar(Guid id, Guid idPrestador,string descricao, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            var orcamento = db.Orcamentos.FirstOrDefault(o => o.Id == id);
            if (orcamento == null)
                return null;

            if (orcamento.IdPrestador != idPrestador)
                throw new UnauthorizedAccessException("Só o prestador que criou o orçamento o pode editar.");

            orcamento.Atualizar(descricao, valorSemIVA, taxaIVA, dataValidade, duracaoDias);
            db.SaveChanges();
            return orcamento;
        }
    }
}
