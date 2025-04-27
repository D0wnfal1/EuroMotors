import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { RouterLink } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Pagination } from '../../../shared/models/pagination';
import { ProductImage } from '../../../shared/models/productImage';
import { ShopParams } from '../../../shared/models/shopParams';
import {
  MatSelectionListChange,
  MatSelectionList,
  MatListOption,
} from '@angular/material/list';
import {
  MatButton,
  MatButtonModule,
  MatIconButton,
} from '@angular/material/button';
import { PageEvent, MatPaginator } from '@angular/material/paginator';
import { CarModel } from '../../../shared/models/carModel';
import { Category } from '../../../shared/models/category';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CategoryService } from '../../../core/services/category.service';
import { MatTableModule } from '@angular/material/table';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { NgFor, NgIf } from '@angular/common';

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
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss',
})
export class AdminProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private carModelService = inject(CarmodelService);
  categories: Category[] = [];
  carModels: CarModel[] = [];
  products?: Pagination<Product>;
  productImages: { [key: string]: ProductImage[] } = {};
  sortOptions = [
    { name: 'Alphabetical', value: '' },
    { name: 'Price: Low-High', value: 'ASC' },
    { name: 'Price: High-Low', value: 'DESC' },
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
        this.getProducts();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
}
