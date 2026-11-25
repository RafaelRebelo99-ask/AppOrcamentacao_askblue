// ===== MODEL: notificações =====
// Cada utilizador só recebe as suas (o backend usa o token).
import { apiRequest } from './api';

// Página que cada notificação abre. O backend envia o destino como número, pela ordem do enum
// DestinoNotificacao (Notificacao.cs). Se um dia passar a enviar texto, também funciona.
const DESTINOS = ['Oportunidades', 'Orcamentos'];
const PAGINAS = {
  Oportunidades: '/oportunidades',
  Orcamentos: '/orcamentos',
};

// Devolve { naoLidas, itens } com as mais recentes primeiro
export async function listarNotificacoes() {
  const { naoLidas, itens } = await apiRequest('/api/notificacoes');

  return {
    naoLidas,
    itens: itens.map(n => {
      const destino = typeof n.destino === 'number' ? DESTINOS[n.destino] : n.destino;

      return {
        id: n.id,
        texto: n.texto,
        lida: n.lida,
        pagina: PAGINAS[destino] ?? '/oportunidades',
        quando: tempoRelativo(n.dataCriacao),
      };
    }),
  };
}

export function marcarComoLida(id) {
  return apiRequest(`/api/notificacoes/${id}/lida`, { method: 'PUT' });
}

export function marcarTodasComoLidas() {
  return apiRequest('/api/notificacoes/lidas', { method: 'PUT' });
}

// 'agora', 'há 5 minutos', 'ontem'... e a data, se tiver mais de uma semana
function tempoRelativo(data) {
  const formato = new Intl.RelativeTimeFormat('pt-PT', { numeric: 'auto' });
  const minutos = Math.floor((Date.now() - new Date(data).getTime()) / 60_000);

  if (minutos < 1) return 'agora';
  if (minutos < 60) return formato.format(-minutos, 'minute');
  if (minutos < 24 * 60) return formato.format(-Math.floor(minutos / 60), 'hour');

  const dias = Math.floor(minutos / (24 * 60));
  if (dias < 7) return formato.format(-dias, 'day');

  return new Date(data).toLocaleDateString('pt-PT');
}
