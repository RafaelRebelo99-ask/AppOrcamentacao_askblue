export default function OrcamentoCard({ orcamento }) {
    return (
        <div className="card column" key={orcamento.id}>
            <div className="row space-between align-center mb-1">
                <div className="id">{orcamento.id}</div>
                <div className="estado">{orcamento.estado}</div>
            </div>

            <div className="column">
                <h2 className="mb-05">{orcamento.titulo}</h2>

                <div className="row align-center">
                    <div className="pill primary">{orcamento.categoria}</div>
                    <div className="row gap-0 align-center">
                        <p>{orcamento.cliente}</p>
                        <i className="bi bi-check-lg"></i>
                    </div>
                </div>
            </div>

            <hr />

            <div className="row">
                <button className="primary">Abrir</button>
                <button className="secondary danger">Eliminar</button>
            </div>
        </div>
    );
}