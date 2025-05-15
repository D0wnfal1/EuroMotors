import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus } from '../models/order';

@Pipe({
  name: 'status',
  standalone: true,
})
export class StatusPipe implements PipeTransform {
  transform(value: OrderStatus | string | undefined): string {
    if (value === undefined || value === null) {
      return 'Невідомо';
    }

    switch (value) {
      case OrderStatus.Pending:
        return 'В обробці';
      case OrderStatus.Paid:
        return 'Оплачено';
      case OrderStatus.Shipped:
        return 'Відправлено';
      case OrderStatus.Completed:
        return 'Завершено';
      case OrderStatus.Canceled:
        return 'Скасовано';
      case OrderStatus.Refunded:
        return 'Повернено';
      default:
        return String(value);
    }
  }
}
