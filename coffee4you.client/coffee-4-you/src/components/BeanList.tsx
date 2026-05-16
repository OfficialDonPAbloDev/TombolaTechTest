import { useCallback, useEffect, useState } from 'react';
import type { Bean } from '../models/Bean';
import { fetchBeans } from '../api/beans';
import BeanItem from './BeanItem';
import './BeanList.css';

const PAGE_SIZE = 6;

interface BeanListProps {
  onBeanSelect: (bean: Bean) => void;
}

function BeanList({ onBeanSelect }: BeanListProps) {
  const [items, setItems] = useState<Bean[]>([]);
  const [page, setPage] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadPage = useCallback(async (pageToLoad: number) => {
    setLoading(true);
    setError(null);
    try {
      const result = await fetchBeans(pageToLoad, PAGE_SIZE);
      setItems((prev) =>
        pageToLoad === 1 ? result.items : [...prev, ...result.items],
      );
      setPage(result.page);
      setHasMore(result.hasMore);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load beans');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadPage(1);
  }, [loadPage]);

  return (
    <div className="bean-list">
      <div className="bean-list__grid">
        {items.map((bean) => (
          <BeanItem key={bean.id} bean={bean} onSelect={onBeanSelect} />
        ))}
      </div>

      {loading && (
        <div className="bean-list__status" role="status" aria-live="polite">
          <span className="bean-list__spinner" aria-hidden="true" />
          <span>Loading beans…</span>
        </div>
      )}

      {error && !loading && (
        <div className="bean-list__error" role="alert">
          <p>{error}</p>
          <button
            type="button"
            className="bean-list__button"
            onClick={() => loadPage(page + 1)}
          >
            Retry
          </button>
        </div>
      )}

      {!loading && !error && hasMore && items.length > 0 && (
        <button
          type="button"
          className="bean-list__button"
          onClick={() => loadPage(page + 1)}
        >
          Load more
        </button>
      )}
    </div>
  );
}

export default BeanList;
