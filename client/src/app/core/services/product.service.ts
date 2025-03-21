import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product';
import { CarModel } from '../../shared/models/carModel';
import { Category } from '../../shared/models/category';
import { Pagination } from '../../shared/models/pagination';
import { ProductImage } from '../../shared/models/productImage';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private categoriesSubject = new BehaviorSubject<Category[]>([]);
  categories$ = this.categoriesSubject.asObservable();
  private carModelsSubject = new BehaviorSubject<CarModel[]>([]);
  carModels$ = this.carModelsSubject.asObservable();

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

  getCategories() {
    this.http.get<Category[]>(this.baseUrl + '/categories').subscribe({
      next: (response) => {
        this.categoriesSubject.next(response);
      },
      error: (err) => console.error('Failed to load categories', err),
    });
  }

  getCarModels() {
    this.http.get<CarModel[]>(this.baseUrl + '/carModels').subscribe({
      next: (response) => {
        this.carModelsSubject.next(response);
      },
      error: (err) => console.error('Failed to load car models', err),
    });
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/products/${id}`);
  }

  createProduct(product: Product): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/products`, product);
  }

  updateProduct(id: string, product: Product): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/products/${id}`, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/products/${id}`);
  }

  getProductImages(productId: string): Observable<ProductImage[]> {
    return this.http.get<ProductImage[]>(
      `${this.baseUrl}/productImages/${productId}/product`
    );
  }

  createProductImage(image: ProductImage): Observable<ProductImage> {
    return this.http.post<ProductImage>(`${this.baseUrl}/productImages`, image);
  }

  uploadProductImage(
    productId: string,
    file: File
  ): Observable<{ id: string; imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('productId', productId);
    return this.http.post<{ id: string; imageUrl: string }>(
      `${this.baseUrl}/productImages/upload`,
      formData
    );
  }

  updateProductImage(image: ProductImage): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}/productImages/${image.id}`,
      image
    );
  }

  deleteProductImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/productImages/${id}`);
  }
}
