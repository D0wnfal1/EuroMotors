import { Routes } from '@angular/router';
import { ShopComponent } from './shop.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoryItemsComponent } from './category-items/category-items.component';

export const shopRoutes: Routes = [
  { path: '', component: ShopComponent },
  { path: ':id', component: ProductDetailsComponent },
  { path: 'category/:id', component: CategoryItemsComponent },
];
