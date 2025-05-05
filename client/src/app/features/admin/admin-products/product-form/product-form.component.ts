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
import { CarbrandService } from '../../../../core/services/carbrand.service';
import { CarBrand } from '../../../../shared/models/carBrand';
import { SnackbarService } from '../../../../core/services/snackbar.service';

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
  carBrands: CarBrand[] = [];
  productImages: ProductImage[] = [];
  productId: string | null = null;
  isEditMode = false;
  selectedImages: (string | ArrayBuffer)[] = [];
  selectedFiles: File[] = [];
  isSaving = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly productService: ProductService,
    private readonly categoryService: CategoryService,
    private readonly carModelService: CarmodelService,
    private readonly carBrandService: CarbrandService,
    private readonly imageService: ImageService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly snackbar: SnackbarService
  ) {}

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.productId;

    this.productForm = this.fb.group({
      name: ['', Validators.required],
      specifications: this.fb.array([], Validators.required),
      vendorCode: ['', Validators.required],
      categoryId: [null, Validators.required],
      carModelIds: [[], Validators.required],
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

    this.carBrandService.carBrands$.subscribe(
      (carBrands) => (this.carBrands = carBrands)
    );

    this.categoryService.getCategories();
    this.carModelService.getCarModels({ pageNumber: 1, pageSize: 0 });
    this.carBrandService.getCarBrands({ pageNumber: 1, pageSize: 0 });

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
            carModelIds: product.carModelIds,
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

  getBrandNameById(brandId: string): string {
    const brand = this.carBrands.find((b) => b.id === brandId);
    return brand ? brand.name : '';
  }

  getCarBrandNameForModel(model: CarModel): string {
    return model.carBrand ? model.carBrand.name : '';
  }

  compareCarModelIds(c1: string, c2: string): boolean {
    return c1 === c2;
  }

  saveCarModels() {
    const carModelIds = this.productForm.get('carModelIds')?.value;

    if (!this.productId || !carModelIds?.length) {
      return;
    }

    const savingEl = document.createElement('span');
    savingEl.innerHTML =
      ' <i class="fas fa-spinner fa-spin"></i> Saving models...';
    const button = document.querySelector('button[type="button"]');
    button?.appendChild(savingEl);
    button?.setAttribute('disabled', 'true');

    this.productService
      .updateProductCarModels(this.productId, carModelIds)
      .subscribe({
        next: () => {
          button?.removeChild(savingEl);
          button?.removeAttribute('disabled');
          this.snackbar.success('Car models updated successfully');
        },
        error: (error) => {
          console.error('Error saving car models:', error);
          console.error('Request payload:', carModelIds);
          this.snackbar.error('Failed to update car models');

          button?.removeChild(savingEl);
          button?.removeAttribute('disabled');
        },
      });
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
                            this.productService.clearCache();
                            this.snackbar.success(
                              'Product updated successfully'
                            );
                            this.router.navigate(['/admin/products']);
                          }
                        },
                        error: (err) => {
                          console.error('Error uploading image', err);
                          this.snackbar.error('Failed to upload image');
                        },
                      });
                  }
                }
              } else {
                this.productService.clearCache();
                this.snackbar.success('Product updated successfully');
                this.router.navigate(['/admin/products']);
              }
            },
            error: (err) => {
              console.error('Error updating product', err);
              this.snackbar.error('Failed to update product');
            },
          });
      } else {
        this.productService.createProduct(this.productForm.value).subscribe({
          next: (id) => {
            const carModelIds = this.productForm.get('carModelIds')?.value;
            if (carModelIds?.length > 0) {
              this.productService
                .updateProductCarModels(id, carModelIds)
                .subscribe({
                  error: (err) => {
                    console.error(
                      'Error saving car models for new product',
                      err
                    );
                    this.snackbar.error(
                      'Failed to attach car models to product'
                    );
                  },
                });
            }

            if (this.selectedFiles.length > 0) {
              let uploadCount = 0;
              for (const file of this.selectedFiles) {
                this.imageService.uploadProductImage(id, file).subscribe({
                  next: () => {
                    uploadCount++;
                    if (uploadCount === this.selectedFiles.length) {
                      this.productService.clearCache();
                      this.snackbar.success('Product created successfully');
                      this.router.navigate(['/admin/products']);
                    }
                  },
                  error: (err) => {
                    console.error('Error uploading image', err);
                    this.snackbar.error('Failed to upload image');
                  },
                });
              }
            } else {
              this.productService.clearCache();
              this.snackbar.success('Product created successfully');
              this.router.navigate(['/admin/products']);
            }
          },
          error: (err) => {
            console.error('Error creating product', err);
            this.snackbar.error('Failed to create product');
          },
        });
      }
    } else {
      this.markFormGroupTouched(this.productForm);
      this.snackbar.error('Please fix the errors in the form');
    }
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      } else if (control instanceof FormArray) {
        for (const ctrl of control.controls) {
          if (ctrl instanceof FormGroup) {
            this.markFormGroupTouched(ctrl);
          } else {
            ctrl.markAsTouched();
          }
        }
      }
    });
  }
}
