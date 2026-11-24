
import { useTheme } from '../controllers/ThemeController'

export default function ThemeToggle() {

  const { darkMode, toggleTheme } = useTheme()

  return (
    <button
      type="button"
      className="more-actions"
      onClick={toggleTheme}
      title={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
      aria-label={darkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro'}
    >
      <i className={`bi ${darkMode ? 'bi-sun' : 'bi-moon'}`}></i>
    </button>
  )
}
