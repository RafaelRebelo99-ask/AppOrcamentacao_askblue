export default function OrcamentoCard({
    orcamento,
    podeGerir,
    emCurso,
    onEnviar,
    onEliminar,
    onAceitar,
    onRecusar,
}) {
    return (
        <div className="card column">
            <div className="row space-between align-center">
                <div className="id" title={orcamento.id}>
                    ORC-{orcamento.id.slice(0, 8)}
                </div>

                <div className="row">
                    <div className="estado">
                        {orcamento.estado}
                    </div>
                    <button type="button" className="secondary transparent more-actions">
                        <i className="bi bi-download"></i>
                    </button>

                </div>

            </div>

            <div className="column">
                <h2 className="mb-05">
                    {orcamento.oportunidade}
                </h2>
                <p className="mb-05">{orcamento.descricao}</p>
                {podeGerir && <p>Cliente: {orcamento.cliente}</p>}
                <hr></hr>

                <div className="column gap-1">
                    <div className="row align-center">
                        <p>Criada em:</p> <div className="date">{orcamento.criadaEm}</div>
                    </div>

                    <div className="row align-center">
                        <p>Validade:</p> <div className="date">{orcamento.validade}</div>
                    </div>

                    <div className="row align-center wrap">
                        <p>Execução:</p>
                        <div className="date">{orcamento.inicio}</div>
                        <p>a</p>
                        <div className="date">{orcamento.fim}</div>
                    </div>
                </div>

                <hr></hr>

                <div className="row align-center">
                    <div className="column">
                        <p>
                            Valor sem IVA:
                        </p>
                        <p>{orcamento.valorSemIVA}</p>
                    </div>

                    <div className="column">
                        <p>
                            IVA:
                        </p>
                        <p>{orcamento.valorIVA}</p>
                    </div>
                </div>

                <hr />

                <div className="empty-box mb-1">
                    Valor Total: {orcamento.valorTotal}
                </div>

                {orcamento.expirado && (
                    <p className="danger">Proposta expirada</p>
                )}
            </div>

            {/* Colaborador e Administrador */}
            {podeGerir && orcamento.estado === 'Rascunho' && (
                <div className="row">
                    <button
                        type="button"
                        className="primary"
                        disabled={emCurso}
                        onClick={() => onEnviar(orcamento)}
                    >
                        Enviar
                    </button>

                    <button
                        type="button"
                        className="secondary danger"
                        disabled={emCurso}
                        onClick={() => onEliminar(orcamento)}
                    >
                        Eliminar rascunho
                    </button>
                </div>
            )}

            {/* Cliente */}
            {!podeGerir && orcamento.podeDecidir && (
                <div className="row">
                    <button
                        type="button"
                        className="primary"
                        disabled={emCurso}
                        onClick={() => onAceitar(orcamento)}
                    >
                        Aceitar
                    </button>

                    <button
                        type="button"
                        className="secondary danger"
                        disabled={emCurso}
                        onClick={() => onRecusar(orcamento)}
                    >
                        Recusar
                    </button>
                </div>
            )}
        </div>
    );
}
