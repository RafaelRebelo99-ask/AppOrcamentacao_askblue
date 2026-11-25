
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../models/loginModel';
import { sessaoInvalida } from '../models/oportunidadesModel';

import {
  cancelarOportunidade,
  iniciarAnalise,
  iniciarPreparacao,
  listarOportunidades,
  listarClientes,
  marcarComoPerdida,
  mensagemDeErro,
  podeGerirOportunidades,
} from '../models/oportunidadesModel';

import { listarCategorias } from '../models/categoriasModel';

// Oportunidades sem prazo ficam sempre no fim
function compararPrazo(a, b, sentido) {
  if (a.prazoTempo === null && b.prazoTempo === null) return 0;
  if (a.prazoTempo === null) return 1;
  if (b.prazoTempo === null) return -1;

  return (a.prazoTempo - b.prazoTempo) * sentido;
}

export default function useOportunidadesController() {
  const navigate = useNavigate();

  const [oportunidades, setOportunidades] = useState([]);
  const [categorias, setCategorias] = useState([]);
  const [clientes, setClientes] = useState([]);

  const [estadoSelecionado, setEstadoSelecionado] = useState('Todas');
  const [categoriaSelecionada, setCategoriaSelecionada] = useState('Todas');
  const [clienteSelecionado, setClienteSelecionado] = useState('Todos');
  const [ordenacao, setOrdenacao] = useState('prazo-asc');

  const [loading, setLoading] = useState(true);
  const [loadingCategorias, setLoadingCategorias] = useState(true);
  const [loadingClientes, setLoadingClientes] = useState(true);

  const [erro, setErro] = useState('');
  const [idEmCurso, setIdEmCurso] = useState(null);

  const [podeGerir] = useState(podeGerirOportunidades);

  // Carregar oportunidades
  useEffect(() => {
    let ativo = true;

    listarOportunidades()
      .then(lista => {
        if (ativo) {
          setOportunidades(lista);
        }
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

    return () => {
      ativo = false;
    };
  }, [navigate]);

  // Carregar categorias
  useEffect(() => {
    let ativo = true;

    listarCategorias()
      .then(lista => {
        if (!ativo) return;

        const categoriasFormatadas = lista.map(categoria => ({
          label: categoria.nome ?? categoria.Nome,
          value: String(categoria.id ?? categoria.Id),
        }));

        setCategorias([
          { label: 'Todas as categorias', value: 'Todas' },
          ...categoriasFormatadas,
        ]);
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
        if (ativo) setLoadingCategorias(false);
      });

    return () => {
      ativo = false;
    };
  }, [navigate]);

  // Carregar clientes
  useEffect(() => {
    let ativo = true;

    listarClientes()
      .then(lista => {
        if (!ativo) return;

        const clientesFormatados = lista.map(cliente => ({
          label: cliente.nome ?? cliente.Nome,
          value: String(cliente.id ?? cliente.Id),
        }));

        setClientes([
          { label: 'Todos os clientes', value: 'Todos' },
          ...clientesFormatados,
        ]);
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
        if (ativo) setLoadingClientes(false);
      });

    return () => {
      ativo = false;
    };
  }, [navigate]);

  // Contagens por estado
  const contar = estado =>
    oportunidades.filter(o => o.estado === estado).length;

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

  // Filtrar por estado, categoria e cliente
  const oportunidadesFiltradas = oportunidades
    .filter(o =>
      estadoSelecionado === 'Todas' ||
      o.estado === estadoSelecionado
    )
    .filter(o =>
      categoriaSelecionada === 'Todas' ||
      String(o.idCategoria) === String(categoriaSelecionada)
    )
    .filter(o =>
      clienteSelecionado === 'Todos' ||
      String(o.idCliente) === String(clienteSelecionado)
    )
    .sort((a, b) =>
      compararPrazo(
        a,
        b,
        ordenacao === 'prazo-desc' ? -1 : 1
      )
    );

  // Executar ação e atualizar lista
  const executar = async (oportunidade, acao) => {
    setErro('');
    setIdEmCurso(oportunidade.id);

    try {
      await acao(oportunidade.id);

      const lista = await listarOportunidades();
      setOportunidades(lista);
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

  const handleCriarOrcamento = oportunidade =>
    navigate(`/orcamentos/novo/${oportunidade.id}`);

  // Perdida e Cancelada não têm volta atrás: pede confirmação
  const handlePerdida = oportunidade => {
    if (
      window.confirm(
        `Marcar "${oportunidade.titulo}" como perdida? Não é possível desfazer.`
      )
    ) {
      executar(oportunidade, marcarComoPerdida);
    }
  };

  const handleCancelar = oportunidade => {
    if (
      window.confirm(
        `Cancelar "${oportunidade.titulo}"? Não é possível desfazer.`
      )
    ) {
      executar(oportunidade, cancelarOportunidade);
    }
  };

  return {
    oportunidades,
    oportunidadesFiltradas,

    estadoSelecionado,
    setEstadoSelecionado,

    categoriaSelecionada,
    setCategoriaSelecionada,

    clienteSelecionado,
    setClienteSelecionado,

    ordenacao,
    setOrdenacao,

    categorias,
    loadingCategorias,

    clientes,
    loadingClientes,

    contagens,
    loading,
    erro,
    idEmCurso,
    podeGerir,

    handleIniciarAnalise,
    handleIniciarPreparacao,
    handleCriarOrcamento,
    handlePerdida,
    handleCancelar,
  };
}
