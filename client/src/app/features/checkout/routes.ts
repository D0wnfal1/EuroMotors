import { Route } from '@angular/router';
import { CheckoutSuccessComponent } from './checkout-success/checkout-success.component';

export const checkoutRourtes: Route[] = [
  { path: 'success/:orderId', component: CheckoutSuccessComponent },
];
