
// ===== VIEW: Nova proposta (Colaborador AskBlue) =====
// Apenas apresentação visual. A lógica será ligada posteriormente
// aos endpoints de oportunidades e orçamentos.

export default function CriarOrcamento() {
  return (
    <>
      <main>
        <div className="orcamento">
          <h1 className="mb-1">Nova proposta</h1>

          {/* Resumo da oportunidade */}
          <section aria-labelledby="titulo-oportunidade">
            <h2 id="titulo-oportunidade" className="mb-05">
              Título da oportunidade
            </h2>

            <div className="row wrap mb-1">
              <div className="pill primary">Categoria</div>

              <div className="row">
                <p>Prazo para propostas:</p>
                <div className="date">25/11/2026</div>
              </div>

              <div className="row">
                <p>Data pretendida:</p>
                <div className="date">23/12/2026</div>
              </div>
            </div>
          </section>

          <hr />

          <form id="form-orcamento">
            <div className="column">

              {/* AskBlue e cliente */}
              <section>
                <div className="row wrap">

                  <div className="input-group column">
                    <p>AskBlue</p>
                    <div className="row">
                      <p>Empresa:</p>
                      <p>AskBlue</p>
                    </div>
                    <div className="row">
                      <p>NIF:</p>
                      <p>XXXXXXXXX</p>
                    </div>
                    <div className="row">
                      <p>Email:</p>
                      <p>info@askblue.com</p>
                    </div>
                  </div>

                  <div className="input-group column">
                    <p>Cliente</p>
                    <div className="row">
                      <p>Nome:</p>
                      <p>Nome do cliente</p>
                    </div>
                    <div className="row">
                      <p>Email:</p>
                      <p>cliente@email.com</p>
                    </div>
                  </div>

                </div>
              </section>

              <hr />

              {/* Descrição da proposta */}
              <section className="mb-1">
                <div className="field">
                  <label htmlFor="descricao">Descrição da proposta *</label>
                  <textarea
                    id="descricao"
                    name="Descricao"
                    required
                    maxLength={2000}
                    placeholder="Descreva o trabalho a realizar, os objetivos e o que está ou não incluído na proposta."
                  />
                </div>
              </section>

              {/* Valores */}
              <section>
                <div className="row mb-1">

                  <div className="field">
                    <label htmlFor="valor">Valor sem IVA *</label>
                    <div className="empty-box">
                      <p className="weak">€</p>
                      <input
                        id="valor"
                        name="ValorSemIVA"
                        inputMode="decimal"
                        required
                        placeholder="0,00"
                      />
                      <p>EUR</p>
                    </div>
                  </div>

                  <div className="field">
                    <label htmlFor="taxa">Taxa de IVA *</label>
                    <select
                      id="taxa"
                      name="TaxaIVA"
                      required
                      defaultValue="23"
                    >
                      <option value="23">23%</option>
                      <option value="13">13%</option>
                      <option value="6">6%</option>
                      <option value="0">0% (isento)</option>
                    </select>
                  </div>

                  <div className="calculado">
                    <label htmlFor="valor-iva">Valor do IVA</label>
                    <output id="valor-iva">—</output>
                  </div>

                  <div className="calculado total">
                    <label htmlFor="valor-total">
                      Valor total (incl. IVA)
                    </label>
                    <output id="valor-total">—</output>
                  </div>

                </div>
              </section>

              <hr />

              {/* Prazos */}
              <section>
                <div className="row wrap mb-1">

                  <div className="field">
                    <label htmlFor="inicio">Data de início *</label>
                    <input
                      id="inicio"
                      name="DataInicio"
                      type="date"
                      required
                    />
                  </div>

                  <div className="field">
                    <label htmlFor="duracao">Duração (dias) *</label>
                    <input
                      id="duracao"
                      name="DuracaoDias"
                      type="number"
                      inputMode="numeric"
                      min="1"
                      step="1"
                      required
                      placeholder="0"
                    />
                  </div>

                  <div className="calculado">
                    <label htmlFor="data-fim">Data de fim</label>
                    <output id="data-fim">—</output>
                  </div>

                </div>

                <div className="field mb-1" style={{ maxWidth: '20rem' }}>
                  <label htmlFor="validade">Validade da proposta *</label>
                  <input
                    id="validade"
                    className="mb-1"
                    name="DataValidade"
                    type="date"
                    required
                    aria-describedby="ajuda-validade"
                  />
                  <p className="hint" id="ajuda-validade">
                    Após esta data, a proposta deixa de ser válida.
                  </p>
                </div>
              </section>

              {/* Mensagens */}
              <div
                className="mensagem"
                role="status"
                aria-live="polite"
                hidden
              />

              {/* Ações */}
              <section>
                <div
                  className="row"
                  style={{ justifyContent: 'space-between' }}
                >
                  <div className="row">
                    <button
                      type="button"
                      className="wide secondary"
                    >
                      Guardar rascunho
                    </button>

                    <button
                      type="submit"
                      className="wide primary"
                    >
                      Enviar proposta
                    </button>
                  </div>

                  <button
                    type="button"
                    className="wide secondary danger"
                  >
                    Cancelar
                  </button>
                </div>
              </section>

            </div>
          </form>
        </div>
      </main>

      {/* Confirmação antes do envio */}
      <dialog aria-labelledby="titulo-confirmar">
        <form method="dialog">
          <h2 id="titulo-confirmar">Enviar proposta?</h2>

          <p>
            Depois de enviada, a proposta não poderá ser editada.
            O cliente poderá aceitá-la ou recusá-la até à data
            de validade.
          </p>

          <div className="row apart">
            <button value="cancelar" className="secondary">
              Voltar
            </button>

            <button value="confirmar" className="primary">
              Enviar
            </button>
          </div>
        </form>
      </dialog>
    </>
  );
}
