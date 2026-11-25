
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { limparSessao } from '../models/loginModel';
import { sessaoInvalida } from '../models/oportunidadesModel';

import {
  apagarUtilizador,
  criarUtilizador,
  editarUtilizador,
  eAdministrador,
  listarUtilizadores,
  mensagemDeErroUtilizador,
  validarUtilizador,
} from '../models/utilizadoresModel';

const FORM_VAZIO = {
  nome: '',
  email: '',
  role: '',
  password: '',
};

export default function useUtilizadores() {
  const navigate = useNavigate();

  // Lista de utilizadores
  const [utilizadores, setUtilizadores] = useState([]);
  const [roleSelecionada, setRoleSelecionada] = useState('Todos');
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');
  const [idEmCurso, setIdEmCurso] = useState(null);

  // Formulário
  const [formAberto, setFormAberto] = useState(false);
  const [dados, setDados] = useState(FORM_VAZIO);
  const [aCriar, setACriar] = useState(false);
  const [erroForm, setErroForm] = useState('');
  const [sucesso, setSucesso] = useState('');

  // Utilizador que está a ser editado (null quando estamos a criar)
  const [utilizadorEmEdicao, setUtilizadorEmEdicao] = useState(null);

  // Carregar utilizadores ao abrir a página
  useEffect(() => {
    if (!eAdministrador()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true;

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

    return () => {
      ativo = false;
    };
  }, [navigate]);

  // Contagens por função
  const contar = role =>
    utilizadores.filter(u => u.role === role).length;

  const contagens = {
    Todos: utilizadores.length,
    Administrador: contar('Administrador'),
    Colaborador: contar('Colaborador'),
    Cliente: contar('Cliente'),
  };

  // Filtrar utilizadores
  const utilizadoresFiltrados = utilizadores.filter(u =>
    roleSelecionada === 'Todos' ||
    u.role === roleSelecionada
  );

  // Abrir formulário para criar
  const handleAbrirForm = () => {
    setUtilizadorEmEdicao(null);
    setDados(FORM_VAZIO);
    setErroForm('');
    setSucesso('');
    setFormAberto(true);
  };

  // Abrir formulário para editar, com os dados preenchidos
  const handleAbrirEdicao = (utilizador) => {
    setUtilizadorEmEdicao(utilizador);

    setDados({
      nome: utilizador.nome,
      email: utilizador.email,
      role: utilizador.role,
      password: '',
    });

    setErroForm('');
    setSucesso('');
    setFormAberto(true);
  };

  // Fechar formulário
  const handleFecharForm = () => {
    setDados(FORM_VAZIO);
    setErroForm('');
    setUtilizadorEmEdicao(null);
    setFormAberto(false);
  };

  // Atualizar campos do formulário
  const handleChange = (e) => {
    const { name, value } = e.target;

    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  // Criar ou editar utilizador
  const handleSubmit = async (e) => {
    e.preventDefault();

    // Validação para edição
    if (utilizadorEmEdicao) {
      const nome = dados.nome.trim();
      const email = dados.email.trim();

      if (nome.length < 2 || nome.length > 100) {
        setErroForm('O nome deve ter entre 2 e 100 caracteres.');
        return;
      }

      if (!email) {
        setErroForm('O email é obrigatório.');
        return;
      }
    } else {
      // Validação para criação
      const problema = validarUtilizador(dados);

      if (problema) {
        setErroForm(problema);
        return;
      }
    }

    setErroForm('');
    setACriar(true);

    try {
      if (utilizadorEmEdicao) {
        // Editar utilizador existente
        const atualizado = await editarUtilizador(
          utilizadorEmEdicao.id,
          dados
        );

        setUtilizadores(prev =>
          prev.map(u =>
            u.id === utilizadorEmEdicao.id
              ? {
                  ...u,
                  nome: atualizado.nome,
                  email: atualizado.email,
                }
              : u
          )
        );

        setSucesso(`Conta de ${atualizado.nome} atualizada.`);
      } else {
        // Criar novo utilizador
        const criado = await criarUtilizador(dados);

        setRoleSelecionada('Todos');
        setSucesso(`Conta de ${criado.nome} criada.`);

        // Atualizar a lista
        try {
          const lista = await listarUtilizadores();
          setUtilizadores(lista);
        } catch (err) {
          setErro(mensagemDeErroUtilizador(err));
        }
      }

      handleFecharForm();
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

  // Apagar utilizador
  const handleApagar = async (utilizador) => {
    if (
      !window.confirm(
        `Apagar a conta de ${utilizador.nome} (${utilizador.email})? Não é possível desfazer.`
      )
    ) {
      return;
    }

    setErro('');
    setSucesso('');
    setIdEmCurso(utilizador.id);

    try {
      await apagarUtilizador(utilizador.id);

      setUtilizadores(prev =>
        prev.filter(u => u.id !== utilizador.id)
      );

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

    // Ações sobre utilizadores
    handleApagar,
    handleAbrirEdicao,

    // Estado e ações do formulário
    utilizadorEmEdicao,
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
