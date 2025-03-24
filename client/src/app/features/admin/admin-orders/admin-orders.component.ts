import { Component, OnInit, inject } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/models/order';
import { OrderParams } from '../../../shared/models/orderParams';
import { DatePipe, CurrencyPipe, CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { StatusPipe } from '../../../shared/pipes/status.pipe';

@Component({
  selector: 'app-admin-orders',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatIcon,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatTooltipModule,
    MatTabsModule,
    RouterLink,
    StatusPipe,
    CommonModule,
  ],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.scss',
})
export class AdminOrdersComponent implements OnInit {
  displayedColumns: string[] = ['id', 'orderDate', 'total', 'status', 'action'];
  dataSource = new MatTableDataSource<Order>([]);
  private orderService = inject(OrderService);
  orderParams = new OrderParams();
  statusOptions = [
    'Pending',
    'Paid',
    'Shipped',
    'Completed',
    'Canceled',
    'Refunded',
  ];
  totalItems = 0;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.getOrders(this.orderParams).subscribe({
      next: (response) => {
        if (response.data) {
          this.dataSource.data = response.data;
          this.totalItems = response.count;
        }
      },
    });
  }

  onPageChange(event: PageEvent) {
    this.orderParams.pageNumber = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }

  onFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;
    this.loadOrders();
  }
}
