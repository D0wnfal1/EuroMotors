import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CartItemComponent } from './cart-item/cart-item.component';
import { CommonModule } from '@angular/common';
import { CheckoutFormComponent } from '../checkout/checkout-form/checkout-form.component';
import { EmptyCartStateComponent } from '../../shared/components/empty-cart-state/empty-cart-state.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    CartItemComponent,
    CommonModule,
    CheckoutFormComponent,
    EmptyCartStateComponent,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit, OnDestroy {
  cartService = inject(CartService);
  private readonly cartId = localStorage.getItem('cart_id');
  private subscription: Subscription | null = null;

  ngOnInit() {
    if (this.cartId) {
      this.refreshCart();
    }
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  private refreshCart() {
    if (this.cartId) {
      this.subscription = this.cartService.getCart(this.cartId).subscribe();
    }
  }
}
