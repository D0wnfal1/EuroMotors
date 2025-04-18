import { Component, OnInit, ViewChild } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
  FormArray,
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
import { CategoryService } from '../../../../core/services/category.service';
import { CarmodelService } from '../../../../core/services/carmodel.service';
import { MatIcon } from '@angular/material/icon';
import {
  MatExpansionModule,
  MatExpansionPanel,
} from '@angular/material/expansion';
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
    MatIcon,
    MatExpansionModule,
  ],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit {
  @ViewChild(MatExpansionPanel) specPanel!: MatExpansionPanel;
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
    private categoryService: CategoryService,
    private carModelService: CarmodelService,
    private imageService: ImageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.productId;

    this.productForm = this.fb.group({
      name: ['', Validators.required],
      specifications: this.fb.array([], Validators.required),
      vendorCode: ['', Validators.required],
      categoryId: [null, Validators.required],
      carModelId: [null, Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.min(0)]],
      stock: [0, [Validators.required, Validators.min(0)]],
    });

    this.categoryService.categories$.subscribe(
      (categories) => (this.categories = categories)
    );
    this.carModelService.carModels$.subscribe(
      (carModels) => (this.carModels = carModels)
    );

    this.categoryService.getCategories();
    this.carModelService.getCarModels({ pageNumber: 1, pageSize: 0 });

    if (this.isEditMode) {
      this.productService
        .getProductById(this.productId!)
        .subscribe((product: Product) => {
          this.specifications.clear();
          product.specifications.forEach((spec) => {
            this.specifications.push(
              this.fb.group({
                specificationName: [
                  spec.specificationName,
                  Validators.required,
                ],
                specificationValue: [
                  spec.specificationValue,
                  Validators.required,
                ],
              })
            );
          });
          this.productForm.patchValue({
            name: product.name,
            vendorCode: product.vendorCode,
            categoryId: product.categoryId,
            carModelId: product.carModelId,
            price: product.price,
            discount: product.discount,
            stock: product.stock,
          });
          this.productImages = product.images;
        });
    }
  }

  get specifications(): FormArray {
    return this.productForm.get('specifications') as FormArray;
  }

  private createSpecification(): FormGroup {
    return this.fb.group({
      specificationName: ['', Validators.required],
      specificationValue: ['', Validators.required],
    });
  }

  addSpecification() {
    this.specifications.push(this.createSpecification());
  }

  removeSpecification(index: number) {
    if (this.specifications.length > 1) {
      this.specifications.removeAt(index);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
      this.selectedImages = [];
      for (const file of this.selectedFiles) {
        const reader = new FileReader();
        reader.onload = () => {
          this.selectedImages.push(reader.result as string | ArrayBuffer);
        };
        reader.readAsDataURL(file);
      }
    }
  }

  deleteImage(imageId: string, event: Event) {
    event.preventDefault();
    event.stopPropagation();

    this.imageService.deleteProductImage(imageId).subscribe({
      next: () => {
        this.productImages = this.productImages.filter(
          (img) => img.productImageId !== imageId
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
