import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  MatPaginatorModule,
  PageEvent,
  MatPaginatorIntl,
} from '@angular/material/paginator';
import { CategoryService } from '../../../core/services/category.service';
import { ProductService } from '../../../core/services/product.service';
import { Category } from '../../../shared/models/category';
import { Pagination } from '../../../shared/models/pagination';
import { Product } from '../../../shared/models/product';
import { ShopParams } from '../../../shared/models/shopParams';
import { ProductItemComponent } from '../product-item/product-item.component';
import { Subscription } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import {
  MatListOption,
  MatSelectionList,
  MatSelectionListChange,
} from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { UkrainianPaginatorIntl } from '../../../shared/i18n/ukrainian-paginator-intl';

@Component({
  selector: 'app-category-items',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatPaginatorModule,
    ProductItemComponent,
    MatIconModule,
    MatMenuTrigger,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatButtonModule,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: UkrainianPaginatorIntl }],
  templateUrl: './category-items.component.html',
  styleUrl: './category-items.component.scss',
})
export class CategoryItemsComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  private subscriptions: Subscription[] = [];

  category?: Category;
  products?: Pagination<Product>;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  totalItems = 0;
  searchTerm: string = '';
  sortOptions = [
    { name: 'За алфавітом', value: '' },
    { name: 'Ціна: від низької до високої', value: 'ASC' },
    { name: 'Ціна: від високої до низької', value: 'DESC' },
  ];

  ngOnInit() {
    const paramSub = this.route.params.subscribe((params) => {
      const categoryId = params['id'];
      if (categoryId) {
        this.shopParams = new ShopParams();

        const querySub = this.route.queryParams.subscribe((queryParams) => {
          if (queryParams['sortOrder']) {
            this.shopParams.sortOrder = queryParams['sortOrder'];
          }
          if (queryParams['pageNumber']) {
            this.shopParams.pageNumber = +queryParams['pageNumber'];
          }
          if (queryParams['pageSize']) {
            this.shopParams.pageSize = +queryParams['pageSize'];
          }
          if (queryParams['searchTerm']) {
            this.searchTerm = queryParams['searchTerm'];
          }

          this.loadCategory(categoryId);
        });

        this.subscriptions.push(querySub);
      }
    });

    this.subscriptions.push(paramSub);
  }

  loadCategory(categoryId: string): void {
    const categorySub = this.categoryService
      .getCategoryById(categoryId)
      .subscribe({
        next: (category) => {
          this.category = category;
          this.loadProductsForCategory(categoryId);
        },
        error: (err) => {
          console.error('Error loading category', err);
        },
      });

    this.subscriptions.push(categorySub);
  }

  loadProductsForCategory(categoryId: string): void {
    const productsSub = this.productService
      .getProductsByCategoryWithChildren(
        categoryId,
        this.shopParams.sortOrder,
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

    this.subscriptions.push(productsSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    if (this.category) {
      this.loadProductsForCategory(this.category.id);
    }
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      if (this.category) {
        this.loadProductsForCategory(this.category.id);
      }
    }
  }

  onSearch(searchTerm: string) {
    this.searchTerm = searchTerm;
    this.shopParams.pageNumber = 1;
    if (this.category) {
      this.loadProductsForCategory(this.category.id);
    }
  }
}
