// ===== MODEL: histórico (logs de atividade) =====
// Acesso exclusivo para Administradores (o backend valida com Politicas.Administrador).
import { apiRequest } from './api';

// Ordem igual ao enum CategoriaHistorico (RegistoHistorico.cs)
const CATEGORIAS = [
  'Autenticacao',
  'Oportunidades',
  'Orcamentos',
  'Utilizadores',
  'Categorias',
];

const TEXTO_CATEGORIA = {
  Autenticacao: 'Autenticação',
  Oportunidades: 'Oportunidades',
  Orcamentos: 'Orçamentos',
  Utilizadores: 'Utilizadores',
  Categorias: 'Categorias',
};

function normalizarDataUtc(data) {
  if (!data) return null;
  if (typeof data !== 'string') return new Date(data);
  const temFuso = /([zZ]|[+-]\d{2}:\d{2})$/.test(data);
  return new Date(temFuso ? data : `${data}Z`);
}

function formatarDataHora(data) {
  const d = normalizarDataUtc(data);
  if (!d || Number.isNaN(d.getTime())) return '—';

  return d.toLocaleString('pt-PT', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export async function listarHistorico() {
  const registos = await apiRequest('/api/historico');

  return registos.map(r => {
    const categoria =
      typeof r.categoria === 'number'
        ? (CATEGORIAS[r.categoria] ?? String(r.categoria))
        : r.categoria;

    return {
      id: r.id,
      nomeUtilizador: r.nomeUtilizador,
      categoria,
      categoriaTexto: TEXTO_CATEGORIA[categoria] ?? categoria,
      acao: r.acao,
      descricao: r.descricao,
      dataHora: formatarDataHora(r.dataHora),
    };
  });
}
