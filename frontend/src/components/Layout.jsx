// ===== Layout comum às páginas da aplicação =====
// Barra lateral (Pesquisa / Trabalhos / Orçamentos) + conteúdo da página + rodapé.
// Passado do DEMO_FRONT (index.html, trabalhos.html, orcamentos.html), que repetiam isto em cada página.
// Uso: <Layout> ...conteúdo da página... </Layout>
import { NavLink, useNavigate } from 'react-router-dom'
import logo from '../assets/askblue-logo-oficial-rgb.png'
import logoDark from '../assets/askblue-logo-negativo-rgb.png'
import { useTheme } from '../controllers/ThemeController'
import { eAdministrador } from '../models/utilizadoresModel'
import { podeGerirOportunidades } from '../models/oportunidadesModel'
import { limparSessao, obterUtilizador } from '../models/loginModel'
import Icon from '../components/Icon'
import { Grid, Clipboard, Invoice, Users, Clock, ClipboardPlus, LogOut } from "@coolicons/react"

// O NavLink marca com "selected" o link da página onde estamos (classe do askblue.css)
function classeLink({ isActive }) {
  return 'transparent secondary w-100 justify-start' + (isActive ? ' selected' : '')
}

export default function Layout({ children }) {
  const { darkMode } = useTheme()
  const navigate = useNavigate()

  // Utilizador autenticado (guardado no loginModel)
  const utilizador = obterUtilizador()

  // iniciais do utilizador (para a foto)
  const iniciais = utilizador?.nome
    ? utilizador.nome
      .trim()
      .split(/\s+/)
      .slice(0, 2)
      .map(palavra => palavra[0])
      .join('')
      .toUpperCase()
    : '??'

  const roles = {
    0: 'Colaborador',
    1: 'Administrador',
    2: 'Cliente',
  };

  const role = roles[utilizador?.role] || 'Sem função';

  // Logout: limpa token e dados do utilizador e volta para o ecrã de login
  const handleLogout = () => {
    limparSessao()
    navigate('/login', { replace: true })
  }

  return (
    <>
      <div className="container pad-05" style={{ minHeight: '95%' }}>
        <div className="row align-start w-100 h-100">
          <div className="column fit gap nav h-100">
            <img src={darkMode ? logoDark : logo} alt="AskBlue Logo" className="logo big" />
            <NavLink to="/dashboard" className={classeLink}>
              <Icon icon={Grid} />Dashboard
            </NavLink>
            <NavLink to="/oportunidades" className={classeLink}>
              <Icon icon={Clipboard} />Oportunidades
            </NavLink>

            <NavLink to="/orcamentos" end className={classeLink}>
              <Icon icon={Invoice} />Orçamentos
            </NavLink>


            {/* Só Colaborador e Administrador criam oportunidades */}
            {podeGerirOportunidades() && (
              <>
                <hr />
                <NavLink to="/registar-oportunidade" className={classeLink}>
                  <Icon icon={ClipboardPlus} />Nova Oportunidade
                </NavLink>
              </>
            )}


            {/* Só o Administrador gere contas */}
            {eAdministrador() && (
              <>
                <hr />
                <NavLink to="/utilizadores" className={classeLink}>
                  <Icon icon={Users} />Utilizadores
                </NavLink>
              </>
            )}
            {/* Só o Administrador vê o histórico de atividade */}
            {eAdministrador() && (
              <NavLink to="/historico" end className={classeLink}>
                <Icon icon={Clock} />Histórico
              </NavLink>
            )}


            <div className="user mt-auto">
              <div className="column gap-1">
                <div className="row align-center wide-left">
                  <div className="photo">
                    {iniciais}
                  </div>
                  <div className="column">
                    <p id="user-name">
                      {utilizador?.nome || 'Utilizador'}
                    </p>

                    <p id="user-role" className="weak">
                      {role || 'Sem função'}
                    </p>
                  </div>
                </div>


                <button
                  type="button"
                  className="more-actions transparent w-100 justify-start"
                  onClick={handleLogout}
                >
                  <Icon icon={LogOut} />
                  <p>Terminar sessão</p>
                </button>


              </div>
            </div>
          </div>

          <div className="box lg column h-100 pad-1 overflow-hidden">
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
