import { Component, Input, SimpleChanges, inject } from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { Product } from '../../../shared/models/product';
import { ProductImage } from '../../../shared/models/productImage';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';

@Component({
  selector: 'app-checkout-review',
  imports: [NgIf, NgFor, CurrencyPipe],
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss',
})
export class CheckoutReviewComponent {
  cartService = inject(CartService);
  productService = inject(ProductService);
  orderService = inject(OrderService);
  imageService = inject(ImageService);

  products: Product[] = [];
  productImages: { [productId: string]: ProductImage } = {};

  @Input() deliveryMethod!: string;
  @Input() selectedWarehouse!: string;
  @Input() paymentMethod!: string;

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    const cartItems = this.cartService.cart()?.cartItems ?? [];
    cartItems.forEach((item) => {
      this.productService
        .getProductById(item.productId)
        .subscribe((product) => {
          this.products.push(product);
          this.loadProductImage(product.id);
        });
    });
  }

  loadProductImage(productId: string) {
    this.imageService.getProductImages(productId).subscribe((images) => {
      if (images.length > 0) {
        this.productImages[productId] = images[0];
      }
    });
  }

  getImageUrl(imagePath: string): string {
    return this.imageService.getImageUrl(imagePath);
  }

  getProductImage(productId: string) {
    return this.productImages[productId];
  }

  getProductName(productId: string) {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.name : 'Product not found';
  }

  getProductPrice(productId: string) {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.price : 0;
  }

  trackByProductId(index: number, item: any) {
    return item.productId;
  }
}
