import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CartItemComponent } from './cart-item/cart-item.component';
import { CommonModule } from '@angular/common';
import { CheckoutFormComponent } from '../checkout/checkout-form/checkout-form.component';
import { EmptyCartStateComponent } from '../../shared/components/empty-cart-state/empty-cart-state.component';

@Component({
  selector: 'app-cart',
  imports: [
    CartItemComponent,
    CommonModule,
    CheckoutFormComponent,
    EmptyCartStateComponent,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent {
  cartService = inject(CartService);
}
