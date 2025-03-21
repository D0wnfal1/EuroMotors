import { Component, inject, Input, OnInit } from '@angular/core';
import { CartItem } from '../../../shared/models/cart';
import { CartService } from '../../../core/services/cart.service';
import { RouterLink } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe, NgIf } from '@angular/common';
import { Product } from '../../../shared/models/product';
import { ProductImage } from '../../../shared/models/productImage';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';

@Component({
  selector: 'app-cart-item',
  imports: [NgIf, RouterLink, MatButton, MatIcon, CurrencyPipe],
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss'],
})
export class CartItemComponent implements OnInit {
  @Input() item: CartItem | undefined;
  product: Product | undefined;
  productImage: ProductImage | undefined;

  cartService = inject(CartService);
  productService = inject(ProductService);
  imageService = inject(ImageService);

  getImageUrl(imagePath: string): string {
    return this.imageService.getImageUrl(imagePath);
  }

  ngOnInit() {
    if (this.item?.productId) {
      this.productService
        .getProductById(this.item.productId)
        .subscribe((product) => {
          this.product = product;
          this.getProductImage(product.id);
        });
    }
  }

  getProductImage(productId: string) {
    this.imageService.getProductImages(productId).subscribe((images) => {
      if (images.length > 0) {
        this.productImage = images[0];
      }
    });
  }

  incrementQuantity() {
    this.cartService.addItemToCart(this.item?.productId ?? '');
  }

  decrementQuantity() {
    if (this.item?.quantity && this.item.quantity > 1) {
      this.cartService.decrementItemQuantity(this.item?.productId ?? '', 1);
    } else {
      this.removeItemFromCart();
    }
  }

  removeItemFromCart() {
    this.cartService.removeItemFromCart(this.item?.productId ?? '');
  }
}
