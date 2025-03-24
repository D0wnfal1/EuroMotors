import { Product } from './product';

export type ProductImage = {
  id: string;
  path: string;
  productId: string;
  product?: Product;
};
