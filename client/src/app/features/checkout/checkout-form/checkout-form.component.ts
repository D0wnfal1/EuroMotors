import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule, AsyncPipe, NgFor, NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

import { CartService } from '../../../core/services/cart.service';
import { CheckoutService } from '../../../core/services/checkout.service';
import { AccountService } from '../../../core/services/account.service';
import { OrderService } from '../../../core/services/order.service';
import { DeliveryPipe } from '../../../shared/pipes/delivery.pipe';
import { PaymentPipe } from '../../../shared/pipes/payment.pipe';
import {
  DeliveryMethod,
  PaymentMethod,
  OrderItem,
} from '../../../shared/models/order';
import { Warehouse } from '../../../shared/models/warehouse';
import { Observable, debounceTime, of, switchMap, tap } from 'rxjs';
import { Router } from '@angular/router';
import { PaymentService } from '../../../core/services/payment.service';

@Component({
  selector: 'app-checkout-form',
  standalone: true,
  imports: [
    CommonModule,
    NgFor,
    NgIf,
    AsyncPipe,
    MatStepperModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    DeliveryPipe,
    PaymentPipe,
    MatButtonModule,
  ],
  templateUrl: './checkout-form.component.html',
  styleUrls: ['./checkout-form.component.scss'],
})
export class CheckoutFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private orderService = inject(OrderService);
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private checkoutService = inject(CheckoutService);
  private paymentService = inject(PaymentService);
  private router = inject(Router);

  checkoutForm!: FormGroup;

  currentUser: any = null;

  deliveryMethods = [DeliveryMethod.Pickup, DeliveryMethod.Delivery];
  paymentMethods = [PaymentMethod.Prepaid, PaymentMethod.Postpaid];

  filteredWarehouses: Observable<Warehouse[]> = of([]);
  isLoading = false;
  selectedWarehouse: Warehouse | null = null;

  ngOnInit(): void {
    this.checkoutForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      city: ['', Validators.required],
      deliveryMethod: [DeliveryMethod.Pickup, Validators.required],
      warehouse: [''],
      warehouseQuery: [''],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /^\+?[0-9]{1,4}?[-. ]?(\(?\d{1,3}?\)?[-. ]?)?\d{1,4}[-. ]?\d{1,4}[-. ]?\d{1,4}$/
          ),
        ],
      ],
      email: ['', [Validators.required, Validators.email]],
      paymentMethod: [PaymentMethod.Prepaid, Validators.required],
    });

    this.accountService.getUserInfo().subscribe((user) => {
      if (user) {
        this.currentUser = user;
        this.checkoutForm.patchValue({
          email: user.email,
          firstName: user.firstName,
          lastName: user.lastName,
          phoneNumber: user.phoneNumber,
          city: user.city,
        });

        if (
          user.city &&
          this.checkoutForm.get('deliveryMethod')?.value ===
            this.deliveryMethods[1]
        ) {
          this.loadWarehouses(user.city);
        }
      }
    });

    this.checkoutForm
      .get('warehouseQuery')
      ?.valueChanges.pipe(
        debounceTime(500),
        switchMap((query: string | Warehouse) => {
          if (typeof query === 'string') {
            const city = this.checkoutForm.get('city')?.value;
            if (
              city &&
              this.checkoutForm.get('deliveryMethod')?.value ===
                this.deliveryMethods[1]
            ) {
              this.isLoading = true;
              return this.checkoutService.getWarehouses(city, query);
            }
          }
          return of([]);
        }),
        tap(() => {
          this.isLoading = false;
        })
      )
      .subscribe((warehouses: Warehouse[]) => {
        this.filteredWarehouses = of(warehouses);
      });

    this.checkoutForm
      .get('deliveryMethod')
      ?.valueChanges.subscribe((method) => {
        const warehouseQueryControl = this.checkoutForm.get('warehouseQuery');
        const warehouseControl = this.checkoutForm.get('warehouse');

        if (method === 'delivery') {
          warehouseQueryControl?.setValidators(Validators.required);
          warehouseControl?.setValidators(Validators.required);
        } else {
          warehouseQueryControl?.clearValidators();
          warehouseControl?.clearValidators();
        }

        warehouseQueryControl?.updateValueAndValidity();
        warehouseControl?.updateValueAndValidity();
      });
  }

  onCityInput(): void {
    this.selectedWarehouse = null;
    this.checkoutForm.get('warehouse')?.setValue('');
    this.checkoutForm.get('warehouseQuery')?.setValue('');

    if (
      this.checkoutForm.get('deliveryMethod')?.value === this.deliveryMethods[1]
    ) {
      const city = this.checkoutForm.get('city')?.value;
      if (city) {
        this.loadWarehouses(city);
      }
    }
  }

  onDeliveryMethodChange(method: string): void {
    if (method === 'delivery') {
      const city = this.checkoutForm.get('city')?.value;
      if (city) {
        this.loadWarehouses(city);
      }
    }
  }

  loadWarehouses(city: string, query: string = ''): void {
    if (city) {
      this.isLoading = true;
      this.checkoutService
        .getWarehouses(city, query)
        .subscribe((warehouses: Warehouse[]) => {
          this.filteredWarehouses = of(warehouses);
          this.isLoading = false;
        });
    }
  }

  onWarehouseSelected(warehouse: Warehouse): void {
    this.selectedWarehouse = warehouse;
    this.checkoutForm.get('warehouse')?.setValue(warehouse.description);
  }

  displayWarehouse(warehouse: Warehouse | null): string {
    return warehouse ? warehouse.description : '';
  }

  submitOrder(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    let shippingAddress = '';
    if (this.checkoutForm.value.deliveryMethod === this.deliveryMethods[1]) {
      shippingAddress = `${this.checkoutForm.value.city}, warehouse: ${this.checkoutForm.value.warehouse}`;
    } else {
      shippingAddress = 'PickUp';
    }

    const cartId = localStorage.getItem('cart_id') || '';
    if (!cartId) {
      return;
    }

    const orderData = {
      CartId: cartId,
      UserId: this.currentUser ? this.currentUser.id : null,
      BuyerName:
        this.checkoutForm.value.firstName +
        ' ' +
        this.checkoutForm.value.lastName,
      BuyerPhoneNumber: this.checkoutForm.value.phoneNumber,
      BuyerEmail: this.checkoutForm.value.email,
      DeliveryMethod: this.checkoutForm.value.deliveryMethod,
      ShippingAddress: shippingAddress,
      PaymentMethod: this.checkoutForm.value.paymentMethod,
    };

    this.orderService.createOrder(orderData).subscribe({
      next: (orderResponse) => {
        const orderId = orderResponse?.orderId;
        if (orderId) {
          if (this.checkoutForm.value.paymentMethod === PaymentMethod.Prepaid) {
            this.paymentService.createPayment(orderId).subscribe({
              next: (paymentResponse) => {
                const { data, signature } = paymentResponse;
                const paymentUrl = `https://www.liqpay.ua/api/3/checkout?data=${data}&signature=${signature}`;
                window.location.href = paymentUrl;
              },
              error: (paymentError) => {
                console.error('Amend for the completed payment:', paymentError);
              },
            });
          } else {
            this.cartService.clearCart(cartId);
            this.router.navigateByUrl(`/checkout/success/${orderId}`);
          }
        } else {
          console.error('Order ID is missing');
        }
      },
      error: (orderError) => {
        console.error('Amends upon completion of the agreement:', orderError);
      },
    });
  }
}
