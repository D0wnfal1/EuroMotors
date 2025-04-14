import { Product } from './product';

export interface Category {
  id: string;
  name: string;
  isAvailable: boolean;
  imagePath?: string;
  parentCategoryId?: string;
  slug: string;
  products: Product[];
  subcategoryNames?: string[];
}
