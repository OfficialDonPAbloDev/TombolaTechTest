import { useCallback, useMemo, useState } from 'react';
import type { Bean } from '../models/Bean';
import type { BasketItem } from '../models/BasketItem';

export interface UseBasketResult {
  items: BasketItem[];
  total: number;
  itemCount: number;
  currency: string;
  addItem: (bean: Bean, quantity: number) => void;
  updateQuantity: (beanId: string, quantity: number) => void;
  removeItem: (beanId: string) => void;
}

export function useBasket(): UseBasketResult {
  const [items, setItems] = useState<BasketItem[]>([]);

  const addItem = useCallback((bean: Bean, quantity: number) => {
    if (quantity < 1) return;
    setItems((prev) => {
      const existing = prev.find((i) => i.bean.id === bean.id);
      if (existing) {
        return prev.map((i) =>
          i.bean.id === bean.id
            ? { ...i, quantity: i.quantity + quantity }
            : i,
        );
      }
      return [...prev, { bean, quantity }];
    });
  }, []);

  const updateQuantity = useCallback((beanId: string, quantity: number) => {
    setItems((prev) => {
      if (quantity < 1) {
        return prev.filter((i) => i.bean.id !== beanId);
      }
      return prev.map((i) =>
        i.bean.id === beanId ? { ...i, quantity } : i,
      );
    });
  }, []);

  const removeItem = useCallback((beanId: string) => {
    setItems((prev) => prev.filter((i) => i.bean.id !== beanId));
  }, []);

  const { total, itemCount, currency } = useMemo(() => {
    let total = 0;
    let itemCount = 0;
    let currency = 'GBP';
    for (const item of items) {
      total += item.bean.cost * item.quantity;
      itemCount += item.quantity;
      currency = item.bean.currency;
    }
    return { total, itemCount, currency };
  }, [items]);

  return {
    items,
    total,
    itemCount,
    currency,
    addItem,
    updateQuantity,
    removeItem,
  };
}
