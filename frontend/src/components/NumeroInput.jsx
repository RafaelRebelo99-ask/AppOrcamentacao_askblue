import React, { useState } from 'react';

function formatarNumero(valor) {
    if (valor === '' || valor === null || valor === undefined) return '';

    const numero = Number(valor);

    if (isNaN(numero)) return valor;

    return numero.toLocaleString('pt-PT', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

export default function NumeroInput({
    id,
    name,
    value,
    onChange,
    placeholder = '1.234.567,89',
    required = false
}) {
    const [focused, setFocused] = useState(false);

    const handleChange = (e) => {
        let valor = e.target.value;

        // Permitir apenas números, pontos e vírgulas
        valor = valor.replace(/[^\d.,]/g, '');

        onChange({
            target: {
                name,
                value: valor
            }
        });
    };

    const handleBlur = () => {
        setFocused(false);

        if (value !== '') {
            const numero = Number(
                String(value)
                    .replace(/\./g, '')
                    .replace(',', '.')
            );

            if (!isNaN(numero)) {
                onChange({
                    target: {
                        name,
                        value: formatarNumero(numero)
                    }
                });
            }
        }
    };

    return (
        <input
            id={id}
            name={name}
            type="text"
            inputMode="decimal"
            placeholder={placeholder}
            value={focused ? value : formatarNumero(value)}
            onChange={handleChange}
            onFocus={() => setFocused(true)}
            onBlur={handleBlur}
            required={required}
        />
    );
}