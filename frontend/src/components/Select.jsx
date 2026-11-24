
import { useEffect, useRef, useState } from 'react';
import '../askblue.css';

export default function Select({
    options = [],
    value,
    onChange,
    placeholder = 'Selecionar...',
    className = '',
    disabled = false,
}) {
    const [isOpen, setIsOpen] = useState(false);
    const selectRef = useRef(null);

    const selectedOption = options.find(
        (option) => option.value === value
    );

    useEffect(() => {
        function handleClickOutside(event) {
            if (
                selectRef.current &&
                !selectRef.current.contains(event.target)
            ) {
                setIsOpen(false);
            }
        }

        function handleEscape(event) {
            if (event.key === 'Escape') {
                setIsOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        document.addEventListener('keydown', handleEscape);

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
            document.removeEventListener('keydown', handleEscape);
        };
    }, []);

    function handleSelect(option) {
        onChange(option.value);
        setIsOpen(false);
    }

    return (
        <div className={`${className}`} ref={selectRef}>
            <button
                type="button"
                className={`wide secondary pos-rel ${isOpen ? 'active' : ''}`}
                onClick={() => setIsOpen(!isOpen)}
                disabled={disabled}
                aria-haspopup="listbox"
                aria-expanded={isOpen}
            >
                <span>{selectedOption?.label || placeholder}</span>
                <i className={`bi bi-caret-${isOpen ? 'up' : 'down'}`} />
            </button>

            {isOpen && (
                <div className="menu custom-select-menu" role="listbox">
                    {options.map((option) => (
                        <button
                            type="button"
                            key={option.value}
                            className={`item w-100 ${option.value === value ? 'selected' : ''
                                }`}
                            onClick={() => handleSelect(option)}
                            role="option"
                            aria-selected={option.value === value}
                        >
                            {option.label}
                            {option.value === value && (
                                <i className="bi bi-check2" />
                            )}
                        </button>
                    ))}
                </div>
            )}
        </div>
    );
}
