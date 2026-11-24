

export default function OportunidadeFilters({
  filtros,
  estadoSelecionado,
  contagens,
  onEstadoChange,
}) {
  return (
    <div className="row align-center scroll">
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