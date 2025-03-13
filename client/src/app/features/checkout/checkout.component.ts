import { Component } from '@angular/core';
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
export class CheckoutComponent {}
