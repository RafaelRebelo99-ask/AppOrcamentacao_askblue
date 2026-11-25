import Layout from '../components/Layout';
import OportunidadeForm from '../components/oportunidades/OportunidadeForm';
import useNovaOportunidade from '../controllers/useNovaOportunidade';

export default function NovaOportunidade() {
    const {
        dados,
        handleChange,
        handleSubmit,
        handleCancelar,
        loading,
        erro,
        clientes,
        categorias,
    } = useNovaOportunidade();

    return (
        <Layout>
            <div className="column gap w-50">

                <h1>Nova oportunidade</h1>
                <p className='mb-1 weak'>Regista uma nova oportunidade comercial da AskBlue.</p>

                <div className="row">
                    <OportunidadeForm
                        dados={dados}
                        clientes={clientes}
                        categorias={categorias}
                        onChange={handleChange}
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