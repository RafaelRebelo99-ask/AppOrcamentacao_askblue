
import { useState } from 'react'
import { validateLogin } from '../models/loginModel'

export default function useLogin() {

  const [passwordVisible, setPasswordVisible] = useState(false)
  const [rememberMe, setRememberMe] = useState(false)

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const [error, setError] = useState('')

  // Show / hide password
  const togglePasswordVisibility = () => {
    setPasswordVisible(prev => !prev)
  }

  // Remember details
  const toggleRememberMe = () => {
    setRememberMe(prev => !prev)
  }

  // Login
  const handleLogin = (event) => {
    event.preventDefault()

    const result = validateLogin(email, password)

    if (!result.valid) {
      setError(result.message)
      return
    }

    setError('')

    // depois ligar ao API
    console.log('Login submitted:', email)
  }

  return {
    passwordVisible,
    rememberMe,
    email,
    password,
    error,

    setEmail,
    setPassword,

    togglePasswordVisibility,
    toggleRememberMe,
    handleLogin
  }
}
