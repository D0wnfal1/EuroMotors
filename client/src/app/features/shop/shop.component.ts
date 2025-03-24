import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import {
  MatSelectionList,
  MatListOption,
  MatSelectionListChange,
} from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ProductImage } from '../../shared/models/productImage';
import { ShopParams } from '../../shared/models/shopParams';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { ProductService } from '../../core/services/product.service';
import { ProductListComponent } from '../../shared/components/product-list/product-list.component';
import { ImageService } from '../../core/services/image.service';
import { CarmodelService } from '../../core/services/carmodel.service';
import { CategoryService } from '../../core/services/category.service';

@Component({
  selector: 'app-shop',
  imports: [
    MatButton,
    MatIcon,
    FormsModule,
    ProductItemComponent,
    ProductListComponent,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private carModelService = inject(CarmodelService);
  private imageService = inject(ImageService);
  private dialogService = inject(MatDialog);
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
    this.initialiseShop();
  }

  initialiseShop() {
    this.categoryService.getCategories({ pageNumber: 1, pageSize: 0 });
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
  openFiltersDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedCategoryIds: this.shopParams.categoryIds ?? [],
        selectedCarModelIds: this.shopParams.carModelIds ?? [],
      },
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (
          result &&
          Array.isArray(result.selectedCategories) &&
          Array.isArray(result.selectedCarModels)
        ) {
          this.shopParams.categoryIds = result.selectedCategories;
          this.shopParams.carModelIds = result.selectedCarModels;
          this.shopParams.pageNumber = 1;
          this.getProducts();
        }
      },
      error: (err) => console.error('Dialog closed with error:', err),
    });
  }
}
