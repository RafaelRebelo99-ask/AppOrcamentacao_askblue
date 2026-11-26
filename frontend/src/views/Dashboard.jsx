import { useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import useDashboard from '../controllers/useDashboard';
import NumeroAnimado from '../components/NumeroAnimado';

export default function Dashboard() {
    const navigate = useNavigate();

    const {
        estatisticas,
        recentes,
        loading,
        erro,
    } = useDashboard();


    const utilizador = JSON.parse(
        localStorage.getItem('utilizador') ||
        sessionStorage.getItem('utilizador') ||
        'null'
    );

    const nome = utilizador?.nome || 'Utilizador';

    return (
        <Layout>
            <main className="dashboard scrollable-y">
                <section className="dashboard-header">
                    <div>
                        <h1>Bem vindo, {nome}</h1>
                        <p className="weak">
                            Aqui está o resumo da tua atividade comercial.
                        </p>
                    </div>
                    <ThemeToggle />
                </section>

                <div className="column gap-1 scrollable-y">

                    <section className="dashboard-stats">
                        {estatisticas.map((item) => (
                            <article className="dashboard-stat" key={item.titulo}>
                                <div className="row align-center justify-between">
                                    <i className={`bi ${item.icon}`}></i>
                                    <p>{item.titulo}</p>
                                </div>

                                <h2><NumeroAnimado valor={item.valor} /></h2>

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
                                    className="transparent slide-right"
                                    onClick={() => navigate('/oportunidades')}
                                >
                                    Ver todas
                                    <i className="bi bi-arrow-right"></i>
                                </button>
                            </div>

                            {loading ? (
                                <p className="weak">A carregar oportunidades...</p>
                            ) : recentes.length === 0 ? (
                                <div className="dashboard-empty">
                                    <i className="bi bi-clipboard"></i>
                                    <p>Ainda não existem oportunidades.</p>
                                </div>
                            ) : (
                                <div className="column ">
                                    {recentes.map(oportunidade => (
                                        <div className="row pad align-center border-bottom" key={oportunidade.id}>
                                            <div className="column">
                                                <h4>{oportunidade.titulo}</h4>
                                                <div className="row align-center">
                                                    <p className='weak'>{oportunidade.cliente}</p>
                                                </div>
                                            </div>
                                            <div className="estado">{oportunidade.estadoTexto}</div>
                                        </div>

                                    ))}
                                </div>
                            )}
                        </article>

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
                </div>
            </main>
        </Layout>
    );
}
