import { Product } from './product';

export type CarModel = {
  id: string;
  brand: string;
  model: string;
  imageUrl?: string;
  products: Product[];
};
