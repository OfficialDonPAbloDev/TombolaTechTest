import type { BasketItem } from '../../models/BasketItem';
import './ShoppingBasket.css';

interface ShoppingBasketProps {
  items: BasketItem[];
  total: number;
  itemCount: number;
  currency: string;
  onUpdateQuantity: (beanId: string, quantity: number) => void;
  onRemoveItem: (beanId: string) => void;
  onCheckout: () => void;
}

function ShoppingBasket({
  items,
  total,
  itemCount,
  currency,
  onUpdateQuantity,
  onRemoveItem,
  onCheckout,
}: ShoppingBasketProps) {
  const formatPrice = (n: number) =>
    new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency,
    }).format(n);

  return (
    <section className="shopping-basket" aria-label="Shopping basket">
      <h2 className="shopping-basket__title">Your basket</h2>

      {items.length === 0 ? (
        <p className="shopping-basket__empty">Your basket is empty.</p>
      ) : (
        <>
          <ul className="shopping-basket__list">
            {items.map((item) => {
              const lineTotal = item.bean.cost * item.quantity;
              return (
                <li className="shopping-basket__row" key={item.bean.id}>
                  <img
                    className="shopping-basket__image"
                    src={item.bean.imageUrl}
                    alt={item.bean.name}
                    loading="lazy"
                  />
                  <div className="shopping-basket__info">
                    <span className="shopping-basket__name">
                      {item.bean.name}
                    </span>
                    <span className="shopping-basket__unit">
                      {formatPrice(item.bean.cost)} each
                    </span>
                  </div>
                  <div
                    className="shopping-basket__qty"
                    role="group"
                    aria-label={`Quantity for ${item.bean.name}`}
                  >
                    <button
                      type="button"
                      aria-label="Decrease quantity"
                      onClick={() =>
                        onUpdateQuantity(item.bean.id, item.quantity - 1)
                      }
                      disabled={item.quantity <= 1}
                    >
                      −
                    </button>
                    <span className="shopping-basket__qty-value">
                      {item.quantity}
                    </span>
                    <button
                      type="button"
                      aria-label="Increase quantity"
                      onClick={() =>
                        onUpdateQuantity(item.bean.id, item.quantity + 1)
                      }
                    >
                      +
                    </button>
                  </div>
                  <span className="shopping-basket__line-total">
                    {formatPrice(lineTotal)}
                  </span>
                  <button
                    type="button"
                    className="shopping-basket__remove"
                    aria-label={`Remove ${item.bean.name}`}
                    onClick={() => onRemoveItem(item.bean.id)}
                  >
                    ×
                  </button>
                </li>
              );
            })}
          </ul>

          <div className="shopping-basket__footer">
            <div className="shopping-basket__total">
              <span>
                Total ({itemCount} item{itemCount === 1 ? '' : 's'})
              </span>
              <span className="shopping-basket__total-value">
                {formatPrice(total)}
              </span>
            </div>
            <button
              type="button"
              className="shopping-basket__checkout"
              onClick={onCheckout}
            >
              Continue to checkout
            </button>
          </div>
        </>
      )}
    </section>
  );
}

export default ShoppingBasket;
