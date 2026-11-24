// ===== Cliente HTTP base para falar com a API .NET =====
// Todos os pedidos passam por aqui para:
// 1. Adicionar Content-Type: application/json
// 2. Enviar automaticamente o token JWT (se o utilizador estiver autenticado)
// 3. Tratar erros HTTP num só sítio

export function obterToken() {
  return localStorage.getItem('token') || sessionStorage.getItem('token')
}

export async function apiRequest(url, options = {}) {
  const token = obterToken()

  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  }

  let resposta
  try {
    resposta = await fetch(url, {
      ...options,
      headers,
    })
  } catch {
    throw new Error('Não foi possível ligar ao servidor. Verifica se o backend (.NET) está a correr.')
  }

  if (!resposta.ok) {
    let mensagem = ''
    try {
      const texto = await resposta.text()
      if (texto) {
        try {
          const json = JSON.parse(texto)
          mensagem = json.erro || json.message || texto
        } catch {
          mensagem = texto
        }
      }
    } catch {
      // ignora erro ao ler corpo
    }

    const erro = new Error(mensagem || `Erro ${resposta.status}`)
    erro.status = resposta.status
    throw erro
  }

  if (resposta.status === 204) {
    return null
  }

  return resposta.json()
}
