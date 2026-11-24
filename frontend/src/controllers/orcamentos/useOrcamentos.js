import { useCallback, useEffect, useMemo, useState } from 'react';

function obterToken() {
  return localStorage.getItem('accessToken') ||
    sessionStorage.getItem('accessToken');
}

function obterRole() {
  const utilizador = localStorage.getItem('utilizador') ||
    sessionStorage.getItem('utilizador');

  if (!utilizador) return null;

  try {
    return JSON.parse(utilizador).role;
  } catch {
    return null;
  }
}

function timestamp(data) {
  return data ? new Date(data).getTime() : 0;
}

export default function useOrcamentos() {
  const [orcamentos, setOrcamentos] = useState([]);
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todos');
  const [ordenacao, setOrdenacao] = useState('data-desc');
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  const carregarOrcamentos = useCallback(async () => {
    const token = obterToken();

    if (!token) {
      setErro('Sessão não encontrada. Inicia sessão novamente.');
      return;
    }

    setLoading(true);
    setErro('');

    try {
      const role = obterRole();

      const endpoint = role === 'Administrador'
        ? '/api/orcamentos/todos'
        : '/api/orcamentos/';

      const response = await fetch(endpoint, {
        headers: {
          Authorization: 'Bearer ' + token,
        },
      });

      if (!response.ok) {
        const body = await response.json().catch(() => null);
        throw new Error(
          body?.erro || 'Não foi possível carregar as propostas.'
        );
      }

      const data = await response.json();
      setOrcamentos(Array.isArray(data) ? data : []);
    } catch (error) {
      setErro(error.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregarOrcamentos();
  }, [carregarOrcamentos]);

  const contagens = useMemo(() => ({
    Todos: orcamentos.length,
    Rascunho: orcamentos.filter(o => o.estado === 'Rascunho').length,
    Enviado: orcamentos.filter(o => o.estado === 'Enviado').length,
    Aceite: orcamentos.filter(o => o.estado === 'Aceite').length,
    Recusado: orcamentos.filter(o => o.estado === 'Recusado').length,
  }), [orcamentos]);

  const orcamentosFiltrados = useMemo(() => {
    return [...orcamentos]
      .filter(orcamento =>
        estadoSelecionado === 'Todos' ||
        orcamento.estado === estadoSelecionado
      )
      .sort((a, b) => {
        switch (ordenacao) {
          case 'data-asc':
            return timestamp(a.dataCriacao) - timestamp(b.dataCriacao);

          case 'data-desc':
            return timestamp(b.dataCriacao) - timestamp(a.dataCriacao);

          case 'valor-asc':
            return Number(a.valorTotal) - Number(b.valorTotal);

          case 'valor-desc':
            return Number(b.valorTotal) - Number(a.valorTotal);

          default:
            return 0;
        }
      });
  }, [orcamentos, estadoSelecionado, ordenacao]);

  const handleEliminar = async (orcamento) => {
    if (orcamento.estado !== 'Rascunho') {
      setErro('Só é possível eliminar propostas em rascunho.');
      return;
    }

    if (!window.confirm('Queres mesmo eliminar este rascunho?')) {
      return;
    }

    const token = obterToken();

    setErro('');

    try {
      const response = await fetch(
        '/api/orcamentos/' + orcamento.id,
        {
          method: 'DELETE',
          headers: {
            Authorization: 'Bearer ' + token,
          },
        }
      );

      if (!response.ok) {
        const body = await response.json().catch(() => null);
        throw new Error(
          body?.erro || 'Não foi possível eliminar a proposta.'
        );
      }

      setOrcamentos(prev =>
        prev.filter(o => o.id !== orcamento.id)
      );
    } catch (error) {
      setErro(error.message);
    }
  };

  return {
    orcamentosFiltrados,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    loading,
    erro,
    carregarOrcamentos,
    handleEliminar,
  };
}