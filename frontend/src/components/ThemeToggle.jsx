
import { useLocation } from 'react-router-dom'
import { useTheme } from '../controllers/ThemeController'
import Notificacoes from './Notificacoes'

import { Sun, Moon } from '@coolicons/react'
import Icon from './Icon'
import Tooltip from './Tooltip'

export default function ThemeToggle() {

  const { darkMode, toggleTheme } = useTheme()
  const { pathname } = useLocation()
  const ThemeIcon = darkMode ? Sun : Moon

  return (
    <div className="row align-center gap-sm ml-auto">
      {/* O login também usa este botão, mas aí ainda não há notificações */}
      {pathname !== '/login' && <Notificacoes />}

      <Tooltip text='Mudar de tema' position='bottom'>
        <button
          type="button"
          className="more-actions"
          onClick={toggleTheme}
          title={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
          aria-label={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
        >
          <Icon icon={ThemeIcon} />
        </button>
      </Tooltip>
    </div>
  )
}
