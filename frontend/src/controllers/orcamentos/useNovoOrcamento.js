import { useCallback, useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

function obterToken() {
  return localStorage.getItem('accessToken') ||
    sessionStorage.getItem('accessToken');
}

export default function useNovoOrcamento() {
  const { oportunidadeId } = useParams();
  const navigate = useNavigate();

  const [oportunidade, setOportunidade] = useState(null);
  const [orcamentoCriadoId, setOrcamentoCriadoId] = useState(null);

  const [dados, setDados] = useState({
    descricao: '',
    valorSemIVA: '',
    taxaIVA: '23',
    dataValidade: '',
    dataInicio: '',
    duracaoDias: '',
  });

  const [loading, setLoading] = useState(false);
  const [loadingOportunidade, setLoadingOportunidade] = useState(true);
  const [erro, setErro] = useState('');

  const carregarOportunidade = useCallback(async () => {
    const token = obterToken();

    if (!token || !oportunidadeId) {
      setErro('Não foi possível identificar a sessão ou a oportunidade.');
      setLoadingOportunidade(false);
      return;
    }

    try {
      const response = await fetch(
        '/api/oportunidades/' + oportunidadeId,
        {
          headers: {
            Authorization: 'Bearer ' + token,
          },
        }
      );

      if (!response.ok) {
        throw new Error('Não foi possível carregar a oportunidade.');
      }

      const data = await response.json();
      setOportunidade(data);
    } catch (error) {
      setErro(error.message);
    } finally {
      setLoadingOportunidade(false);
    }
  }, [oportunidadeId]);

  useEffect(() => {
    carregarOportunidade();
  }, [carregarOportunidade]);

  const handleChange = (event) => {
    const { name, value } = event.target;

    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const submeter = async (enviar) => {
    const token = obterToken();

    if (!token || !oportunidadeId) {
      setErro('Sessão ou oportunidade não identificada.');
      return;
    }

    setLoading(true);
    setErro('');

    try {
      let idOrcamento = orcamentoCriadoId;

      // Se ainda não existe rascunho, criar primeiro.
      if (!idOrcamento) {
        const pedido = {
          idOportunidade: oportunidadeId,
          descricao: dados.descricao.trim(),
          valorSemIVA: Number(dados.valorSemIVA),
          taxaIVA: Number(dados.taxaIVA),
          dataValidade: dados.dataValidade,
          dataInicio: dados.dataInicio,
          duracaoDias: Number(dados.duracaoDias),
        };

        const response = await fetch('/api/orcamentos/', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: 'Bearer ' + token,
          },
          body: JSON.stringify(pedido),
        });

        if (!response.ok) {
          const body = await response.json().catch(() => null);
          throw new Error(
            body?.erro || 'Não foi possível criar a proposta.'
          );
        }

        const criado = await response.json();

        idOrcamento = criado.id;
        setOrcamentoCriadoId(criado.id);
      }

      // Enviar é uma operação separada da criação.
      if (enviar) {
        const response = await fetch(
          '/api/orcamentos/' + idOrcamento + '/enviar',
          {
            method: 'POST',
            headers: {
              Authorization: 'Bearer ' + token,
            },
          }
        );

        if (!response.ok) {
          const body = await response.json().catch(() => null);
          throw new Error(
            body?.erro ||
            'O rascunho foi guardado, mas não foi possível enviá-lo.'
          );
        }
      }

      navigate('/orcamentos');
    } catch (error) {
      setErro(error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleGuardarRascunho = () => {
    submeter(false);
  };

  const handleEnviar = (event) => {
    event.preventDefault();
    submeter(true);
  };

  const handleCancelar = () => {
    navigate('/oportunidades');
  };

  return {
    oportunidade,
    dados,
    loading,
    loadingOportunidade,
    erro,
    handleChange,
    handleGuardarRascunho,
    handleEnviar,
    handleCancelar,
  };
}