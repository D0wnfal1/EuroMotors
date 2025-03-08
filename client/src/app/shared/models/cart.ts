import { v4 as uuidv4 } from 'uuid';

export type CartType = {
  id: string;
  cartItems: CartItem[];
  totalPrice: number;
};

export type CartItem = {
  productId: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
};

export class Cart implements CartType {
  id = uuidv4();
  cartItems: CartItem[] = [];
  totalPrice: number = 0;
}
