using System.Collections.Concurrent;

namespace GestaoServicosWeb.Aplicacao
{
    public class OrcamentoService
    {
        private readonly ConcurrentDictionary<Guid, Orcamento> orcamentos = new();

        public Orcamento Criar(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            var orcamento = new Orcamento(idPrestador, idServico, valorSemIVA, taxaIVA, dataValidade, duracaoDias);
            orcamentos[orcamento.Id] = orcamento;
            return orcamento;
        }

        public Orcamento[] Listar() => orcamentos.Values.ToArray();

        public Orcamento? Buscar(Guid id) =>
        orcamentos.TryGetValue(id, out var orcamento) ? orcamento : null;
    }
}
        