import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { RouterLink } from '@angular/router';
import { CommonModule, CurrencyPipe, NgFor } from '@angular/common';
import { Pagination } from '../../../shared/models/pagination';
import { ProductImage } from '../../../shared/models/productImage';
import { ShopParams } from '../../../shared/models/shopParams';
import { MatSelectionListChange } from '@angular/material/list';
import { MatButton } from '@angular/material/button';
import { ProductListComponent } from '../../../shared/components/product-list/product-list.component';
import { PageEvent } from '@angular/material/paginator';
import { CarModel } from '../../../shared/models/carModel';
import { Category } from '../../../shared/models/category';
import { MatIcon } from '@angular/material/icon';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CategoryService } from '../../../core/services/category.service';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-admin-products',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    CurrencyPipe,
    MatButton,
    ProductListComponent,
    MatIcon,
    CommonModule,
    MatTableModule,
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
    'carModel',
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

  loadCategories() {
    this.categoryService.getCategories({ pageNumber: 1, pageSize: 0 });
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
    return this.categories.find((c) => c.id === categoryId)?.name || '—';
  }

  getCarModelName(carModelId: string): string {
    const model = this.carModels.find((cm) => cm.id === carModelId);
    return model ? `${model.brand} ${model.model}` : '—';
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
}
