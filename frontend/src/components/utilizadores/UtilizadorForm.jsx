
import Select from '../Select';

const funcoes = [
  { value: 'Cliente', label: 'Cliente' },
  { value: 'Colaborador', label: 'Colaborador' },
];

export default function UtilizadorForm({
  dados,
  onChange,
  onSubmit,
  onCancelar,
  erro,
  loading,
  modoEdicao = false,
}) {
  return (
    <form onSubmit={onSubmit} className="card column mb-1 w-100">
      <h2 className="mb-05">
        {modoEdicao ? 'Editar utilizador' : 'Novo utilizador'}
      </h2>

      <div className="row wrap align-start mb-05">
        <div className="column">
          <label htmlFor="nome">Nome *</label>
          <input
            id="nome"
            name="nome"
            value={dados.nome}
            onChange={onChange}
            minLength={2}
            maxLength={100}
            required
          />
        </div>

        <div className="column">
          <label htmlFor="email">Email *</label>
          <input
            id="email"
            name="email"
            type="email"
            value={dados.email}
            onChange={onChange}
            maxLength={254}
            required
          />
        </div>

        {modoEdicao ? (
          <div className="column">
            <label>Função</label>
            <div className='estado'>{dados.role}</div>
          </div>
        ) : (
          <>
            <div className="column">
              <label htmlFor="password">Password inicial *</label>
              <input
                id="password"
                name="password"
                type="password"
                value={dados.password}
                onChange={onChange}
                minLength={8}
                autoComplete="new-password"
                required
              />
            </div>

            <div className="column">
              <label>Função *</label>
              <Select
                value={dados.role}
                onChange={value =>
                  onChange({ target: { name: 'role', value } })
                }
                placeholder="Selecionar função"
                options={funcoes}
              />
            </div>
          </>
        )}
      </div>

      {erro && <p className="danger mb-05">{erro}</p>}

      <div className="row">
        <button
          type="button"
          className="secondary"
          onClick={onCancelar}
          disabled={loading}
        >
          Cancelar
        </button>

        <button type="submit" className="primary" disabled={loading}>
          {loading
            ? 'A guardar...'
            : modoEdicao
              ? 'Guardar alterações'
              : 'Criar utilizador'}
        </button>
      </div>
    </form>
  );
}
