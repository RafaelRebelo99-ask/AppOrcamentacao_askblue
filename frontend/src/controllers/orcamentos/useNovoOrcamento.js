import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { limparSessao } from '../../models/loginModel';
import {
  mensagemDeErro,
  obterOportunidade,
  podeGerirOportunidades,
  sessaoInvalida,
} from '../../models/oportunidadesModel';
import {
  atualizarOrcamento,
  calcularResumo,
  criarOrcamento,
  enviarOrcamento,
  validarOrcamento,
} from '../../models/orcamentosModel';

export default function useNovoOrcamento() {
  const { oportunidadeId } = useParams();
  const navigate = useNavigate();

  const [oportunidade, setOportunidade] = useState(null);
  const [orcamentoCriadoId, setOrcamentoCriadoId] = useState(null); // rascunho já guardado

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
  const [sucesso, setSucesso] = useState('');
  const [confirmarEnvio, setConfirmarEnvio] = useState(false);

  // Carregar a oportunidade ao abrir a página
  useEffect(() => {
    // Só Colaborador e Administrador criam propostas
    if (!podeGerirOportunidades()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    obterOportunidade(oportunidadeId)
      .then(resposta => {
        if (ativo) setOportunidade(resposta);
      })
      .catch(err => {
        if (!ativo) return;

        if (sessaoInvalida(err)) {
          limparSessao();
          navigate('/login', { replace: true });
          return;
        }
        setErro(err.status === 404
          ? 'Não foi possível encontrar a oportunidade.'
          : mensagemDeErro(err));
      })
      .finally(() => {
        if (ativo) setLoadingOportunidade(false);
      });

    return () => { ativo = false; };
  }, [oportunidadeId, navigate]);

  const resumo = calcularResumo(dados);

  const handleChange = (event) => {
    const { name, value } = event.target;

    setSucesso('');
    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  // Cria o rascunho, ou atualiza-o se já tiver sido guardado. Devolve o id.
  const guardar = async () => {
    if (orcamentoCriadoId) {
      await atualizarOrcamento(orcamentoCriadoId, dados);
      return orcamentoCriadoId;
    }

    const criado = await criarOrcamento(oportunidadeId, dados);
    setOrcamentoCriadoId(criado.id);
    return criado.id;
  };

  const tratarErro = (err, prefixo = '') => {
    if (sessaoInvalida(err)) {
      limparSessao();
      navigate('/login', { replace: true });
      return;
    }
    setErro(prefixo + mensagemDeErro(err));
  };

  const handleGuardarRascunho = async () => {
    setSucesso('');

    const problema = validarOrcamento(dados, { paraEnviar: false });
    if (problema) {
      setErro(problema);
      return;
    }

    setErro('');
    setLoading(true);

    try {
      await guardar();
      setSucesso('Rascunho guardado.');
    } catch (err) {
      tratarErro(err);
    } finally {
      setLoading(false);
    }
  };

  // Submeter o formulário: valida e pede confirmação, porque enviar não tem volta atrás
  const handleEnviar = (event) => {
    event.preventDefault(); // impede o browser de recarregar a página
    setSucesso('');

    const problema = validarOrcamento(dados, { paraEnviar: true });
    if (problema) {
      setErro(problema);
      return;
    }

    setErro('');
    setConfirmarEnvio(true);
  };

  const handleConfirmarEnvio = async () => {
    setConfirmarEnvio(false);
    setLoading(true);

    let guardado = false;

    try {
      const id = await guardar();
      guardado = true;

      await enviarOrcamento(id);
      navigate('/oportunidades');
    } catch (err) {
      tratarErro(err, guardado ? 'O rascunho foi guardado, mas não foi enviado. ' : '');
    } finally {
      setLoading(false);
    }
  };

  const handleFecharConfirmacao = () => {
    setConfirmarEnvio(false);
  };

  const handleCancelar = () => {
    navigate('/oportunidades');
  };

  return {
    oportunidade,
    dados,
    resumo,
    loading,
    loadingOportunidade,
    erro,
    sucesso,
    confirmarEnvio,
    handleChange,
    handleGuardarRascunho,
    handleEnviar,
    handleConfirmarEnvio,
    handleFecharConfirmacao,
    handleCancelar,
  };
}
