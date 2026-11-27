import { useCallback, useEffect, useId, useLayoutEffect, useRef, useState } from 'react'
import { createPortal } from 'react-dom'

export default function Tooltip({
    text,
    children,
    position = 'top', // 'top' | 'bottom'
    className = '',
}) {
    const id = useId()
    const triggerRef = useRef(null)
    const bubbleRef = useRef(null)
    const [open, setOpen] = useState(false)
    const [coords, setCoords] = useState({ top: -9999, left: -9999 })
    const [positioned, setPositioned] = useState(false)

    const updatePosition = useCallback(() => {
        const trigger = triggerRef.current
        const bubble = bubbleRef.current
        if (!trigger || !bubble) return

        const margin = 8
        const triggerRect = trigger.getBoundingClientRect()
        const bubbleWidth = bubble.offsetWidth
        const bubbleHeight = bubble.offsetHeight

        if (!bubbleWidth || !bubbleHeight) return

        let left = Math.round(triggerRect.left + triggerRect.width / 2 - bubbleWidth / 2)
        left = Math.max(
            margin,
            Math.min(left, window.innerWidth - bubbleWidth - margin)
        )

        let top
        if (position === 'bottom') {
            top = Math.round(triggerRect.bottom + 6)
            if (top + bubbleHeight > window.innerHeight - margin) {
                top = Math.round(triggerRect.top - bubbleHeight - 6)
            }
        } else {
            top = Math.round(triggerRect.top - bubbleHeight - 6)
            if (top < margin) {
                top = Math.round(triggerRect.bottom + 6)
            }
        }

        top = Math.max(
            margin,
            Math.min(top, window.innerHeight - bubbleHeight - margin)
        )

        setCoords((prev) => {
            if (prev.top === top && prev.left === left) return prev
            return { top, left }
        })
        setPositioned(true)
    }, [position])

    useLayoutEffect(() => {
        if (!open) return
        updatePosition()
    }, [open, text, updatePosition])

    useEffect(() => {
        if (!open) return
        const handleSync = () => updatePosition()
        window.addEventListener('scroll', handleSync, true)
        window.addEventListener('resize', handleSync)
        return () => {
            window.removeEventListener('scroll', handleSync, true)
            window.removeEventListener('resize', handleSync)
        }
    }, [open, updatePosition])

    const handleOpen = () => {
        setOpen(true)
    }

    const handleClose = () => {
        setOpen(false)
        setPositioned(false)
        setCoords({ top: -9999, left: -9999 })
    }

    if (!text) return children

    return (
        <>
            <span
                ref={triggerRef}
                className={`tooltip-wrapper ${className}`}
                aria-describedby={open ? id : undefined}
                onMouseEnter={handleOpen}
                onMouseLeave={handleClose}
                onFocus={handleOpen}
                onBlur={handleClose}
            >
                {children}
            </span>

            {open &&
                createPortal(
                    <span
                        ref={bubbleRef}
                        id={id}
                        role="tooltip"
                        className="tooltip-bubble tooltip-bubble--portal"
                        style={{
                            top: `${coords.top}px`,
                            left: `${coords.left}px`,
                            opacity: positioned ? 1 : 0,
                        }}
                    >
                        {text}
                    </span>,
                    document.body
                )}
        </>
    )
}