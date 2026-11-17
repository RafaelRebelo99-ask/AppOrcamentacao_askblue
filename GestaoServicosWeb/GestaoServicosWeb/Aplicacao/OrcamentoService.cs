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

        public Orcamento? Editar(Guid id, Guid idPrestador, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            if (!orcamentos.TryGetValue(id, out var orcamento))
                return null;

            if (orcamento.IdPrestador != idPrestador)
                throw new UnauthorizedAccessException("Só o prestador que criou o orçamento o pode editar.");

            orcamento.Atualizar(valorSemIVA, taxaIVA, dataValidade, duracaoDias);
            return orcamento;
        }
    }
}
        