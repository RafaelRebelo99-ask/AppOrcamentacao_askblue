import useNotificacoes from '../controllers/useNotificacoes';
import Icon from './Icon';
import { Bell } from '@coolicons/react'
import Tooltip from './Tooltip';

// Sino com o número de notificações por ler e a lista das mais recentes
export default function Notificacoes() {
  const {
    itens,
    naoLidas,
    aberto,
    contentor,
    alternar,
    handleAbrir,
    handleMarcarTodas,
  } = useNotificacoes();

  const descricao = naoLidas === 0
    ? 'Notificações'
    : `Notificações (${naoLidas} por ler)`;

  return (
    <div className="notificacoes" ref={contentor}>
      <Tooltip text='Notificações' position='bottom'>
        <button
          type="button"
          className="sino"
          onClick={alternar}
          title={descricao}
          aria-label={descricao}
          aria-expanded={aberto}
        >
          <Icon icon={Bell} />

          {naoLidas > 0 && (
            <span className="sino-total" aria-hidden="true">
              {naoLidas > 99 ? '99+' : naoLidas}
            </span>
          )}
        </button>
      </Tooltip>

      {aberto && (
        <div className="menu pad-1 right notificacoes-lista">
          <div className="row space-between align-center">
            <h2>Notificações</h2>

            <button
              type="button"
              className="secondary"
              onClick={handleMarcarTodas}
              disabled={naoLidas === 0}
            >
              Marcar como lidas
            </button>
          </div>

          <hr />

          {itens.length === 0 && <p className="weak">Não tens notificações.</p>}

          {itens.map(notificacao => (
            <button
              type="button"
              key={notificacao.id}
              className={'item notificacao' + (notificacao.lida ? '' : ' por-ler')}
              onClick={() => handleAbrir(notificacao)}
            >
              <span>{notificacao.texto}</span>
              <span className="weak">{notificacao.quando}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
