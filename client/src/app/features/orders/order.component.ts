import { Component, OnInit, inject } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../shared/models/order';
import { AccountService } from '../../core/services/account.service';
import { RouterLink } from '@angular/router';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { StatusPipe } from '../../shared/pipes/status.pipe';
import { MatTableModule } from '@angular/material/table';
import { EmptyOrdersStateComponent } from '../../shared/components/empty-orders-state/empty-orders-state.component';

@Component({
  selector: 'app-order',
  imports: [
    RouterLink,
    CurrencyPipe,
    DatePipe,
    StatusPipe,
    CommonModule,
    MatTableModule,
    EmptyOrdersStateComponent,
  ],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss',
})
export class OrderComponent implements OnInit {
  private orderService = inject(OrderService);
  private accountService = inject(AccountService);
  displayedColumns: string[] = ['order', 'date', 'total', 'status'];

  orders: Order[] = [];
  ngOnInit(): void {
    this.accountService.checkAuth().subscribe({
      next: (auth) => {
        if (auth.user) {
          this.loadOrders(auth.user.id);
        }
      },
      error: (error) => {
        console.error('Error loading user info:', error);
      },
    });
  }

  loadOrders(id: string) {
    this.orderService.getUserOrders(id).subscribe((orders) => {
      this.orders = orders;
    });
  }
}
