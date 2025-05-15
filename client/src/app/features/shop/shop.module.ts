import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { shopRoutes } from './shop.routes';
import { ShopComponent } from './shop.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoryItemsComponent } from './category-items/category-items.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { RelatedProductsComponent } from './related-products/related-products.component';
import { ImageOptimizationModule } from '../../shared/modules/image-optimization.module';
import {
  MatPaginatorModule,
  MatPaginatorIntl,
} from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule } from '@angular/material/list';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UkrainianPaginatorIntl } from '../../shared/i18n/ukrainian-paginator-intl';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(shopRoutes),
    ImageOptimizationModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    ShopComponent,
    ProductDetailsComponent,
    CategoryItemsComponent,
    ProductItemComponent,
    RelatedProductsComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: UkrainianPaginatorIntl }],
})
export class ShopModule {}
