import { Route } from '@angular/router';
import { AdminComponent } from './admin.component';
import { AdminOrdersComponent } from './admin-orders/admin-orders.component';
import { adminGuard } from '../../core/guards/admin.guard';
import { authGuard } from '../../core/guards/auth.guard';
import { ProductCreateComponent } from './admin-products/product-create/product-create.component';
import { ProductEditComponent } from './admin-products/product-edit/product-edit.component';
import { AdminProductsComponent } from './admin-products/admin-products.component';

export const adminRourtes: Route[] = [
  {
    path: '',
    component: AdminComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'orders',
    component: AdminOrdersComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'products',
    component: AdminProductsComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'products/create',
    component: ProductCreateComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'products/edit/:id',
    component: ProductEditComponent,
    canActivate: [authGuard, adminGuard],
  },
];
