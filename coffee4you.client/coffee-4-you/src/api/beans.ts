import type { Bean, BeanColour } from '../models/Bean';

interface RawBean {
  _id: string;
  index: number;
  isBOTD: boolean;
  Cost: string;
  Image: string;
  colour: string;
  Name: string;
  Description: string;
  Country: string;
}

export interface BeansPage {
  items: Bean[];
  total: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

const SYMBOL_TO_CURRENCY: Record<string, string> = {
  '£': 'GBP',
  $: 'USD',
  '€': 'EUR',
};

function parseCost(raw: string): { cost: number; currency: string } {
  const match = raw.match(/^\s*([£$€])\s*([\d.]+)\s*$/);
  if (!match) {
    return { cost: 0, currency: 'GBP' };
  }
  return {
    cost: Number.parseFloat(match[2]),
    currency: SYMBOL_TO_CURRENCY[match[1]] ?? 'GBP',
  };
}

function normalise(raw: RawBean): Bean {
  const { cost, currency } = parseCost(raw.Cost);
  return {
    id: raw._id,
    index: raw.index,
    isBeanOfTheDay: raw.isBOTD,
    cost,
    currency,
    imageUrl: raw.Image,
    colour: raw.colour as BeanColour,
    name: raw.Name,
    description: raw.Description,
    country: raw.Country,
  };
}

let cache: Promise<Bean[]> | null = null;

function loadAll(): Promise<Bean[]> {
  if (!cache) {
    cache = fetch('/AllTheBeans.json')
      .then((res) => {
        if (!res.ok) {
          throw new Error(`Failed to load beans (${res.status})`);
        }
        return res.json() as Promise<RawBean[]>;
      })
      .then((raw) => raw.map(normalise))
      .catch((err) => {
        cache = null;
        throw err;
      });
  }
  return cache;
}

export async function fetchBeans(
  page: number,
  pageSize: number,
): Promise<BeansPage> {
  await new Promise((r) => setTimeout(r, 400));

  const all = await loadAll();
  const start = (page - 1) * pageSize;
  const end = start + pageSize;
  return {
    items: all.slice(start, end),
    total: all.length,
    page,
    pageSize,
    hasMore: end < all.length,
  };
}
