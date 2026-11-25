import Layout from '../components/Layout';
import ThemeToggle from '../components/ThemeToggle';
import OportunidadeForm from '../components/oportunidades/OportunidadeForm';
import useNovaOportunidade from '../controllers/useNovaOportunidade';

export default function NovaOportunidade() {
    const {
        dados,
        documentos,
        handleChange,
        handleAdicionarDocumentos,
        handleRemoverDocumento,
        handleSubmit,
        handleCancelar,
        loading,
        erro,
        clientes,
        categorias,
    } = useNovaOportunidade();

    return (
        <Layout>
            <div className="column gap">

                <div className="row align-center">
                    <h1>Nova oportunidade</h1>
                    <ThemeToggle />
                </div>
                <p className='mb-1 weak'>Regista uma nova oportunidade comercial da AskBlue.</p>

                <div className="row">
                    <OportunidadeForm
                        dados={dados}
                        documentos={documentos}
                        clientes={clientes}
                        categorias={categorias}
                        onChange={handleChange}
                        onAdicionarDocumentos={handleAdicionarDocumentos}
                        onRemoverDocumento={handleRemoverDocumento}
                        onSubmit={handleSubmit}
                        onCancelar={handleCancelar}
                        erro={erro}
                        loading={loading}
                    />
                </div>
            </div>
        </Layout>
    );
}