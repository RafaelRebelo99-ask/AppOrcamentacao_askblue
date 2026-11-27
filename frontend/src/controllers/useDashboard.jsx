
import useOportunidades from './useOportunidades';
import useOrcamentos from './orcamentos/useOrcamentos';

import {
    Clipboard,
    Invoice,
    ArrowUpRight,
    ClipboardCheckCorner,
} from '@coolicons/react';

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

    // Get the authenticated user
    const utilizador = JSON.parse(
        localStorage.getItem('utilizador') ||
        sessionStorage.getItem('utilizador') ||
        'null'
    );

    const role = (
        utilizador?.role ||
        utilizador?.perfil ||
        utilizador?.tipoUtilizador ||
        utilizador?.tipo ||
        ''
    )

    const isCliente = role === 2;

    // Opportunity statistics
    const oportunidadesAtivas = oportunidades.filter(o =>
        ['Registada', 'EmAnalise', 'EmPreparacao'].includes(o.estado)
    ).length;

    const oportunidadesGanhas = oportunidades.filter(
        o => o.estado === 'Ganha'
    ).length;

    // Proposal statistics
    const propostasRascunho = contagensOrcamentos?.Rascunho ?? 0;
    const propostasEnviadas = contagensOrcamentos?.Enviado ?? 0;
    const propostasAceites = contagensOrcamentos?.Aceite ?? 0;
    const propostasRecusadas = contagensOrcamentos?.Recusado ?? 0;

    // Employee / Admin statistics
    const estatisticasColaborador = [
        {
            titulo: 'Oportunidades ativas',
            valor: oportunidadesAtivas,
            descricao: 'Registadas, em análise ou preparação',
            icon: Clipboard,
        },
        {
            titulo: 'Propostas em rascunho',
            valor: propostasRascunho,
            descricao: 'Por concluir',
            icon: Invoice,
        },
        {
            titulo: 'Propostas enviadas',
            valor: propostasEnviadas,
            descricao: 'À espera de resposta',
            icon: ArrowUpRight,
        },
        {
            titulo: 'Oportunidades ganhas',
            valor: oportunidadesGanhas,
            descricao: 'Com proposta aceite',
            icon: ClipboardCheckCorner,
        },
    ];

    // Client statistics
    const estatisticasCliente = [
        {
            titulo: 'Pedidos submetidos',
            valor: oportunidades.length,
            descricao: 'Total de pedidos registados',
            icon: Clipboard,
        },
        {
            titulo: 'Propostas pendentes',
            valor: propostasEnviadas,
            descricao: 'À espera da tua decisão',
            icon: ArrowUpRight,
        },
        {
            titulo: 'Propostas aceites',
            valor: propostasAceites,
            descricao: 'Propostas aprovadas',
            icon: ClipboardCheckCorner,
        },
        {
            titulo: 'Propostas recusadas',
            valor: propostasRecusadas,
            descricao: 'Propostas recusadas',
            icon: Invoice,
        },
    ];

    // Recent opportunities
    const recentes = [...oportunidades]
        .sort((a, b) =>
            new Date(b.dataCriacao) - new Date(a.dataCriacao)
        )
        .slice(0, 5);

    return {
        estatisticas: isCliente
            ? estatisticasCliente
            : estatisticasColaborador,

        recentes,

        isCliente,

        loading: loadingOportunidades || loadingOrcamentos,

        erro: erroOportunidades || erroOrcamentos,
    };
}
