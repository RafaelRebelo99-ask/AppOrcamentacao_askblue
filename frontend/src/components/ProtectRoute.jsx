import { Navigate, Outlet } from 'react-router-dom';
import { obterUtilizador } from '../models/loginModel';

export default function ProtectedRoute() {
  const utilizador = obterUtilizador();

  if (!utilizador) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
