import { useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  listarNotificacoes,
  marcarComoLida,
  marcarTodasComoLidas,
} from '../models/notificacoesModel';

const INTERVALO = 30_000; // de quanto em quanto tempo pergunta ao backend se há novas

export default function useNotificacoes() {
  const navigate = useNavigate();
  const { pathname } = useLocation();

  const [itens, setItens] = useState([]);
  const [naoLidas, setNaoLidas] = useState(0);
  const [aberto, setAberto] = useState(false);

  const contentor = useRef(null); // para saber se se clicou fora

  // Carregar ao abrir a página, a cada 30 segundos e quando se volta ao separador
  useEffect(() => {
    let ativo = true; // evita mexer no estado se o utilizador já saiu da página

    const carregar = () => {
      listarNotificacoes()
        .then(resposta => {
          if (!ativo) return;
          setItens(resposta.itens);
          setNaoLidas(resposta.naoLidas);
        })
        // O sino não é essencial: se falhar, fica como está e tenta outra vez depois.
        // Uma sessão expirada é tratada pela própria página.
        .catch(() => {});
    };

    carregar();
    const intervalo = setInterval(carregar, INTERVALO);
    window.addEventListener('focus', carregar);

    return () => {
      ativo = false;
      clearInterval(intervalo);
      window.removeEventListener('focus', carregar);
    };
  }, []);

  // Fechar ao clicar fora ou com Esc
  useEffect(() => {
    if (!aberto) return;

    const aoClicar = (event) => {
      if (!contentor.current?.contains(event.target)) setAberto(false);
    };
    const aoTeclar = (event) => {
      if (event.key === 'Escape') setAberto(false);
    };

    document.addEventListener('mousedown', aoClicar);
    document.addEventListener('keydown', aoTeclar);

    return () => {
      document.removeEventListener('mousedown', aoClicar);
      document.removeEventListener('keydown', aoTeclar);
    };
  }, [aberto]);

  const alternar = () => setAberto(prev => !prev);

  // Marca como lida e abre a página a que diz respeito
  const handleAbrir = async (notificacao) => {
    setAberto(false);

    if (!notificacao.lida) {
      try {
        await marcarComoLida(notificacao.id);
      } catch {
        // se não der para marcar, abre a página na mesma
      }
    }

    // Já nessa página: recarrega, para mostrar os dados atuais
    if (pathname === notificacao.pagina) navigate(0);
    else navigate(notificacao.pagina);
  };

  const handleMarcarTodas = async () => {
    try {
      await marcarTodasComoLidas();
      setItens(prev => prev.map(n => ({ ...n, lida: true })));
      setNaoLidas(0);
    } catch {
      // fica como está; a próxima atualização mostra o estado certo
    }
  };

  return {
    itens,
    naoLidas,
    aberto,
    contentor,
    alternar,
    handleAbrir,
    handleMarcarTodas,
  };
}
