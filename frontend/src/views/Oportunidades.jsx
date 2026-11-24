
import { useState } from 'react';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import Filters from '../components/Filters';
import OportunidadeRow from '../components/oportunidades/OportunidadeRow';
import Sort from '../components/Sort';

// vai se buscar à API
const oportunidades = [
  {
    id: 1,
    titulo: 'Implementação de solução tecnológica',
    estado: 'EmPreparacao',
    cliente: 'Cliente A',
    categoria: 'Tecnologia',
    prazo: '20/11/2026',
    aviso: { texto: 'Em 2 dias', cor: 'yellow' },
    valor: '12.000,00 €',
  },
  {
    id: 2,
    titulo: 'Desenvolvimento de aplicação',
    estado: 'Registada',
    cliente: 'Cliente B',
    categoria: 'Tecnologia',
    prazo: '18/11/2026',
    aviso: { texto: 'Atrasado', cor: 'red' },
    valor: '52.000,00 €',
  },
];

//isto pode ser hardcoded
const opcoesOrdenacao = [
  { label: 'Data (Mais recente)', value: 'data-desc' },
  { label: 'Data (Mais antigo)', value: 'data-asc' },
  { label: 'Prazo (Mais próximo)', value: 'prazo-asc' },
  { label: 'Prazo (Mais distante)', value: 'prazo-desc' },
];

// vai se buscar à API (são os estados)
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

const contagens = {
  Todas: oportunidades.length,
  Registada: oportunidades.filter(o => o.estado === 'Registada').length,
  EmAnalise: oportunidades.filter(o => o.estado === 'EmAnalise').length,
  EmPreparacao: oportunidades.filter(o => o.estado === 'EmPreparacao').length,
  PropostaEnviada: oportunidades.filter(o => o.estado === 'PropostaEnviada').length,
  Ganha: oportunidades.filter(o => o.estado === 'Ganha').length,
  Perdida: oportunidades.filter(o => o.estado === 'Perdida').length,
  Cancelada: oportunidades.filter(o => o.estado === 'Cancelada').length,
};

export default function Oportunidades() {
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todas');

  const oportunidadesFiltradas = oportunidades.filter(oportunidade =>
    estadoSelecionado === 'Todas' ||
    oportunidade.estado === estadoSelecionado
  );

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

        <div className='vl'>
        </div>

        <Sort
        opcoes={opcoesOrdenacao}
          ordenacao={ordenacao}
          onOrdenacaoChange={setOrdenacao}
        />

        <ThemeToggle />
      </div>

      <table className="table">
        <thead>
          <tr className="header">
            <th>ID</th>
            <th>Título</th>
            <th>Estado</th>
            <th>Cliente</th>
            <th>Categoria</th>
            <th>Prazo da proposta</th>
            <th className="right">Valor total</th>
            <th></th>
          </tr>
        </thead>

        <tbody>
          {oportunidadesFiltradas.length > 0 ? (
            oportunidadesFiltradas.map(oportunidade => (
              <OportunidadeRow
                key={oportunidade.id}
                oportunidade={oportunidade}
              />
            ))
          ) : (
            <tr>
              <td colSpan="8" style={{ textAlign: 'center' }}>
                Não existem oportunidades neste estado.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </Layout>
  );
}
