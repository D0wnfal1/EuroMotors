import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { ProductImage } from '../../shared/models/productImage';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  private readonly DEFAULT_IMAGE = '/images/no-image.jpeg';
  private readonly DEFAULT_BRAND_LOGO = '/images/no-image.jpeg';
  private readonly IMAGE_SIZES = {
    thumbnail: { width: 100, height: 100, quality: 70 },
    small: { width: 300, height: 300, quality: 80 },
    medium: { width: 600, height: 600, quality: 85 },
    large: { width: 1200, height: 1200, quality: 90 },
  };

  getImageUrl(imagePath: string): string {
    if (!imagePath) return this.DEFAULT_IMAGE;
    return `${this.baseUrl.replace('/api', '')}${imagePath}`;
  }

  getOptimizedImageUrl(
    imagePath: string,
    size: keyof typeof this.IMAGE_SIZES = 'medium'
  ): string {
    if (!imagePath) return this.DEFAULT_IMAGE;

    const baseUrl = this.baseUrl.replace('/api', '');
    const sizeParams = this.IMAGE_SIZES[size];

    const hasParams = imagePath.includes('?');
    const separator = hasParams ? '&' : '?';

    return `${baseUrl}${imagePath}${separator}w=${sizeParams.width}&h=${sizeParams.height}&q=${sizeParams.quality}`;
  }

  getBrandLogoUrl(logoPath: string): string {
    if (!logoPath) return this.DEFAULT_BRAND_LOGO;
    return this.getOptimizedImageUrl(logoPath, 'small');
  }

  getLowQualityImageUrl(imagePath: string): string {
    if (!imagePath) return this.DEFAULT_IMAGE;

    const baseUrl = this.baseUrl.replace('/api', '');
    const hasParams = imagePath.includes('?');
    const separator = hasParams ? '&' : '?';

    return `${baseUrl}${imagePath}${separator}w=20&h=20&q=20&blur=10`;
  }

  getImageDimensions(
    url: string
  ): Observable<{ width: number; height: number }> {
    return new Observable<{ width: number; height: number }>((observer) => {
      const img = new Image();
      img.onload = () => {
        observer.next({ width: img.naturalWidth, height: img.naturalHeight });
        observer.complete();
      };
      img.onerror = (err) => {
        observer.error('Error loading image: ' + err);
      };
      img.src = url;
    }).pipe(
      catchError(() => {
        console.error('Error getting image dimensions');
        return of({ width: 0, height: 0 });
      })
    );
  }

  uploadProductImage(
    productId: string,
    file: File
  ): Observable<{ id: string; imagePath: string }> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('productId', productId);
    return this.http.post<{ id: string; imagePath: string }>(
      `${this.baseUrl}/productImages/upload`,
      formData,
      {
        withCredentials: true,
      }
    );
  }

  updateProductImage(image: ProductImage): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}/productImages/${image.productImageId}`,
      image,
      {
        withCredentials: true,
      }
    );
  }

  deleteProductImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/productImages/${id}`, {
      withCredentials: true,
    });
  }
}
