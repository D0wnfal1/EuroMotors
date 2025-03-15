export type Order = {
  id: string;
  userId?: string;
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

export enum DeliveryMethod {
  Pickup = 1,
  Delivery = 2,
}

export enum PaymentMethod {
  Prepaid = 1,
  Postpaid = 2,
}
