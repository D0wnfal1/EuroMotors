import { CarModel } from './carModel';

export interface CarBrand {
  id: string;
  name: string;
  slug: string;
  logoPath?: string;
  models?: CarModel[];
}

export interface CarBrandFormData {
  name: string;
  logo?: File;
}
