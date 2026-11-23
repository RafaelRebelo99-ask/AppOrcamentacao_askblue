// ===== VIEW: ecrã de login =====
// Só o visual (passado do DEMO_FRONT/login.html para JSX, com as mesmas classes do askblue.css).
// Ainda sem lógica: o formulário não faz login, e os botões de tema, mostrar password
// e "Lembrar detalhes" ainda não fazem nada. Isso fica para o controller (controllers/useLogin.js).
import logo from '../assets/askblue-logo-oficial-rgb.png'

export default function Login() {
  return (
    <div className="container align-center">
      <div className="card column" style={{ padding: '4rem' }}>
        <div className="row center mb-1">
          <img src={logo} className="logo" style={{ width: '15rem' }} alt="AskBlue" />
        </div>

        <div className="row mb-1">
          <h1>Log In</h1>
          <button type="button" className="more-actions" title="Mudar tema">
            <i className="bi bi-moon"></i>
          </button>
        </div>

        <form>
          <div className="input-group">
            <label htmlFor="email">Email</label>
            <div className="empty-box mb-05">
              <input id="email" name="email" type="email" required placeholder="Email" autoComplete="username" />
            </div>
          </div>

          <div className="input-group">
            <label htmlFor="password">Password</label>
            <div className="empty-box">
              <input id="password" name="password" type="password" required placeholder="Password" autoComplete="current-password" />
              <button type="button" className="icon ml-auto" title="Mostrar password">
                <i className="bi bi-eye"></i>
              </button>
            </div>
          </div>

          <p className="hint mb-05">A password deve conter pelo menos 8 caracteres.</p>

          <div className="row mb-1">
            <button type="button" className="checkbox">
              <i className="bi bi-check"></i>
            </button>
            <p>Lembrar detalhes</p>
          </div>

          <button type="submit" className="primary w-100">Log In</button>
        </form>
      </div>
    </div>
  )
}
