export default function UtilizadorRow({ utilizador, emCurso, onApagar }) {
  return (
    <tr>
      <td>{utilizador.nome}</td>
      <td>{utilizador.email}</td>
      <td>
        <div className="estado">{utilizador.role}</div>
      </td>
      <td className="ml-auto">
        {/* Os administradores não se apagam (o backend também recusa) */}
        {utilizador.role !== 'Administrador' && (
          <button
            type="button"
            className="secondary danger"
            disabled={emCurso}
            onClick={() => onApagar(utilizador)}
          >
            <i className="bi bi-trash"></i>
            {emCurso ? 'A apagar...' : 'Apagar'}
          </button>
        )}
      </td>
    </tr>
  );
}
