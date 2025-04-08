import { Product } from './product';

export type CarModel = {
  id: string;
  brand: string;
  model: string;
  startYear: number;
  endYear?: number;
  bodyType: BodyType;
  volumeLiters: number;
  fuelType: FuelType;
  horsePower: number;
  slug: string;
  imagePath?: string;
  products: Product[];
};

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
