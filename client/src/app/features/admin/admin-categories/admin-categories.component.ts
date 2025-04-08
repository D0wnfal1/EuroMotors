import { Component, OnInit, inject } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category';
import { ShopParams } from '../../../shared/models/shopParams';
import { NgFor, CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { ImageService } from '../../../core/services/image.service';
import { MatIcon } from '@angular/material/icon';
import { Observable } from 'rxjs';

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
    MatIcon,
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
  subcategoriesMap: { [parentId: string]: Category[] } = {};
  visibleSubcategories: { [categoryId: string]: boolean } = {};
  subcategoriesLoaded: { [categoryId: string]: boolean } = {};

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService
      .getCategories(this.shopParams)
      .subscribe((response) => {
        this.categories = response.data;
        this.totalItems = response.count;
      });
  }

  toggleSubcategoriesVisibility(categoryId: string): void {
    this.visibleSubcategories[categoryId] =
      !this.visibleSubcategories[categoryId];
    if (!this.subcategoriesLoaded[categoryId]) {
      this.getSubcategories(categoryId).subscribe((subcategories) => {
        this.subcategoriesMap[categoryId] = subcategories;
        this.subcategoriesLoaded[categoryId] = true;
      });
    }
  }

  getSubcategories(parentCategoryId: string): Observable<Category[]> {
    return this.categoryService.getSubcategories(parentCategoryId);
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.loadCategories();
  }

  getCarModelImage(imagePath?: string): string {
    return imagePath
      ? this.imageService.getImageUrl(imagePath)
      : '/images/no-image.jpeg';
  }

  deleteCategory(categoryId: string): void {
    this.categoryService.deleteCategory(categoryId).subscribe(() => {
      this.categories = this.categories.filter(
        (category) => category.id !== categoryId
      );
    });
  }
}
