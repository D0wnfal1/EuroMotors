import { Product } from './product';

export type CarModel = {
  id: string;
  brand: string;
  model: string;
  imagePath?: string;
  products: Product[];
};
