import { Component, inject, OnInit, HostListener } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatSelectionListChange,
  MatSelectionList,
  MatListOption,
} from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ProductImage } from '../../shared/models/productImage';
import { ShopParams } from '../../shared/models/shopParams';
import { ProductItemComponent } from './product-item/product-item.component';
import { ProductService } from '../../core/services/product.service';
import { ActivatedRoute } from '@angular/router';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { NgFor } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shop',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    ProductItemComponent,
    MatPaginator,
    MatMenu,
    MatIcon,
    MatSelectionList,
    MatListOption,
    NgFor,
    MatMenuTrigger,
    MatButtonModule,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  private productService = inject(ProductService);
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

      const searchTerm = params['search'];
      if (searchTerm) {
        this.shopParams.searchTerm = searchTerm;
      }

      const pageNumber = params['pageNumber'];
      if (pageNumber) {
        this.shopParams.pageNumber = +pageNumber;
      }

      this.initialiseShop();
    });
  }

  initialiseShop() {
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
}
