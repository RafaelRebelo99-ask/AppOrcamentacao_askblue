export default function OportunidadeRow({ oportunidade }) {
  return (
    <tr>
      <td>{oportunidade.id}</td>
      <td>{oportunidade.titulo}</td>
      <td>
        <div className="estado">{oportunidade.estado}</div>
      </td>
      <td>{oportunidade.cliente}</td>
      <td>{oportunidade.categoria}</td>
      <td>
        {oportunidade.prazo}
        <div className={`pill ${oportunidade.aviso.cor}`}>
          {oportunidade.aviso.texto}
        </div>
      </td>
      <td className="right">{oportunidade.valor}</td>
      <td className="ml-auto">
        <button className="primary wide">
          Consultar oportunidade
        </button>
      </td>
    </tr>
  );
}