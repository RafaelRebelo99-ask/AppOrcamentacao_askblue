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
                <h2 className="mb-05">Informação geral</h2>

                <label>Título *</label>
                <input
                    name="titulo"
                    value={dados.titulo}
                    onChange={onChange}
                    maxLength={200}
                    required
                    className="mb-05"
                />

                <label>Descrição *</label>
                <textarea
                    name="descricao"
                    value={dados.descricao}
                    onChange={onChange}
                    rows={5}
                    required
                    className="mb-05"
                />
            </div>

            <hr className="mb-1" />

            <div className="row">
                <div className="column mb-1">
                    <h2 className="mb-05">Classificação</h2>

                    <label>Cliente *</label>
                    <select
                        name="idCliente"
                        value={dados.idCliente}
                        onChange={onChange}
                        required
                        className="mb-05"
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
                    <h2 className="mb-05">Datas</h2>

                    <label>Prazo da proposta</label>
                    <input
                        type="date"
                        name="prazoProposta"
                        value={dados.prazoProposta}
                        onChange={onChange}
                        className="mb-05"
                    />

                    <label>Data pretendida</label>
                    <input
                        type="date"
                        name="dataPretendida"
                        value={dados.dataPretendida}
                        onChange={onChange}
                    />

                </div>


                {erro && <p className="danger">{erro}</p>}
            </div>


            <hr />

            <div className="row mt-auto">
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