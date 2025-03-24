import { Pipe, PipeTransform } from '@angular/core';
import { PaymentMethod } from '../models/order';

@Pipe({
  name: 'payment',
})
export class PaymentPipe implements PipeTransform {
  transform(value: PaymentMethod | number): string {
    switch (value) {
      case PaymentMethod.Prepaid:
        return 'Prepaid';
      case PaymentMethod.Postpaid:
        return 'Postpaid';
      default:
        return 'Unknown';
    }
  }
}
