
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { fazerLogin, guardarSessao, validateLogin } from '../models/loginModel'

export default function useLogin() {
  const navigate = useNavigate()

  const [passwordVisible, setPasswordVisible] = useState(false)
  const [rememberMe, setRememberMe] = useState(false)

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  // Show / hide password
  const togglePasswordVisibility = () => {
    setPasswordVisible(prev => !prev)
  }

  // Remember details
  const toggleRememberMe = () => {
    setRememberMe(prev => !prev)
  }

  // Login
  const handleLogin = async (event) => {
    event.preventDefault()

    const result = validateLogin(email, password)

    if (!result.valid) {
      setError(result.message)
      return
    }

    setError('')
    setLoading(true)

    try {
      const dados = await fazerLogin(email, password)
      guardarSessao(dados, rememberMe)
      navigate('/dashboard')
    } catch (err) {
      if (err.status === 401) {
        setError('Email ou palavra-passe inválidos.')
      } else {
        setError(err.message || 'Ocorreu um erro ao iniciar sessão.')
      }
    } finally {
      setLoading(false)
    }
  }

  return {
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
  }
}
