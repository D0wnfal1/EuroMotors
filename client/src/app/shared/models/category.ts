import { Product } from './product';

export interface Category {
  id: string;
  name: string;
  isArchived: boolean;
  imagePath?: string;
  parentCategoryId?: string;
  slug: string;
  products: Product[];
  subcategoryNames?: string[];
}
