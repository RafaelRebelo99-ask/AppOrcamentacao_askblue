import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../models/loginModel';
import {
  cancelarOportunidade,
  iniciarAnalise,
  iniciarPreparacao,
  listarOportunidades,
  marcarComoPerdida,
  mensagemDeErro,
  podeGerirOportunidades,
  sessaoInvalida,
} from '../models/oportunidadesModel';

// Oportunidades sem prazo ficam sempre no fim, qualquer que seja a ordem
function compararPrazo(a, b, sentido) {
  if (a.prazoTempo === null && b.prazoTempo === null) return 0;
  if (a.prazoTempo === null) return 1;
  if (b.prazoTempo === null) return -1;
  return (a.prazoTempo - b.prazoTempo) * sentido;
}

export default function useOportunidadesController() {
  const navigate = useNavigate();

  const [oportunidades, setOportunidades] = useState([]);
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todas');
  const [ordenacao, setOrdenacao] = useState('prazo-asc');

  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');
  const [idEmCurso, setIdEmCurso] = useState(null); // oportunidade com uma ação a decorrer

  const [podeGerir] = useState(podeGerirOportunidades);

  // Carregar a lista ao abrir a página
  useEffect(() => {
    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    listarOportunidades()
      .then(lista => {
        if (ativo) setOportunidades(lista);
      })
      .catch(err => {
        if (!ativo) return;

        if (sessaoInvalida(err)) {
          limparSessao();
          navigate('/login', { replace: true });
          return;
        }
        setErro(mensagemDeErro(err));
      })
      .finally(() => {
        if (ativo) setLoading(false);
      });

    return () => { ativo = false; };
  }, [navigate]);

  const contar = estado => oportunidades.filter(o => o.estado === estado).length;

  const contagens = {
    Todas: oportunidades.length,
    Registada: contar('Registada'),
    EmAnalise: contar('EmAnalise'),
    EmPreparacao: contar('EmPreparacao'),
    PropostaEnviada: contar('PropostaEnviada'),
    Ganha: contar('Ganha'),
    Perdida: contar('Perdida'),
    Cancelada: contar('Cancelada'),
  };

  const oportunidadesFiltradas = oportunidades
    .filter(o =>
      estadoSelecionado === 'Todas' ||
      o.estado === estadoSelecionado
    )
    .sort((a, b) => compararPrazo(a, b, ordenacao === 'prazo-desc' ? -1 : 1));

  // Chama a API e volta a carregar a lista, para mostrar o estado que ficou no backend
  const executar = async (oportunidade, acao) => {
    setErro('');
    setIdEmCurso(oportunidade.id);

    try {
      await acao(oportunidade.id);
      setOportunidades(await listarOportunidades());
    } catch (err) {
      if (sessaoInvalida(err)) {
        limparSessao();
        navigate('/login', { replace: true });
        return;
      }
      setErro(mensagemDeErro(err));
    } finally {
      setIdEmCurso(null);
    }
  };

  const handleIniciarAnalise = oportunidade =>
    executar(oportunidade, iniciarAnalise);

  const handleIniciarPreparacao = oportunidade =>
    executar(oportunidade, iniciarPreparacao);

  // Perdida e Cancelada não têm volta atrás: pede confirmação
  const handlePerdida = (oportunidade) => {
    if (window.confirm(`Marcar "${oportunidade.titulo}" como perdida? Não é possível desfazer.`))
      executar(oportunidade, marcarComoPerdida);
  };

  const handleCancelar = (oportunidade) => {
    if (window.confirm(`Cancelar "${oportunidade.titulo}"? Não é possível desfazer.`))
      executar(oportunidade, cancelarOportunidade);
  };

  return {
    oportunidades,
    oportunidadesFiltradas,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    loading,
    erro,
    idEmCurso,
    podeGerir,
    handleIniciarAnalise,
    handleIniciarPreparacao,
    handlePerdida,
    handleCancelar,
  };
}
