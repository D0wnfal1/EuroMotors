import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product, ProductResponse } from '../../shared/models/product';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);
  private readonly productCache = new Map<
    string,
    Observable<Pagination<Product>>
  >();
  private readonly productDetailCache = new Map<string, Observable<Product>>();

  getProducts(shopParams: ShopParams): Observable<Pagination<Product>>;
  getProducts(
    shopParams: ShopParams,
    responseType: 'product'
  ): Observable<Pagination<Product>>;
  getProducts(
    shopParams: ShopParams,
    responseType: 'suggestion'
  ): Observable<Pagination<ProductResponse>>;
  getProducts(
    shopParams: ShopParams,
    responseType: 'product' | 'suggestion' = 'product'
  ): Observable<Pagination<Product> | Pagination<ProductResponse>> {
    if (responseType === 'product') {
      const cacheKey = this.getCacheKey(shopParams);
      if (this.productCache.has(cacheKey)) {
        return this.productCache.get(cacheKey)!;
      }
    }

    let params = new HttpParams();

    if (shopParams.categoryIds?.length) {
      shopParams.categoryIds.forEach((id) => {
        params = params.append('categoryIds', id);
      });
    }

    if (shopParams.carModelIds?.length) {
      shopParams.carModelIds.forEach((id) => {
        params = params.append('carModelIds', id);
      });
    }

    if (shopParams.sortOrder) {
      params = params.append('sortOrder', shopParams.sortOrder);
    }

    if (shopParams.searchTerm) {
      params = params.append('searchTerm', shopParams.searchTerm);
    }

    if (shopParams.isDiscounted !== undefined) {
      params = params.append(
        'isDiscounted',
        shopParams.isDiscounted.toString()
      );
    }

    if (shopParams.isNew !== undefined) {
      params = params.append('isNew', shopParams.isNew.toString());
    }

    if (shopParams.isPopular !== undefined) {
      params = params.append('isPopular', shopParams.isPopular.toString());
    }

    params = params.append('pageSize', shopParams.pageSize.toString());
    params = params.append('pageNumber', shopParams.pageNumber.toString());

    if (responseType === 'suggestion') {
      return this.http.get<Pagination<ProductResponse>>(
        this.baseUrl + '/products',
        { params }
      );
    } else {
      const products$ = this.http
        .get<Pagination<Product>>(this.baseUrl + '/products', {
          params,
        })
        .pipe(shareReplay(1));

      this.productCache.set(this.getCacheKey(shopParams), products$);
      return products$;
    }
  }

  getProductById(id: string): Observable<Product> {
    if (this.productDetailCache.has(id)) {
      return this.productDetailCache.get(id)!;
    }

    const product$ = this.http
      .get<Product>(`${this.baseUrl}/products/${id}`)
      .pipe(shareReplay(1));

    this.productDetailCache.set(id, product$);

    return product$;
  }

  private getCacheKey(params: ShopParams): string {
    const categoryKey = params.categoryIds?.join(',') || 'none';
    const modelKey = params.carModelIds?.join(',') || 'none';
    const sortKey = params.sortOrder || 'default';
    const searchKey = params.searchTerm || 'none';
    const discountKey = params.isDiscounted?.toString() ?? 'all';
    const newKey = params.isNew?.toString() ?? 'all';
    const popularKey = params.isPopular?.toString() ?? 'all';
    const paginationKey = `${params.pageNumber}_${params.pageSize}`;

    return `${categoryKey}_${modelKey}_${sortKey}_${searchKey}_${discountKey}_${newKey}_${popularKey}_${paginationKey}`;
  }

  clearCache() {
    this.productCache.clear();
    this.productDetailCache.clear();
  }

  createProduct(product: Product): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/products`, product, {
      withCredentials: true,
    });
  }

  copyProduct(id: string): Observable<string> {
    return this.http.post<string>(
      `${this.baseUrl}/products/${id}/copy`,
      {},
      {
        withCredentials: true,
      }
    );
  }

  updateProduct(id: string, product: Product): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/products/${id}`, product, {
      withCredentials: true,
    });
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/products/${id}`, {
      withCredentials: true,
    });
  }

  setProductAvailability(id: string, isAvailable: boolean): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/products/${id}`,
      { isAvailable },
      { withCredentials: true }
    );
  }

  updateProductCarModels(
    productId: string,
    carModelIds: string[]
  ): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}/products/${productId}/car-models`,
      carModelIds,
      { withCredentials: true }
    );
  }

  getProductsByBrandName(
    brandName: string,
    sortOrder?: string,
    searchTerm?: string,
    pageNumber: number = 1,
    pageSize: number = 10
  ) {
    let params = new HttpParams();

    if (sortOrder) {
      params = params.append('sortOrder', sortOrder);
    }

    if (searchTerm) {
      params = params.append('searchTerm', searchTerm);
    }

    params = params.append('brandName', brandName);
    params = params.append('pageSize', pageSize.toString());
    params = params.append('pageNumber', pageNumber.toString());

    return this.http.get<Pagination<Product>>(
      this.baseUrl + '/products/by-brand-name',
      {
        params,
      }
    );
  }

  getProductsByCategoryWithChildren(
    categoryId: string,
    sortOrder?: string,
    searchTerm?: string,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<Pagination<Product>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<Pagination<Product>>(
      `${this.baseUrl}/products/by-category/${categoryId}`,
      { params }
    );
  }
}
