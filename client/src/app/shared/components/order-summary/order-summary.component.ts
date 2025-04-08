import { CurrencyPipe, Location } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-order-summary',
  imports: [RouterLink, CurrencyPipe, MatButton],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss',
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
  location = inject(Location);
}
