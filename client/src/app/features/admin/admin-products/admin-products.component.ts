import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { ImportProductsResult, Product } from '../../../shared/models/product';
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
    const category = this.categories.find((cm) => cm.id === categoryId);
    return category ? `${category.name}` : '—';
  }

  getCarModelNames(carModelIds: string[]): string {
    if (!carModelIds || carModelIds.length === 0) return '—';

    return carModelIds
      .map((id) => {
        const model = this.carModels.find((cm) => cm.id === id);
        return model ? `${model.brandName || ''} ${model.modelName}` : '';
      })
      .filter((name) => name !== '')
      .join(', ');
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

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
  }

  toggleAvailability(product: Product) {
    const newAvailability = !product.isAvailable;
    this.productService
      .setProductAvailability(product.id, newAvailability)
      .subscribe({
        next: () => {
          product.isAvailable = newAvailability;
        },
        error: (err) => {
          console.error(err);
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

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];

      if (file.type !== 'text/csv') {
        this.snackBar.open('Please select a CSV file', 'Close', {
          duration: 3000,
        });
        return;
      }

      this.productService.importProducts(file).subscribe({
        next: (result: ImportProductsResult) => {
          this.snackBar.open(
            `Import completed. Successfully imported ${result.successfullyImported} of ${result.totalProcessed} products.`,
            'Close',
            { duration: 5000 }
          );

          if (result.errors.length > 0) {
            this.snackBar.open(
              `Errors occurred during import: ${result.errors.join(', ')}`,
              'Close',
              { duration: 7000 }
            );
          }

          this.productService.clearCache();
          this.getProducts();

          input.value = '';
        },
        error: (error) => {
          console.error('Import failed:', error);
          this.snackBar.open('Import failed. Please try again.', 'Close', {
            duration: 3000,
          });
          input.value = '';
        },
      });
    }
  }
}
