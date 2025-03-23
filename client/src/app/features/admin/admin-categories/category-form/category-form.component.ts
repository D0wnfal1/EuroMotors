import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../shared/models/category';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-category-form',
  imports: [
    ReactiveFormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    RouterLink,
  ],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss',
})
export class CategoryFormComponent implements OnInit {
  categoryForm: FormGroup = new FormGroup({});
  isEditMode: boolean = false;
  categoryId: string | null = null;
  imageInvalid: boolean = false;
  selectedImage: File | null = null;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private snackBar: MatSnackBar,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();

    this.categoryId = this.activatedRoute.snapshot.paramMap.get('id');
    if (this.categoryId) {
      this.isEditMode = true;
      this.loadCategoryData();
    }
  }

  initializeForm() {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  loadCategoryData() {
    if (this.categoryId) {
      this.categoryService.getCategoryById(this.categoryId).subscribe({
        next: (category) => {
          this.categoryForm.patchValue({
            name: category.name,
          });
        },
        error: (error) => {
          this.snackBar.open('Failed to load category data', 'Close', {
            duration: 3000,
            panelClass: ['snack-error'],
          });
        },
      });
    }
  }

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      this.selectedImage = file;
      this.imageInvalid = false;
    } else {
      this.selectedImage = null;
      this.imageInvalid = true;
    }
  }

  onSubmit() {
    if (this.categoryForm.invalid) {
      return;
    }

    const formData = new FormData();
    formData.append('name', this.categoryForm.get('name')?.value);

    if (this.selectedImage) {
      formData.append('image', this.selectedImage, this.selectedImage.name);
    }

    if (this.isEditMode && this.categoryId) {
      this.categoryService.updateCategory(this.categoryId, formData).subscribe({
        next: () => {
          this.snackBar.open('Category updated successfully!', 'Close', {
            duration: 3000,
            panelClass: ['snack-success'],
          });
          this.router.navigate(['/admin/categories']);
        },
        error: () => {
          this.snackBar.open('Failed to update category', 'Close', {
            duration: 3000,
            panelClass: ['snack-error'],
          });
        },
      });
    } else {
      this.categoryService.createCategory(formData).subscribe({
        next: (newCategoryId) => {
          this.snackBar.open('Category created successfully!', 'Close', {
            duration: 3000,
            panelClass: ['snack-success'],
          });
          this.router.navigate(['/admin/categories']);
        },
        error: () => {
          this.snackBar.open('Failed to create category', 'Close', {
            duration: 3000,
            panelClass: ['snack-error'],
          });
        },
      });
    }
  }
}
