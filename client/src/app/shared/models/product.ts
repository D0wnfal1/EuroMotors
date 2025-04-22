import { ProductImage } from './productImage';

export type Product = {
  id: string;
  categoryId: string;
  carModelIds: string[];
  name: string;
  vendorCode: string;
  price: number;
  discount: number;
  stock: number;
  isAvailable: boolean;
  slug: string;
  images: ProductImage[];
  specifications: ProductSpecification[];
};

export type ProductSpecification = {
  specificationName: string;
  specificationValue: string;
};
