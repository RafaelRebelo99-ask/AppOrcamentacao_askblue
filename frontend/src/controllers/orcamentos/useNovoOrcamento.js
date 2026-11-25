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
  obterOrcamento,
  validarOrcamento,
} from '../../models/orcamentosModel';
import { listarCategorias } from '../../models/categoriasModel';

// Serve para criar (/orcamentos/novo/:oportunidadeId) e para editar um rascunho (/orcamentos/:orcamentoId/editar)
export default function useNovoOrcamento() {
  const { oportunidadeId, orcamentoId } = useParams();
  const navigate = useNavigate();

  const modoEdicao = Boolean(orcamentoId);
  const paginaAnterior = modoEdicao ? '/orcamentos' : '/oportunidades';

  const [oportunidade, setOportunidade] = useState(null);
  const [orcamentoCriadoId, setOrcamentoCriadoId] = useState(orcamentoId ?? null); // rascunho já guardado

  const [categorias, setCategorias] = useState([]);

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

  // Carregar a oportunidade (e o rascunho, se for para editar) ao abrir a página
  useEffect(() => {
    if (!podeGerirOportunidades()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true;

    async function carregarDados() {
      try {
        const rascunho = orcamentoId ? await obterOrcamento(orcamentoId) : null;

        if (!ativo) return;

        if (rascunho && rascunho.estado !== 'Rascunho') {
          setErro('Só é possível editar propostas em rascunho.');
          return;
        }

        const [resposta, listaCategorias] = await Promise.all([
          obterOportunidade(rascunho?.idOportunidade ?? oportunidadeId),
          listarCategorias().catch(() => [])
        ]);

        if (!ativo) return;

        if (rascunho) setDados(rascunho.dados);

        const categoria = listaCategorias.find(
          c => String(c.id ?? c.Id) === String(resposta.categoria)
        );

        //console.log("categoria: ", categoria, "resposta", resposta.categoria)

        setOportunidade({
          ...resposta,
          categoria: categoria?.nome ?? categoria?.Nome ?? 'Categoria não encontrada'
        });

      } catch (erro) {
        if (!ativo) return;

        if (erro.status === 401) {
          limparSessao();
          navigate('/login');
          return;
        }

        if (erro.status === 404) {
          setErro(orcamentoId ? 'Proposta não encontrada.' : 'Oportunidade não encontrada.');
          return;
        }

        setErro(mensagemDeErro(erro));
      } finally {
        if (ativo) {
          setLoadingOportunidade(false);
        }
      }
    }

    carregarDados();

    return () => {
      ativo = false;
    };
  }, [oportunidadeId, orcamentoId, navigate]);

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
    const dadosParaEnviar = {
      ...dados,
      valorSemIVA: Number(
        String(dados.valorSemIVA ?? '')
          .replace(/[.\s\u00A0\u202F]/g, '')
          .replace(',', '.')
      )
};

if (orcamentoCriadoId) {
  await atualizarOrcamento(orcamentoCriadoId, dadosParaEnviar);
  return orcamentoCriadoId;
}

const criado = await criarOrcamento(oportunidadeId, dadosParaEnviar);
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
    navigate(paginaAnterior);
  } catch (err) {
    tratarErro(err, guardado ? 'O rascunho foi guardado, mas não foi enviado. ' : '');
  } finally {
    setLoading(false);
  }
};

const handleSelectChange = (name, value) => {
  setSucesso('');

  setDados(prev => ({
    ...prev,
    [name]: value,
  }));
};

const handleFecharConfirmacao = () => {
  setConfirmarEnvio(false);
};

const handleCancelar = () => {
  navigate(paginaAnterior);
};

return {
  modoEdicao,
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
  handleSelectChange
};
}
