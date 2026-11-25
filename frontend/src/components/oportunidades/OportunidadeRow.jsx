
import { useState } from 'react';
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
  onIniciarAnalise,
  onIniciarPreparacao,
  onCriarOrcamento,
  onPerdida,
  onCancelar,
}) {
  const [menuAberto, setMenuAberto] = useState(false);
  const [acaoPendente, setAcaoPendente] = useState(null);
  const [docsAberto, setDocsAberto] = useState(false);
  const [docs, setDocs] = useState([]);
  const [loadingDocs, setLoadingDocs] = useState(false);
  const [erroDocs, setErroDocs] = useState('');

  const mostrarAcoes =
    podeGerir && !ESTADOS_FINAIS.includes(oportunidade.estado);

  const abrirDocumentos = async () => {
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
          <div className="row">

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

                {/* Com uma proposta já enviada, dá para enviar outra */}
                {['EmPreparacao', 'PropostaEnviada'].includes(oportunidade.estado) && (
                  <button
                    className="primary wide"
                    disabled={emCurso}
                    onClick={() => onCriarOrcamento(oportunidade)}
                  >
                    Criar orçamento
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
                        type="button"
                        className="item"
                        onClick={abrirDocumentos}
                        title="Ver documentos"
                        aria-label="Ver documentos"
                      >
                        <i className="bi bi-paperclip"></i>
                        Consultar anexos
                      </button>
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
              <h2>Anexos <i className='bi bi-dash'></i> {oportunidade.titulo}</h2>
              <button
                className="more-actions"
                type="button"
                onClick={() => setDocsAberto(false)}
              >
                <i className="bi bi-x-lg"></i>
              </button>
            </div>

            {loadingDocs && <p className="weak">A carregar documentos...</p>}
            {erroDocs && <p className="danger mb-05">{erroDocs}</p>}

            {!loadingDocs && !erroDocs && docs.length === 0 && (
              <p className="weak">Esta oportunidade não tem documentos anexados.</p>
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
