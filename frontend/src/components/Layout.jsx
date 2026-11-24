// ===== Layout comum às páginas da aplicação =====
// Barra lateral (Pesquisa / Trabalhos / Orçamentos) + conteúdo da página + rodapé.
// Passado do DEMO_FRONT (index.html, trabalhos.html, orcamentos.html), que repetiam isto em cada página.
// Uso: <Layout> ...conteúdo da página... </Layout>
import { NavLink } from 'react-router-dom'
import logo from '../assets/askblue-logo-oficial-rgb.png'
import logoDark from '../assets/askblue-logo-negativo-rgb.png'
import { useTheme } from '../controllers/ThemeController'

// O NavLink marca com "selected" o link da página onde estamos (classe do askblue.css)
function classeLink({ isActive }) {
  return 'transparent secondary w-100 justify-start' + (isActive ? ' selected' : '')
}

export default function Layout({ children }) {
  const { darkMode } = useTheme()

  return (
    <>
      <div className="container pad-05" style={{ minHeight: '95%' }}>
        <div className="row align-start w-100 h-100">
          <div className="column fit gap nav">
            <img src={darkMode ? logoDark : logo} alt="AskBlue Logo" className="logo big" />
            <NavLink to="/dashboard" className={classeLink}>
              <i className="bi bi-grid"></i>Dashboard
            </NavLink>
            <NavLink to="/oportunidades" className={classeLink}>
              <i className="bi bi-clipboard"></i>Oportunidades
            </NavLink>

            <NavLink to="/orcamentos" end className={classeLink}>
              <i className="bi bi-file-earmark-spreadsheet"></i>Orçamentos
            </NavLink>
            <NavLink to="" end className={classeLink}>
              <i className="bi bi-clock"></i>Histórico
            </NavLink>
            <hr></hr>
            <NavLink to="" className={classeLink}>
              <i className="bi bi-clipboard-plus"></i>Nova Oportunidade
            </NavLink>
          </div>

          <div className="box lg column h-100 pad-1 scrollable">
            {children}
          </div>
        </div>
      </div>

      <footer className="row footer">
        <p>Academia .NET - Sistema de Gestão de Serviços</p>
        <div className="row">
          <a href="https://askblue.com" target="_blank" rel="noreferrer">AskBlue</a>
          <i className="bi bi-dot"></i>
          <p>2026</p>
        </div>
      </footer>
    </>
  )
}
