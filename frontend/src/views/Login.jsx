
import logo from '../assets/askblue-logo-oficial-rgb.png'
import logoDark from '../assets/askblue-logo-negativo-rgb.png'
import useLogin from '../controllers/useLogin'
import { useTheme } from '../controllers/ThemeController'
import ThemeToggle from '../components/ThemeToggle';

export default function Login() {

  const { darkMode } = useTheme();

  const {
    passwordVisible,
    rememberMe,
    email,
    password,
    error,
    loading,
    setEmail,
    setPassword,
    togglePasswordVisibility,
    toggleRememberMe,
    handleLogin
  } = useLogin()

  return (
    <div className="container align-center">
      <div className="card column" style={{ padding: '4rem' }}>

        <div className="row center mb-1">
          <img
            src={darkMode ? logoDark : logo}
            className="logo"
            style={{ width: '15rem' }}
            alt="AskBlue"
          />
        </div>

        <div className="row mb-1">
          <h1>Log In</h1>

          <ThemeToggle/>
        </div>

        <form onSubmit={handleLogin}>

          <div className="input-group">
            <label htmlFor="email">Email</label>

            <div className="empty-box mb-05">
              <input
                id="email"
                name="email"
                type="email"
                required
                placeholder="Email"
                autoComplete="username"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
          </div>

          <div className="input-group">
            <label htmlFor="password">Password</label>

            <div className="empty-box">
              <input
                id="password"
                name="password"
                type={passwordVisible ? 'text' : 'password'}
                required
                placeholder="Password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />

              <button
                type="button"
                className="icon ml-auto"
                title="Mostrar password"
                onClick={togglePasswordVisibility}
              >
                <i className={`bi ${
                  passwordVisible ? 'bi-eye-slash' : 'bi-eye'
                }`}></i>
              </button>
            </div>
          </div>

          <p className="hint mb-05">
            A password deve conter pelo menos 8 caracteres.
          </p>

          <div className="row mb-1">
            <button
              type="button"
              className={`checkbox ${rememberMe ? 'selected' : ''}`}
              onClick={toggleRememberMe}
              aria-pressed={rememberMe}
            >
              <i className="bi bi-check"></i>
            </button>

            <p>Lembrar detalhes</p>
          </div>

          {error && <p className="error">{error}</p>}

          <button type="submit" className="primary w-100" disabled={loading}>
            {loading ? 'A entrar...' : 'Log In'}
          </button>

        </form>
      </div>
    </div>
  )
}
