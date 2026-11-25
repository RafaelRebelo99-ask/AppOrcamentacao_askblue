function SkeletonBlock({ width = '100%', height = '1.5rem' }) {
  return (
    <div
      className="skeleton"
      style={{
        width,
        height,
        borderRadius: '6px',
      }}
    />
  );
}

export default function OportunidadeRowSkeleton() {
  return (
    <tr>
      {/* ID */}
      <td>
        <SkeletonBlock width="30%" />
      </td>

      {/* Título */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Estado */}
      <td>
        <SkeletonBlock width="60%" />
      </td>

      {/* Cliente */}
      <td>
        <SkeletonBlock width="70%"/>
      </td>

      {/* Categoria */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Prazo da proposta */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Data pretendida */}
      <td>
        <SkeletonBlock width="60%"/>
      </td>

      {/* Ações */}
      <td>
        <div className="row align-center">
          <SkeletonBlock />
          <SkeletonBlock />
        </div>
      </td>
    </tr>
  );
}
