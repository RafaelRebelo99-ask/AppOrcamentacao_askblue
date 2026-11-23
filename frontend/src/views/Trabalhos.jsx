// ===== VIEW: Trabalhos =====
// Só o visual, passado do DEMO_FRONT/trabalhos.html. Ainda sem lógica:
// filtros por estado, "Ordenar por", tema e os botões das linhas não fazem nada.
// Os dados abaixo são de exemplo; vão ser substituídos pelos da API.
import Layout from '../components/Layout'

const filtros = [
  { nome: 'Todos', total: 2, selecionado: true },
  { nome: 'Atribuído', total: 1 },
  { nome: 'Em Curso', total: 1 },
  { nome: 'Concluído', total: 0 },
  { nome: 'Por Aprovar', total: 0 },
]

const trabalhosExemplo = [
  { id: 1, titulo: 'Serviço de consultoria de IT', estado: 'Em curso', cliente: 'AskBlue', categoria: 'Tecnologia', prazo: '20/11/2026', aviso: { texto: 'Em 2 dias', cor: 'yellow' }, valor: '12.000,00 €', acao: 'Concluir Trabalho' },
  { id: 2, titulo: 'Serviço de consultoria de IT', estado: 'Atribuído', cliente: 'AskBlue', categoria: 'Tecnologia', prazo: '18/11/2026', aviso: { texto: 'Atrasado', cor: 'red' }, valor: '52.000,00 €', acao: 'Começar Trabalho' },
]

export default function Trabalhos() {
  return (
    <Layout>
      <div className="row mb-1">
        {filtros.map((f) => (
          <button key={f.nome} className={'wide-left secondary' + (f.selecionado ? ' selected' : '')}>
            {f.nome}<div className="counter">{f.total}</div>
          </button>
        ))}
        <div className="vl"></div>
        <button className="wide secondary" id="sortBtn">Ordenar por<i className="bi bi-caret-down"></i></button>
        <div className="menu hidden" id="sortMenu">
          <div className="item wide"><i className="bi bi-sort-alpha-down"></i><p>Título (A-Z)</p></div>
          <div className="item wide"><i className="bi bi-sort-alpha-up"></i><p>Título (Z-A)</p></div>
          <div className="item wide"><i className="bi bi-clock"></i><p>Prazo (Mais próximo)</p></div>
          <div className="item wide"><i className="bi bi-clock-history"></i><p>Prazo (Mais distante)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-down"></i><p>Valor (Mais baixo)</p></div>
          <div className="item wide"><i className="bi bi-sort-numeric-up"></i><p>Valor (Mais alto)</p></div>
        </div>
        <button className="more-actions" title="Mudar tema"><i className="bi bi-moon"></i></button>
      </div>

      {/* Em React a tabela precisa de <thead>/<tbody>; o aspeto é o mesmo */}
      <table className="table">
        <thead>
          <tr className="header">
            <th>ID</th>
            <th>Título</th>
            <th>Estado</th>
            <th>Cliente</th>
            <th>Categoria</th>
            <th>Prazo de Conclusão</th>
            <th className="right">Valor Total (incl. IVA)</th>
            <th className="ml-auto"></th>
          </tr>
        </thead>
        <tbody>
          {trabalhosExemplo.map((t) => (
            <tr key={t.id}>
              <td>{t.id}</td>
              <td>{t.titulo}</td>
              <td><div className="estado">{t.estado}</div></td>
              <td>
                <div className="row gap-0">
                  <p>{t.cliente}</p><i className="bi bi-check-lg"></i>
                </div>
              </td>
              <td>{t.categoria}</td>
              <td>{t.prazo} <div className={'pill ' + t.aviso.cor}>{t.aviso.texto}</div></td>
              <td className="right">{t.valor}</td>
              <td className="ml-auto">
                <button className="primary wide">{t.acao}</button>
                {/* O menu abre ao passar o rato (só CSS, já vem do askblue.css) */}
                <button className="more-actions pos-rel">
                  <i className="bi bi-three-dots-vertical"></i>
                  <div className="menu">
                    <div className="item wide disabled row"><i className="bi bi-pencil"></i><p>Editar</p></div>
                    <div className="item wide"><i className="bi bi-journal-text"></i><p>Consultar serviço</p></div>
                    <div className="item wide danger"><i className="bi bi-trash"></i><p>Eliminar</p></div>
                  </div>
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </Layout>
  )
}
