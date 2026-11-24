export default function OportunidadeForm({
    dados,
    clientes,
    categorias,
    onChange,
    onSubmit,
    onCancelar,
    erro,
    loading,
}) {
    return (
        <form onSubmit={onSubmit} className="column gap-2">

            <div className="column">
                <h2>Informação geral</h2>

                <label>Título *</label>
                <input
                    name="titulo"
                    value={dados.titulo}
                    onChange={onChange}
                    maxLength={200}
                    required
                />

                <label>Descrição *</label>
                <textarea
                    name="descricao"
                    value={dados.descricao}
                    onChange={onChange}
                    rows={5}
                    required
                />
            </div>

            <hr className="mb-1" />

            <div className="row">
                <div className="column">
                    <h2>Classificação</h2>

                    <label>Cliente *</label>
                    <select
                        name="idCliente"
                        value={dados.idCliente}
                        onChange={onChange}
                        required
                    >
                        <option value="">Selecionar cliente</option>

                        {clientes.map(cliente => (
                            <option key={cliente.id} value={cliente.id}>
                                {cliente.nome}
                            </option>
                        ))}
                    </select>

                    <label>Categoria *</label>
                    <select
                        name="idCategoria"
                        value={dados.idCategoria}
                        onChange={onChange}
                        required
                    >
                        <option value="">Selecionar categoria</option>

                        {categorias.map(categoria => (
                            <option key={categoria.id} value={categoria.id}>
                                {categoria.nome}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="column">
                    <h2>Datas</h2>

                    <div className="column">
                        <div className="column" style={{ flex: 1 }}>
                            <label>Prazo da proposta</label>
                            <input
                                type="date"
                                name="prazoProposta"
                                value={dados.prazoProposta}
                                onChange={onChange}
                            />
                        </div>

                        <div className="column" style={{ flex: 1 }}>
                            <label>Data pretendida</label>
                            <input
                                type="date"
                                name="dataPretendida"
                                value={dados.dataPretendida}
                                onChange={onChange}
                            />
                        </div>
                    </div>
                </div>

                {erro && <p className="danger">{erro}</p>}
            </div>


            <hr />

            <div className="row">
                <button
                    type="button"
                    className="secondary"
                    onClick={onCancelar}
                >
                    Cancelar
                </button>

                <button
                    type="submit"
                    className="primary"
                    disabled={loading}
                >
                    {loading ? 'A registar...' : 'Registar oportunidade'}
                </button>
            </div>

        </form>
    );
}