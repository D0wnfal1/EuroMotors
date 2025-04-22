import { Product } from './product';

export interface CarModel {
  id: string;
  carBrandId: string;
  brandName: string;
  modelName: string;
  startYear: number;
  bodyType: BodyType;
  volumeLiters: number;
  fuelType: FuelType;
  slug: string;
  products: Product[];
  carBrand?: {
    id: string;
    name: string;
    slug: string;
  };
}

export enum FuelType {
  Petrol = 'Petrol',
  Diesel = 'Diesel',
  Electric = 'Electric',
  Hybrid = 'Hybrid',
}

export enum BodyType {
  Sedan = 'Sedan',
  Hatchback = 'Hatchback',
  SUV = 'SUV',
  Coupe = 'Coupe',
  Wagon = 'Wagon',
  Convertible = 'Convertible',
  Van = 'Van',
  Pickup = 'Pickup',
}

export interface CarSelectionFilter {
  brand?: string;
  model?: string;
  startYear?: number;
  bodyType?: BodyType;
}

export interface SelectedCar {
  brand: string;
  model: string;
  startYear?: number;
  bodyType?: BodyType;
  engineSpec?: string;
}
