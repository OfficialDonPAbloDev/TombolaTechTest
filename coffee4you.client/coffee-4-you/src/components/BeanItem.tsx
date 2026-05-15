import type { Bean } from '../models/Bean';
import './BeanItem.css';

interface BeanItemProps {
  bean: Bean;
  onSelect: (bean: Bean) => void;
}

const COUNTRY_TO_ISO: Record<string, string> = {
  Peru: 'pe',
  Vietnam: 'vn',
  Colombia: 'co',
  Brazil: 'br',
  Honduras: 'hn',
};

function BeanItem({ bean, onSelect }: BeanItemProps) {
  const iso = COUNTRY_TO_ISO[bean.country];
  const price = new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: bean.currency,
  }).format(bean.cost);

  return (
    <button
      type="button"
      className="bean-item"
      onClick={() => onSelect(bean)}
    >
      {bean.isBeanOfTheDay && (
        <span
          className="bean-item__botd"
          title="Bean of the Day"
          aria-label="Bean of the Day"
        >
          ★
        </span>
      )}
      <img
        className="bean-item__image"
        src={bean.imageUrl}
        alt={bean.name}
        loading="lazy"
      />
      <div className="bean-item__body">
        <h3 className="bean-item__name">{bean.name}</h3>
        <div className="bean-item__meta">
          {iso && (
            <img
              className="bean-item__flag"
              src={`https://flagcdn.com/w40/${iso}.png`}
              srcSet={`https://flagcdn.com/w80/${iso}.png 2x`}
              alt=""
              width={20}
              height={15}
            />
          )}
          <span>{bean.country}</span>
          <span className="bean-item__dot" aria-hidden="true">•</span>
          <span className="bean-item__colour">{bean.colour}</span>
        </div>
        <p className="bean-item__description">{bean.description}</p>
        <div className="bean-item__price">{price}</div>
      </div>
    </button>
  );
}

export default BeanItem;
