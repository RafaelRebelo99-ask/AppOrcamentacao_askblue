import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import Sort from '../components/Sort';
import OrcamentoCard from '../components/orcamentos/OrcamentoCard';
import useOrcamentos from '../controllers/orcamentos/useOrcamentos';

const filtros = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Rascunho', value: 'Rascunho' },
  { label: 'Enviado', value: 'Enviado' },
  { label: 'Aceite', value: 'Aceite' },
  { label: 'Recusado', value: 'Recusado' },
];

const opcoesOrdenacao = [
  { label: 'Data (Mais recente)', value: 'data-desc' },
  { label: 'Data (Mais antigo)', value: 'data-asc' },
  { label: 'Valor (Mais baixo)', value: 'valor-asc' },
  { label: 'Valor (Mais alto)', value: 'valor-desc' },
];

export default function Orcamentos() {
  const {
    orcamentosFiltrados,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    loading,
    erro,
    handleEliminar,
  } = useOrcamentos();

  return (
    <Layout>
      <div className="row align-center">
              <h1 className='mb-05'>Orçamentos</h1>
              <ThemeToggle/>
            </div>
      <div className="row mb-1 align-center wrap">
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

        <ThemeToggle />
      </div>

      {erro && <p className="danger">{erro}</p>}

      {loading ? (
        <p>A carregar propostas...</p>
      ) : (
        <div className="grid">
          {orcamentosFiltrados.length > 0 ? (
            orcamentosFiltrados.map(orcamento => (
              <OrcamentoCard
                key={orcamento.id}
                orcamento={orcamento}
                onEliminar={handleEliminar}
              />
            ))
          ) : (
            <p>Não existem propostas neste estado.</p>
          )}
        </div>
      )}
    </Layout>
  );
}