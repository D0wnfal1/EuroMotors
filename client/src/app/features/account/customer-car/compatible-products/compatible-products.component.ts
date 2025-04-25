import {
  Component,
  Input,
  OnChanges,
  OnInit,
  OnDestroy,
  SimpleChanges,
  inject,
  HostListener,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Product } from '../../../../shared/models/product';
import { ProductService } from '../../../../core/services/product.service';
import { ShopParams } from '../../../../shared/models/shopParams';
import { ProductItemComponent } from '../../../shop/product-item/product-item.component';
import { Subject, Subscription, interval, takeUntil } from 'rxjs';

@Component({
  selector: 'app-compatible-products',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, ProductItemComponent],
  templateUrl: './compatible-products.component.html',
  styleUrls: ['./compatible-products.component.scss'],
})
export class CompatibleProductsComponent
  implements OnChanges, OnInit, OnDestroy
{
  @Input() carModelId?: string;

  private productService = inject(ProductService);
  private destroy$ = new Subject<void>();
  private autoplaySubscription?: Subscription;

  compatibleProducts: Product[] = [];
  visibleProducts: Product[] = [];
  productsPerPage = 3;
  currentSlide = 0;
  maxSlide = 0;
  autoplayInterval = 5000;

  ngOnInit(): void {
    this.adjustProductsPerPage(window.innerWidth);
    this.startAutoplay();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.stopAutoplay();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['carModelId'] && this.carModelId) {
      this.loadCompatibleProducts();
    }
  }

  startAutoplay(): void {
    this.stopAutoplay();

    this.autoplaySubscription = interval(this.autoplayInterval)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.nextSlide();
      });
  }

  stopAutoplay(): void {
    if (this.autoplaySubscription) {
      this.autoplaySubscription.unsubscribe();
      this.autoplaySubscription = undefined;
    }
  }

  pauseAutoplay(): void {
    this.stopAutoplay();
  }

  resumeAutoplay(): void {
    this.startAutoplay();
  }

  goToSlide(slideIndex: number): void {
    if (slideIndex >= 0 && slideIndex <= this.maxSlide) {
      this.currentSlide = slideIndex;
      this.updateVisibleProducts();

      if (this.autoplaySubscription) {
        this.stopAutoplay();
        this.startAutoplay();
      }
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any): void {
    this.adjustProductsPerPage(event.target.innerWidth);
    this.updateVisibleProducts();
  }

  adjustProductsPerPage(windowWidth: number): void {
    if (windowWidth < 640) {
      this.productsPerPage = 1;
    } else if (windowWidth < 768) {
      this.productsPerPage = 2;
    } else {
      this.productsPerPage = 3;
    }

    if (this.compatibleProducts.length > 0) {
      this.maxSlide = Math.max(
        0,
        Math.ceil(this.compatibleProducts.length / this.productsPerPage) - 1
      );

      if (this.currentSlide > this.maxSlide) {
        this.currentSlide = this.maxSlide;
      }
    }
  }

  loadCompatibleProducts(): void {
    if (!this.carModelId) return;

    const shopParams: ShopParams = {
      categoryIds: [],
      carModelIds: [this.carModelId],
      pageSize: 12,
      pageNumber: 1,
      sortOrder: '',
      searchTerm: '',
    };

    this.productService.getProducts(shopParams).subscribe({
      next: (response) => {
        this.compatibleProducts = response.data;
        this.maxSlide = Math.max(
          0,
          Math.ceil(this.compatibleProducts.length / this.productsPerPage) - 1
        );
        this.updateVisibleProducts();
      },
      error: (error) =>
        console.error('Error loading compatible products', error),
    });
  }

  updateVisibleProducts(): void {
    const startIndex = this.currentSlide * this.productsPerPage;
    this.visibleProducts = this.compatibleProducts.slice(
      startIndex,
      startIndex + this.productsPerPage
    );
  }

  nextSlide(): void {
    if (this.currentSlide < this.maxSlide) {
      this.currentSlide++;
    } else {
      this.currentSlide = 0;
    }
    this.updateVisibleProducts();
  }

  previousSlide(): void {
    if (this.currentSlide > 0) {
      this.currentSlide--;
    } else {
      this.currentSlide = this.maxSlide;
    }
    this.updateVisibleProducts();
  }

  trackByProductId(index: number, item: Product): string {
    return item.id;
  }
}
