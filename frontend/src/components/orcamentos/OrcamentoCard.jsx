function formatarData(data) {
  if (!data) return '—';

  return new Date(data).toLocaleDateString('pt-PT');
}

function formatarValor(valor) {
  return Number(valor).toLocaleString('pt-PT', {
    style: 'currency',
    currency: 'EUR',
  });
}

export default function OrcamentoCard({ orcamento, onEliminar }) {
  return (
    <div className="card column">
      <div className="row space-between align-center mb-1">
        <div className="id">
          {orcamento.id.slice(0, 8)}
        </div>

        <div className="estado">
          {orcamento.estado}
        </div>
      </div>

      <div className="column">
        <h2 className="mb-05">
          {orcamento.descricao}
        </h2>

        <p>
          Oportunidade: {orcamento.idOportunidade}
        </p>

        <p>
          Criada em: {formatarData(orcamento.dataCriacao)}
        </p>

        <p>
          Validade: {formatarData(orcamento.dataValidade)}
        </p>

        <p>
          Valor sem IVA: {formatarValor(orcamento.valorSemIVA)}
        </p>

        <p>
          IVA ({orcamento.taxaIVA}%):{' '}
          {formatarValor(orcamento.valorTotal - orcamento.valorSemIVA)}
        </p>

        <strong>
          Total: {formatarValor(orcamento.valorTotal)}
        </strong>

        {orcamento.expirado && (
          <p className="danger">Proposta expirada</p>
        )}
      </div>

      {orcamento.estado === 'Rascunho' && (
        <div className="row">
          <button
            className="secondary danger"
            onClick={() => onEliminar(orcamento)}
          >
            Eliminar rascunho
          </button>
        </div>
      )}
    </div>
  );
}