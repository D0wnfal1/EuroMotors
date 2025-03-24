import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/models/order';
import { StatusPipe } from '../../../shared/pipes/status.pipe';
import { PaymentPipe } from '../../../shared/pipes/payment.pipe';
import { DeliveryPipe } from '../../../shared/pipes/delivery.pipe';
import { ProductService } from '../../../core/services/product.service';
import { forkJoin, map } from 'rxjs';
import { Product } from '../../../shared/models/product';

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
    StatusPipe,
    PaymentPipe,
    DeliveryPipe,
  ],
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss'],
})
export class CheckoutSuccessComponent implements OnInit {
  order: Order | null = null;
  products: Product[] = [];
  isLoading = true;
  error = '';
  orderService = inject(OrderService);
  productService = inject(ProductService);
  route = inject(ActivatedRoute);
  productNames: { [key: string]: string } = {};

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('orderId');
    if (orderId) {
      this.orderService.getOrderById(orderId).subscribe({
        next: (orderData: Order) => {
          this.order = orderData;
          this.isLoading = false;
          this.loadProductDetails();
        },
      });
    } else {
      this.error = 'Order ID not passed.';
      this.isLoading = false;
    }
  }

  loadProductDetails(): void {
    if (this.order) {
      const productRequests = this.order.orderItems.map((item) =>
        this.productService.getProductById(item.productId)
      );

      forkJoin(productRequests).subscribe({
        next: (products: Product[]) => {
          this.products = products;
          this.products.forEach((product) => {
            this.productNames[product.id] = product.name;
          });
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Error loading products.';
          this.isLoading = false;
        },
      });
    }
  }
}
