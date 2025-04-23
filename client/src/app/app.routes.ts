import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { CartComponent } from './features/cart/cart.component';
import { CategoryItemsComponent } from './features/shop/category-items/category-items.component';
import { CarBrandProductsComponent } from './features/car-brand/car-brand-products/car-brand-products.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'shop', component: ShopComponent },
  { path: 'shop/:id', component: ProductDetailsComponent },
  { path: 'shop/category/:id', component: CategoryItemsComponent },
  { path: 'brand/:id', component: CarBrandProductsComponent },
  { path: 'cart', component: CartComponent },
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
