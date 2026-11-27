// src/components/AppIcon.jsx
export default function Icon({
  icon: IconComponent,
  size = 21,
  strokeWidth = 1.5,
  className = '',
  ...props
}) {
  if (!IconComponent) return null

  return (
    <IconComponent
      size={size}
      strokeWidth={strokeWidth}
      className={className}
      {...props}
    />
  )
}