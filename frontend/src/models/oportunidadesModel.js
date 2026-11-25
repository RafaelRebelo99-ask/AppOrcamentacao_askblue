// ===== MODEL: oportunidades =====
// Fala com a API e traduz os dados para o formato que os ecrãs usam.
// Não sabe nada de formulários, botões nem páginas (isso é dos controllers e das views).
import { apiRequest } from './api';
import { obterUtilizador } from './loginModel';

// O backend envia os enums como números, pela ordem em que estão no C#
// (Oportunidade.cs e Utilizador.cs). Se um dia passar a enviar texto, também funciona.
const ESTADOS = [
  'Registada',
  'EmAnalise',
  'EmPreparacao',
  'PropostaEnviada',
  'Ganha',
  'Perdida',
  'Cancelada',
];
const ROLES = ['Colaborador', 'Administrador', 'Cliente'];

const TEXTO_ESTADO = {
  Registada: 'Registada',
  EmAnalise: 'Em análise',
  EmPreparacao: 'Em preparação',
  PropostaEnviada: 'Proposta enviada',
  Ganha: 'Ganha',
  Perdida: 'Perdida',
  Cancelada: 'Cancelada',
};

// Estados em que o prazo ainda interessa (a proposta ainda não foi enviada)
const ESTADOS_COM_PRAZO = ['Registada', 'EmAnalise', 'EmPreparacao'];

function deEnum(valor, nomes) {
  return typeof valor === 'number' ? nomes[valor] : valor;
}

// ----- Permissões -----

export function obterRole() {
  return deEnum(obterUtilizador()?.role, ROLES);
}

// Só Colaborador e Administrador criam e gerem oportunidades.
// Isto só serve para mostrar/esconder botões; quem garante a regra é o backend.
export function podeGerirOportunidades() {
  return ['Colaborador', 'Administrador'].includes(obterRole());
}

// ----- Erros -----

export function sessaoInvalida(erro) {
  return erro.status === 401;
}

export function mensagemDeErro(erro) {
  if (erro.status === 403) return 'Não tens permissão para esta ação.';
  // Com o backend desligado, o proxy do Vite responde 5xx
  if (erro.status >= 500) return 'O servidor não respondeu. Verifica se o backend está a correr.';
  return erro.message || 'Ocorreu um erro inesperado.';
}

// ----- Leitura -----

export function listarCategorias() {
  return apiRequest('/api/categorias');
}

// Id e nome dos clientes ativos (Colaborador e Administrador)
export function listarClientes() {
  return apiRequest('/api/utilizadores/clientes');
}

// Devolve as oportunidades já com os nomes do cliente e da categoria.
// O backend decide quais: o Cliente recebe só as suas, os outros recebem todas.
export async function listarOportunidades() {
  const [oportunidades, categorias, clientes] = await Promise.all([
    apiRequest('/api/oportunidades'),
    listarCategorias(),
    clientesVisiveis(),
  ]);

  const nomeCategoria = new Map(categorias.map(c => [c.id, c.nome]));
  const nomeCliente = new Map(clientes.map(c => [c.id, c.nome]));

  return oportunidades.map(o => {
    const estado = deEnum(o.estado, ESTADOS);

    return {
      id: o.id,
      titulo: o.titulo,
      estado,                                        // para filtrar: 'EmAnalise'
      estadoTexto: TEXTO_ESTADO[estado] ?? estado,   // para mostrar: 'Em análise'
      cliente: nomeCliente.get(o.idCliente) ?? '—',
      categoria: nomeCategoria.get(o.idCategoria) ?? '—',
      idCategoria: o.idCategoria, // para filtrar
      prazo: formatarData(o.prazoProposta),          // para mostrar: '10/12/2026'
      prazoTempo: o.prazoProposta                    // para ordenar (null = sem prazo)
        ? new Date(o.prazoProposta).getTime()
        : null,
      dataPretendida: formatarData(o.dataPretendida),
      aviso: calcularAviso(o.prazoProposta, estado), // null = sem aviso
    };
  });
}

// Uma oportunidade, para mostrar no formulário de orçamento
export async function obterOportunidade(id) {
  const o = await apiRequest(`/api/oportunidades/${id}`);
  const estado = deEnum(o.estado, ESTADOS);

  console.log(o);

  return {
    id: o.id,
    titulo: o.titulo,
    descricao: o.descricao,
    categoria: o.idCategoria,
    estado,
    estadoTexto: TEXTO_ESTADO[estado] ?? estado,
    prazo: formatarData(o.prazoProposta),
    dataPretendida: formatarData(o.dataPretendida),
  };
}

// A API das oportunidades só envia o id do cliente; os nomes vêm de onde a role permitir
async function clientesVisiveis() {
  if (podeGerirOportunidades()) return listarClientes();
  if (obterRole() === 'Cliente') return [obterUtilizador()];
  return [];
}

// ----- Criação -----

// Devolve uma mensagem de erro, ou '' se estiver tudo bem.
// São as mesmas regras do backend, para avisar antes de enviar.
export function validarOportunidade(dados) {
  if (!dados.titulo.trim()) return 'O título não pode estar vazio.';
  if (!dados.descricao.trim()) return 'A descrição não pode estar vazia.';
  if (!dados.idCliente) return 'Seleciona um cliente.';
  if (!dados.idCategoria) return 'Seleciona uma categoria.';

  // Datas no formato aaaa-mm-dd: comparar como texto dá a ordem certa
  if (dados.prazoProposta && dados.prazoProposta <= hoje())
    return 'O prazo da proposta deve ser uma data futura.';

  if (dados.prazoProposta && dados.dataPretendida &&
    dados.dataPretendida < dados.prazoProposta)
    return 'A data pretendida deve ser igual ou posterior ao prazo da proposta.';

  return '';
}

// O responsável não é enviado: o backend usa quem fez login
export function criarOportunidade(dados) {
  return apiRequest('/api/oportunidades', {
    method: 'POST',
    body: JSON.stringify({
      idCliente: dados.idCliente,
      idCategoria: dados.idCategoria,
      titulo: dados.titulo.trim(),
      descricao: dados.descricao.trim(),
      // Um input de data vazio dá '' e o backend espera null
      prazoProposta: dados.prazoProposta || null,
      dataPretendida: dados.dataPretendida || null,
    }),
  });
}

// ----- Mudanças de estado -----

function mudarEstado(id, acao) {
  return apiRequest(`/api/oportunidades/${id}/${acao}`, { method: 'PUT' });
}

export const iniciarAnalise = id => mudarEstado(id, 'iniciar-analise');
export const iniciarPreparacao = id => mudarEstado(id, 'iniciar-preparacao');
export const marcarComoPerdida = id => mudarEstado(id, 'perdida');
export const cancelarOportunidade = id => mudarEstado(id, 'cancelar');

// ----- Datas -----

export function hoje() {
  const d = new Date();
  const mes = String(d.getMonth() + 1).padStart(2, '0');
  const dia = String(d.getDate()).padStart(2, '0');
  return `${d.getFullYear()}-${mes}-${dia}`;
}

export function formatarData(data) {
  return data ? new Date(data).toLocaleDateString('pt-PT') : '—';
}

function calcularAviso(prazo, estado) {
  if (!prazo || !ESTADOS_COM_PRAZO.includes(estado)) return null;

  const diaPrazo = new Date(prazo).setHours(0, 0, 0, 0);
  const diaHoje = new Date().setHours(0, 0, 0, 0);
  const dias = Math.round((diaPrazo - diaHoje) / 86_400_000);

  if (dias < 0) return { texto: 'Atrasado', cor: 'red' };
  if (dias === 0) return { texto: 'Hoje', cor: 'yellow' };
  if (dias === 1) return { texto: 'Amanhã', cor: 'yellow' };
  if (dias <= 7) return { texto: `Em ${dias} dias`, cor: 'yellow' };
  return null;
}
