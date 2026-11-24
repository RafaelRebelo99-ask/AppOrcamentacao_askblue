
import { useState } from 'react';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import Sort from '../components/Sort';
import OrcamentoCard from '../components/orcamentos/OrcamentoCard';

const orcamentosExemplo = [
  {
    id: 'ORC-2026-001',
    estado: 'Aceite',
    titulo: 'Implementação de solução tecnológica',
    categoria: 'Tecnologia',
    cliente: 'Cliente A',
    dataCriacao: '15/11/2026',
    valor: 12000,
  },
  {
    id: 'ORC-2026-002',
    estado: 'Recusado',
    titulo: 'Desenvolvimento de aplicação',
    categoria: 'Tecnologia',
    cliente: 'Cliente B',
    dataCriacao: '12/11/2026',
    valor: 52000,
  },
  {
    id: 'ORC-2026-003',
    estado: 'Aceite',
    titulo: 'Consultoria tecnológica',
    categoria: 'Tecnologia',
    cliente: 'Cliente C',
    dataCriacao: '10/11/2026',
    valor: 8000,
  },
];

const opcoesOrdenacao = [
  { label: 'Data (Mais recente)', value: 'data-desc' },
  { label: 'Data (Mais antigo)', value: 'data-asc' },
  { label: 'Valor (Mais baixo)', value: 'valor-asc' },
  { label: 'Valor (Mais alto)', value: 'valor-desc' },
];

const filtros = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Aceite', value: 'Aceite' },
  { label: 'Recusado', value: 'Recusado' },
  { label: 'Enviado', value: 'Enviado' },
  { label: 'Rascunho', value: 'Rascunho' },
];

const contagens = {
  Todos: orcamentosExemplo.length,
  Aceite: orcamentosExemplo.filter(o => o.estado === 'Aceite').length,
  Recusado: orcamentosExemplo.filter(o => o.estado === 'Recusado').length,
  Enviado: orcamentosExemplo.filter(o => o.estado === 'Enviado').length,
  Rascunho: orcamentosExemplo.filter(o => o.estado === 'Rascunho').length,
};

export default function Orcamentos() {
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todos');

  const orcamentosFiltrados = [...orcamentosExemplo]
    .filter(orcamento =>
      estadoSelecionado === 'Todos' ||
      orcamento.estado === estadoSelecionado
    )

  const [ordenacao, setOrdenacao] = useState('');

  return (
    <Layout>
      <div className="row mb-1 align-center">
        <Filters
          filtros={filtros}
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
        <ThemeToggle />
      </div>

      <div className="grid">
        {orcamentosFiltrados.length > 0 ? (
          orcamentosFiltrados.map(orcamento => (
            <OrcamentoCard
            orcamento={orcamento}
            />
          ))
        ) : (
          <p>Não existem propostas neste estado.</p>
        )}
      </div>
    </Layout>
  );
}

function converterData(data) {
  const [dia, mes, ano] = data.split('/');
  return new Date(ano, mes - 1, dia).getTime();
}
