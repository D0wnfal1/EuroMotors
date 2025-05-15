import { Pipe, PipeTransform } from '@angular/core';
import { FuelType } from '../models/carModel';

@Pipe({
  name: 'fuelType',
  standalone: true,
})
export class FuelTypePipe implements PipeTransform {
  transform(value: FuelType | string | undefined): string {
    if (value === undefined || value === null) {
      return 'Невідомо';
    }

    switch (value) {
      case FuelType.Petrol:
        return 'Бензин';
      case FuelType.Gas:
        return 'Газ';
      case FuelType.Diesel:
        return 'Дизель';
      case FuelType.Electric:
        return 'Електро';
      case FuelType.Hybrid:
        return 'Гібрид';
      default:
        return String(value);
    }
  }
}
