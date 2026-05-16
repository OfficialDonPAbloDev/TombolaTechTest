import { useCallback, useEffect, useState } from 'react';
import type { Bean } from '../../models/Bean';
import type { BeanFilters } from '../../models/BeanFilters';
import { fetchBeans, fetchCountries } from '../../api/beans';
import { useDebouncedValue } from '../../hooks/useDebouncedValue';
import BeanItem from '../BeanItem/BeanItem';
import BeanFilter from '../BeanFilter/BeanFilter';
import './BeanList.css';

const PAGE_SIZE = 6;
const EMPTY_FILTERS: BeanFilters = { country: '', name: '' };

interface BeanListProps {
  onBeanSelect: (bean: Bean) => void;
}

function BeanList({ onBeanSelect }: BeanListProps) {
  const [filters, setFilters] = useState<BeanFilters>(EMPTY_FILTERS);
  const debouncedFilters = useDebouncedValue(filters, 300);

  const [items, setItems] = useState<Bean[]>([]);
  const [page, setPage] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [countries, setCountries] = useState<string[]>([]);

  useEffect(() => {
    fetchCountries()
      .then(setCountries)
      .catch(() => {
        // non-blocking: filter still works without country options
      });
  }, []);

  const loadPage = useCallback(
    async (pageToLoad: number, activeFilters: BeanFilters) => {
      setLoading(true);
      setError(null);
      try {
        const result = await fetchBeans(pageToLoad, PAGE_SIZE, activeFilters);
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
    },
    [],
  );

  useEffect(() => {
    setItems([]);
    setPage(0);
    setHasMore(true);
    loadPage(1, debouncedFilters);
  }, [debouncedFilters, loadPage]);

  return (
    <div className="bean-list">
      <BeanFilter
        filters={filters}
        onFiltersChange={setFilters}
        countries={countries}
      />

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

      {!loading && !error && items.length === 0 && (
        <div className="bean-list__empty">No beans match your filter.</div>
      )}

      {error && !loading && (
        <div className="bean-list__error" role="alert">
          <p>{error}</p>
          <button
            type="button"
            className="bean-list__button"
            onClick={() => loadPage(page + 1, debouncedFilters)}
          >
            Retry
          </button>
        </div>
      )}

      {!loading && !error && hasMore && items.length > 0 && (
        <button
          type="button"
          className="bean-list__button"
          onClick={() => loadPage(page + 1, debouncedFilters)}
        >
          Load more
        </button>
      )}
    </div>
  );
}

export default BeanList;
