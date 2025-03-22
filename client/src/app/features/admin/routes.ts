import { Route } from '@angular/router';
import { AdminComponent } from './admin.component';
import { AdminOrdersComponent } from './admin-orders/admin-orders.component';
import { adminGuard } from '../../core/guards/admin.guard';
import { authGuard } from '../../core/guards/auth.guard';
import { AdminProductsComponent } from './admin-products/admin-products.component';
import { ProductFormComponent } from './admin-products/product-form/product-form.component';
import { AdminCategoriesComponent } from './admin-categories/admin-categories.component';
import { CategoryFormComponent } from './admin-categories/category-form/category-form.component';
import { AdminCarmodelsComponent } from './admin-carmodels/admin-carmodels.component';
import { CarmodelFormComponent } from './admin-carmodels/carmodel-form/carmodel-form.component';

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
    component: ProductFormComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'products/edit/:id',
    component: ProductFormComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'categories',
    component: AdminCategoriesComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'categories/create',
    component: CategoryFormComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'categories/edit/:id',
    component: CategoryFormComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'carmodels',
    component: AdminCarmodelsComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'carmodels/create',
    component: CarmodelFormComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'carmodels/edit/:id',
    component: CarmodelFormComponent,
    canActivate: [authGuard, adminGuard],
  },
];
