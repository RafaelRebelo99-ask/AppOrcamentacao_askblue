import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import Sort from '../components/Sort';
import Aviso from '../components/Aviso';
import OrcamentoCard from '../components/orcamentos/OrcamentoCard';
import useOrcamentos from '../controllers/orcamentos/useOrcamentos';

const filtros = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Rascunho', value: 'Rascunho' },
  { label: 'Enviado', value: 'Enviado' },
  { label: 'Aceite', value: 'Aceite' },
  { label: 'Recusado', value: 'Recusado' },
];

// Texto da confirmação e do botão, para cada ação
const confirmacoes = {
  enviar: {
    botao: 'Enviar',
    texto: o => `Enviar a proposta de ${o.valorTotal} ao cliente? Depois de enviada, não pode ser editada.`,
  },
  eliminar: {
    botao: 'Eliminar',
    texto: o => `Eliminar o rascunho de ${o.valorTotal}? Esta ação não pode ser desfeita.`,
  },
  aceitar: {
    botao: 'Aceitar',
    texto: o => `Aceitar a proposta de ${o.valorTotal}? As outras propostas para "${o.oportunidade}" serão recusadas.`,
  },
  recusar: {
    botao: 'Recusar',
    texto: o => `Recusar a proposta de ${o.valorTotal}? Esta ação não pode ser desfeita.`,
  },
};

const opcoesOrdenacao = [
  { label: 'Data (Mais recente)', value: 'data-desc' },
  { label: 'Data (Mais antigo)', value: 'data-asc' },
  { label: 'Valor (Mais baixo)', value: 'valor-asc' },
  { label: 'Valor (Mais alto)', value: 'valor-desc' },
];

export default function Orcamentos() {
  const {
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
  } = useOrcamentos();

  // Sem ação pendente, o aviso fica fechado
  const confirmacao = confirmacoes[acaoPendente?.tipo];

  // O Cliente nunca recebe rascunhos
  const filtrosVisiveis = podeGerir
    ? filtros
    : filtros.filter(f => f.value !== 'Rascunho');

  return (
    <Layout>
      <div className="row align-center">
              <h1 className='mb-05'>Orçamentos</h1>
              <ThemeToggle/>
            </div>
      <div className="row mb-1 align-center">
        <Filters
          filtros={filtrosVisiveis}
          estadoSelecionado={estadoSelecionado}
          contagens={contagens}
          onEstadoChange={setEstadoSelecionado}
        />

        <div className="vl"></div>

        <Sort
          opcoes={opcoesOrdenacao}
          ordenacao={ordenacao}
          onOrdenacaoChange={setOrdenacao}
        />

      </div>

      {erro && <p className="danger">{erro}</p>}

      {loading ? (
        <p>A carregar propostas...</p>
      ) : (
        <div className="grid">
          {orcamentosFiltrados.length > 0 ? (
            orcamentosFiltrados.map(orcamento => (
              <OrcamentoCard
                key={orcamento.id}
                orcamento={orcamento}
                podeGerir={podeGerir}
                emCurso={idEmCurso === orcamento.id}
                onEnviar={handleEnviar}
                onEliminar={handleEliminar}
                onAceitar={handleAceitar}
                onRecusar={handleRecusar}
              />
            ))
          ) : (
            <p>Não existem propostas neste estado.</p>
          )}
        </div>
      )}

      <Aviso
        mensagem={confirmacao?.texto(acaoPendente.orcamento)}
        confirmar={true}
        textoConfirmar={confirmacao?.botao}
        textoCancelar="Voltar"
        onFechar={handleFecharConfirmacao}
        onConfirmar={handleConfirmar}
      />
    </Layout>
  );
}