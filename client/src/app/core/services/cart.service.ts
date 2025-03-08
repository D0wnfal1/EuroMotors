import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart } from '../../shared/models/cart';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  itemCount = computed(() => {
    return this.cart()?.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  });
  totals = computed(() => {
    const cart = this.cart();
    if (!cart) return null;
    const subtotal = cart.cartItems.reduce(
      (sum, item) => sum + item.unitPrice * item.quantity,
      0
    );
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount,
    };
  });

  getCart(id: string) {
    return this.http.get<Cart>(`${this.baseUrl}carts/${id}`).pipe(
      map((cart) => {
        this.cart.set(cart);
        return cart;
      })
    );
  }

  addItemToCart(productId: string, quantity: number | undefined = undefined) {
    const cart = this.cart() ?? this.createCart();
    const request = {
      cartId: cart.id,
      productId,
      quantity: quantity !== undefined ? quantity : 1,
    };
    this.http.post<Cart>(`${this.baseUrl}carts`, request).subscribe({
      next: () => this.getCart(cart.id).subscribe(),
    });
  }

  decrementItemQuantity(productId: string, quantity: number) {
    const cart = this.cart() ?? this.createCart();
    const request = {
      cartId: cart.id,
      productId,
      quantity: -quantity,
    };
    this.http
      .post<Cart>(`${this.baseUrl}carts/update-quantity`, request)
      .subscribe({
        next: () => this.getCart(cart.id).subscribe(),
      });
  }

  removeItemFromCart(productId: string) {
    const cart = this.cart() ?? this.createCart();
    return this.http
      .delete(
        `${this.baseUrl}carts/item?cartId=${cart.id}&productId=${productId}`
      )
      .subscribe({
        next: () => this.getCart(cart.id).subscribe(),
      });
  }

  clearCart(cartId: string) {
    return this.http
      .delete(`${this.baseUrl}carts/clear?cartId=${cartId}`)
      .subscribe({
        next: () => {
          localStorage.removeItem('cart_id');
          this.cart.set(null);
        },
      });
  }

  createCart(): Cart {
    const cart = new Cart();
    this.cart.set(cart);
    localStorage.setItem('cart_id', cart.id);
    return cart;
  }
}
