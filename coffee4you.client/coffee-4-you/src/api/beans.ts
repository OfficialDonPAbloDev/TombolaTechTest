import type { Bean } from '../models/Bean';
import type { BeanFilters } from '../models/BeanFilters';

export interface BeansPage {
  items: Bean[];
  total: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

export async function fetchBeans(
  page: number,
  pageSize: number,
  filters: BeanFilters = { country: '', name: '' },
): Promise<BeansPage> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });
  if (filters.country) params.set('country', filters.country);
  if (filters.name) params.set('beanName', filters.name);

  const res = await fetch(`/api/beans?${params}`);
  if (!res.ok) {
    throw new Error(`Failed to load beans (${res.status})`);
  }
  return (await res.json()) as BeansPage;
}

export async function fetchCountries(): Promise<string[]> {
  const res = await fetch('/api/countries');
  if (!res.ok) {
    throw new Error(`Failed to load countries (${res.status})`);
  }
  return (await res.json()) as string[];
}
