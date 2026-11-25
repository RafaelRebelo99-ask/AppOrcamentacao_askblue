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
            <div className="row space-between align-center mb-1">
                <div className="id" title={orcamento.id}>
                    {orcamento.id.slice(0, 8)}
                </div>

                <div className="estado">
                    {orcamento.estado}
                </div>
            </div>

            <div className="column">
                <h2 className="mb-05">
                    {orcamento.oportunidade}
                </h2>

                {podeGerir && <p>Cliente: {orcamento.cliente}</p>}

                <p className="mb-05">{orcamento.descricao}</p>

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

                <div className="row">
                    <p>
                        Valor sem IVA: {orcamento.valorSemIVA}
                    </p>
                    <div className="vl"></div>
                    <p>
                        IVA ({orcamento.taxaIVA}%): {orcamento.valorIVA}
                    </p>
                </div>

                <hr/>

                <div className="empty-box">
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
