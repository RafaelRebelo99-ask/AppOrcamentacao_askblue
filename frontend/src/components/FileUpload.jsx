import { useRef } from 'react'
import Icon from './Icon'
import { Upload } from '@coolicons/react'

export default function FileUpload({
  id = 'ficheiros-input',
  multiple = true,
  accept,
  onChange,
  disabled = false,
  buttonText = 'Escolher ficheiros',
  hintText = 'PDF, DOC, DOCX, XLS, XLSX, PNG, JPG, TXT (máx. 10 MB cada)',
  className = '',
}) {
  const inputRef = useRef(null)

  const handleClick = () => {
    if (!disabled) {
      inputRef.current?.click()
    }
  }

  const handleChange = (e) => {
    onChange?.(e)
    // Reset value so selecting the same file again still triggers onChange
    e.target.value = ''
  }

  return (
    <div className={`column gap-sm ${className}`}>
      <input
        ref={inputRef}
        id={id}
        type="file"
        multiple={multiple}
        accept={accept}
        onChange={handleChange}
        disabled={disabled}
        style={{ display: 'none' }}
      />

      <div className="row align-center gap wrap">
        <button
          type="button"
          className="secondary"
          onClick={handleClick}
          disabled={disabled}
        >
          <Icon icon={Upload}/>
          <span>{buttonText}</span>
        </button>

        {hintText && <span className="hint">{hintText}</span>}
      </div>
    </div>
  )
}