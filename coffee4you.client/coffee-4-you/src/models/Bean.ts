export type BeanColour =
  | 'dark roast'
  | 'medium roast'
  | 'light roast'
  | 'golden'
  | 'green';

export interface Bean {
  id: string;
  index: number;
  isBeanOfTheDay: boolean;
  cost: number;
  currency: string;
  imageUrl: string;
  colour: BeanColour;
  name: string;
  description: string;
  country: string;
}
