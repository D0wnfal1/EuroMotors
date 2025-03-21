import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ProductImage } from '../../shared/models/productImage';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  getImageUrl(imagePath: string): string {
    return `${this.baseUrl.replace('/api', '')}${imagePath}`;
  }

  getProductImages(productId: string): Observable<ProductImage[]> {
    return this.http.get<ProductImage[]>(
      `${this.baseUrl}/productImages/${productId}/product`
    );
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
