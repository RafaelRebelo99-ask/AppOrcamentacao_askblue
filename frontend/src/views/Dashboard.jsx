import { useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import useOportunidades from '../controllers/useOportunidades';

export default function Dashboard() {
    const navigate = useNavigate();

    const {
        oportunidades,
        loading,
        erro
    } = useOportunidades();


    const utilizador = JSON.parse(
        localStorage.getItem('utilizador') ||
        sessionStorage.getItem('utilizador') ||
        'null'
    );

    const nome = utilizador?.nome || 'Utilizador';

    const contar = (estado) =>
        oportunidades.filter(o => o.estado === estado).length;

    const oportunidadesAtivas = oportunidades.filter(o =>
        ['Registada', 'EmAnalise', 'EmPreparacao'].includes(o.estado)
    ).length;

    const estatisticas = [
        {
            titulo: 'Oportunidades ativas',
            valor: oportunidadesAtivas,
            descricao: 'Registadas, em análise ou preparação',
            icon: 'bi-briefcase',
        },
        {
            titulo: 'Propostas em rascunho',
            valor: '—',
            descricao: 'Por concluir',
            icon: 'bi-file-earmark-text',
        },
        {
            titulo: 'Propostas enviadas',
            valor: contar('PropostaEnviada'),
            descricao: 'À espera de resposta',
            icon: 'bi-send',
        },
        {
            titulo: 'Oportunidades ganhas',
            valor: contar('Ganha'),
            descricao: 'Com proposta aceite',
            icon: 'bi-trophy',
        },
    ];

    const recentes = [...oportunidades]
        .sort((a, b) =>
            new Date(b.dataCriacao) - new Date(a.dataCriacao)
        )
        .slice(0, 5);

    return (
        <Layout>
            <main className="dashboard">
                <section className="dashboard-header">
                    <div>
                        <h1>Bom dia, {nome}!</h1>
                        <p className="weak">
                            Aqui está o resumo da tua atividade comercial.
                        </p>
                    </div>
                    <ThemeToggle/>
                </section>

                <section className="dashboard-stats">
                    {estatisticas.map((item) => (
                        <article className="dashboard-stat" key={item.titulo}>
                            <div className="row align-center justify-between">
                                <i className={`bi ${item.icon}`}></i>
                                <p>{item.titulo}</p>
                            </div>

                            <h2>{item.valor}</h2>

                            <p className="weak">{item.descricao}</p>
                        </article>
                    ))}
                </section>

                <section className="dashboard-content">

                    <article className="dashboard-section">
                        <div className="row space-between align-center">
                            <div>
                                <h3>Oportunidades recentes</h3>
                                <p className="weak">
                                    As últimas oportunidades registadas.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="transparent"
                                onClick={() => navigate('/oportunidades')}
                            >
                                Ver todas
                                <i className="bi bi-arrow-right"></i>
                            </button>
                        </div>

                        <div className="dashboard-empty">
                            <i className="bi bi-clipboard"></i>
                            <p>Ainda não existem oportunidades para apresentar.</p>
                            <button
                                type="button"
                                className="secondary"
                                onClick={() => navigate('/registar-oportunidade')}
                            >
                                Criar oportunidade
                            </button>
                        </div>
                    </article>

                    {/* Pending actions */}
                    <article className="dashboard-section">
                        <div>
                            <h3>Ações pendentes</h3>
                            <p className="weak">
                                Atividades que podem precisar da tua atenção.
                            </p>
                        </div>

                        <div className="dashboard-empty">
                            <i className="bi bi-check-all"></i>
                            <p>Não existem ações pendentes.</p>
                        </div>
                    </article>

                </section>
            </main>
        </Layout>
    );
}
