import { useState } from 'react';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import Sort from '../components/Sort';
import Select from '../components/Select';
import OportunidadeRow from '../components/oportunidades/OportunidadeRow';
import OportunidadeRowSkeleton from '../components/skeletons/OportunidadeSkeleton';

import useOportunidadesController from '../controllers/useOportunidades';

const filtros = [
  { label: 'Todas', value: 'Todas' },
  { label: 'Registadas', value: 'Registada' },
  { label: 'Em análise', value: 'EmAnalise' },
  { label: 'Em preparação', value: 'EmPreparacao' },
  { label: 'Proposta enviada', value: 'PropostaEnviada' },
  { label: 'Ganhas', value: 'Ganha' },
  { label: 'Perdidas', value: 'Perdida' },
  { label: 'Canceladas', value: 'Cancelada' },
];

const opcoesOrdenacao = [
  {
    icon: 'bi bi-clock',
    label: 'Prazo (Mais próximo)',
    value: 'prazo-asc',
  },
  {
    icon: 'bi bi-clock-history',
    label: 'Prazo (Mais distante)',
    value: 'prazo-desc',
  },
];

export default function Oportunidades() {
  const [menuAbertoId, setMenuAbertoId] = useState(null);

  const {
    clientes,
    loadingClientes,
    oportunidadesFiltradas,
    estadoSelecionado,
    setEstadoSelecionado,
    categoriaSelecionada,
    setCategoriaSelecionada,
    clienteSelecionado,
    setClienteSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    categorias,
    loading,
    loadingCategorias,
    erro,
    idEmCurso,
    podeGerir,
    handleIniciarAnalise,
    handleIniciarPreparacao,
    handleCriarOrcamento,
    handlePerdida,
    handleCancelar,
  } = useOportunidadesController();

  return (
    <Layout>
      <div className="row align-center">
        <h1 className="mb-05">Oportunidades</h1>
        <ThemeToggle />
      </div>

      <div className="row mb-1 align-center wrap">
        <Filters
          filtros={filtros}
          estadoSelecionado={estadoSelecionado}
          contagens={contagens}
          onEstadoChange={setEstadoSelecionado}
        />

        <div className="vl"></div>

        <Select
          options={categorias}
          value={categoriaSelecionada}
          onChange={setCategoriaSelecionada}
          placeholder={loadingCategorias ? 'A carregar categorias...' : 'Categoria'}
          disabled={loadingCategorias}
        />
        <Select
          options={clientes}
          value={clienteSelecionado}
          onChange={setClienteSelecionado}
          placeholder={loadingClientes ? 'A carregar clientes...' : 'Cliente'}
          disabled={loadingClientes}
        />

        <Sort
          opcoes={opcoesOrdenacao}
          ordenacao={ordenacao}
          onOrdenacaoChange={setOrdenacao}
        />
      </div>

      {erro && <p className="error mb-05">{erro}</p>}

      <table className="table">
        <thead>
          <tr className="header">
            <th>ID</th>
            <th>Título</th>
            <th>Estado</th>
            <th>Cliente</th>
            <th>Categoria</th>
            <th>Prazo da proposta</th>
            <th>Data pretendida</th>
            <th></th>
          </tr>
        </thead>

        <tbody>
          {loading ? (
            Array.from({ length: 10 }).map((_, index) => (
              <OportunidadeRowSkeleton key={index} />
            ))
          ) : oportunidadesFiltradas.length > 0 ? (
            oportunidadesFiltradas.map(oportunidade => (
              <OportunidadeRow
                key={oportunidade.id}
                oportunidade={oportunidade}
                menuAberto={menuAbertoId === oportunidade.id}
                onToggleMenu={() =>
                  setMenuAbertoId(
                    menuAbertoId === oportunidade.id ? null : oportunidade.id
                  )
                }
                onFecharMenu={() => setMenuAbertoId(null)}
                podeGerir={podeGerir}
                emCurso={idEmCurso === oportunidade.id}
                onIniciarAnalise={handleIniciarAnalise}
                onIniciarPreparacao={handleIniciarPreparacao}
                onCriarOrcamento={handleCriarOrcamento}
                onPerdida={handlePerdida}
                onCancelar={handleCancelar}
              />
            ))
          ) : (
            <tr>
              <td colSpan="8" style={{ textAlign: 'center' }}>
                Não existem oportunidades com estes filtros.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </Layout>
  );
}
