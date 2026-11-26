import Layout from '../components/Layout'
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import UtilizadorForm from '../components/utilizadores/UtilizadorForm';
import UtilizadorRow from '../components/utilizadores/UtilizadorRow';
import useUtilizadores from '../controllers/useUtilizadores';

const filtros = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Administradores', value: 'Administrador' },
  { label: 'Colaboradores', value: 'Colaborador' },
  { label: 'Clientes', value: 'Cliente' },
];

export default function Utilizadores() {
  const {
    utilizadoresFiltrados,
    roleSelecionada,
    setRoleSelecionada,
    contagens,
    loading,
    erro,
    sucesso,
    idEmCurso,
    handleApagar,
    formAberto,
    dados,
    aCriar,
    erroForm,
    handleAbrirForm,
    handleFecharForm,
    handleChange,
    handleSubmit,
    utilizadorEmEdicao,
    handleAbrirEdicao,
  } = useUtilizadores();

  return (
    <Layout>
      <div className="row align-center">
        <h1 className='mb-05'>Utilizadores</h1>
        <ThemeToggle />
      </div>
      <div className="row mb-1 align-center wrap">
        <Filters
          filtros={filtros}
          estadoSelecionado={roleSelecionada}
          contagens={contagens}
          onEstadoChange={setRoleSelecionada}
        />

        <div className="vl"></div>

        <button
          type="button"
          className="primary wide"
          onClick={handleAbrirForm}
          disabled={formAberto}
        >
          <i className="bi bi-person-plus"></i>Novo utilizador
        </button>
      </div>

      {formAberto && (
        <UtilizadorForm
          dados={dados}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancelar={handleFecharForm}
          erro={erroForm}
          loading={aCriar}
          modoEdicao={!!utilizadorEmEdicao}
        />
      )}

      {sucesso && <p className="mb-05" role="status">{sucesso}</p>}
      {erro && <p className="error mb-05">{erro}</p>}
      {loading && <p>A carregar utilizadores...</p>}
      <div className="table-container">
        <table className="table utilizadores" hidden={loading}>
          <thead>
            <tr className="header">
              <th>Nome</th>
              <th>Email</th>
              <th>Função</th>
              <th></th>
            </tr>
          </thead>

          <tbody>
            {utilizadoresFiltrados.length > 0 ? (
              utilizadoresFiltrados.map(utilizador => (
                <UtilizadorRow
                  key={utilizador.id}
                  utilizador={utilizador}
                  emCurso={idEmCurso === utilizador.id}
                  onApagar={handleApagar}
                  onEditar={handleAbrirEdicao}
                />
              ))
            ) : (
              <tr>
                <td>Não existem utilizadores com esta função.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </Layout>
  )
}
