import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shopParams';
import { Product } from '../../shared/models/product';
import { ProductImage } from '../../shared/models/productImage';
import { Category } from '../../shared/models/category';
import { CarModel } from '../../shared/models/carModel';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);
  categories: Category[] = [];
  carModels: CarModel[] = [];

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();
  
    if (shopParams.categoryName.length > 0) {
      params = params.append('categoryName', shopParams.categoryName.join(','));
    }
  
    if (shopParams.carModelBrand.length > 0) {
      params = params.append('carModelBrand', shopParams.carModelBrand.join(','));
    }

    if (shopParams.carModelModel.length > 0) {
      params = params.append('carModelModel', shopParams.carModelModel.join(','));
    }

    if (shopParams.sortOrder) {
      params = params.append('sortOrder', shopParams.sortOrder);
    }
  
    if (shopParams.searchTerm) {
      params = params.append('searchTerm', shopParams.searchTerm);
    }
  
    params = params.append('pageSize', shopParams.pageSize.toString());
    params = params.append('pageNumber', shopParams.pageNumber.toString());
  
    return this.http.get<Pagination<Product>>(this.baseUrl + 'products/search', {params});
  }
  

  getProduct(id: string) {
    return this.http.get<Product>(`${this.baseUrl}products/${id}`);
  }

  getProductImages(productId: string) {
    return this.http.get<ProductImage[]>(`${this.baseUrl}productImages/${productId}/product`);
  }

  getCategories() {
    if (this.categories.length > 0) return;
    this.http.get<Category[]>(this.baseUrl + 'categories').subscribe({
      next: (response) => {
        this.categories = response;
      },
      error: (err) => console.error('Failed to load categories', err)
    });
}

getCarModels() {
    if (this.carModels.length > 0) return;
    this.http.get<CarModel[]>(this.baseUrl + 'carModels').subscribe({
      next: (response) => {
        this.carModels = response;
      },
      error: (err) => console.error('Failed to load car models', err)
    });
}

}
