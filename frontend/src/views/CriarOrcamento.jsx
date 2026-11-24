import Layout from '../components/Layout';
import useCriarOrcamento from '../controllers/orcamentos/useNovoOrcamento';

export default function CriarOrcamento() {
  const {
    oportunidade,
    dados,
    loading,
    loadingOportunidade,
    erro,
    handleChange,
    handleGuardarRascunho,
    handleEnviar,
    handleCancelar,
  } = useCriarOrcamento();

  if (loadingOportunidade) {
    return (
      <Layout>
        <p>A carregar oportunidade...</p>
      </Layout>
    );
  }

  if (!oportunidade) {
    return (
      <Layout>
        <p className="danger">
          {erro || 'Não foi possível encontrar a oportunidade.'}
        </p>

        <button className="secondary" onClick={handleCancelar}>
          Voltar
        </button>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="orcamento">
        <h1 className="mb-1">Nova proposta</h1>

        <section className="mb-1">
          <h2>{oportunidade.titulo}</h2>

          <p>{oportunidade.descricao}</p>

          <div className="row wrap">
            <div className="pill primary">
              {oportunidade.estado}
            </div>

            <p>
              Prazo da proposta:{' '}
              {oportunidade.prazoProposta
                ? new Date(oportunidade.prazoProposta).toLocaleDateString('pt-PT')
                : 'Não definido'}
            </p>

            <p>
              Data pretendida:{' '}
              {oportunidade.dataPretendida
                ? new Date(oportunidade.dataPretendida).toLocaleDateString('pt-PT')
                : 'Não definida'}
            </p>
          </div>
        </section>

        <hr />

        <form onSubmit={handleEnviar}>
          <div className="column gap-1">
            <div className="field">
              <label htmlFor="descricao">Descrição da proposta *</label>

              <textarea
                id="descricao"
                name="descricao"
                value={dados.descricao}
                onChange={handleChange}
                required
              />
            </div>

            <div className="row wrap">
              <div className="field">
                <label htmlFor="valorSemIVA">Valor sem IVA (€) *</label>

                <input
                  id="valorSemIVA"
                  name="valorSemIVA"
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={dados.valorSemIVA}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="field">
                <label htmlFor="taxaIVA">Taxa de IVA (%) *</label>

                <select
                  id="taxaIVA"
                  name="taxaIVA"
                  value={dados.taxaIVA}
                  onChange={handleChange}
                  required
                >
                  <option value="23">23%</option>
                  <option value="13">13%</option>
                  <option value="6">6%</option>
                  <option value="0">0%</option>
                </select>
              </div>
            </div>

            <div className="row wrap">
              <div className="field">
                <label htmlFor="dataInicio">Data de início *</label>

                <input
                  id="dataInicio"
                  name="dataInicio"
                  type="date"
                  value={dados.dataInicio}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="field">
                <label htmlFor="duracaoDias">Duração (dias) *</label>

                <input
                  id="duracaoDias"
                  name="duracaoDias"
                  type="number"
                  min="1"
                  step="1"
                  value={dados.duracaoDias}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="field">
                <label htmlFor="dataValidade">Validade da proposta *</label>

                <input
                  id="dataValidade"
                  name="dataValidade"
                  type="date"
                  value={dados.dataValidade}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            {erro && (
              <p className="danger" role="alert">
                {erro}
              </p>
            )}

            <div className="row">
              <button
                type="button"
                className="secondary"
                onClick={handleCancelar}
                disabled={loading}
              >
                Cancelar
              </button>

              <button
                type="button"
                className="secondary"
                onClick={handleGuardarRascunho}
                disabled={loading}
              >
                Guardar rascunho
              </button>

              <button
                type="submit"
                className="primary"
                disabled={loading}
              >
                {loading ? 'A processar...' : 'Enviar proposta'}
              </button>
            </div>
          </div>
        </form>
      </div>
    </Layout>
  );
}