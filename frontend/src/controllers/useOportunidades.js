import { useState } from 'react';

const oportunidadesMock = [
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

function converterData(data) {
  const [dia, mes, ano] = data.split('/');
  return new Date(ano, mes - 1, dia).getTime();
}

export default function useOportunidadesController() {
  const [oportunidades, setOportunidades] = useState(oportunidadesMock);
  const [estadoSelecionado, setEstadoSelecionado] = useState('Todas');
  const [ordenacao, setOrdenacao] = useState('data-desc');

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

  const oportunidadesFiltradas = [...oportunidades]
    .filter(o =>
      estadoSelecionado === 'Todas' ||
      o.estado === estadoSelecionado
    )
    .sort((a, b) => {
      switch (ordenacao) {
        case 'data-desc':
          return converterData(b.prazo) - converterData(a.prazo);
        case 'data-asc':
          return converterData(a.prazo) - converterData(b.prazo);
        case 'prazo-asc':
          return converterData(a.prazo) - converterData(b.prazo);
        case 'prazo-desc':
          return converterData(b.prazo) - converterData(a.prazo);
        default:
          return 0;
      }
    });

  const handleConsultar = (oportunidade) => {
    // Navegar para os detalhes da oportunidade
    console.log('Consultar:', oportunidade.id);
  };

  const handleEditar = (oportunidade) => {
    // Navegar para edição da oportunidade
    console.log('Editar:', oportunidade.id);
  };

  const handleEliminar = (oportunidade) => {
    // Futuramente, chamar o serviço para eliminar na API
    console.log('Eliminar:', oportunidade.id);
  };

  return {
    oportunidadesFiltradas,
    estadoSelecionado,
    setEstadoSelecionado,
    ordenacao,
    setOrdenacao,
    contagens,
    handleConsultar,
    handleEditar,
    handleEliminar,
  };
}