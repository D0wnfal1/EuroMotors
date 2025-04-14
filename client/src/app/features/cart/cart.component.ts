import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CartItemComponent } from './cart-item/cart-item.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { CommonModule } from '@angular/common';
import { CheckoutFormComponent } from '../checkout/checkout-form/checkout-form.component';

@Component({
  selector: 'app-cart',
  imports: [
    CartItemComponent,
    EmptyStateComponent,
    CommonModule,
    CheckoutFormComponent,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent {
  cartService = inject(CartService);
}
