import Select from "../Select";
import { formatarTamanhoFicheiro } from "../../models/oportunidadesModel";
import DateInput from "../DateInput";
import FileUpload from "../FileUpload";
import Icon from "../Icon";
import { File } from "@coolicons/react";

export default function OportunidadeForm({
    dados,
    documentos = [],
    clientes,
    categorias,
    onChange,
    onAdicionarDocumentos,
    onRemoverDocumento,
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
                    <h2 className="mb-1">Classificação</h2>

                    <label>Cliente *</label>
                    <Select
                        className="mb-1"
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
                    <h2 className="mb-1">Datas</h2>

                    <label>Prazo da proposta</label>
                    <DateInput
                        name="prazoProposta"
                        value={dados.prazoProposta}
                        onChange={onChange}
                        className='mb-1'
                    />

                    <label>Data pretendida</label>
                    <DateInput
                        name="dataPretendida"
                        value={dados.dataPretendida}
                        onChange={onChange}
                    />

                </div>


                {erro && <p className="danger">{erro}</p>}
            </div>


            <hr />

            <div className="column">
                <h2 className="mb-05">Documentos (opcional)</h2>
                <label className="mb-1">Anexar ficheiros (máx. 10 MB cada)</label>
                <FileUpload
                    id="documentos-upload"
                    multiple
                    accept=".pdf,.doc,.docx,.xls,.xlsx,.png,.jpg,.jpeg,.txt"
                    onChange={onAdicionarDocumentos}
                    disabled={loading}
                    buttonText="Selecionar ficheiros"
                    hintText="Máx. 10 MB por ficheiro"
                    className="mb-05"
                />

                {documentos.length > 0 && (
                    <div className="column gap">
                        {documentos.map((doc, indice) => (
                            <div
                                key={`${doc.name}-${doc.size}`}
                                className="row align-center space-between"
                            >
                                <div className="row align-center">
                                    <Icon icon={File}/>{' '}
                                    {doc.name}{' '}
                                    <span className="weak">
                                        ({formatarTamanhoFicheiro(doc.size)})
                                    </span>
                                </div>

                                <button
                                    type="button"
                                    className="more-actions"
                                    onClick={() => onRemoverDocumento(indice)}
                                    disabled={loading}
                                    aria-label={`Remover ${doc.name}`}
                                >
                                    <i className="bi bi-x-lg"></i>
                                </button>
                            </div>
                        ))}
                    </div>
                )}
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