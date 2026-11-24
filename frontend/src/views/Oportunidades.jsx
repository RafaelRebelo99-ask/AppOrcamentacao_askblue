import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import Sort from '../components/Sort';
import OportunidadeRow from '../components/oportunidades/OportunidadeRow';
import useOportunidadesController from '../controllers/useOportunidades';

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

const opcoesOrdenacao = [
  { label: 'Prazo (Mais próximo)', value: 'prazo-asc' },
  { label: 'Prazo (Mais distante)', value: 'prazo-desc' },
];

export default function Oportunidades() {
  const {
    oportunidadesFiltradas,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    loading,
    erro,
    idEmCurso,
    podeGerir,
    handleIniciarAnalise,
    handleIniciarPreparacao,
    handlePerdida,
    handleCancelar,
  } =useOportunidadesController();

  return (
    <Layout>
      <div className="row align-center">
              <h1 className='mb-05'>Oportunidades</h1>
              <ThemeToggle/>
            </div>
      <div className="row mb-1 align-center">
        <Filters
          filtros={filtros}
          estadoSelecionado={estadoSelecionado}
          contagens={contagens}
          onEstadoChange={setEstadoSelecionado}
        />

        <div className="vl"></div>

        <Sort
          opcoes={opcoesOrdenacao}
          ordenacao={ordenacao}
          onOrdenacaoChange={setOrdenacao}
        />

      </div>

      {erro && <p className="error mb-05">{erro}</p>}
      {loading && <p>A carregar oportunidades...</p>}

      <table className="table" hidden={loading}>
        <thead>
          <tr className="header">
            <th>ID</th>
            <th>Título</th>
            <th>Estado</th>
            <th>Cliente</th>
            <th>Categoria</th>
            <th>Prazo da proposta</th>
            <th>Data pretendida</th>
            <th></th>
          </tr>
        </thead>

        <tbody>
          {oportunidadesFiltradas.length > 0 ? (
            oportunidadesFiltradas.map(oportunidade => (
              <OportunidadeRow
                key={oportunidade.id}
                oportunidade={oportunidade}
                podeGerir={podeGerir}
                emCurso={idEmCurso === oportunidade.id}
                onIniciarAnalise={handleIniciarAnalise}
                onIniciarPreparacao={handleIniciarPreparacao}
                onPerdida={handlePerdida}
                onCancelar={handleCancelar}
              />
            ))
          ) : (

            <td colSpan="8" style={{ textAlign: 'center' }}>
              Não existem oportunidades neste estado.
            </td>

          )}
        </tbody>
      </table>
    </Layout>
  );
}