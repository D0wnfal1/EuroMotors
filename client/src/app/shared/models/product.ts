import { ProductImage } from './productImage';

export type Product = {
  id: string;
  categoryId: string;
  carModelId: string;
  name: string;
  description: string;
  vendorCode: string;
  price: number;
  discount: number;
  stock: number;
  isAvailable: boolean;
  images: ProductImage[];
};
