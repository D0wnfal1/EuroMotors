import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { RouterLink } from '@angular/router';
import { CommonModule, CurrencyPipe, NgFor } from '@angular/common';
import { Pagination } from '../../../shared/models/pagination';
import { ProductImage } from '../../../shared/models/productImage';
import { ShopParams } from '../../../shared/models/shopParams';
import {
  MatSelectionListChange,
  MatSelectionList,
  MatListOption,
} from '@angular/material/list';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import {
  PageEvent,
  MatPaginator,
  MatPaginatorIntl,
} from '@angular/material/paginator';
import { CarModel } from '../../../shared/models/carModel';
import { Category } from '../../../shared/models/category';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CategoryService } from '../../../core/services/category.service';
import { MatTableModule } from '@angular/material/table';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UkrainianPaginatorIntl } from '../../../shared/i18n/ukrainian-paginator-intl';

@Component({
  selector: 'app-admin-products',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    RouterLink,
    CurrencyPipe,
    MatButtonModule,
    MatIconButton,
    CommonModule,
    MatTableModule,
    MatSlideToggleModule,
    MatIcon,
    MatPaginator,
    MatMenu,
    MatSelectionList,
    MatListOption,
    NgFor,
    MatMenuTrigger,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: UkrainianPaginatorIntl }],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss',
})
export class AdminProductsComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly carModelService = inject(CarmodelService);
  private readonly snackBar = inject(MatSnackBar);
  categories: Category[] = [];
  carModels: CarModel[] = [];
  products?: Pagination<Product>;
  productImages: { [key: string]: ProductImage[] } = {};
  sortOptions = [
    { name: 'За алфавітом', value: '' },
    { name: 'Ціна: від низької до високої', value: 'ASC' },
    { name: 'Ціна: від високої до низької', value: 'DESC' },
  ];
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  displayedColumns: string[] = [
    'name',
    'category',
    'carModels',
    'vendorCode',
    'price',
    'discount',
    'stock',
    'isAvailable',
    'actions',
  ];

  ngOnInit() {
    this.getProducts();
    this.loadCategories();
    this.loadCarModels();
  }
  getProducts() {
    this.productService.getProducts(this.shopParams).subscribe({
      next: (response) => {
        this.products = response;
      },
      error: (error) => console.error(error),
    });
  }

  loadCategories(): void {
    this.categoryService.getCategories();
    this.categoryService.categories$.subscribe((data) => {
      this.categories = data;
    });
  }

  loadCarModels() {
    this.carModelService.getCarModels({ pageNumber: 1, pageSize: 0 });
    this.carModelService.carModels$.subscribe((data) => {
      this.carModels = data;
    });
  }

  getCategoryName(categoryId: string): string {
    const category = this.categories.find((c) => c.id === categoryId);
    return category ? category.name : 'Не вказано';
  }

  getCarModelNames(carModelIds: string[]): string {
    if (!carModelIds || carModelIds.length === 0) return 'Не вказано';
    const models = carModelIds
      .map((id) => {
        const model = this.carModels.find((m) => m.id === id);
        return model ? model.modelName : '';
      })
      .filter((name) => name !== '');
    return models.length > 0 ? models.join(', ') : 'Не вказано';
  }

  deleteProduct(productId: string): void {
    this.productService.deleteProduct(productId).subscribe(() => {
      if (this.products) {
        this.products.data = this.products.data.filter(
          (p: Product) => p.id !== productId
        );
      }
    });
  }

  onSearchChange() {
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();
  }

  onSortChange(event: MatSelectionListChange): void {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.getProducts();
    }
  }

  toggleAvailability(product: Product): void {
    product.isAvailable = !product.isAvailable;

    this.productService.setProductAvailability(product.id, product.isAvailable)
      .subscribe({
        next: () => {
          this.snackBar.open(
            `Доступність товару "${product.name}" змінено на ${
              product.isAvailable ? 'доступний' : 'недоступний'
            }`,
            'OK',
            { duration: 3000 }
          );
        },
        error: (error: any) => {
          console.error('Error updating product availability:', error);
          product.isAvailable = !product.isAvailable; // Revert the change
          this.snackBar.open(
            'Помилка при зміні доступності товару',
            'OK',
            { duration: 3000 }
          );
        },
      });
  }

  copyProduct(productId: string): void {
    this.productService.copyProduct(productId).subscribe({
      next: () => {
        this.productService.clearCache();
        this.getProducts();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
}
