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
  router = inject(Router);

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
  }

  get checkoutForm(): FormGroup {
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
      !this.checkoutForm.valid ||
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

    this.isProcessing = false;
    this.router.navigateByUrl('/checkout/success');
  }
}
