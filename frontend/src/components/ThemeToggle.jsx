
import { useLocation } from 'react-router-dom'
import { useTheme } from '../controllers/ThemeController'
import Notificacoes from './Notificacoes'

export default function ThemeToggle() {

  const { darkMode, toggleTheme } = useTheme()
  const { pathname } = useLocation()

  return (
    <div className="row align-center gap-sm ml-auto">
      {/* O login também usa este botão, mas aí ainda não há notificações */}
      {pathname !== '/login' && <Notificacoes />}

      <button
        type="button"
        className="more-actions"
        onClick={toggleTheme}
        title={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
        aria-label={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
      >
        <i className={`bi ${darkMode ? 'bi-sun' : 'bi-moon'}`}></i>
      </button>
    </div>
  )
}
