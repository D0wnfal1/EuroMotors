import { Component, Input, OnInit, inject } from '@angular/core';
import { Product, ProductSpecification } from '../../../shared/models/product';
import {
  MatCard,
  MatCardActions,
  MatCardContent,
} from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe, NgIf } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { ImageService } from '../../../core/services/image.service';
import { ImagePreloaderService } from '../../../core/services/image-preloader.service';
import { ProductImage } from '../../../shared/models/productImage';
import { MatTooltip } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { CallbackComponent } from '../../../layout/callback/callback.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [
    MatCard,
    MatCardContent,
    MatCardActions,
    MatIcon,
    CurrencyPipe,
    MatButton,
    RouterLink,
    NgIf,
    MatTooltip,
  ],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.scss',
})
export class ProductItemComponent implements OnInit {
  @Input() product?: Product;
  readonly cartService = inject(CartService);
  private readonly imageService = inject(ImageService);
  private readonly imagePreloader = inject(ImagePreloaderService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  lowQualityImageUrl = '';
  optimizedImageUrl = '';
  isImageLoaded = false;

  ngOnInit(): void {
    if (this.product && this.product.images && this.product.images.length > 0) {
      this.lowQualityImageUrl = this.imageService.getLowQualityImageUrl(
        this.product.images[0].path
      );

      this.optimizedImageUrl = this.imageService.getOptimizedImageUrl(
        this.product.images[0].path,
        'medium'
      );

      this.imagePreloader.preloadImage(this.optimizedImageUrl).then(() => {
        this.isImageLoaded = true;
      });

      if (this.product.images.length > 1) {
        const additionalImages = this.product.images
          .slice(1)
          .map((img) =>
            this.imageService.getOptimizedImageUrl(img.path, 'thumbnail')
          );
        this.imagePreloader.preloadImages(additionalImages);
      }
    }
  }

  openCallbackDialog() {
    this.dialog.open(CallbackComponent, {
      width: '500px',
      data: {
        productId: this.product?.id,
        productName: this.product?.name,
      },
    });
  }

  addToCart(productId: string) {
    this.cartService.addItemToCart(productId);
    this.snackBar.open('Product added to cart', 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['success-snackbar'],
    });
  }

  getImageUrl(image: ProductImage): string {
    if (this.isImageLoaded && image.path === this.product?.images[0]?.path) {
      return this.optimizedImageUrl;
    } else if (
      !this.isImageLoaded &&
      image.path === this.product?.images[0]?.path
    ) {
      return (
        this.lowQualityImageUrl || this.imageService.getImageUrl(image.path)
      );
    } else {
      return this.imageService.getImageUrl(image.path);
    }
  }

  getSpecificationsTooltip(): string {
    if (!this.product) return '';

    if (
      !this.product.specifications ||
      this.product.specifications.length === 0
    ) {
      return 'No specifications available';
    }

    return this.product.specifications
      .map(
        (spec: ProductSpecification) =>
          `${spec.specificationName}: ${spec.specificationValue}`
      )
      .join('\n');
  }
}
