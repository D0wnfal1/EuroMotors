import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/models/order';
import { CurrencyPipe, NgIf, NgFor } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { StatusPipe } from '../../../shared/pipes/status.pipe';
import { AccountService } from '../../../core/services/account.service';

@Component({
  selector: 'app-order-detailed',
  imports: [MatCardModule, MatButton, CurrencyPipe, NgIf, NgFor, StatusPipe],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss',
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  order?: Order;
  private router = inject(Router);
  buttonText = this.accountService.isAdmin()
    ? 'Return to admin'
    : 'Return to orders';

  ngOnInit(): void {
    this.loadOrder();
  }

  onReturnClick() {
    this.accountService.isAdmin()
      ? this.router.navigateByUrl('/admin')
      : this.router.navigateByUrl('/orders');
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.orderService.getOrderById(id).subscribe({
      next: (order) => (this.order = order),
    });
  }
}
