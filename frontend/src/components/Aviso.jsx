export default function Aviso({
    mensagem,
    onFechar,
    confirmar = false,
    onConfirmar,
    textoConfirmar = 'Confirmar',
    textoCancelar = 'Cancelar',
}) {
    if (!mensagem) return null;

    return (
        <div className="overlay">

            <div className="box modal">
                <div className="row align-center space-between mb-1">
                    <h2>AskBlue</h2>
                    <button className="more-actions" type="button" onClick={onFechar}>
                        <i className="bi bi-x-lg"></i>
                    </button>
                </div>
                <div
                    className="column"
                    role="alertdialog"
                    aria-modal="true"
                    aria-label="Aviso"
                >
                    <p className="mb-1">{mensagem}</p>

                    <div className="row align-center">
                        {confirmar && (
                            <button
                                className="secondary"
                                onClick={onFechar}
                            >
                                {textoCancelar}
                            </button>
                        )}

                        <button
                            className="secondary danger"
                            onClick={confirmar ? onConfirmar : onFechar}
                        >
                            {confirmar ? textoConfirmar : 'OK'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
