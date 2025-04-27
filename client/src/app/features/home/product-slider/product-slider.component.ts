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
  private readonly productsPerGroup = 5;
  private readonly autoSlideInterval = 10000;

  activeFilter: 'popular' | 'new' | 'discount' = 'discount';
  loading = false;

  constructor(
    private productService: ProductService,
    private imageService: ImageService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    this.autoSlideSubscription?.unsubscribe();
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
      this.productGroups.push(
        this.products.slice(i, i + this.productsPerGroup)
      );
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
