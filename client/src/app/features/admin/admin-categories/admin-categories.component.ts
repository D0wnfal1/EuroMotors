import { Component, OnInit, inject } from '@angular/core';
import { MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category';
import { ShopParams } from '../../../shared/models/shopParams';
import { NgFor, CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { ImageService } from '../../../core/services/image.service';

@Component({
  selector: 'app-admin-categories',
  imports: [
    ReactiveFormsModule,
    MatPaginator,
    RouterLink,
    NgFor,
    MatButton,
    CommonModule,
    FormsModule,
  ],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.scss',
})
export class AdminCategoriesComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private imageService = inject(ImageService);
  categories: Category[] = [];
  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  count = 0;

  ngOnInit() {
    this.getCategories();
  }

  getCategories() {
    this.categoryService.getCategories(this.shopParams);
    this.categoryService.categories$.subscribe((response) => {
      this.categories = response;
    });

    this.categoryService.totalItems$.subscribe((count) => {
      this.totalItems = count;
    });
  }

  getCategoryName(categoryId: string): string {
    return this.categories.find((c) => c.id === categoryId)?.name || '—';
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getCategories();
  }

  getCarModelImage(imagePath?: string): string {
    return imagePath
      ? this.imageService.getImageUrl(imagePath)
      : '/images/no-image.jpeg';
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getCategories();
    }
  }

  deleteCategory(categoryId: string): void {
    this.categoryService.deleteCategory(categoryId).subscribe(() => {
      if (this.categories) {
        this.categories = this.categories.filter(
          (p: Category) => p.id !== categoryId
        );
      }
    });
  }
}
