import { Component, Input, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Product } from '../../../shared/models/product';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';
import { CartService } from '../../../core/services/cart.service';
import { Router } from '@angular/router';
import { ShopParams } from '../../../shared/models/shopParams';
import { ProductItemComponent } from '../product-item/product-item.component';

@Component({
  selector: 'app-related-products',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, ProductItemComponent],
  templateUrl: './related-products.component.html',
  styleUrl: './related-products.component.scss',
})
export class RelatedProductsComponent implements OnInit {
  @Input() categoryId?: string;
  @Input() currentProductId?: string;

  private productService = inject(ProductService);
  private imageService = inject(ImageService);
  private cartService = inject(CartService);
  private router = inject(Router);

  relatedProducts: Product[] = [];
  visibleProducts: Product[] = [];
  productsPerPage = 3;
  currentSlide = 0;
  maxSlide = 0;

  @HostListener('window:resize')
  onResize() {
    this.setProductsPerPage();
    this.maxSlide = Math.max(
      0,
      Math.ceil(this.relatedProducts.length / this.productsPerPage) - 1
    );

    if (this.currentSlide > this.maxSlide) {
      this.currentSlide = this.maxSlide;
    }

    this.updateVisibleProducts();
  }

  ngOnInit(): void {
    this.setProductsPerPage();
    this.loadRelatedProducts();
  }

  loadRelatedProducts(): void {
    if (!this.categoryId) return;

    const shopParams: ShopParams = {
      categoryIds: [this.categoryId],
      carModelIds: [],
      pageSize: 9,
      pageNumber: 1,
      sortOrder: 'nameAsc',
      searchTerm: '',
    };

    this.productService.getProducts(shopParams).subscribe({
      next: (response) => {
        this.relatedProducts = response.data.filter(
          (p) => p.id !== this.currentProductId
        );
        this.maxSlide = Math.max(
          0,
          Math.ceil(this.relatedProducts.length / this.productsPerPage) - 1
        );
        this.updateVisibleProducts();
      },
      error: (error) => console.error('Error loading related products', error),
    });
  }

  updateVisibleProducts(): void {
    const startIndex = this.currentSlide * this.productsPerPage;
    this.visibleProducts = this.relatedProducts.slice(
      startIndex,
      startIndex + this.productsPerPage
    );
  }

  nextSlide(): void {
    if (!this.relatedProducts.length) return;

    if (this.currentSlide < this.maxSlide) {
      this.currentSlide++;
    } else {
      this.currentSlide = 0;
    }
    this.updateVisibleProducts();
  }

  previousSlide(): void {
    if (!this.relatedProducts.length) return;

    if (this.currentSlide > 0) {
      this.currentSlide--;
    } else {
      this.currentSlide = this.maxSlide;
    }
    this.updateVisibleProducts();
  }

  getImageUrl(imagePath?: string): string {
    if (!imagePath) {
      return 'images/no-image.jpeg';
    }
    return this.imageService.getImageUrl(imagePath);
  }

  addToCart(product: Product): void {
    this.cartService.addItemToCart(product.id, 1);
  }

  navigateToProduct(productId: string): void {
    this.router.navigate(['/shop/product', productId]);
  }

  goToSlide(index: number): void {
    if (index >= 0 && index <= this.maxSlide) {
      this.currentSlide = index;
      this.updateVisibleProducts();
    }
  }

  setProductsPerPage(): void {
    const width = window.innerWidth;
    if (width < 640) {
      this.productsPerPage = 1;
    } else if (width < 1024) {
      this.productsPerPage = 2;
    } else {
      this.productsPerPage = 3;
    }
  }
}
