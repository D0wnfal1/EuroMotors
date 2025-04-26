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
  sortOrder: string = 'name_asc';
  searchTerm: string = '';

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
              this.loadProducts(categoryId);
            })
          );
        })
      )
      .subscribe({
        error: (err) => {
          console.error('Error loading category or products', err);
        },
      });

    this.route.queryParams.subscribe((params) => {
      if (params['sortOrder']) {
        this.sortOrder = params['sortOrder'];
      }
      if (params['pageNumber']) {
        this.shopParams.pageNumber = +params['pageNumber'];
      }
      if (params['pageSize']) {
        this.shopParams.pageSize = +params['pageSize'];
      }
      if (params['searchTerm']) {
        this.searchTerm = params['searchTerm'];
      }
    });
  }

  loadProducts(categoryId: string) {
    this.productService
      .getProductsByCategoryWithChildren(
        categoryId,
        this.sortOrder,
        this.searchTerm,
        this.shopParams.pageNumber,
        this.shopParams.pageSize
      )
      .subscribe({
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
    if (this.category) {
      this.loadProducts(this.category.id);
    }
  }

  onSortOrderChange(newSortOrder: string) {
    this.sortOrder = newSortOrder;
    if (this.category) {
      this.loadProducts(this.category.id);
    }
  }

  onSearch(searchTerm: string) {
    this.searchTerm = searchTerm;
    if (this.category) {
      this.loadProducts(this.category.id);
    }
  }
}
