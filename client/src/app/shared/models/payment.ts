export enum PaymentStatus {
  Pending = 'Pending',
  Success = 'Success',
  Failure = 'Failure',
  Error = 'Error',
  Refunded = 'Refunded',
}

export interface Payment {
  id: string; // GUID платежа
  orderId: string; // GUID заказа
  transactionId: string; // Идентификатор транзакции
  status: PaymentStatus; // Статус платежа
  amount: number; // Сумма платежа
  amountRefunded?: number; // Сумма, возвращённая (если есть)
  createdAtUtc: Date; // Дата создания платежа в UTC
  refundedAtUtc?: Date; // Дата возврата (если применимо)
  // При необходимости можно добавить вложенный объект Order:
  // order?: Order;
}
