import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { CategoryService } from './category.service';
import { HierarchicalCategory } from '../../shared/models/category';
import { PaginationParams } from '../../shared/models/paginationParams';

@Injectable({
  providedIn: 'root',
})
export class LayoutService {
  private readonly categoryService = inject(CategoryService);

  private cachedHeaderCategories: Observable<HierarchicalCategory[]> | null =
    null;

  getHeaderCategories(): Observable<HierarchicalCategory[]> {
    if (!this.cachedHeaderCategories) {
      const params = new PaginationParams();
      params.pageNumber = 1;
      params.pageSize = 50;

      this.cachedHeaderCategories = this.categoryService
        .getHierarchicalCategories(params)
        .pipe(
          shareReplay({
            bufferSize: 1,
            refCount: false,
            windowTime: 60 * 60 * 1000,
          })
        );
    }

    return this.cachedHeaderCategories;
  }

  clearCache(): void {
    this.cachedHeaderCategories = null;
  }
}
