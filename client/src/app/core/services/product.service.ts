import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getProducts(shopParams: ShopParams) {
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

    params = params.append('pageSize', shopParams.pageSize.toString());
    params = params.append('pageNumber', shopParams.pageNumber.toString());

    return this.http.get<Pagination<Product>>(this.baseUrl + '/products', {
      params,
    });
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/products/${id}`);
  }

  createProduct(product: Product): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/products`, product, {
      withCredentials: true,
    });
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
}
