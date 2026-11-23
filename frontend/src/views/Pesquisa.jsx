// ===== VIEW: Pesquisa (serviços disponíveis para o Prestador) =====
// Só o visual, passado do DEMO_FRONT/index.html. Ainda sem lógica:
// filtros, "Ordenar por", tema e "Apresentar orçamento" não fazem nada.
// Os dados abaixo são de exemplo; vão ser substituídos pelos de GET /api/servicos/disponiveis.
import Layout from '../components/Layout'

const servicosExemplo = [
  { id: 1, cliente: 'AskBlue', categoria: 'Tecnologias', titulo: 'Serviço de consultoria de IT', valorMaximo: '10.000 €', dataConclusao: '30/12/2026', descricao: 'Isto é uma pequena descrição do serviço a ser prestado.' },
  { id: 2, cliente: 'AskBlue', categoria: 'Tecnologias', titulo: 'Serviço de consultoria de IT', valorMaximo: '10.000 €', dataConclusao: '30/12/2026', descricao: 'Isto é uma pequena descrição do serviço a ser prestado.' },
  { id: 3, cliente: 'AskBlue', categoria: 'Tecnologias', titulo: 'Serviço de consultoria de IT', valorMaximo: '10.000 €', dataConclusao: '30/12/2026', descricao: 'Isto é uma pequena descrição do serviço a ser prestado.' },
  { id: 4, cliente: 'AskBlue', categoria: 'Tecnologias', titulo: 'Serviço de consultoria de IT', valorMaximo: '10.000 €', dataConclusao: '30/12/2026', descricao: 'Isto é uma pequena descrição do serviço a ser prestado.' },
]

export default function Pesquisa() {
  return (
    <Layout>
      <div className="row mb-1">
        <button className="wide secondary">Categoria<i className="bi bi-caret-down"></i></button>
        <button className="wide secondary">Localização<i className="bi bi-caret-down"></i></button>
        <button className="wide secondary">Preço<i className="bi bi-caret-down"></i></button>
        <button className="wide secondary" id="sortBtn">Ordenar por<i className="bi bi-caret-down"></i></button>
        {/* Menu fechado (classe "hidden"); abrir/fechar fica para a lógica */}
        <div className="menu hidden" id="sortMenu">
          <div className="item wide"><i className="bi bi-clock"></i><p>Data (Mais recente)</p></div>
          <div className="item wide"><i className="bi bi-clock-history"></i><p>Data (Mais antigo)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-down"></i><p>Valor (Mais baixo)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-up"></i><p>Valor (Mais alto)</p></div>
        </div>
        <button className="more-actions" title="Mudar tema"><i className="bi bi-moon"></i></button>
      </div>

      <div className="grid servicos">
        {servicosExemplo.map((s) => (
          <div className="card column" key={s.id}>
            <div className="row space-between">
              <div className="row gap-0">
                <p>{s.cliente}</p><i className="bi bi-check-lg"></i>
              </div>
              <div className="pill primary">{s.categoria}</div>
            </div>
            <div className="column">
              <h2 className="mb-05">{s.titulo}</h2>
              <div className="row mb-05">
                <div className="estado">Valor máximo: {s.valorMaximo}</div>
                <div className="estado">Data de Conclusão: {s.dataConclusao}</div>
              </div>
              <p>{s.descricao}</p>
            </div>
            <hr />
            <div className="row">
              <button className="primary">Apresentar orçamento</button>
            </div>
          </div>
        ))}
      </div>
    </Layout>
  )
}
