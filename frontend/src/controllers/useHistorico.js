import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { listarHistorico } from '../models/historicoModel';
import { limparSessao } from '../models/loginModel';
import { mensagemDeErro, sessaoInvalida } from '../models/oportunidadesModel';
import { eAdministrador } from '../models/utilizadoresModel';

export default function useHistorico() {
  const navigate = useNavigate();

  const [registos, setRegistos] = useState([]);
  const [categoriaSelecionada, setCategoriaSelecionada] = useState('Todas');
  const [pesquisa, setPesquisa] = useState('');
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');

  useEffect(() => {
    // Só o Administrador pode consultar o histórico
    if (!eAdministrador()) {
      navigate('/oportunidades', { replace: true });
      return;
    }

    let ativo = true;

    listarHistorico()
      .then(lista => {
        if (ativo) setRegistos(lista);
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

  const recarregar = useCallback(async () => {
    setLoading(true);
    setErro('');

    try {
      setRegistos(await listarHistorico());
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
  }, [navigate]);

  const contar = cat => registos.filter(r => r.categoria === cat).length;

  const contagens = {
    Todas: registos.length,
    Autenticacao: contar('Autenticacao'),
    Oportunidades: contar('Oportunidades'),
    Orcamentos: contar('Orcamentos'),
    Utilizadores: contar('Utilizadores'),
    Categorias: contar('Categorias'),
  };

  const termo = pesquisa.trim().toLowerCase();

  const registosFiltrados = registos.filter(r => {
    const passaCategoria =
      categoriaSelecionada === 'Todas' ||
      r.categoria === categoriaSelecionada;

    if (!passaCategoria) return false;
    if (!termo) return true;

    return (
      r.nomeUtilizador.toLowerCase().includes(termo) ||
      r.acao.toLowerCase().includes(termo) ||
      r.descricao.toLowerCase().includes(termo)
    );
  });

  return {
    registosFiltrados,
    categoriaSelecionada,
    setCategoriaSelecionada,
    pesquisa,
    setPesquisa,
    contagens,
    loading,
    erro,
    recarregar,
  };
}
