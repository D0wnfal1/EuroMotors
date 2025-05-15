import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CarSelectionComponent } from '../../home/car-selection/car-selection.component';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';
import { ShopParams } from '../../../shared/models/shopParams';
import { Subscription } from 'rxjs';
import { CarbrandService } from '../../../core/services/carbrand.service';
import { CarBrand } from '../../../shared/models/carBrand';
import { Product } from '../../../shared/models/product';
import { ProductItemComponent } from '../../shop/product-item/product-item.component';
import { CartService } from '../../../core/services/cart.service';
import {
  MatPaginatorModule,
  PageEvent,
  MatPaginatorIntl,
} from '@angular/material/paginator';
import { Pagination } from '../../../shared/models/pagination';
import {
  MatListOption,
  MatSelectionList,
  MatSelectionListChange,
} from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { UkrainianPaginatorIntl } from '../../../shared/i18n/ukrainian-paginator-intl';

@Component({
  selector: 'app-car-brand-products',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CarSelectionComponent,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    ProductItemComponent,
    MatPaginatorModule,
    NgFor,
    MatMenuTrigger,
    MatMenu,
    MatSelectionList,
    MatListOption,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: UkrainianPaginatorIntl }],
  templateUrl: './car-brand-products.component.html',
  styleUrls: ['./car-brand-products.component.scss'],
})
export class CarBrandProductsComponent implements OnInit, OnDestroy {
  brandId: string = '';
  carBrand?: CarBrand;
  products?: Pagination<Product>;
  totalItems: number = 0;
  loading: boolean = true;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  sortOptions = [
    { name: 'Alphabetical', value: '' },
    { name: 'Price: Low-High', value: 'ASC' },
    { name: 'Price: High-Low', value: 'DESC' },
  ];
  private subscriptions: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private carBrandService: CarbrandService,
    private productService: ProductService,
    private imageService: ImageService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    const paramSub = this.route.params.subscribe((params) => {
      this.brandId = params['id'];
      if (this.brandId) {
        this.loadCarBrand();
      } else {
        this.router.navigate(['/']);
      }
    });

    this.subscriptions.push(paramSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  loadCarBrand(): void {
    const brandSub = this.carBrandService
      .getCarBrandById(this.brandId)
      .subscribe({
        next: (brand) => {
          this.carBrand = brand;
          this.loadProductsForBrand();
        },
        error: (err) => {
          console.error('Failed to load car brand', err);
          this.router.navigate(['/']);
        },
      });

    this.subscriptions.push(brandSub);
  }

  loadProductsForBrand(): void {
    if (!this.carBrand) return;

    this.loading = true;

    const productsSub = this.productService
      .getProductsByBrandName(
        this.carBrand.name,
        this.shopParams.sortOrder,
        this.shopParams.searchTerm,
        this.shopParams.pageNumber,
        this.shopParams.pageSize
      )
      .subscribe({
        next: (response: Pagination<Product>) => {
          this.products = response;
          this.totalItems = response.count;
          this.loading = false;
        },
        error: (err: any) => {
          console.error('Failed to load products for brand', err);
          this.loading = false;
        },
      });

    this.subscriptions.push(productsSub);
  }

  handlePageEvent(event: PageEvent): void {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.loadProductsForBrand();
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.loadProductsForBrand();
    }
  }

  getBrandLogo(): string {
    return this.carBrand && this.carBrand.logoPath
      ? this.imageService.getImageUrl(this.carBrand.logoPath)
      : '/images/no-image.jpeg';
  }

  addToCart(productId: string, event: Event): void {
    event.stopPropagation();
    this.cartService.addItemToCart(productId);
  }
}
