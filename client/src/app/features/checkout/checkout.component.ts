import {
  ChangeDetectorRef,
  Component,
  ViewChild,
  AfterViewInit,
  inject,
} from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { CheckoutInformationComponent } from './checkout-information/checkout-information.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { CheckoutPaymentComponent } from './checkout-payment/checkout-payment.component';
import { Router, RouterLink } from '@angular/router';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { NgIf } from '@angular/common';
import { DeliveryMethod, PaymentMethod } from '../../shared/models/order';
import { AccountService } from '../../core/services/account.service';
import { User } from '../../shared/models/user';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    CheckoutDeliveryComponent,
    CheckoutInformationComponent,
    MatStepperModule,
    CheckoutReviewComponent,
    MatProgressSpinner,
    CheckoutPaymentComponent,
    RouterLink,
    NgIf,
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements AfterViewInit {
  cartService = inject(CartService);
  orderService = inject(OrderService);
  paymentService = inject(PaymentService);
  accountService = inject(AccountService);
  router = inject(Router);

  currentUser: User | null = null;
  selectedCity: string = '';
  selectedDeliveryMethod: string = '';
  selectedWarehouseName: string = '';
  isProcessing = false;

  @ViewChild(CheckoutInformationComponent)
  checkoutInformationComponent!: CheckoutInformationComponent;
  @ViewChild(CheckoutDeliveryComponent)
  checkoutDeliveryComponent!: CheckoutDeliveryComponent;
  @ViewChild(CheckoutPaymentComponent)
  checkoutPaymentComponent!: CheckoutPaymentComponent;

  constructor(private cdRef: ChangeDetectorRef, private fb: FormBuilder) {}

  ngAfterViewInit() {
    this.cdRef.detectChanges();
    this.accountService.getAuthState().subscribe((authState) => {
      if (authState.isAuthenticated) {
        this.accountService.getUserInfo().subscribe((user) => {
          this.currentUser = user;
        });
      }
    });
  }

  get informationForm(): FormGroup {
    return this.checkoutInformationComponent?.checkoutForm || this.fb.group({});
  }

  get deliveryGroup(): FormGroup {
    return this.checkoutDeliveryComponent?.deliveryGroup || this.fb.group({});
  }

  get paymentForm(): FormGroup {
    return this.checkoutPaymentComponent?.paymentForm || this.fb.group({});
  }

  onCityChanged(city: string): void {
    this.selectedCity = city;
  }

  saveInformation(): void {
    if (this.checkoutInformationComponent) {
      this.checkoutInformationComponent.onSubmit();
    }
  }

  onDeliveryMethodChanged(method: string): void {
    this.selectedDeliveryMethod = method;
  }

  onWarehouseSelected(warehouseName: string): void {
    this.selectedWarehouseName = warehouseName;
  }

  completeCheckout(): void {
    if (
      !this.informationForm.valid ||
      !this.deliveryGroup.valid ||
      !this.paymentForm.valid
    ) {
      return;
    }
    this.isProcessing = true;

    const cartId = localStorage.getItem('cart_id') || '';
    if (!cartId) {
      this.isProcessing = false;
      return;
    }

    const deliveryMethodValue =
      this.selectedDeliveryMethod === 'delivery'
        ? DeliveryMethod.Delivery
        : DeliveryMethod.Pickup;

    const shippingAddress =
      this.selectedDeliveryMethod === 'delivery'
        ? this.selectedCity + this.selectedWarehouseName
        : null;

    const paymentMethodValue = this.paymentForm.get('paymentMethod')?.value;

    const orderData = {
      CartId: cartId,
      UserId: this.currentUser ? this.currentUser.id : null,
      BuyerName:
        this.informationForm.get('firstName')?.value +
        ' ' +
        this.informationForm.get('lastName')?.value,
      BuyerPhoneNumber: this.informationForm.get('phoneNumber')?.value,
      BuyerEmail: this.informationForm.get('email')?.value,
      DeliveryMethod: deliveryMethodValue,
      ShippingAddress: shippingAddress,
      PaymentMethod: paymentMethodValue,
    };

    this.orderService.createOrder(orderData).subscribe({
      next: (orderResponse) => {
        const orderId = orderResponse?.orderId;
        if (orderId) {
          if (paymentMethodValue === PaymentMethod.Prepaid) {
            this.paymentService.createPayment(orderId).subscribe({
              next: (paymentResponse) => {
                this.isProcessing = false;
                const { data, signature } = paymentResponse;
                const paymentUrl = `https://www.liqpay.ua/api/3/checkout?data=${data}&signature=${signature}`;
                window.location.href = paymentUrl;
              },
              error: (paymentError) => {
                console.error('Amend for the completed payment:', paymentError);
                this.isProcessing = false;
              },
            });
          } else {
            this.isProcessing = false;
            this.cartService.clearCart(cartId);
            this.router.navigateByUrl(`/checkout/success/${orderId}`);
          }
        } else {
          console.error('Order ID is missing');
          this.isProcessing = false;
        }
      },
      error: (orderError) => {
        console.error('Amends upon completion of the agreement:', orderError);
        this.isProcessing = false;
      },
    });
  }
}
