import { Injectable, inject } from '@angular/core';
import { CategoryService } from './category.service';
import { ProductService } from './product.service';
import { HierarchicalCategory } from '../../shared/models/category';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';
import { PaginationParams } from '../../shared/models/paginationParams';
import { map, Observable, shareReplay } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HomePageService {
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(ProductService);

  private cachedMainCategories: Observable<HierarchicalCategory[]> | null =
    null;
  private cachedPopularProducts: Observable<Product[]> | null = null;
  private cachedNewProducts: Observable<Product[]> | null = null;
  private cachedDiscountedProducts: Observable<Product[]> | null = null;

  getMainCategories(): Observable<HierarchicalCategory[]> {
    if (!this.cachedMainCategories) {
      const params = new PaginationParams();
      params.pageNumber = 1;
      params.pageSize = 10;

      this.cachedMainCategories = this.categoryService
        .getHierarchicalCategories(params)
        .pipe(
          shareReplay({ bufferSize: 1, refCount: false }) // Сохраняем кеш даже после отписки всех подписчиков
        );
    }

    return this.cachedMainCategories;
  }

  getPopularProducts(limit: number = 10): Observable<Product[]> {
    if (!this.cachedPopularProducts) {
      const params = new ShopParams();
      params.pageNumber = 1;
      params.pageSize = limit;
      params.isPopular = true;

      this.cachedPopularProducts = this.productService.getProducts(params).pipe(
        map((response) => response.data),
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }

    return this.cachedPopularProducts;
  }

  getNewProducts(limit: number = 10): Observable<Product[]> {
    if (!this.cachedNewProducts) {
      const params = new ShopParams();
      params.pageNumber = 1;
      params.pageSize = limit;
      params.isNew = true;

      this.cachedNewProducts = this.productService.getProducts(params).pipe(
        map((response) => response.data),
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }

    return this.cachedNewProducts;
  }

  getDiscountedProducts(limit: number = 10): Observable<Product[]> {
    if (!this.cachedDiscountedProducts) {
      const params = new ShopParams();
      params.pageNumber = 1;
      params.pageSize = limit;
      params.isDiscounted = true;

      this.cachedDiscountedProducts = this.productService
        .getProducts(params)
        .pipe(
          map((response) => response.data),
          shareReplay({ bufferSize: 1, refCount: false })
        );
    }

    return this.cachedDiscountedProducts;
  }

  clearCache(): void {
    this.cachedMainCategories = null;
    this.cachedPopularProducts = null;
    this.cachedNewProducts = null;
    this.cachedDiscountedProducts = null;
  }
}
