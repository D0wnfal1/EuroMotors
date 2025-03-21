import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CarModel } from '../../../../shared/models/carModel';
import { Category } from '../../../../shared/models/category';
import { Product } from '../../../../shared/models/product';
import { MatOption } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ProductImage } from '../../../../shared/models/productImage';
import { ImageService } from '../../../../core/services/image.service';

@Component({
  selector: 'app-product-form',
  imports: [
    ReactiveFormsModule,
    MatOption,
    ReactiveFormsModule,
    MatSelectModule,
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  categories: Category[] = [];
  carModels: CarModel[] = [];
  productImages: ProductImage[] = [];
  productId: string | null = null;
  isEditMode = false;
  selectedImages: (string | ArrayBuffer)[] = [];
  selectedFiles: File[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private imageService: ImageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.productId;

    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      vendorCode: ['', Validators.required],
      categoryId: [null, Validators.required],
      carModelId: [null, Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.min(0)]],
      stock: [0, [Validators.required, Validators.min(0)]],
    });

    this.productService.categories$.subscribe(
      (categories) => (this.categories = categories)
    );
    this.productService.carModels$.subscribe(
      (carModels) => (this.carModels = carModels)
    );

    this.productService.getCategories();
    this.productService.getCarModels();

    if (this.isEditMode) {
      this.productService
        .getProductById(this.productId!)
        .subscribe((product: Product) => {
          this.productForm.patchValue(product);
        });
      this.imageService.getProductImages(this.productId!).subscribe({
        next: (images) => (this.productImages = images),
        error: (err) => console.error('Error loading images', err),
      });
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
      this.selectedImages = []; // Очищаем предыдущие превью
      for (const file of this.selectedFiles) {
        const reader = new FileReader();
        reader.onload = () => {
          this.selectedImages.push(reader.result as string | ArrayBuffer);
        };
        reader.readAsDataURL(file);
      }
    }
  }

  deleteImage(imageId: string) {
    this.imageService.deleteProductImage(imageId).subscribe({
      next: () => {
        this.productImages = this.productImages.filter(
          (img) => img.id !== imageId
        );
      },
      error: (err) => console.error('Error deleting image', err),
    });
  }

  getImageUrl(imagePath: string): string {
    return this.imageService.getImageUrl(imagePath);
  }

  onSubmit() {
    if (this.productForm.valid) {
      if (this.isEditMode && this.productId) {
        this.productService
          .updateProduct(this.productId, this.productForm.value)
          .subscribe({
            next: () => {
              if (this.selectedFiles.length > 0) {
                let uploadCount = 0;
                for (const file of this.selectedFiles) {
                  if (this.productId) {
                    this.imageService
                      .uploadProductImage(this.productId, file)
                      .subscribe({
                        next: () => {
                          uploadCount++;
                          if (uploadCount === this.selectedFiles.length) {
                            this.router.navigate(['/admin/products']);
                          }
                        },
                        error: (err) =>
                          console.error('Error uploading image', err),
                      });
                  }
                }
              } else {
                this.router.navigate(['/admin/products']);
              }
            },
            error: (err) => console.error('Error updating product', err),
          });
      } else {
        this.productService.createProduct(this.productForm.value).subscribe({
          next: (id) => {
            if (this.selectedFiles.length > 0) {
              let uploadCount = 0;
              for (const file of this.selectedFiles) {
                this.imageService.uploadProductImage(id, file).subscribe({
                  next: () => {
                    uploadCount++;
                    if (uploadCount === this.selectedFiles.length) {
                      this.router.navigate(['/admin/products']);
                    }
                  },
                  error: (err) => console.error('Error uploading image', err),
                });
              }
            } else {
              this.router.navigate(['/admin/products']);
            }
          },
          error: (err) => console.error('Error creating product', err),
        });
      }
    }
  }
}
