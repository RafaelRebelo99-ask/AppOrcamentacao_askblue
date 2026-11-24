import { useState } from 'react';

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

  const mostrarAcoes = podeGerir && !ESTADOS_FINAIS.includes(oportunidade.estado);

  return (
    <tr>
      {/* O id é um Guid: mostra só o início, e o resto ao passar o rato */}
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
                    onClick={() => {
                      setMenuAberto(false);
                      onPerdida(oportunidade);
                    }}
                  >
                    <i className="bi bi-x-circle"></i>
                    <p>Marcar como perdida</p>
                  </button>

                  <button
                    className="item wide danger"
                    onClick={() => {
                      setMenuAberto(false);
                      onCancelar(oportunidade);
                    }}
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
  );
}
