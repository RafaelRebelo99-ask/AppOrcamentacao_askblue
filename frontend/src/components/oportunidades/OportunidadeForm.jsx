import Select from "../Select";

export default function OportunidadeForm({
    dados,
    clientes,
    categorias,
    onChange,
    onSubmit,
    onCancelar,
    erro,
    loading,
    className
}) {
    return (
        <form onSubmit={onSubmit} className={`column gap-1 ${className}`}>

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

            <hr />

            <div className="row">
                <div className="column w-50">
                    <h2 className="mb-05">Classificação</h2>

                    <label>Cliente *</label>
                    <Select
                    className="mb-05"
                        name="idCliente"
                        value={dados.idCliente}
                        onChange={(value) =>
                            onChange({
                                target: {
                                    name: 'idCliente',
                                    value
                                }
                            })
                        }
                        placeholder="Selecionar cliente"
                        options={clientes.map(cliente => ({
                            value: cliente.id,
                            label: cliente.nome
                        }))}
                    />

                    <label>Categoria *</label>
                    <Select
                    name="idCategoria"
                        value={dados.idCategoria}
                        onChange={(value) =>
                            onChange({
                                target: {
                                    name: 'idCategoria',
                                    value
                                }
                            })
                        }
                        placeholder="Selecionar categoria"
                        options={categorias.map(categorias => ({
                            value: categorias.id,
                            label: categorias.nome
                        }))}
                    />
                </div>

                <div className="column w-50">
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