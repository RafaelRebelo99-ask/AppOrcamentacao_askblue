import { useRef } from 'react'
import { Calendar } from '@coolicons/react'

function emitSyntheticChange(name, value, onChange) {
  if (!onChange) return
  onChange({
    target: { name, value },
    currentTarget: { name, value },
  })
}

export default function DateInput({
  id,
  name,
  value = '',
  onChange,
  placeholder = 'AAAA-MM-DD',
  required = false,
  disabled = false,
  min,
  max,
  className = '',
  buttonLabel = 'Abrir calendário',
}) {
  const pickerRef = useRef(null)

  const handleOpenPicker = () => {
    if (disabled) return
    const picker = pickerRef.current
    if (!picker) return

    if (typeof picker.showPicker === 'function') {
      picker.showPicker()
    } else {
      picker.focus()
      picker.click()
    }
  }

  const handleTextChange = (e) => {
    // Allows typing or pasting dates freely, e.g. 2026-12-01
    emitSyntheticChange(name, e.target.value, onChange)
  }

  const handlePickerChange = (e) => {
    // Native date picker always produces YYYY-MM-DD
    emitSyntheticChange(name, e.target.value, onChange)
  }

  return (
    <div className={`row align-center gap-sm ${className}`}>
      <input
        id={id}
        name={name}
        type="text"
        inputMode="numeric"
        value={value}
        onChange={handleTextChange}
        placeholder={placeholder}
        pattern="\d{4}-\d{2}-\d{2}"
        required={required}
        disabled={disabled}
        autoComplete="off"
      />

      <button
        type="button"
        className="secondary"
        onClick={handleOpenPicker}
        disabled={disabled}
        aria-label={buttonLabel}
        title={buttonLabel}
      >
        <Calendar size={18} strokeWidth={1.5} />
      </button>

      {/* Hidden native date picker anchored to trigger the calendar popup */}
      <input
        ref={pickerRef}
        type="date"
        tabIndex={-1}
        aria-hidden="true"
        value={/^\d{4}-\d{2}-\d{2}$/.test(value) ? value : ''}
        min={min}
        max={max}
        disabled={disabled}
        onChange={handlePickerChange}
        style={{
          position: 'fixed',
          opacity: 0,
          pointerEvents: 'none',
          width: 0,
          height: 0,
        }}
      />
    </div>
  )
}