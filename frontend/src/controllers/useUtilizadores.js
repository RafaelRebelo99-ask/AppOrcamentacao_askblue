import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../models/loginModel';
import { sessaoInvalida } from '../models/oportunidadesModel';
import {
  apagarUtilizador,
  criarUtilizador,
  eAdministrador,
  listarUtilizadores,
  mensagemDeErroUtilizador,
  validarUtilizador,
} from '../models/utilizadoresModel';

const FORM_VAZIO = { nome: '', email: '', role: '', password: '' };

export default function useUtilizadores() {
  const navigate = useNavigate();

  // Lista
  const [utilizadores, setUtilizadores] = useState([]);
  const [roleSelecionada, setRoleSelecionada] = useState('Todos');
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');
  const [idEmCurso, setIdEmCurso] = useState(null); // conta que está a ser apagada

  // Formulário
  const [formAberto, setFormAberto] = useState(false);
  const [dados, setDados] = useState(FORM_VAZIO);
  const [aCriar, setACriar] = useState(false);
  const [erroForm, setErroForm] = useState('');
  const [sucesso, setSucesso] = useState('');

  // Carregar a lista ao abrir a página
  useEffect(() => {
    // Só o Administrador gere contas; os outros voltam às oportunidades
    if (!eAdministrador()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    listarUtilizadores()
      .then(lista => {
        if (ativo) setUtilizadores(lista);
      })
      .catch(err => {
        if (!ativo) return;

        if (sessaoInvalida(err)) {
          limparSessao();
          navigate('/login', { replace: true });
          return;
        }
        setErro(mensagemDeErroUtilizador(err));
      })
      .finally(() => {
        if (ativo) setLoading(false);
      });

    return () => { ativo = false; };
  }, [navigate]);

  const contar = role => utilizadores.filter(u => u.role === role).length;

  const contagens = {
    Todos: utilizadores.length,
    Administrador: contar('Administrador'),
    Colaborador: contar('Colaborador'),
    Cliente: contar('Cliente'),
  };

  const utilizadoresFiltrados = utilizadores.filter(u =>
    roleSelecionada === 'Todos' ||
    u.role === roleSelecionada
  );

  const handleAbrirForm = () => {
    setSucesso('');
    setFormAberto(true);
  };

  const handleFecharForm = () => {
    setDados(FORM_VAZIO);
    setErroForm('');
    setFormAberto(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  // Criar a conta e atualizar a tabela
  const handleSubmit = async (e) => {
    e.preventDefault(); // impede o browser de recarregar a página

    const problema = validarUtilizador(dados);
    if (problema) {
      setErroForm(problema);
      return;
    }

    setErroForm('');
    setACriar(true);

    try {
      const criado = await criarUtilizador(dados);

      handleFecharForm();
      setRoleSelecionada('Todos'); // para a conta nova não ficar escondida pelo filtro
      setSucesso(`Conta de ${criado.nome} criada.`);

      // A conta já existe: se só a tabela falhar, o erro aparece na página e não no formulário
      await listarUtilizadores()
        .then(setUtilizadores)
        .catch(err => setErro(mensagemDeErroUtilizador(err)));
    } catch (err) {
      if (sessaoInvalida(err)) {
        limparSessao();
        navigate('/login', { replace: true });
        return;
      }
      setErroForm(mensagemDeErroUtilizador(err));
    } finally {
      setACriar(false);
    }
  };

  // Apagar a conta (não tem volta atrás: pede confirmação)
  const handleApagar = async (utilizador) => {
    if (!window.confirm(`Apagar a conta de ${utilizador.nome} (${utilizador.email})? Não é possível desfazer.`))
      return;

    setErro('');
    setSucesso('');
    setIdEmCurso(utilizador.id);

    try {
      await apagarUtilizador(utilizador.id);
      setUtilizadores(prev => prev.filter(u => u.id !== utilizador.id));
      setSucesso(`Conta de ${utilizador.nome} apagada.`);
    } catch (err) {
      if (sessaoInvalida(err)) {
        limparSessao();
        navigate('/login', { replace: true });
        return;
      }
      setErro(mensagemDeErroUtilizador(err));
    } finally {
      setIdEmCurso(null);
    }
  };

  return {
    utilizadoresFiltrados,
    roleSelecionada,
    setRoleSelecionada,
    contagens,
    loading,
    erro,
    sucesso,
    idEmCurso,
    handleApagar,

    formAberto,
    dados,
    aCriar,
    erroForm,
    handleAbrirForm,
    handleFecharForm,
    handleChange,
    handleSubmit,
  };
}
