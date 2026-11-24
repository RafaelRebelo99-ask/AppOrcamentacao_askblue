// ===== Rotas da aplicação =====
// Cada <Route> liga um endereço (path) a uma view.
// Para um ecrã novo: criar a view em views/, o controller em controllers/,
// as chamadas à API em models/ e acrescentar a rota aqui.
import { Navigate, Route, Routes } from 'react-router-dom'
import Login from './views/Login'
import Dashboard from './views/Dashboard'
import Oportunidades from './views/Oportunidades'
import Orcamentos from './views/Orcamentos'
import CriarOrcamento from './views/CriarOrcamento'
import Historico from './views/Historico'
import NovaOportunidade from './views/NovaOportunidade'
import Utilizadores from './views/Utilizadores'
import ProtectedRoute from './components/ProtectRoute'

function App() {
  return (
    <Routes>
      {/* Página pública */}
      <Route path="/login" element={<Login />} />

      {/* Páginas protegidas */}
      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/oportunidades" element={<Oportunidades />} />
<<<<<<< Updated upstream
        <Route path="/oportunidades/adicionar" element={<NovaOportunidade />} />
        <Route path="/historico" element={<Historico />} />
        <Route path="/utilizadores" element={<Utilizadores />} />
=======
        <Route path="/registar-oportunidade" element={<NovaOportunidade />} />
>>>>>>> Stashed changes
        <Route path="/orcamentos" element={<Orcamentos />} />
        <Route path="/orcamentos/novo/:oportunidadeId" element={<CriarOrcamento />} />
      </Route>

      {/* Qualquer rota desconhecida */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
