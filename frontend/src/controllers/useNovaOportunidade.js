import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../models/loginModel';
import {
  criarOportunidade,
  listarCategorias,
  listarClientes,
  mensagemDeErro,
  podeGerirOportunidades,
  sessaoInvalida,
  validarOportunidade,
} from '../models/oportunidadesModel';

export default function useNovaOportunidade() {
  const navigate = useNavigate();

  // Dados do formulário
  const [dados, setDados] = useState({
    idCliente: '',
    idCategoria: '',
    titulo: '',
    descricao: '',
    prazoProposta: '',
    dataPretendida: '',
  });

  // Dados provenientes do backend
  const [clientes, setClientes] = useState([]);
  const [categorias, setCategorias] = useState([]);

  // Estados da página
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  // Carregar clientes e categorias ao abrir a página
  useEffect(() => {
    // Quem não pode criar (Cliente, ou sem sessão) não fica nesta página
    if (!podeGerirOportunidades()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    Promise.all([listarClientes(), listarCategorias()])
      .then(([listaClientes, listaCategorias]) => {
        if (!ativo) return;
        setClientes(listaClientes);
        setCategorias(listaCategorias);

        if (listaClientes.length === 0)
          setErro('Ainda não existem clientes. É preciso criar um antes de registar a oportunidade.');
      })
      .catch(err => {
        if (!ativo) return;

        if (sessaoInvalida(err)) {
          limparSessao();
          navigate('/login', { replace: true });
          return;
        }
        setErro(mensagemDeErro(err));
      });

    return () => { ativo = false; };
  }, [navigate]);

  // Atualizar campos do formulário
  const handleChange = (e) => {
    const { name, value } = e.target;

    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  // Registar oportunidade no backend
  const handleSubmit = async (e) => {
    e.preventDefault(); // impede o browser de recarregar a página

    const problema = validarOportunidade(dados);
    if (problema) {
      setErro(problema);
      return;
    }

    setErro('');
    setLoading(true);

    try {
      await criarOportunidade(dados);
      navigate('/oportunidades');
    } catch (err) {
      if (sessaoInvalida(err)) {
        limparSessao();
        navigate('/login', { replace: true });
        return;
      }
      setErro(mensagemDeErro(err));
    } finally {
      setLoading(false);
    }
  };

  // Cancelar criação da oportunidade
  const handleCancelar = () => {
    navigate('/oportunidades');
  };

  return {
    dados,
    clientes,
    categorias,
    loading,
    erro,
    handleChange,
    handleSubmit,
    handleCancelar,
  };
}
