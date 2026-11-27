import { useState } from 'react';
import { ChevronDown, ChevronUp } from '@coolicons/react';
import Icon from './Icon';

export default function Sort({
  opcoes,
  ordenacao,
  onOrdenacaoChange,
}) {
  const [menuAberto, setMenuAberto] = useState(false);

  const opcaoSelecionada = opcoes.find(
    opcao => opcao.value === ordenacao
  );

  const icon = menuAberto ? ChevronUp : ChevronDown;

  return (
    <div className="sort">
      <button
        className="wide secondary"
        onClick={() => setMenuAberto(!menuAberto)}
      >
        {opcaoSelecionada?.label || 'Ordenar por'}
        <Icon icon={icon}/>
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
              {opcao.icon && <Icon icon={opcao.icon}/>}
              {opcao.label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}