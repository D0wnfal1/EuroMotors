import { Pipe, PipeTransform } from '@angular/core';
import { BodyType } from '../models/carModel';

@Pipe({
  name: 'bodyType',
  standalone: true,
})
export class BodyTypePipe implements PipeTransform {
  transform(value: BodyType | string | undefined): string {
    if (value === undefined || value === null) {
      return 'Невідомо';
    }

    switch (value) {
      case BodyType.Sedan:
        return 'Седан';
      case BodyType.Hatchback:
        return 'Хетчбек';
      case BodyType.SUV:
        return 'Позашляховик';
      case BodyType.Coupe:
        return 'Купе';
      case BodyType.Wagon:
        return 'Універсал';
      case BodyType.Convertible:
        return 'Кабріолет';
      case BodyType.Van:
        return 'Мікроавтобус';
      case BodyType.Pickup:
        return 'Пікап';
      default:
        return String(value);
    }
  }
}
