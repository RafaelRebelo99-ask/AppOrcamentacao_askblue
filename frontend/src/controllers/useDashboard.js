import useOportunidades from './useOportunidades';
import useOrcamentos from './orcamentos/useOrcamentos';

export default function useDashboard() {
    const {
        oportunidades,
        loading: loadingOportunidades,
        erro: erroOportunidades,
    } = useOportunidades();

    const {
        contagens: contagensOrcamentos,
        loading: loadingOrcamentos,
        erro: erroOrcamentos,
    } = useOrcamentos();

    const oportunidadesAtivas = oportunidades.filter(o =>
        ['Registada', 'EmAnalise', 'EmPreparacao'].includes(o.estado)
    ).length;

    const oportunidadesGanhas = oportunidades.filter(
        o => o.estado === 'Ganha'
    ).length;

    const estatisticas = [
        {
            titulo: 'Oportunidades ativas',
            valor: oportunidadesAtivas,
            descricao: 'Registadas, em análise ou preparação',
            icon: 'bi-clipboard',
        },
        {
            titulo: 'Propostas em rascunho',
            valor: contagensOrcamentos.Rascunho,
            descricao: 'Por concluir',
            icon: 'bi-file-earmark-spreadsheet',
        },
        {
            titulo: 'Propostas enviadas',
            valor: contagensOrcamentos.Enviado,
            descricao: 'À espera de resposta',
            icon: 'bi-send',
        },
        {
            titulo: 'Oportunidades ganhas',
            valor: oportunidadesGanhas,
            descricao: 'Com proposta aceite',
            icon: 'bi-clipboard-check',
        },
    ];

    const recentes = [...oportunidades]
        .sort((a, b) =>
            new Date(b.dataCriacao) - new Date(a.dataCriacao)
        )
        .slice(0, 5);

    return {
        estatisticas,
        recentes,
        loading: loadingOportunidades || loadingOrcamentos,
        erro: erroOportunidades || erroOrcamentos,
    };
}