import { Component, OnInit, OnDestroy } from '@angular/core';
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
import { BreakpointObserver } from '@angular/cdk/layout';

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
  private breakpointSubscription?: Subscription;
  private readonly autoSlideInterval = 10000;

  activeFilter: 'popular' | 'new' | 'discount' = 'discount';
  loading = false;

  constructor(
    private readonly productService: ProductService,
    private readonly imageService: ImageService,
    private readonly breakpointObserver: BreakpointObserver
  ) {}

  ngOnInit(): void {
    this.setupResponsiveLayout();
    this.loadProducts();
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    this.autoSlideSubscription?.unsubscribe();
    this.breakpointSubscription?.unsubscribe();
  }

  setupResponsiveLayout(): void {
    this.breakpointSubscription = this.breakpointObserver
      .observe([
        '(max-width: 479px)',
        '(min-width: 480px) and (max-width: 639px)',
        '(min-width: 640px) and (max-width: 767px)',
        '(min-width: 768px) and (max-width: 1023px)',
        '(min-width: 1024px)',
      ])
      .subscribe((result) => {
        let productsPerGroup = 5;

        if (result.breakpoints['(max-width: 479px)']) {
          productsPerGroup = 1;
        } else if (
          result.breakpoints['(min-width: 480px) and (max-width: 639px)']
        ) {
          productsPerGroup = 2;
        } else if (
          result.breakpoints['(min-width: 640px) and (max-width: 767px)']
        ) {
          productsPerGroup = 3;
        } else if (
          result.breakpoints['(min-width: 768px) and (max-width: 1023px)']
        ) {
          productsPerGroup = 4;
        }

        if (this.products.length > 0) {
          this.groupProducts(productsPerGroup);
          setTimeout(() => {
            this.currentSlide = 0;
          }, 0);
        }
      });
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
        const currentBreakpoint = this.determineCurrentBreakpoint();
        this.groupProducts(currentBreakpoint);
        this.currentSlide = 0;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loading = false;
      },
    });
  }

  private determineCurrentBreakpoint(): number {
    const width = window.innerWidth;
    if (width < 480) return 1;
    if (width < 640) return 2;
    if (width < 768) return 3;
    if (width < 1024) return 4;
    return 5;
  }

  private groupProducts(productsPerGroup: number): void {
    this.productGroups = [];

    for (let i = 0; i < this.products.length; i += productsPerGroup) {
      const group = this.products.slice(i, i + productsPerGroup);

      if (group.length > 0) {
        this.productGroups.push(group);
      }

      if (i + productsPerGroup >= this.products.length) {
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

  allGroupsEmpty(): boolean {
    if (!this.productGroups || this.productGroups.length === 0) {
      return true;
    }

    return this.productGroups.every((group) => !group || group.length === 0);
  }

  getCategoryDisplayName(): string {
    switch (this.activeFilter) {
      case 'popular':
        return 'Popular';
      case 'discount':
        return 'Discounted';
      case 'new':
        return 'New';
      default:
        return '';
    }
  }

  hasAlternativeCategory(): boolean {
    return true;
  }

  getAlternativeCategory(): 'popular' | 'new' | 'discount' {
    if (this.activeFilter === 'popular') {
      return 'new';
    } else if (this.activeFilter === 'new') {
      return 'discount';
    } else {
      return 'popular';
    }
  }

  getAlternativeCategoryName(): string {
    const alternative = this.getAlternativeCategory();
    switch (alternative) {
      case 'popular':
        return 'Popular';
      case 'discount':
        return 'Discounted';
      case 'new':
        return 'New';
      default:
        return '';
    }
  }
}
