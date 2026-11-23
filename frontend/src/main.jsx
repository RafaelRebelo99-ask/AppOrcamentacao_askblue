import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import './askblue.css'
import './app.css'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    {/* O BrowserRouter permite ter vários ecrãs (/login, /servicos, ...) na mesma aplicação */}
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </StrictMode>,
)
