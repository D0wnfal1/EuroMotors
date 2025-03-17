import { Route } from '@angular/router';
import { emptyCartGuard } from '../../core/guards/epmty-cart.guard';
import { CheckoutSuccessComponent } from './checkout-success/checkout-success.component';
import { CheckoutComponent } from './checkout.component';

export const checkoutRourtes: Route[] = [
  {
    path: '',
    component: CheckoutComponent,
    // canActivate: [authGuard, emptyCartGuard],
    canActivate: [emptyCartGuard],
  },
  { path: 'success/:orderId', component: CheckoutSuccessComponent },
];
