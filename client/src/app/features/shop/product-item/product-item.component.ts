import { Component, Input, inject } from '@angular/core';
import { Product } from '../../../shared/models/product';
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
import { ProductImage } from '../../../shared/models/productImage';
import { MatTooltip } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { CallbackComponent } from '../../../layout/callback/callback.component';

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
export class ProductItemComponent {
  @Input() product?: Product;
  cartService = inject(CartService);
  imageService = inject(ImageService);
  private dialog = inject(MatDialog);

  openCallbackDialog() {
    this.dialog.open(CallbackComponent, {
      width: '400px',
      panelClass: 'callback-dialog',
      disableClose: false,
    });
  }
  getImageUrl(image: ProductImage): string {
    return this.imageService.getImageUrl(image.path);
  }

  getSpecificationsTooltip(): string {
    if (
      !this.product ||
      !this.product.specifications ||
      this.product.specifications.length === 0
    ) {
      return 'No specifications available';
    }

    return this.product.specifications
      .map((spec) => `${spec.specificationName}: ${spec.specificationValue}`)
      .join('\n');
  }
}
