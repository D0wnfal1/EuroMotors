import { Component, OnInit, inject } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order, OrderStatus } from '../../shared/models/order';
import { AccountService } from '../../core/services/account.service';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss',
})
export class OrderComponent implements OnInit {
  private orderService = inject(OrderService);
  private accountService = inject(AccountService);

  orders: Order[] = [];
  ngOnInit(): void {
    this.accountService.getUserInfo().subscribe((user) => {
      if (user) {
        this.orderService.getUserOrders(user.id).subscribe((orders) => {
          this.orders = orders;
        });
      }
    });
  }

  getStatusName(status: OrderStatus): string {
    return OrderStatus[status];
  }
}
