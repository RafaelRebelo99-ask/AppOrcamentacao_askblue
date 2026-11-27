
import { useNavigate } from 'react-router-dom';

import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';

import useDashboard from '../controllers/useDashboard';

import NumeroAnimado from '../components/NumeroAnimado';
import Icon from '../components/Icon';

import { DashboardRecentSkeleton, DashboardStatsSkeleton } from '../components/skeletons/DashboardSkeleton';

import {
    Clipboard
} from '@coolicons/react';

export default function Dashboard() {
    const navigate = useNavigate();

    const {
        estatisticas,
        recentes,
        loading,
        erro,
        isCliente,
    } = useDashboard();

    const utilizador = JSON.parse(
        localStorage.getItem('utilizador') ||
        sessionStorage.getItem('utilizador') ||
        'null'
    );

    const nome = utilizador?.nome || 'Utilizador';

    return (
        <Layout>
            <div className="dashboard scrollable-y">

                {/* Header */}
                <div className="dashboard-header">
                    <div>
                        <h1 className='mb-0'>
                            Olá, {nome}
                        </h1>
                    </div>

                    <ThemeToggle />
                </div>
                <div className="column gap-1 scrollable-y">
                    {/* Statistics */}
                    <div className="dashboard-stats">
                        {loading
                            ? (<DashboardStatsSkeleton />)
                            : estatisticas.map((stat, index) => {

                                return (
                                    <div
                                        className="dashboard-stat"
                                        key={index}
                                    >
                                        <p className='mb-2'>{stat.titulo}</p>
                                        <h2>
                                            <NumeroAnimado
                                                valor={stat.valor}
                                            />
                                        </h2>

                                        <p className="weak">
                                            {stat.descricao}
                                        </p>
                                    </div>
                                );
                            })}
                    </div>

                    {/* Recent opportunities */}
                    <div className="dashboard-section">
                        <div className="row align/center space-between">
                            <div className='column'>
                                <h2>
                                    {isCliente
                                        ? 'Os meus pedidos'
                                        : 'Oportunidades recentes'}
                                </h2>

                                <p>
                                    {isCliente
                                        ? 'Consulte o estado dos seus pedidos mais recentes.'
                                        : 'Últimas oportunidades registadas.'}
                                </p>
                            </div>

                            <button
                                className="btn-secondary"
                                onClick={() => navigate('/oportunidades')}
                            >
                                Ver todas
                            </button>
                        </div>

                        {erro && (
                            <div className="error-message">
                                {erro}
                            </div>
                        )}

                        {loading ? (
                            <DashboardRecentSkeleton />

                        ) : recentes.length === 0 ? (
                        <div className="dashboard-empty">
                            <Icon icon={Clipboard} />
                            <p>
                                {isCliente
                                    ? 'Ainda não submeteu nenhum pedido.'
                                    : 'Ainda não existem oportunidades.'}
                            </p>
                        </div>
                        ) : (
                        <div className="column gap-1">
                            {recentes.map(oportunidade => (
                                <div
                                    className="row pad-05 align-center border-bottom"
                                    key={oportunidade.id}
                                >
                                    <div className="column">
                                        <h4>
                                            {oportunidade.titulo}
                                        </h4>

                                        {!isCliente && (
                                            <p className='weak'>
                                                {oportunidade.cliente}
                                            </p>
                                        )}
                                    </div>

                                    <div className="estado">
                                        {oportunidade.estadoTexto ||
                                            oportunidade.estado}
                                    </div>

                                    <Icon icon={Clipboard} />
                                </div>
                            ))}
                        </div>
                        )}
                    </div>
                </div>

            </div>
        </Layout>
    );
}
