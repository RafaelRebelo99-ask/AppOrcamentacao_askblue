import { useEffect, useState } from 'react';

export default function NumeroAnimado({
    valor,
    duracao = 1200
}) {
    const numeroFinal = Number(valor) || 0;
    const [numero, setNumero] = useState(0);

    useEffect(() => {
        let frame;
        let inicio;

        setNumero(0);

        const animar = (timestamp) => {
            if (!inicio) inicio = timestamp;

            const progresso = Math.min(
                (timestamp - inicio) / duracao,
                1
            );

            // Ease-out cubic: começa rápido e abranda no final
            const easing = 1 - Math.pow(1 - progresso, 3);

            setNumero(
                Math.round(numeroFinal * easing)
            );

            if (progresso < 1) {
                frame = requestAnimationFrame(animar);
            }
        };

        frame = requestAnimationFrame(animar);

        return () => cancelAnimationFrame(frame);
    }, [numeroFinal, duracao]);

    return <>{numero.toLocaleString('pt-PT')}</>;
}