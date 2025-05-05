import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { CarBrandProductsComponent } from './features/car-brand/car-brand-products/car-brand-products.component';
import { DeliveryPaymentComponent } from './features/home/delivery-payment/delivery-payment.component';
import { WarrantyReturnsComponent } from './features/home/warranty-returns/warranty-returns.component';
import { PublicOfferComponent } from './features/home/public-offer/public-offer.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'shop',
    loadChildren: () =>
      import('./features/shop/shop.module').then((m) => m.ShopModule),
  },
  { path: 'delivery-payment', component: DeliveryPaymentComponent },
  { path: 'warranty-returns', component: WarrantyReturnsComponent },
  { path: 'public-offer', component: PublicOfferComponent },
  { path: 'brand/:id', component: CarBrandProductsComponent },
  {
    path: 'cart',
    loadChildren: () =>
      import('./features/cart/cart.module').then((m) => m.CartModule),
  },
  {
    path: 'checkout',
    loadChildren: () =>
      import('./features/checkout/routes').then((r) => r.checkoutRourtes),
  },
  {
    path: 'orders',
    loadChildren: () =>
      import('./features/orders/routes').then((r) => r.orderRourtes),
  },
  {
    path: 'account',
    loadChildren: () =>
      import('./features/account/routes').then((r) => r.accountRourtes),
  },
  { path: 'not-found', component: NotFoundComponent },
  {
    path: 'admin',
    loadChildren: () =>
      import('./features/admin/routes').then((c) => c.adminRourtes),
  },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
