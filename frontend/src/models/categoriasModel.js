import { apiRequest } from './api';

export async function listarCategorias() {
  const categorias = await apiRequest('/api/categorias');

  return categorias
    .map(u => ({
      id: u.id,
      nome: u.nome,
    }))
    .sort((a, b) => a.nome.localeCompare(b.nome, 'pt'));
}