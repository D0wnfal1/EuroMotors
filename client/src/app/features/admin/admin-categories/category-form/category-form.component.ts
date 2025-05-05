import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../shared/models/category';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import {
  MatChipEditedEvent,
  MatChipInputEvent,
  MatChipsModule,
} from '@angular/material/chips';
import { MatIcon } from '@angular/material/icon';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { SnackbarService } from '../../../../core/services/snackbar.service';

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
    MatChipsModule,
    MatIcon,
  ],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss',
})
export class CategoryFormComponent implements OnInit {
  categoryForm: FormGroup = new FormGroup({});
  isEditMode: boolean = false;
  categoryId: string | null = null;
  allCategories: Category[] = [];
  imageInvalid: boolean = false;
  selectedImage: File | null = null;
  isParentCategorySelected: boolean = false;
  childCategoryNames: string[] = [];
  readonly separatorKeysCodes = [ENTER, COMMA] as const;
  addOnBlur = true;

  constructor(
    private readonly fb: FormBuilder,
    private readonly categoryService: CategoryService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly snackbar: SnackbarService
  ) {}
  ngOnInit(): void {
    this.initializeForm();

    this.categoryService.getCategories();
    this.categoryService.categories$.subscribe((data) => {
      this.allCategories = data;
    });

    this.categoryId = this.activatedRoute.snapshot.paramMap.get('id');
    if (this.categoryId) {
      this.isEditMode = true;
      this.loadCategoryData();
    }

    this.categoryForm.get('parentCategoryId')?.valueChanges.subscribe(() => {
      this.onParentCategoryChange();
    });
  }

  initializeForm() {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      parentCategoryId: [null],
      childCategoryNames: [[]],
    });
  }

  loadCategoryData() {
    if (this.categoryId) {
      this.categoryService.getCategoryById(this.categoryId).subscribe({
        next: (category) => {
          this.categoryForm.patchValue({
            name: category.name,
            parentCategoryId: category.parentCategoryId,
          });
        },
        error: (error) => {
          console.error('Error loading category data:', error);
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

  onParentCategoryChange(): void {
    this.isParentCategorySelected =
      !!this.categoryForm.get('parentCategoryId')?.value;

    if (this.isParentCategorySelected) {
      this.childCategoryNames = [];
    }
  }

  trackByName(index: number, item: string): string {
    return item;
  }

  addChildCategory(event: MatChipInputEvent): void {
    const input = event.chipInput.inputElement;
    const value = event.value;

    if ((value || '').trim()) {
      this.childCategoryNames.push(value.trim());
    }

    if (input) {
      input.value = '';
    }
  }

  removeChildCategory(name: string): void {
    const index = this.childCategoryNames.indexOf(name);
    if (index >= 0) {
      this.childCategoryNames.splice(index, 1);
    }
  }

  editChildCategory(name: string, event: MatChipEditedEvent): void {
    const value = event.value.trim();

    if (!value) {
      this.removeChildCategory(name);
      return;
    }

    const index = this.childCategoryNames.indexOf(name);
    if (index >= 0) {
      this.childCategoryNames[index] = value;
    }
  }

  onSubmit() {
    if (this.categoryForm.invalid) {
      this.snackbar.error('Please fix the errors in the form');
      return;
    }

    const formData = new FormData();
    formData.append('name', this.categoryForm.get('name')?.value);

    const parentCategoryId = this.categoryForm.get('parentCategoryId')?.value;
    if (parentCategoryId) {
      formData.append('parentCategoryId', parentCategoryId);
    }

    this.childCategoryNames.forEach((name) => {
      formData.append('subcategoryNames', name);
    });

    if (this.selectedImage) {
      formData.append('image', this.selectedImage, this.selectedImage.name);
    }

    if (this.isEditMode && this.categoryId) {
      this.categoryService.updateCategory(this.categoryId, formData).subscribe({
        next: () => {
          this.categoryService.clearCache();
          this.snackbar.success('Category updated successfully');
          this.router.navigate(['/admin/categories']);
        },
        error: (error) => {
          this.snackbar.error('Failed to update category');
          console.error('Error updating category:', error);
        },
      });
    } else {
      this.categoryService.createCategory(formData).subscribe({
        next: () => {
          this.categoryService.clearCache();
          this.snackbar.success('Category created successfully');
          this.router.navigate(['/admin/categories']);
        },
        error: (error) => {
          this.snackbar.error('Failed to create category');
          console.error('Error creating category:', error);
        },
      });
    }
  }
}
