import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import {
  Order,
  OrderStatus,
  PaymentMethod,
  DeliveryMethod,
} from '../../../shared/models/order';
import { CurrencyPipe, NgIf, NgFor } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { StatusPipe } from '../../../shared/pipes/status.pipe';
import { AccountService } from '../../../core/services/account.service';
import { PaymentPipe } from '../../../shared/pipes/payment.pipe';
import { DeliveryPipe } from '../../../shared/pipes/delivery.pipe';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';

@Component({
  selector: 'app-order-detailed',
  imports: [
    MatCardModule,
    MatButton,
    CurrencyPipe,
    NgIf,
    NgFor,
    StatusPipe,
    PaymentPipe,
    DeliveryPipe,
  ],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss',
})
export class OrderDetailedComponent implements OnInit {
  public OrderStatus = OrderStatus;
  public PaymentMethod = PaymentMethod;
  public DeliveryMethod = DeliveryMethod;

  private orderService = inject(OrderService);
  accountService = inject(AccountService);
  private productService = inject(ProductService);
  private activatedRoute = inject(ActivatedRoute);
  order?: Order;
  private router = inject(Router);
  buttonText: string | undefined;
  nextStatus: string = '';
  isLoading: boolean = true;
  productNames: { [key: string]: string } = {};

  constructor(private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void {
    const orderId = this.activatedRoute.snapshot.paramMap.get('id');
    if (orderId) {
      this.orderService.getOrderById(orderId).subscribe({
        next: (order) => {
          if (order) {
            this.order = order;
            this.isLoading = false;
            this.setNextStatus();
            this.updateButtonText();
            this.loadProductNames();
            this.changeDetectorRef.detectChanges();
          }
        },
        error: (error) => {
          this.isLoading = false;
          console.error('Failed to load order', error);
          this.changeDetectorRef.detectChanges();
        },
      });
    }
  }

  loadProductNames(): void {
    if (this.order) {
      this.order.orderItems.forEach((item) => {
        this.productService.getProductById(item.productId).subscribe({
          next: (product: Product) => {
            this.productNames[item.productId] = product.name;
            this.changeDetectorRef.detectChanges();
          },
          error: (error) => {
            console.error('Failed to fetch product name', error);
          },
        });
      });
    }
  }

  onReturnClick() {
    this.accountService.isAdmin()
      ? this.router.navigateByUrl('/admin/orders')
      : this.router.navigateByUrl('/orders');
  }

  updateButtonText() {
    this.buttonText = this.accountService.isAdmin()
      ? 'Return to admin'
      : 'Return to orders';
  }

  setNextStatus() {
    if (!this.order) return;
    const statuses: OrderStatus[] = [
      OrderStatus.Pending,
      OrderStatus.Shipped,
      OrderStatus.Completed,
    ];
    const currentIndex = statuses.indexOf(this.order.status);
    if (currentIndex !== -1 && currentIndex < statuses.length - 1) {
      const nextStatusEnum = statuses[currentIndex + 1];
      switch (nextStatusEnum) {
        case OrderStatus.Shipped:
          this.nextStatus = 'Відправлено';
          break;
        case OrderStatus.Completed:
          this.nextStatus = 'Завершено';
          break;
        default:
          this.nextStatus = OrderStatus[nextStatusEnum];
      }
    } else {
      this.nextStatus = '';
    }
  }

  changeOrderStatus() {
    if (!this.order) return;
    if (
      this.order.status === OrderStatus.Completed ||
      this.order.status === OrderStatus.Canceled
    ) {
      return;
    }
    let nextStatus: OrderStatus;
    switch (this.order.status) {
      case OrderStatus.Pending:
        nextStatus = OrderStatus.Shipped;
        break;
      case OrderStatus.Shipped:
        nextStatus = OrderStatus.Completed;
        break;
      default:
        return;
    }
    this.updateOrderStatus(nextStatus);
  }

  cancelOrder() {
    if (!this.order) return;
    if (this.order.status !== OrderStatus.Canceled) {
      this.updateOrderStatus(OrderStatus.Canceled);
    } else {
      this.deleteOrder();
    }
  }

  refundOrder() {
    if (!this.order) return;
    this.updateOrderStatus(OrderStatus.Refunded);
  }

  private updateOrderStatus(newStatus: OrderStatus) {
    if (!this.order) return;
    this.orderService.updateOrderStatus(this.order.id, newStatus).subscribe({
      next: () => {
        this.orderService.getOrderById(this.order!.id).subscribe({
          next: (updatedOrder) => {
            this.order = updatedOrder;
            this.setNextStatus();
            this.changeDetectorRef.detectChanges();
          },
          error: (err) => {
            console.error('Failed to refresh order data', err);
          },
        });
      },
      error: (err) => {
        console.error('Failed to update order status', err);
      },
    });
  }

  deleteOrder() {
    if (!this.order) return;
    this.orderService.deleteOrder(this.order.id).subscribe({
      next: () => {
        this.router.navigate(['/admin/orders']);
      },
      error: (err) => {
        console.error('Failed to delete order', err);
      },
    });
  }
}
