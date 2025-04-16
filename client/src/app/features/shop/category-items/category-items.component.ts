import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { switchMap, tap } from 'rxjs/operators';
import { CategoryService } from '../../../core/services/category.service';
import { ProductService } from '../../../core/services/product.service';
import { Category } from '../../../shared/models/category';
import { Pagination } from '../../../shared/models/pagination';
import { Product } from '../../../shared/models/product';
import { ShopParams } from '../../../shared/models/shopParams';
import { ProductItemComponent } from '../product-item/product-item.component';

@Component({
  selector: 'app-category-items',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatPaginatorModule,
    ProductItemComponent,
  ],
  templateUrl: './category-items.component.html',
  styleUrl: './category-items.component.scss',
})
export class CategoryItemsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  category?: Category;
  products?: Pagination<Product>;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  totalItems = 0;

  ngOnInit() {
    this.route.params
      .pipe(
        tap(() => {
          this.shopParams = new ShopParams();
        }),
        switchMap((params) => {
          const categoryId = params['id'];
          if (!categoryId) {
            return [];
          }

          return this.categoryService.getCategoryById(categoryId).pipe(
            tap((category) => {
              this.category = category;

              this.shopParams.categoryIds = [categoryId];

              this.loadProducts();
            })
          );
        })
      )
      .subscribe({
        error: (err) => {
          console.error('Error loading category or products', err);
        },
      });
  }

  loadProducts() {
    this.productService.getProducts(this.shopParams).subscribe({
      next: (response) => {
        this.products = response;
        this.totalItems = response.count;
      },
      error: (err) => {
        console.error('Error loading products', err);
      },
    });
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.loadProducts();
  }
}
