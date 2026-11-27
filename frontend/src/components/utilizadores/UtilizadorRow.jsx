import { Pencil, Trash, TrashFull } from "@coolicons/react";
import Icon from "../Icon";

export default function UtilizadorRow({ utilizador, emCurso, onApagar, onEditar }) {
  return (
    <tr>
      <td>{utilizador.nome}</td>
      <td>{utilizador.email}</td>
      <td>
        <div className="estado">{utilizador.role}</div>
      </td>
      <td className="row">
        <button
            type="button"
            className="secondary"
            disabled={emCurso}
            onClick={() => onEditar(utilizador)}
          >
            <Icon icon={Pencil}/>
            Editar
          </button>
        {/* Os administradores não se apagam (o backend também recusa) */}
        {utilizador.role !== 'Administrador' && (
          
          <button
            type="button"
            className="secondary danger"
            disabled={emCurso}
            onClick={() => onApagar(utilizador)}
          >
            <Icon icon={TrashFull}/>
            {emCurso ? 'A apagar...' : 'Apagar'}
          </button>
        )}
      </td>
    </tr>
  );
}
