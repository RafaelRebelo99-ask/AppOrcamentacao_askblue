
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
        <SkeletonBlock />
      </td>

      {/* Título */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Estado */}
      <td>
        <SkeletonBlock  />
      </td>

      {/* Cliente */}
      <td>
        <SkeletonBlock  />
      </td>

      {/* Categoria */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Prazo da proposta */}
      <td>
        <div className="column gap-05">
          <SkeletonBlock />
          <SkeletonBlock />
        </div>
      </td>

      {/* Data pretendida */}
      <td>
        <SkeletonBlock />
      </td>

      {/* Ações */}
      <td>
        <div className="row align-center">
          <SkeletonBlock  />
          <SkeletonBlock />
        </div>
      </td>
    </tr>
  );
}
