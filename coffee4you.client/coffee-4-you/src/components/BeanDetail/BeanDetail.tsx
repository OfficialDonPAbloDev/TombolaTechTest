import { useEffect, useRef, useState } from 'react';
import type { MouseEvent as ReactMouseEvent } from 'react';
import type { Bean } from '../../models/Bean';
import './BeanDetail.css';

const COUNTRY_TO_ISO: Record<string, string> = {
  Peru: 'pe',
  Vietnam: 'vn',
  Colombia: 'co',
  Brazil: 'br',
  Honduras: 'hn',
};

interface BeanDetailProps {
  bean: Bean | null;
  onClose: () => void;
  onAddToCart: (bean: Bean, quantity: number) => void;
}

function BeanDetail({ bean, onClose, onAddToCart }: BeanDetailProps) {
  const dialogRef = useRef<HTMLDialogElement>(null);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (bean && !dialog.open) {
      dialog.showModal();
      setQuantity(1);
    } else if (!bean && dialog.open) {
      dialog.close();
    }
  }, [bean]);

  const handleBackdropClick = (e: ReactMouseEvent<HTMLDialogElement>) => {
    if (e.target === dialogRef.current) {
      onClose();
    }
  };

  if (!bean) {
    return (
      <dialog ref={dialogRef} className="bean-detail" onClose={onClose} />
    );
  }

  const iso = COUNTRY_TO_ISO[bean.country];
  const price = new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: bean.currency,
  }).format(bean.cost);

  const handleAdd = () => {
    onAddToCart(bean, quantity);
    onClose();
  };

  return (
    <dialog
      ref={dialogRef}
      className="bean-detail"
      onClose={onClose}
      onClick={handleBackdropClick}
    >
      <div className="bean-detail__content">
        <button
          type="button"
          className="bean-detail__close"
          aria-label="Close"
          onClick={onClose}
        >
          ×
        </button>
        {bean.isBeanOfTheDay && (
          <span className="bean-detail__botd" title="Bean of the Day">
            ★ Bean of the Day
          </span>
        )}
        <img
          className="bean-detail__image"
          src={bean.imageUrl}
          alt={bean.name}
        />
        <div className="bean-detail__body">
          <h2 className="bean-detail__name">{bean.name}</h2>
          <div className="bean-detail__meta">
            {iso && (
              <img
                className="bean-detail__flag"
                src={`https://flagcdn.com/w40/${iso}.png`}
                srcSet={`https://flagcdn.com/w80/${iso}.png 2x`}
                alt=""
                width={20}
                height={15}
              />
            )}
            <span>{bean.country}</span>
            <span className="bean-detail__dot" aria-hidden="true">•</span>
            <span className="bean-detail__colour">{bean.colour}</span>
          </div>
          <p className="bean-detail__description">{bean.description}</p>
          <div className="bean-detail__price">{price}</div>
          <div className="bean-detail__actions">
            <div
              className="bean-detail__qty"
              role="group"
              aria-label="Quantity"
            >
              <button
                type="button"
                aria-label="Decrease quantity"
                onClick={() => setQuantity((q) => Math.max(1, q - 1))}
                disabled={quantity <= 1}
              >
                −
              </button>
              <span
                className="bean-detail__qty-value"
                aria-live="polite"
              >
                {quantity}
              </span>
              <button
                type="button"
                aria-label="Increase quantity"
                onClick={() => setQuantity((q) => q + 1)}
              >
                +
              </button>
            </div>
            <button
              type="button"
              className="bean-detail__add"
              onClick={handleAdd}
            >
              Add to cart
            </button>
          </div>
        </div>
      </div>
    </dialog>
  );
}

export default BeanDetail;
