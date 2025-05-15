import { Pipe, PipeTransform } from '@angular/core';
import { FuelType } from '../models/carModel';

@Pipe({
  name: 'engineSpec',
  standalone: true,
})
export class EngineSpecPipe implements PipeTransform {
  transform(value: string | undefined): string {
    if (!value) return '';

    const parts = value.split('L ');
    if (parts.length !== 2) return value;

    const volume = parts[0];
    const fuelType = parts[1];

    const translatedFuelType = this.translateFuelType(fuelType);

    return `${volume}L ${translatedFuelType}`;
  }

  private translateFuelType(value: string): string {
    if (!value) return 'Невідомо';

    const normalizedValue = value.trim();

    switch (normalizedValue) {
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
        if (normalizedValue.toLowerCase().includes('petrol')) return 'Бензин';
        if (normalizedValue.toLowerCase().includes('gas')) return 'Газ';
        if (normalizedValue.toLowerCase().includes('diesel')) return 'Дизель';
        if (normalizedValue.toLowerCase().includes('electric'))
          return 'Електро';
        if (normalizedValue.toLowerCase().includes('hybrid')) return 'Гібрид';
        return value;
    }
  }
}
