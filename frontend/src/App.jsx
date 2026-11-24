// ===== Rotas da aplicação =====
// Cada <Route> liga um endereço (path) a uma view.
// Para um ecrã novo: criar a view em views/, o controller em controllers/,
// as chamadas à API em models/ e acrescentar a rota aqui.
import { Navigate, Route, Routes } from 'react-router-dom'
import Login from './views/Login'
import Pesquisa from './views/Pesquisa'
import Trabalhos from './views/Oportunidades'
import Orcamentos from './views/Orcamentos'
import CriarOrcamento from './views/CriarOrcamento'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/pesquisa" element={<Pesquisa />} />
      <Route path="/trabalhos" element={<Trabalhos />} />
      <Route path="/orcamentos" element={<Orcamentos />} />
      <Route path="/orcamentos/novo" element={<CriarOrcamento />} />

      {/* Por agora, qualquer outro endereço vai para o login */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
