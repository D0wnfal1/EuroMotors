export type Order = {
  id: string;
  userId?: string;
  buyerName: string;
  buyerPhoneNumber: string;
  buyerEmail: string;
  status: OrderStatus;
  totalPrice: number;
  orderItems: OrderItem[];
  paymentId: string;
  deliveryMethod: DeliveryMethod;
  shippingAddress: string;
  paymentMethod: PaymentMethod;
  createdAtUtc: Date;
  updatedAtUtc: Date;
};

export type OrderItem = {
  orderId: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  price: number;
};

export enum OrderStatus {
  Pending = 1,
  Paid = 2,
  Shipped = 3,
  Completed = 4,
  Canceled = 5,
  Refunded = 6,
}

export enum DeliveryMethod {
  Pickup = 1,
  Delivery = 2,
}

export enum PaymentMethod {
  Prepaid = 1,
  Postpaid = 2,
}
