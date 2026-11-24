// ===== MODEL: utilizadores =====
// Listar e criar contas. No backend, estas ações são do Administrador.
import { apiRequest } from './api';
import { mensagemDeErro, obterRole } from './oportunidadesModel';

// O backend envia a role como número, pela ordem do enum Role (Utilizador.cs).
// Se um dia passar a enviar texto, também funciona.
const ROLES = ['Colaborador', 'Administrador', 'Cliente'];

export function eAdministrador() {
  return obterRole() === 'Administrador';
}

// Os erros deste endpoint vêm como texto JSON, entre aspas: "Já existe..."
export function mensagemDeErroUtilizador(erro) {
  const mensagem = mensagemDeErro(erro);
  try {
    const texto = JSON.parse(mensagem);
    return typeof texto === 'string' ? texto : mensagem;
  } catch {
    return mensagem;
  }
}

// Todos os utilizadores, por ordem alfabética
export async function listarUtilizadores() {
  const utilizadores = await apiRequest('/api/utilizadores');

  return utilizadores
    .map(u => ({
      id: u.id,
      nome: u.nome,
      email: u.email,
      role: typeof u.role === 'number' ? ROLES[u.role] : u.role,
    }))
    .sort((a, b) => a.nome.localeCompare(b.nome, 'pt'));
}

// O backend recusa apagar administradores e contas com oportunidades ou orçamentos
export function apagarUtilizador(id) {
  return apiRequest(`/api/utilizadores/${id}`, { method: 'DELETE' });
}

// Devolve uma mensagem de erro, ou '' se estiver tudo bem (regras iguais às do backend)
export function validarUtilizador(dados) {
  const nome = dados.nome.trim();
  if (nome.length < 2 || nome.length > 100) return 'O nome deve ter entre 2 e 100 caracteres.';
  if (!dados.email.trim()) return 'O email é obrigatório.';
  if (!['Cliente', 'Colaborador'].includes(dados.role)) return 'Seleciona a função.';
  if (!dados.password.trim() || dados.password.length < 8)
    return 'A password deve ter pelo menos 8 caracteres.';
  return '';
}

// Os administradores não se criam aqui: vêm dos user-secrets (AdminSeeder)
export function criarUtilizador(dados) {
  const conta = {
    nome: dados.nome.trim(),
    email: dados.email.trim(),
    password: dados.password,
  };

  // O colaborador tem um endpoint próprio, que o associa à AskBlue
  if (dados.role === 'Colaborador') {
    return apiRequest('/api/utilizadores/colaboradores', {
      method: 'POST',
      body: JSON.stringify(conta),
    });
  }

  return apiRequest('/api/utilizadores', {
    method: 'POST',
    body: JSON.stringify({ ...conta, role: ROLES.indexOf('Cliente') }),
  });
}
