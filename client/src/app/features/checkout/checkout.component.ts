import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { CheckoutInformationComponent } from './checkout-information/checkout-information.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { CheckoutPaymentComponent } from './checkout-payment/checkout-payment.component';
import { RouterLink } from '@angular/router';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';

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
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  selectedCity: string = '';

  @ViewChild(CheckoutInformationComponent)
  checkoutInformationComponent!: CheckoutInformationComponent;

  @ViewChild(CheckoutDeliveryComponent)
  checkoutDeliveryComponent!: CheckoutDeliveryComponent;

  @ViewChild(CheckoutPaymentComponent)
  checkoutPaymentComponent!: CheckoutPaymentComponent;

  constructor(private cdRef: ChangeDetectorRef) {}

  ngAfterViewInit() {
    this.cdRef.detectChanges();
  }

  onCityChanged(city: string): void {
    this.selectedCity = city;
  }

  saveInformation(): void {
    if (this.checkoutInformationComponent) {
      this.checkoutInformationComponent.onSubmit();
    }
  }

  isStepValid(stepIndex: number): boolean {
    if (stepIndex === 0) {
      return this.checkoutInformationComponent?.checkoutForm?.valid || false;
    }
    if (stepIndex === 1) {
      return this.checkoutDeliveryComponent?.deliveryGroup?.valid || false;
    }
    if (stepIndex === 2) {
      return this.checkoutPaymentComponent?.paymentForm?.valid || false;
    }
    return true;
  }
}
