import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { ShopParams } from '../../../shared/models/shopParams';
import { ImageService } from '../../../core/services/image.service';
import { interval, Subscription } from 'rxjs';
import { ProductItemComponent } from '../../shop/product-item/product-item.component';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-product-slider',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    ProductItemComponent,
    MatButtonToggleModule,
  ],
  templateUrl: './product-slider.component.html',
  styleUrls: ['./product-slider.component.scss'],
})
export class ProductSliderComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  productGroups: Product[][] = [];
  currentSlide = 0;
  private autoSlideSubscription?: Subscription;
  private productsPerGroup = 4;
  private readonly autoSlideInterval = 10000;
  private breakpointSubscription?: Subscription;

  activeFilter: 'popular' | 'new' | 'discount' = 'discount';
  loading = false;
  isXSmall = false;
  isSmall = false;
  isMedium = false;
  isLarge = false;

  constructor(
    private productService: ProductService,
    private imageService: ImageService,
    private breakpointObserver: BreakpointObserver
  ) {}

  ngOnInit(): void {
    this.setupResponsiveBreakpoints();
    this.loadProducts();
    this.startAutoSlide();
  }

  setupResponsiveBreakpoints(): void {
    this.breakpointSubscription = this.breakpointObserver
      .observe([
        Breakpoints.XSmall,
        Breakpoints.Small,
        Breakpoints.Medium,
        Breakpoints.Large,
        Breakpoints.XLarge,
      ])
      .subscribe((result) => {
        this.isXSmall = result.breakpoints[Breakpoints.XSmall];
        this.isSmall = result.breakpoints[Breakpoints.Small];
        this.isMedium = result.breakpoints[Breakpoints.Medium];
        this.isLarge = result.breakpoints[Breakpoints.Large];

        if (this.isXSmall) {
          this.productsPerGroup = 1;
        } else if (this.isSmall) {
          this.productsPerGroup = 2;
        } else if (this.isMedium) {
          this.productsPerGroup = 3;
        } else if (this.isLarge) {
          this.productsPerGroup = 5;
        } else {
          this.productsPerGroup = 5;
        }

        if (this.products.length > 0) {
          this.groupProducts();
          setTimeout(() => {
            this.currentSlide = 0;
          }, 0);
        }
      });
  }

  ngOnDestroy(): void {
    this.autoSlideSubscription?.unsubscribe();
    this.breakpointSubscription?.unsubscribe();
  }

  loadProducts(): void {
    if (this.loading) return;

    this.loading = true;
    const params = new ShopParams();
    params.pageSize = 25;
    params.pageNumber = 1;
    params.sortOrder = '';
    params.isPopular = false;
    params.isNew = false;
    params.isDiscounted = false;

    switch (this.activeFilter) {
      case 'popular':
        params.isPopular = true;
        break;
      case 'new':
        params.isNew = true;
        break;
      case 'discount':
        params.isDiscounted = true;
        params.sortOrder = '';
        break;
    }

    this.productService.getProducts(params).subscribe({
      next: (response) => {
        this.products = response.data;
        this.groupProducts();
        this.currentSlide = 0;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loading = false;
      },
    });
  }

  private groupProducts(): void {
    this.productGroups = [];

    for (let i = 0; i < this.products.length; i += this.productsPerGroup) {
      const group = this.products.slice(i, i + this.productsPerGroup);

      if (group.length > 0) {
        this.productGroups.push(group);
      }

      if (i + this.productsPerGroup >= this.products.length) {
        break;
      }
    }

    if (this.productGroups.length === 0) {
      this.productGroups = [[]];
    }
  }

  startAutoSlide(): void {
    this.autoSlideSubscription = interval(this.autoSlideInterval).subscribe(
      () => {
        if (!this.loading) {
          this.nextSlide();
        }
      }
    );
  }

  previousSlide(): void {
    this.currentSlide =
      this.currentSlide === 0
        ? this.productGroups.length - 1
        : this.currentSlide - 1;
  }

  nextSlide(): void {
    this.currentSlide =
      this.currentSlide === this.productGroups.length - 1
        ? 0
        : this.currentSlide + 1;
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
  }

  setFilter(filter: 'popular' | 'new' | 'discount'): void {
    if (this.activeFilter === filter) return;
    this.activeFilter = filter;
    this.loadProducts();
  }

  getImageUrl(path: string): string {
    return this.imageService.getImageUrl(path);
  }

  trackByProductId(index: number, item: Product): string {
    return item.id;
  }
}
