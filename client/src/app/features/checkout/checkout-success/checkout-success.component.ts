import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/models/order';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [
    RouterLink,
    MatProgressSpinnerModule,
    MatButtonModule,
    NgIf,
    CurrencyPipe,
    DatePipe,
    NgFor,
  ],
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss'],
})
export class CheckoutSuccessComponent implements OnInit {
  order: Order | null = null;
  isLoading = true;
  error = '';
  orderService = inject(OrderService);
  route = inject(ActivatedRoute);

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('orderId');
    if (orderId) {
      this.orderService.getOrderById(orderId).subscribe({
        next: (orderData: Order) => {
          this.order = orderData;
          this.isLoading = false;
        },
      });
    } else {
      this.error = 'Order ID not passed.';
      this.isLoading = false;
    }
  }
}
