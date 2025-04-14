import { Pipe, PipeTransform } from '@angular/core';
import { DeliveryMethod } from '../models/order';

@Pipe({
  name: 'delivery',
})
export class DeliveryPipe implements PipeTransform {
  transform(value: DeliveryMethod | number): string {
    switch (value) {
      case DeliveryMethod.Pickup:
        return 'Pickup From Store';
      case DeliveryMethod.Delivery:
        return 'Delivery By NovaPoshta';
      default:
        return 'Unknown';
    }
  }
}
