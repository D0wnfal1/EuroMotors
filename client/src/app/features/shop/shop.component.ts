import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSelectionListChange } from '@angular/material/list';
import { PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ProductImage } from '../../shared/models/productImage';
import { ShopParams } from '../../shared/models/shopParams';
import { ProductItemComponent } from './product-item/product-item.component';
import { ProductService } from '../../core/services/product.service';
import { ProductListComponent } from '../../shared/components/product-list/product-list.component';
import { CarmodelService } from '../../core/services/carmodel.service';
import { CategoryService } from '../../core/services/category.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-shop',
  imports: [FormsModule, ProductItemComponent, ProductListComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private carModelService = inject(CarmodelService);
  private route = inject(ActivatedRoute);
  products?: Pagination<Product>;
  productImages: { [key: string]: ProductImage[] } = {};
  sortOptions = [
    { name: 'Alphabetical', value: '' },
    { name: 'Price: Low-High', value: 'ASC' },
    { name: 'Price: High-Low', value: 'DESC' },
  ];
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      const carModelId = params['carModelId'];
      if (carModelId) {
        this.shopParams.carModelIds = [carModelId];
      }
      this.initialiseShop();
    });
  }

  initialiseShop() {
    this.categoryService.getCategories();
    this.carModelService.getCarModels({ pageNumber: 1, pageSize: 0 });
    this.getProducts();
  }

  getProducts() {
    this.productService.getProducts(this.shopParams).subscribe({
      next: (response) => {
        this.products = response;
      },
      error: (error) => console.error(error),
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

  onFilterChange(filters: any) {
    if (filters) {
      this.shopParams.categoryIds = filters.selectedCategories;
      this.shopParams.carModelIds = filters.selectedCarModels;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
  }
}
