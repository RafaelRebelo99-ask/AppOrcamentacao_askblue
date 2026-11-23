// ===== Layout comum às páginas da aplicação =====
// Barra lateral (Pesquisa / Trabalhos / Orçamentos) + conteúdo da página + rodapé.
// Passado do DEMO_FRONT (index.html, trabalhos.html, orcamentos.html), que repetiam isto em cada página.
// Uso: <Layout> ...conteúdo da página... </Layout>
import { NavLink } from 'react-router-dom'
import logo from '../assets/askblue-logo-oficial-rgb.png'

// O NavLink marca com "selected" o link da página onde estamos (classe do askblue.css)
function classeLink({ isActive }) {
  return 'transparent secondary w-100 justify-start' + (isActive ? ' selected' : '')
}

export default function Layout({ children }) {
  return (
    <>
      <div className="container pad-05" style={{ minHeight: '95%' }}>
        <div className="row align-start w-100 h-100">
          <div className="column fit gap nav">
            <img src={logo} alt="AskBlue Logo" className="logo big" />
            <NavLink to="/pesquisa" className={classeLink}>
              <i className="bi bi-search"></i>Pesquisa
            </NavLink>
            <NavLink to="/trabalhos" className={classeLink}>
              <i className="bi bi-journals"></i>Trabalhos
            </NavLink>
            <NavLink to="/orcamentos" end className={classeLink}>
              <i className="bi bi-file-earmark-spreadsheet"></i>Orçamentos
            </NavLink>
          </div>

          <div className="box lg column h-100 pad-1">
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
