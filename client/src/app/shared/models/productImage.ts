import { Product } from './product';

export type ProductImage = {
  productImageId: string;
  path: string;
  productId: string;
  product?: Product;
};
