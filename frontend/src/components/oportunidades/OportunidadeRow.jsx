import { useState } from 'react';
import { createPortal } from 'react-dom';

import Aviso from '../Aviso';
import {
  descarregarDocumentoOportunidade,
  formatarTamanhoFicheiro,
  listarDocumentosOportunidade,
  mensagemDeErro,
} from '../../models/oportunidadesModel';

// Estados finais: já não há nada a fazer com a oportunidade
const ESTADOS_FINAIS = ['Ganha', 'Perdida', 'Cancelada'];

export default function OportunidadeRow({
  oportunidade,
  podeGerir,
  emCurso,
  menuAberto,
  onToggleMenu,
  onFecharMenu,
  onIniciarAnalise,
  onIniciarPreparacao,
  onCriarOrcamento,
  onPerdida,
  onCancelar,
}) {
  const [acaoPendente, setAcaoPendente] = useState(null);
  const [docsAberto, setDocsAberto] = useState(false);
  const [docs, setDocs] = useState([]);
  const [loadingDocs, setLoadingDocs] = useState(false);
  const [erroDocs, setErroDocs] = useState('');
  const [menuPosition, setMenuPosition] = useState({ top: 0, left: 0 });

  const mostrarAcoes =
    podeGerir && !ESTADOS_FINAIS.includes(oportunidade.estado);

  const abrirDocumentos = async () => {
    onFecharMenu();
    setDocsAberto(true);
    setLoadingDocs(true);
    setErroDocs('');

    try {
      setDocs(await listarDocumentosOportunidade(oportunidade.id));
    } catch (err) {
      setErroDocs(mensagemDeErro(err));
    } finally {
      setLoadingDocs(false);
    }
  };

  const handleDescarregar = async (nome) => {
    setErroDocs('');

    try {
      await descarregarDocumentoOportunidade(oportunidade.id, nome);
    } catch (err) {
      setErroDocs(mensagemDeErro(err));
    }
  };

  const handleToggleMenu = (event) => {
    if (menuAberto) {
      onToggleMenu();
      return;
    }

    const rect = event.currentTarget.getBoundingClientRect();

    setMenuPosition({
      top: rect.bottom + 5,
      left: rect.right - 190,
    });

    onToggleMenu();
  };

  const abrirConfirmacao = (acao) => {
    onFecharMenu();
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
      <tr className='scrollable-x'>
        <td title={oportunidade.id}>
          {oportunidade.id.slice(0, 4)}
        </td>

        <td>{oportunidade.titulo}</td>

        <td>
          <div className="estado">{oportunidade.estadoTexto}</div>
        </td>

        <td>{oportunidade.cliente}</td>

        <td>{oportunidade.categoria}</td>

        <td>
          <div className="row align-center">
            {oportunidade.prazo}
            {oportunidade.aviso && (
              <div className={`pill ${oportunidade.aviso.cor}`}>
                {oportunidade.aviso.texto}
              </div>
            )}
          </div>

        </td>

        <td>{oportunidade.dataPretendida}</td>

        <td>
          <div className="row justify-end">
            {mostrarAcoes && (
              <>
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

                {['EmPreparacao', 'PropostaEnviada'].includes(
                  oportunidade.estado
                ) && (
                    <button
                      className="primary wide"
                      disabled={emCurso}
                      onClick={() => onCriarOrcamento(oportunidade)}
                    >
                      Apresentar orçamento
                    </button>
                  )}

                <div className="pos-rel">
                  <button
                    className="more-actions ml-auto"
                    disabled={emCurso}
                    onClick={handleToggleMenu}
                    aria-label="Mais ações"
                    aria-expanded={menuAberto}
                  >
                    <i className="bi bi-three-dots-vertical"></i>
                  </button>

                  {menuAberto && createPortal(
                    <div
                      className="menu"
                      style={{
                        position: 'fixed',
                        top: menuPosition.top,
                        left: menuPosition.left,
                        width: '190px',
                        zIndex: 9999,
                      }}
                    >
                      <button
                        type="button"
                        className="item"
                        onClick={abrirDocumentos}
                      >
                        <i className="bi bi-paperclip"></i>
                        Consultar anexos
                      </button>

                      <button
                        className="item wide danger"
                        onClick={() => abrirConfirmacao('perdida')}
                      >
                        <i className="bi bi-clipboard-x"></i>
                        <p>Marcar como perdida</p>
                      </button>

                      <button
                        className="item wide danger"
                        onClick={() => abrirConfirmacao('cancelar')}
                      >
                        <i className="bi bi-clipboard-minus"></i>
                        <p>Cancelar oportunidade</p>
                      </button>
                    </div>,
                    document.body
                  )}
                </div>
              </>
            )}
          </div>
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

      {docsAberto && (
        <div className="overlay">
          <div className="box pad-1 modal">
            <div className="row align-center space-between mb-1">
              <h2>
                Anexos <i className="bi bi-dash"></i> {oportunidade.titulo}
              </h2>

              <button
                className="more-actions"
                type="button"
                onClick={() => setDocsAberto(false)}
              >
                <i className="bi bi-x-lg"></i>
              </button>
            </div>

            {loadingDocs && (
              <p className="weak">A carregar documentos...</p>
            )}

            {erroDocs && (
              <p className="danger mb-05">{erroDocs}</p>
            )}

            {!loadingDocs && !erroDocs && docs.length === 0 && (
              <p className="weak">
                Esta oportunidade não tem documentos anexados.
              </p>
            )}

            {!loadingDocs && docs.length > 0 && (
              <div className="column gap">
                {docs.map(doc => (
                  <div
                    key={doc.nome}
                    className="row align-center space-between"
                  >
                    <span>
                      <i className="bi bi-file-earmark"></i> {doc.nome}{' '}
                      <span className="weak">
                        ({formatarTamanhoFicheiro(doc.tamanho)})
                      </span>
                    </span>

                    <button
                      type="button"
                      className="secondary"
                      onClick={() => handleDescarregar(doc.nome)}
                    >
                      <i className="bi bi-download"></i> Descarregar
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </>
  );
}
