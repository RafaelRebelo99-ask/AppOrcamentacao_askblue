import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import useHistorico from '../controllers/useHistorico';

const filtros = [
  { label: 'Todas', value: 'Todas' },
  { label: 'Autenticação', value: 'Autenticacao' },
  { label: 'Oportunidades', value: 'Oportunidades' },
  { label: 'Orçamentos', value: 'Orcamentos' },
  { label: 'Utilizadores', value: 'Utilizadores' },
  { label: 'Categorias', value: 'Categorias' },
];

export default function Historico() {
  const {
    registosFiltrados,
    categoriaSelecionada,
    setCategoriaSelecionada,
    pesquisa,
    setPesquisa,
    contagens,
    loading,
    erro,
    recarregar,
  } = useHistorico();

  return (
    <Layout>
      <div className="row mb-1 align-center">
        <Filters
          filtros={filtros}
          estadoSelecionado={categoriaSelecionada}
          contagens={contagens}
          onEstadoChange={setCategoriaSelecionada}
        />

        <div className="vl"></div>

        <div className="empty-box">
          <i className='bi bi-search'></i>
          <input
            type="search"
            placeholder="Pesquisar utilizador ou ação..."
            value={pesquisa}
            onChange={e => setPesquisa(e.target.value)}
          />
        </div>

        <button
          type="button"
          className="secondary"
          onClick={recarregar}
          disabled={loading}
          title="Atualizar histórico"
        >
          <i className="bi bi-arrow-clockwise"></i>
        </button>

        <ThemeToggle />
      </div>

      {erro && <p className="error mb-05">{erro}</p>}
      {loading && <p>A carregar histórico...</p>}

      <table className="table historico" hidden={loading}>
        <thead>
          <tr className="header">
            <th>Data / Hora</th>
            <th>Utilizador</th>
            <th>Categoria</th>
            <th>Ação</th>
            <th>Descrição</th>
          </tr>
        </thead>

        <tbody>
          {registosFiltrados.length > 0 ? (
            registosFiltrados.map(registo => (
              <tr key={registo.id}>
                <td>
                  <div className="date">{registo.dataHora}</div>
                </td>
                <td>{registo.nomeUtilizador}</td>
                <td>
                  <div className="estado">{registo.categoriaTexto}</div>
                </td>
                <td>{registo.acao}</td>
                <td>{registo.descricao}</td>
              </tr>
            ))
          ) : (
            <tr>
              <td
                colSpan="5"
                style={{ gridColumn: '1 / -1', justifyContent: 'center' }}
              >
                Não existem registos de atividade.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </Layout>
  );
}
