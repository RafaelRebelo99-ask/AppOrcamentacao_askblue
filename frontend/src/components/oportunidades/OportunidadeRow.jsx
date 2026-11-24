
import { useState } from 'react';
import Aviso from '../Aviso';

// Estados finais: já não há nada a fazer com a oportunidade
const ESTADOS_FINAIS = ['Ganha', 'Perdida', 'Cancelada'];

export default function OportunidadeRow({
  oportunidade,
  podeGerir,
  emCurso,
  onIniciarAnalise,
  onIniciarPreparacao,
  onPerdida,
  onCancelar,
}) {
  const [menuAberto, setMenuAberto] = useState(false);
  const [acaoPendente, setAcaoPendente] = useState(null);

  const mostrarAcoes =
    podeGerir && !ESTADOS_FINAIS.includes(oportunidade.estado);

  const abrirConfirmacao = (acao) => {
    setMenuAberto(false);
    setAcaoPendente(acao);
  };

  const confirmarAcao = () => {
    if (acaoPendente === 'perdida') {
      onPerdida(oportunidade);
    }

    if (acaoPendente === 'cancelar') {
      onCancelar(oportunidade);
    }

    setAcaoPendente(null);
  };

  return (
    <>
      <tr>
        <td title={oportunidade.id}>{oportunidade.id.slice(0, 4)}</td>

        <td>{oportunidade.titulo}</td>

        <td>
          <div className="estado">{oportunidade.estadoTexto}</div>
        </td>

        <td>{oportunidade.cliente}</td>

        <td>{oportunidade.categoria}</td>

        <td>
          {oportunidade.prazo}
          {oportunidade.aviso && (
            <div className={`pill ${oportunidade.aviso.cor}`}>
              {oportunidade.aviso.texto}
            </div>
          )}
        </td>

        <td>{oportunidade.dataPretendida}</td>

        <td className="ml-auto">
          {mostrarAcoes && (
            <div className="row">
              {oportunidade.estado === 'Registada' && (
                <button
                  className="primary wide"
                  disabled={emCurso}
                  onClick={() => onIniciarAnalise(oportunidade)}
                >
                  Iniciar análise
                </button>
              )}

              {oportunidade.estado === 'EmAnalise' && (
                <button
                  className="primary wide"
                  disabled={emCurso}
                  onClick={() => onIniciarPreparacao(oportunidade)}
                >
                  Iniciar preparação
                </button>
              )}

              <div className="pos-rel">
                <button
                  className="more-actions"
                  disabled={emCurso}
                  onClick={() => setMenuAberto(!menuAberto)}
                  aria-label="Mais ações"
                  aria-expanded={menuAberto}
                >
                  <i className="bi bi-three-dots-vertical"></i>
                </button>

                {menuAberto && (
                  <div className="menu right">
                    <button
                      className="item wide danger"
                      onClick={() => abrirConfirmacao('perdida')}
                    >
                      <i className="bi bi-x-circle"></i>
                      <p>Marcar como perdida</p>
                    </button>

                    <button
                      className="item wide danger"
                      onClick={() => abrirConfirmacao('cancelar')}
                    >
                      <i className="bi bi-slash-circle"></i>
                      <p>Cancelar oportunidade</p>
                    </button>
                  </div>
                )}
              </div>
            </div>
          )}
        </td>
      </tr>

      <Aviso
        mensagem={
          acaoPendente === 'perdida'
            ? `Tens a certeza de que pretendes marcar "${oportunidade.titulo}" como perdida? Esta ação não pode ser desfeita.`
            : acaoPendente === 'cancelar'
              ? `Tens a certeza de que pretendes cancelar "${oportunidade.titulo}"? Esta ação não pode ser desfeita.`
              : ''
        }
        confirmar={true}
        textoConfirmar="Confirmar"
        textoCancelar="Voltar"
        onFechar={() => setAcaoPendente(null)}
        onConfirmar={confirmarAcao}
      />
    </>
  );
}
