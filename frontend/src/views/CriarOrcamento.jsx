// ===== VIEW: Novo orçamento (Prestador) =====
// Só o visual, passado do DEMO_FRONT/criar-orcamento.html. Ainda sem lógica:
// os valores calculados (IVA, total, data de fim) mostram "—", os botões não fazem nada
// e o diálogo de confirmação está fechado.
// Os dados do serviço e das partes são de exemplo; vão vir de GET /api/servicos/{id} e do utilizador com login.

export default function CriarOrcamento() {
  return (
    <>
      <main>
        <div className="orcamento">
          <h1 className="mb-1">Novo orçamento</h1>

          {/* Resumo do serviço que o prestador está a orçamentar */}
          <section aria-labelledby="titulo-servico">
            <h2 id="titulo-servico" className="mb-05">Título do serviço</h2>

            {/* No DEMO_FRONT havia <div> dentro de <p>, o que não é HTML válido; aqui usa-se uma row */}
            <div className="row wrap mb-1">
              <div className="pill primary">Tecnologia</div>
              <div className="row"><p>Propostas até:</p><div className="date">25/11/2026</div></div>
              <div className="row"><p>Data desejada:</p><div className="date">23/12/2026</div></div>
            </div>
          </section>

          <hr />

          <form id="form-orcamento">
            <div className="column">
              {/* Partes: prestador e cliente */}
              <section>
                <div className="row wrap">
                  <div className="input-group column">
                    <div className="row"><p>Nome:</p><p>Prestador</p></div>
                    <div className="row"><p>NIF:</p><p>123456789</p></div>
                    <div className="row"><p>Email:</p><p>prestador@email.com</p></div>
                    <div className="row"><p>Telefone:</p><p>+351 912345678</p></div>
                  </div>

                  <div className="input-group column">
                    <div className="row"><p>Nome:</p><p>Cliente</p></div>
                    <div className="row"><p>NIF:</p><p className="weak">Disponível após aceitação</p></div>
                    <div className="row"><p>Email:</p><p className="weak">Disponível após aceitação</p></div>
                    <div className="row"><p>Telefone:</p><p className="weak">Disponível após aceitação</p></div>
                  </div>
                </div>
              </section>

              <hr />

              {/* Âmbito */}
              <section className="mb-1">
                <div className="field">
                  <label htmlFor="descricao">Descrição *</label>
                  <textarea id="descricao" name="descricao" required maxLength={2000}
                    placeholder="O que vai ser feito, e o que não está incluído."></textarea>
                </div>
              </section>

              {/* Preço */}
              <section>
                <div className="row mb-1">
                  <div className="field">
                    <label htmlFor="valor">Valor sem IVA *</label>
                    <div className="empty-box">
                      <p className="weak">€</p>
                      <input id="valor" name="valor" inputMode="decimal" required placeholder="0,00" />
                      <p>EUR</p>
                    </div>
                  </div>

                  <div className="field">
                    <label htmlFor="taxa">Taxa de IVA *</label>
                    <select id="taxa" name="taxa" required defaultValue="23">
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
                    <label htmlFor="valor-total">Valor total (incl. IVA)</label>
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
                    <input id="inicio" name="inicio" type="date" required />
                  </div>

                  <div className="field">
                    <label htmlFor="duracao">Duração (dias) *</label>
                    <input id="duracao" name="duracao" type="number" inputMode="numeric" min="1" step="1" required placeholder="0" />
                  </div>

                  <div className="calculado">
                    <label htmlFor="data-fim">Data de fim</label>
                    <output id="data-fim">—</output>
                  </div>
                </div>

                <div className="field mb-1" style={{ maxWidth: '20rem' }}>
                  <label htmlFor="validade">Validade da proposta *</label>
                  <input id="validade" className="mb-1" name="validade" type="date" required aria-describedby="ajuda-validade" />
                  <p className="hint" id="ajuda-validade">Depois desta data a proposta deixa de ser válida.</p>
                </div>
              </section>

              {/* Mensagens do envio (escondida até haver lógica) */}
              <div className="mensagem" role="status" aria-live="polite" hidden></div>

              {/* Ações */}
              <section>
                <div className="row" style={{ justifyContent: 'space-between' }}>
                  <div className="row">
                    <button type="button" className="wide secondary">Guardar rascunho</button>
                    <button type="submit" className="wide primary">Enviar orçamento</button>
                  </div>
                  <button type="button" className="wide secondary danger">Cancelar</button>
                </div>
              </section>
            </div>
          </form>
        </div>
      </main>

      {/* Confirmação antes de enviar (fechada; abre-se com a lógica) */}
      <dialog aria-labelledby="titulo-confirmar">
        <form method="dialog">
          <h2 id="titulo-confirmar">Enviar orçamento?</h2>
          <p>Depois de enviado, o orçamento <strong>não pode ser editado</strong> e vincula-te até à data de validade.
            O cliente pode aceitá-lo ou recusá-lo.</p>
          <div className="row apart">
            <button value="cancelar" className="secondary">Voltar</button>
            <button value="confirmar" className="primary">Enviar</button>
          </div>
        </form>
      </dialog>
    </>
  )
}
