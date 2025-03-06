import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { ProductImage } from '../../../shared/models/productImage';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    MatDivider
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  product?: Product;
  productImages: ProductImage[] = [];
  activeIndex: number = 0;

  nextImage() {
    if (this.product && this.productImages.length > 0 && this.activeIndex < (this.productImages.length - 1)) {
      this.activeIndex++;
    }
  }
  
  previousImage() {
    if (this.activeIndex > 0) {
      this.activeIndex--;
    }
  }
  
  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
  
    this.shopService.getProduct(id).subscribe({
      next: product => {
        this.product = product;
        this.shopService.getProductImages(id).subscribe({
          next: images => {
            this.productImages = images;
            if (this.productImages.length) {
              this.activeIndex = 0; 
            }
          },
          error: error => console.log('Failed to load product images', error)
        });
  
      },
      error: error => console.log(error)
    });
  }
}
