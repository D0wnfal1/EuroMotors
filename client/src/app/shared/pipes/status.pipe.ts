import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus } from '../models/order';

@Pipe({
  name: 'status',
  standalone: true,
})
export class StatusPipe implements PipeTransform {
  transform(value: OrderStatus | undefined): string {
    if (value === undefined || value === null) {
      return 'Unknown';
    }
    return OrderStatus[value] ?? 'Unknown';
  }
}
