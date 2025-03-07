import { Product } from './product';

export type Category = {
  id: string;
  name: string;
  isArchived: boolean;
  imageUrl?: string;
  products: Product[];
};
