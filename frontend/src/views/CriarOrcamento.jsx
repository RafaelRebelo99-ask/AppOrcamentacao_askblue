import Layout from '../components/Layout';
import Aviso from '../components/Aviso';
import useCriarOrcamento from '../controllers/orcamentos/useNovoOrcamento';
import Select from '../components/Select';
import NumeroInput from '../components/NumeroInput';
import ThemeToggle from '../components/ThemeToggle';

const taxasIVA = [
  { label: '23%', value: '23' },
  { label: '10%', value: '10' },
  { label: '0% (isento)', value: '0' },
];

export default function CriarOrcamento() {
  const {
    oportunidade,
    dados,
    resumo,
    loading,
    loadingOportunidade,
    erro,
    sucesso,
    confirmarEnvio,
    handleChange,
    handleGuardarRascunho,
    handleEnviar,
    handleConfirmarEnvio,
    handleFecharConfirmacao,
    handleCancelar,
    handleSelectChange
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
      <div className="row">
        <h1 className="mb-1">Novo Orçamento</h1>
        <ThemeToggle/>
      </div>

      <section>
        <h2>{oportunidade.titulo}</h2>

        <p className='mb-1'>{oportunidade.descricao}</p>

        <div className="row wrap align-center">
          <div className="pill primary">
            {oportunidade.categoria}
          </div>

          <p>Prazo da proposta:</p><div className="date">{oportunidade.prazo}</div>

          <p>Data pretendida:</p><div className="date">{oportunidade.dataPretendida}</div>
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
              maxLength={4000}
              required
            />
          </div>

          <div className="row wrap gap-1 align-center">
            <div className="field">
              <label htmlFor="valorSemIVA">Valor sem IVA *</label>
              <div className="empty-box">

                <NumeroInput
                  id="valorSemIVA"
                  name="valorSemIVA"
                  value={dados.valorSemIVA}
                  onChange={handleChange}
                  required
                />

                <p id="moeda" className="weak">€</p>

              </div>

            </div>

            <div className="field">
              <label htmlFor="taxaIVA">Taxa de IVA (%) *</label>

              <Select
                options={taxasIVA}
                className='w-100'
                value={dados.taxaIVA}
                onChange={(value) => handleSelectChange('taxaIVA', value)}
              />


            </div>

            {/* Só para ajudar quem preenche; o valor final é calculado no backend */}
            <div className="field">
              <label>Valor do IVA</label>
              <div className='empty-box'>{resumo.valorIVA}</div>
            </div>

            <div className="field">
              <label>Valor total (incl. IVA)</label>
              <div className='empty-box'>{resumo.valorTotal}</div>
            </div>
          </div>

          <div className="row wrap gap-1">
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
              <label>Data de fim</label>
              <div className='date'>{resumo.dataFim}</div>
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

          {sucesso && <p role="status">{sucesso}</p>}
          <hr></hr>
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


      {/* Fora do <form>, para os botões do aviso não o submeterem */}
      <Aviso
        mensagem={confirmarEnvio
          ? 'Enviar a proposta ao cliente? Depois de enviada, não pode ser editada.'
          : ''}
        confirmar={true}
        textoConfirmar="Enviar"
        textoCancelar="Voltar"
        onFechar={handleFecharConfirmacao}
        onConfirmar={handleConfirmarEnvio}
      />
    </Layout>
  );
}