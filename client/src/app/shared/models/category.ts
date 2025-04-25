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

export interface HierarchicalCategory {
  id: string;
  name: string;
  isAvailable: boolean;
  imagePath?: string;
  slug: string;
  subCategories: HierarchicalCategory[];
}

export interface CategoryNode {
  id: string;
  name: string;
  isAvailable: boolean;
  imagePath?: string;
  parentCategoryId?: string;
  level: number;
  expandable: boolean;
  isLoading?: boolean;
  children?: CategoryNode[];
}

export interface FlatCategoryNode {
  id: string;
  name: string;
  isAvailable: boolean;
  imagePath?: string;
  parentCategoryId?: string;
  level: number;
  expandable: boolean;
  isLoading?: boolean;
}
