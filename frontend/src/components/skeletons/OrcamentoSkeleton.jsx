
export default function OrcamentoCardSkeleton() {
  return (
    <div className="card column skeleton-card">
      {/* Header */}
      <div className="row space-between align-center">
        <SkeletonBlock width="90px" height="24px" />

        <div className="row align-center mb-05">
          <SkeletonBlock width="65px" height="24px" />
          <SkeletonBlock width="80px" height="24px" />
        </div>
      </div>

      {/* Content */}
      <div className="column">
        <SkeletonBlock width="75%" height="24px" />

        <SkeletonBlock width="100%" height="14px" />
        <SkeletonBlock width="90%" height="14px" />

        <SkeletonBlock width="140px" height="14px" />

        <hr />

        {/* Dates */}
        <div className="column gap-1">
          <div className="row align-center">
            <SkeletonBlock width="75px" height="14px" />
            <SkeletonBlock width="100px" height="20px" />
          </div>

          <div className="row align-center">
            <SkeletonBlock width="65px" height="14px" />
            <SkeletonBlock width="100px" height="20px" />
          </div>

          <div className="row align-center wrap">
            <SkeletonBlock width="75px" height="14px" />
            <SkeletonBlock width="100px" height="20px" />
            <SkeletonBlock width="15px" height="14px" />
            <SkeletonBlock width="100px" height="20px" />
          </div>
        </div>

        <hr />

        {/* Prices */}
        <div className="row align-center">
          <div className="column">
            <SkeletonBlock width="95px" height="14px" />
            <SkeletonBlock width="100px" height="20px" />
          </div>

          <div className="column">
            <SkeletonBlock width="40px" height="14px" />
            <SkeletonBlock width="80px" height="20px" />
          </div>
        </div>

        <hr />

        {/* Total */}
        <div className="empty-box mb-1">
          <SkeletonBlock width="180px" height="20px" />
        </div>
      </div>

      {/* Action buttons */}
      <div className="row">
        <SkeletonBlock width="30%" height="38px" />
        <SkeletonBlock width="30%" height="38px" />
      </div>
    </div>
  );
}

function SkeletonBlock({ width = '100%', height = '16px' }) {
  return (
    <div
      className="skeleton"
      style={{
        width,
        height,
        flexShrink: 0,
      }}
    />
  );
}
