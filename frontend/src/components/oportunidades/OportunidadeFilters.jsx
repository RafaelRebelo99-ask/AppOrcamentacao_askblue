const filtros = [
  { label: 'Todas', value: 'Todas' },
  { label: 'Registadas', value: 'Registada' },
  { label: 'Em análise', value: 'EmAnalise' },
  { label: 'Em preparação', value: 'EmPreparacao' },
  { label: 'Proposta enviada', value: 'PropostaEnviada' },
  { label: 'Ganhas', value: 'Ganha' },
  { label: 'Perdidas', value: 'Perdida' },
  { label: 'Canceladas', value: 'Cancelada' },
];

export default function OportunidadeFilters({
  estadoSelecionado,
  contagens,
  onEstadoChange,
}) {
  return (
    <div className="row mb-1">
      {filtros.map((filtro) => (
        <button
          key={filtro.value}
          className={
            'wide-left secondary' +
            (estadoSelecionado === filtro.value ? ' selected' : '')
          }
          onClick={() => onEstadoChange(filtro.value)}
        >
          {filtro.label}
          <div className="counter">
            {contagens[filtro.value] ?? 0}
          </div>
        </button>
      ))}
    </div>
  );
}