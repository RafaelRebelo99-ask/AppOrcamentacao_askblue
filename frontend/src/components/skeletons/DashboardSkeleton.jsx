export function DashboardStatsSkeleton() {
    return (
        <>
            {[1, 2, 3, 4].map(item => (
                <div className="dashboard-stat column gap-1" key={item}>
                    <SkeletonBlock height="1rem" width="60%"/>
                    <SkeletonBlock height="3rem" width="20%"/>
                    <SkeletonBlock height="1rem"/>
                </div>
            ))}
        </>
    );
}

export function DashboardRecentSkeleton({ isCliente = false }) {
    return (
        <div className="column gap-1">
            {[1, 2, 3, 4, 5].map(item => (
                <div
                    className="row pad-05 align-center border-bottom"
                    key={item}
                >
                    <div className="column" style={{ flex: 1 }}>
                        <SkeletonBlock width="20%" height="1rem" />

                        {!isCliente && (
                            <SkeletonBlock width="5%" height="1rem" />
                        )}
                    </div>

                    <SkeletonBlock
                        width="85px"
                        height="24px"
                    />

                    <SkeletonBlock
                        width="20px"
                        height="20px"
                    />
                </div>
            ))}
        </div>
    );
}

function SkeletonBlock({ width = '100%', height = '15rem' }) {
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