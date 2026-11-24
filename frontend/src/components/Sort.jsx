import { useState } from 'react';

export default function Sort({
  opcoes,
  ordenacao,
  onOrdenacaoChange,
}) {
  const [menuAberto, setMenuAberto] = useState(false);

  const opcaoSelecionada = opcoes.find(
    opcao => opcao.value === ordenacao
  );

  return (
    <div className="sort">
      <button
        className="wide secondary"
        onClick={() => setMenuAberto(!menuAberto)}
      >
        {opcaoSelecionada?.label || 'Ordenar por'}
        <i className="bi bi-caret-down"></i>
      </button>

      {menuAberto && (
        <div className="menu">
          {opcoes.map(opcao => (
            <button
              type="button"
              className="item"
              key={opcao.value}
              onClick={() => {
                onOrdenacaoChange(opcao.value);
                setMenuAberto(false);
              }}
            >
              {opcao.icon && <i className={`bi ${opcao.icon}`}></i>}
              {opcao.label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}