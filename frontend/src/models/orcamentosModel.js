// ===== MODEL: orçamentos (propostas) =====
// Colaborador e Administrador: listar, criar, atualizar, enviar e eliminar propostas.
// Cliente: ver as que recebeu, aceitar e recusar.
import { apiRequest } from './api';
import { formatarData, hoje, listarOportunidades, obterRole } from './oportunidadesModel';

// O backend envia o estado como número, pela ordem do enum EstadoOrcamento (Orcamento.cs).
// Se um dia passar a enviar texto, também funciona.
const ESTADOS = ['Rascunho', 'Enviado', 'Aceite', 'Recusado'];

// Cada role tem o seu endpoint
const ENDPOINT_LISTA = {
  Administrador: '/api/orcamentos/todos', // todas
  Colaborador: '/api/orcamentos',         // as que criou
  Cliente: '/api/orcamentos/recebidos',   // as que recebeu (sem rascunhos)
};

export async function listarOrcamentos() {
  const [orcamentos, oportunidades] = await Promise.all([
    apiRequest(ENDPOINT_LISTA[obterRole()] ?? ENDPOINT_LISTA.Cliente),
    listarOportunidades(),
  ]);

  // A API das propostas só envia o id da oportunidade
  const oportunidadePorId = new Map(oportunidades.map(o => [o.id, o]));

  return orcamentos.map(o => {
    const oportunidade = oportunidadePorId.get(o.idOportunidade);
    const estado = typeof o.estado === 'number' ? ESTADOS[o.estado] : o.estado;

    return {
      id: o.id,
      numero: o.id.slice(0, 8).toUpperCase(), // igual ao do PDF
      estado,
      expirado: o.expirado,
      // O cliente só decide sobre uma proposta enviada, dentro da validade e com a oportunidade em aberto
      podeDecidir: estado === 'Enviado' && !o.expirado && oportunidade?.estado === 'PropostaEnviada',
      oportunidade: oportunidade?.titulo ?? '—',
      cliente: oportunidade?.cliente ?? '—',
      descricao: o.descricao,
      taxaIVA: o.taxaIVA,
      criadaEm: formatarData(o.dataCriacao),
      validade: formatarData(o.dataValidade),
      inicio: formatarData(o.dataInicio),
      fim: formatarData(o.dataFim),
      valorSemIVA: formatarEuros(o.valorSemIVA),
      valorIVA: formatarEuros(o.valorTotal - o.valorSemIVA),
      valorTotal: formatarEuros(o.valorTotal),

      // para ordenar
      criadaTempo: new Date(o.dataCriacao).getTime(),
      total: o.valorTotal,
    };
  });
}

// Devolve uma mensagem de erro, ou '' se estiver tudo bem (regras iguais às do backend).
// Um rascunho pode ter datas passadas; para enviar, não.
export function validarOrcamento(dados, { paraEnviar }) {
  const descricao = dados.descricao.trim();
  const valor = Number(dados.valorSemIVA);
  const duracao = Number(dados.duracaoDias);

  if (!descricao) return 'A descrição da proposta é obrigatória.';
  if (descricao.length > 4000) return 'A descrição não pode ter mais de 4000 caracteres.';
  if (!(valor > 0)) return 'O valor tem de ser positivo.';
  if (!dados.dataInicio) return 'A data de início é obrigatória.';
  if (!Number.isInteger(duracao) || duracao <= 0) return 'A duração tem de ser um número inteiro de dias.';
  if (!dados.dataValidade) return 'A validade da proposta é obrigatória.';

  if (paraEnviar) {
    // Datas no formato aaaa-mm-dd: comparar como texto dá a ordem certa
    if (dados.dataValidade <= hoje()) return 'A validade tem de ser uma data futura.';
    if (dados.dataInicio < hoje()) return 'A data de início não pode estar no passado.';
  }

  return '';
}

// Pré-visualização enquanto se escreve. Os valores que contam são os que o backend calcula.
export function calcularResumo(dados) {
  const valor = Number(dados.valorSemIVA);
  const duracao = Number(dados.duracaoDias);

  let valorIVA = '—';
  let valorTotal = '—';
  let dataFim = '—';

  if (valor > 0) {
    const total = Math.round((valor * (1 + Number(dados.taxaIVA) / 100) + Number.EPSILON) * 100) / 100;
    valorIVA = formatarEuros(total - valor);
    valorTotal = formatarEuros(total);
  }

  if (dados.dataInicio && duracao > 0) {
    const [ano, mes, dia] = dados.dataInicio.split('-').map(Number);
    dataFim = formatarData(new Date(ano, mes - 1, dia + duracao));
  }

  return { valorIVA, valorTotal, dataFim };
}

function formatarEuros(valor) {
  return valor.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
}

function corpo(dados) {
  return {
    descricao: dados.descricao.trim(),
    valorSemIVA: Number(dados.valorSemIVA),
    taxaIVA: Number(dados.taxaIVA),
    dataValidade: dados.dataValidade,
    dataInicio: dados.dataInicio,
    duracaoDias: Number(dados.duracaoDias),
  };
}

// Cria em rascunho. O criador não é enviado: o backend usa quem fez login.
export function criarOrcamento(idOportunidade, dados) {
  return apiRequest('/api/orcamentos', {
    method: 'POST',
    body: JSON.stringify({ idOportunidade, ...corpo(dados) }),
  });
}

// Só enquanto for rascunho
export function atualizarOrcamento(id, dados) {
  return apiRequest(`/api/orcamentos/${id}`, {
    method: 'PUT',
    body: JSON.stringify(corpo(dados)),
  });
}

// Rascunho -> Enviado. Depois de enviado já não pode ser editado.
export function enviarOrcamento(id) {
  return apiRequest(`/api/orcamentos/${id}/enviar`, { method: 'POST' });
}

// Devolve o ficheiro PDF (gerado no backend)
export function obterPdf(id) {
  return apiRequest(`/api/orcamentos/${id}/pdf`, { comoBlob: true });
}

// Aceitar: a oportunidade fica Ganha e as outras propostas enviadas são recusadas (feito no backend)
export function aceitarOrcamento(id) {
  return apiRequest(`/api/orcamentos/${id}/aceitar`, { method: 'POST' });
}

export function recusarOrcamento(id) {
  return apiRequest(`/api/orcamentos/${id}/recusar`, { method: 'POST' });
}

// O backend só deixa eliminar rascunhos
export function eliminarOrcamento(id) {
  return apiRequest(`/api/orcamentos/${id}`, { method: 'DELETE' });
}
