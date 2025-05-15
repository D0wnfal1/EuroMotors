import { Pipe, PipeTransform } from '@angular/core';
import { DeliveryMethod } from '../models/order';

@Pipe({
  name: 'delivery',
})
export class DeliveryPipe implements PipeTransform {
  transform(value: DeliveryMethod | number): string {
    switch (value) {
      case DeliveryMethod.Pickup:
        return 'Самовивіз з Магазину';
      case DeliveryMethod.Delivery:
        return 'Доставка Новою Поштою';
      default:
        return 'Невідомо';
    }
  }
}
