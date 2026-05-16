import type { ChangeEvent } from 'react';
import type { BeanFilters } from '../../models/BeanFilters';
import './BeanFilter.css';

interface BeanFilterProps {
  filters: BeanFilters;
  onFiltersChange: (filters: BeanFilters) => void;
  countries: string[];
}

function BeanFilter({
  filters,
  onFiltersChange,
  countries,
}: BeanFilterProps) {
  const handleCountry = (e: ChangeEvent<HTMLSelectElement>) => {
    onFiltersChange({ ...filters, country: e.target.value });
  };
  const handleName = (e: ChangeEvent<HTMLInputElement>) => {
    onFiltersChange({ ...filters, name: e.target.value });
  };
  const handleClear = () => {
    onFiltersChange({ country: '', name: '' });
  };
  const hasFilters = filters.country !== '' || filters.name !== '';

  return (
    <div className="bean-filter" role="search">
      <label className="bean-filter__field">
        <span className="bean-filter__label">Country</span>
        <select
          className="bean-filter__select"
          value={filters.country}
          onChange={handleCountry}
        >
          <option value="">All countries</option>
          {countries.map((c) => (
            <option key={c} value={c}>
              {c}
            </option>
          ))}
        </select>
      </label>
      <label className="bean-filter__field bean-filter__field--grow">
        <span className="bean-filter__label">Name</span>
        <input
          className="bean-filter__input"
          type="text"
          value={filters.name}
          onChange={handleName}
          placeholder="Search by name (min 2 chars)"
        />
      </label>
      <button
        type="button"
        className="bean-filter__clear"
        onClick={handleClear}
        disabled={!hasFilters}
      >
        Clear
      </button>
    </div>
  );
}

export default BeanFilter;
