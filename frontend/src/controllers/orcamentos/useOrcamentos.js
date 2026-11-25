import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../../models/loginModel';
import {
  mensagemDeErro,
  podeGerirOportunidades,
  sessaoInvalida,
} from '../../models/oportunidadesModel';
import {
  aceitarOrcamento,
  eliminarOrcamento,
  enviarOrcamento,
  listarOrcamentos,
  recusarOrcamento,
} from '../../models/orcamentosModel';

// O que cada ação chama na API, depois de confirmada
const ACOES = {
  enviar: enviarOrcamento,
  eliminar: eliminarOrcamento,
  aceitar: aceitarOrcamento,
  recusar: recusarOrcamento,
};

export default function useOrcamentos() {
  const navigate = useNavigate();

  // Colaborador e Administrador gerem as propostas; o Cliente aceita ou recusa
  const [podeGerir] = useState(podeGerirOportunidades);

  const [orcamentos, setOrcamentos] = useState([]);
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todos');
  const [ordenacao, setOrdenacao] = useState('data-desc');
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');

  const [acaoPendente, setAcaoPendente] = useState(null); // { tipo, orcamento } à espera de confirmação
  const [idEmCurso, setIdEmCurso] = useState(null);       // proposta com uma ação a decorrer

  // Carregar a lista ao abrir a página
  useEffect(() => {
    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    listarOrcamentos()
      .then(lista => {
        if (ativo) setOrcamentos(lista);
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

  const contar = estado => orcamentos.filter(o => o.estado === estado).length;

  const contagens = {
    Todos: orcamentos.length,
    Rascunho: contar('Rascunho'),
    Enviado: contar('Enviado'),
    Aceite: contar('Aceite'),
    Recusado: contar('Recusado'),
  };

  const orcamentosFiltrados = orcamentos
    .filter(orcamento =>
      estadoSelecionado === 'Todos' ||
      orcamento.estado === estadoSelecionado
    )
    .sort((a, b) => {
      switch (ordenacao) {
        case 'data-asc':
          return a.criadaTempo - b.criadaTempo;

        case 'data-desc':
          return b.criadaTempo - a.criadaTempo;

        case 'valor-asc':
          return a.total - b.total;

        case 'valor-desc':
          return b.total - a.total;

        default:
          return 0;
      }
    });

  // Nenhuma destas ações tem volta atrás: primeiro pede-se confirmação
  const handleEnviar = orcamento => setAcaoPendente({ tipo: 'enviar', orcamento });
  const handleEliminar = orcamento => setAcaoPendente({ tipo: 'eliminar', orcamento });
  const handleAceitar = orcamento => setAcaoPendente({ tipo: 'aceitar', orcamento });
  const handleRecusar = orcamento => setAcaoPendente({ tipo: 'recusar', orcamento });
  const handleFecharConfirmacao = () => setAcaoPendente(null);

  const handleConfirmar = async () => {
    const { tipo, orcamento } = acaoPendente;

    setAcaoPendente(null);
    setErro('');
    setIdEmCurso(orcamento.id);

    try {
      await ACOES[tipo](orcamento.id);

      // Volta a carregar, para mostrar o que ficou no backend
      setOrcamentos(await listarOrcamentos());
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

  return {
    podeGerir,
    orcamentosFiltrados,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    loading,
    erro,
    idEmCurso,
    acaoPendente,
    handleEnviar,
    handleEliminar,
    handleAceitar,
    handleRecusar,
    handleConfirmar,
    handleFecharConfirmacao,
  };
}
