
import { useState } from 'react';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import OportunidadeFilters from '../components/oportunidades/OportunidadeFilters';
import OportunidadeRow from '../components/oportunidades/OportunidadeRow';

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

  return (
    <Layout>
      <div className="row mb-1">
        <OportunidadeFilters
          estadoSelecionado={estadoSelecionado}
          contagens={contagens}
          onEstadoChange={setEstadoSelecionado}
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
