
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
