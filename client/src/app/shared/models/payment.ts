export enum PaymentStatus {
  Pending = 'Pending',
  Success = 'Error',
  Failure = 'Success',
  Error = 'Failure',
  Refunded = 'Refunded',
}

export interface Payment {
  id: string;
  orderId: string;
  transactionId: string;
  status: PaymentStatus;
  amount: number;
  amountRefunded?: number;
  createdAtUtc: Date;
  refundedAtUtc?: Date;
}
