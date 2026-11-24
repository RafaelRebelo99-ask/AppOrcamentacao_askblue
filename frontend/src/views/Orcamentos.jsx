// ===== VIEW: Orçamentos =====
// Só o visual, passado do DEMO_FRONT/orcamentos.html. Ainda sem lógica:
// filtros por estado, "Ordenar por", tema, "Abrir" e "Eliminar" não fazem nada.
// Os dados abaixo são de exemplo; vão ser substituídos pelos de GET /api/orcamentos.
import Layout from '../components/Layout'
import ThemeToggle from '../components/ThemeToggle'
import { ThemeProvider, useTheme } from '../controllers/ThemeController'

const filtros = [
  { nome: 'Todos', total: 3, selecionado: true },
  { nome: 'Aceite', total: 2 },
  { nome: 'Recusado', total: 1 },
  { nome: 'Enviado', total: 0 },
  { nome: 'Rascunho', total: 0 },
]

const orcamentosExemplo = [
  { id: 'ORC-2026-001', estado: 'Aceite', servico: 'Serviço', categoria: 'Tecnologias', cliente: 'AskBlue' },
  { id: 'ORC-2026-002', estado: 'Recusado', servico: 'Serviço', categoria: 'Tecnologias', cliente: 'Ageas' },
  { id: 'ORC-2026-003', estado: 'Aceite', servico: 'Serviço', categoria: 'Tecnologias', cliente: 'Santander' },
]

export default function Orcamentos() {
  const { darkMode, toggleTheme } = useTheme();

  return (
    <Layout>
      <div className="row mb-1">
        {filtros.map((f) => (
          <button key={f.nome} className={'wide-left secondary' + (f.selecionado ? ' selected' : '')}>
            {f.nome}<div className="counter">{f.total}</div>
          </button>
        ))}
        <div className="vl"></div>
        <button className="wide secondary" id="sortBtn">Ordenar por<i className="bi bi-caret-down"></i></button>
        <div className="menu hidden" id="sortMenu">
          <div className="item wide"><i className="bi bi-clock"></i><p>Data (Mais recente)</p></div>
          <div className="item wide"><i className="bi bi-clock-history"></i><p>Data (Mais antigo)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-down"></i><p>Valor (Mais baixo)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-up"></i><p>Valor (Mais alto)</p></div>
        </div>
        <ThemeToggle/>
      </div>

      <div className="grid">
        {orcamentosExemplo.map((o) => (
          <div className="card column" key={o.id}>
            <div className="row space-between">
              <div className="id">{o.id}</div>
              <div className="estado">{o.estado}</div>
            </div>
            <div className="column">
              <h2 className="mb-05">{o.servico}</h2>
              <div className="row">
                <div className="pill primary">{o.categoria}</div>
                <div className="row gap-0">
                  <p>{o.cliente}</p><i className="bi bi-check-lg"></i>
                </div>
              </div>
            </div>
            <hr />
            <div className="row">
              <button className="primary">Abrir</button>
              <button className="secondary danger">Eliminar</button>
            </div>
          </div>
        ))}
      </div>
    </Layout>
  )
}
