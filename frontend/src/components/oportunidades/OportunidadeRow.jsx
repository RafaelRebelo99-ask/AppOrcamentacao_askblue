import { useState } from 'react';

export default function OportunidadeRow({ oportunidade, onConsultar, onEditar, onEliminar }) {
  const [menuAberto, setMenuAberto] = useState(false);

  return (
    <tr>
      <td>{oportunidade.id}</td>

      <td>{oportunidade.titulo}</td>

      <td>
        <div className="estado">{oportunidade.estado}</div>
      </td>

      <td>
        <div className="row gap-0">
          <p>{oportunidade.cliente}</p>
          {oportunidade.clienteVerificado && (
            <i className="bi bi-check-lg"></i>
          )}
        </div>
      </td>

      <td>{oportunidade.categoria}</td>

      <td>
        {oportunidade.prazo}
        <div className={`pill ${oportunidade.aviso.cor}`}>
          {oportunidade.aviso.texto}
        </div>
      </td>

      <td className="right">{oportunidade.valor}</td>

      <td className="ml-auto">
        <div className="row">
          <button
            className="primary wide"
            onClick={() => onConsultar?.(oportunidade)}
          >
            Concluir Trabalho
          </button>

          <div className="pos-rel">
            <button
              className="more-actions"
              onClick={() => setMenuAberto(!menuAberto)}
              aria-label="Mais ações"
              aria-expanded={menuAberto}
            >
              <i className="bi bi-three-dots-vertical"></i>
            </button>

            {menuAberto && (
              <div className="menu right">
                <button
                  className="item wide"
                  onClick={() => {
                    setMenuAberto(false);
                    onEditar?.(oportunidade);
                  }}
                >
                  <i className="bi bi-pencil"></i>
                  <p>Editar</p>
                </button>

                <button
                  className="item wide"
                  onClick={() => {
                    setMenuAberto(false);
                    onConsultar?.(oportunidade);
                  }}
                >
                  <i className="bi bi-journal-text"></i>
                  <p>Consultar</p>
                </button>

                <button
                  className="item wide danger"
                  onClick={() => {
                    setMenuAberto(false);
                    onEliminar?.(oportunidade);
                  }}
                >
                  <i className="bi bi-trash"></i>
                  <p>Eliminar</p>
                </button>
              </div>
            )}
          </div>
        </div>
      </td>
    </tr>
  );
}