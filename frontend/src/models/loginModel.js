
import { apiRequest } from './api'

export function validateLogin(email, password) {
  if (!email || !password) {
    return {
      valid: false,
      message: 'Deve preencher todos os campos.'
    }
  }

  if (password.length < 8) {
    return {
      valid: false,
      message: 'A palavra-passe deve ter pelo menos 8 caracteres.'
    }
  }

  return {
    valid: true,
    message: ''
  }
}

// Chama POST /api/auth/login no backend .NET (via proxy do Vite)
export async function fazerLogin(email, password) {
  return apiRequest('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  })
}

// Guarda o token e os dados do utilizador:
// - localStorage se "Lembrar detalhes" estiver ativo (fica guardado mesmo fechando o browser)
// - sessionStorage caso contrário (apaga-se quando fechas o separador)
export function guardarSessao(dados, rememberMe) {
  limparSessao()
  const storage = rememberMe ? localStorage : sessionStorage
  storage.setItem('token', dados.accessToken)
  storage.setItem('utilizador', JSON.stringify(dados.utilizador))
}

export function obterUtilizador() {
  const texto = localStorage.getItem('utilizador') || sessionStorage.getItem('utilizador')
  return texto ? JSON.parse(texto) : null
}

export function limparSessao() {
  localStorage.removeItem('token')
  localStorage.removeItem('utilizador')
  sessionStorage.removeItem('token')
  sessionStorage.removeItem('utilizador')
}
