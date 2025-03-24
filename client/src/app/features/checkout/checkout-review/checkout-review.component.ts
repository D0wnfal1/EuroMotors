import { Component, Input, SimpleChanges, inject } from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { Product } from '../../../shared/models/product';
import { ProductImage } from '../../../shared/models/productImage';
import { CurrencyPipe, NgFor, NgIf, TitleCasePipe } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';
import { PaymentPipe } from '../../../shared/pipes/payment.pipe';

@Component({
  selector: 'app-checkout-review',
  imports: [NgIf, NgFor, CurrencyPipe, PaymentPipe, TitleCasePipe],
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
  @Input() paymentMethod!: number;

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
        });
    });
  }

  getProductImages(productId: string) {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.images : 'Product not found';
  }

  getImageUrl(image: string | ProductImage): string {
    if (typeof image === 'string') {
      return this.imageService.getImageUrl(image);
    } else {
      return this.imageService.getImageUrl(image.path);
    }
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
