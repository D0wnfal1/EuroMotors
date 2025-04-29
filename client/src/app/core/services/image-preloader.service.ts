import { Injectable, inject } from '@angular/core';
import { ImageCacheService } from './image-cache.service';
import { ImageService } from './image.service';

@Injectable({
  providedIn: 'root',
})
export class ImagePreloaderService {
  private readonly imageCacheService = inject(ImageCacheService);
  private readonly imageService = inject(ImageService);

  private readonly loadingImages = new Set<string>();

  private imageQueue: string[] = [];

  private readonly maxConcurrentLoads = 4;

  private currentLoads = 0;

  queueImageForPreload(imageUrl: string): void {
    if (!imageUrl || this.loadingImages.has(imageUrl)) {
      return;
    }

    const fullUrl = this.getFullImageUrl(imageUrl);
    this.imageQueue.push(fullUrl);
    this.processQueue();
  }

  preloadImage(imageUrl: string): Promise<string> {
    if (!imageUrl) {
      return Promise.resolve('');
    }

    const fullUrl = this.getFullImageUrl(imageUrl);

    if (this.loadingImages.has(fullUrl)) {
      return new Promise<string>((resolve) => {
        const checkInterval = setInterval(() => {
          const cachedUrl = this.imageCacheService.getImage(fullUrl);
          if (cachedUrl !== fullUrl) {
            clearInterval(checkInterval);
            resolve(cachedUrl);
          }
        }, 100);
      });
    }

    this.loadingImages.add(fullUrl);
    this.currentLoads++;

    return this.imageCacheService
      .cacheImage(fullUrl)
      .then((cachedUrl) => {
        this.loadingImages.delete(fullUrl);
        this.currentLoads--;
        this.processQueue();
        return cachedUrl;
      })
      .catch((error) => {
        console.error('Failed to preload image:', fullUrl, error);
        this.loadingImages.delete(fullUrl);
        this.currentLoads--;
        this.processQueue();
        return fullUrl;
      });
  }

  preloadImages(imageUrls: string[]): void {
    if (!imageUrls || imageUrls.length === 0) {
      return;
    }

    const uniqueUrls = [...new Set(imageUrls)]
      .filter((url) => !!url)
      .map((url) => this.getFullImageUrl(url));

    this.imageQueue.push(...uniqueUrls);
    this.processQueue();
  }

  private processQueue(): void {
    if (
      this.imageQueue.length === 0 ||
      this.currentLoads >= this.maxConcurrentLoads
    ) {
      return;
    }

    const nextUrl = this.imageQueue.shift();
    if (!nextUrl || this.loadingImages.has(nextUrl)) {
      this.processQueue();
      return;
    }

    this.loadingImages.add(nextUrl);
    this.currentLoads++;

    this.imageCacheService
      .cacheImage(nextUrl)
      .then(() => {
        this.loadingImages.delete(nextUrl);
        this.currentLoads--;
        this.processQueue();
      })
      .catch((error) => {
        console.error('Failed to preload image from queue:', nextUrl, error);
        this.loadingImages.delete(nextUrl);
        this.currentLoads--;
        this.processQueue();
      });
  }

  private getFullImageUrl(url: string): string {
    if (!url) return '';

    if (url.startsWith('http') || url.startsWith('data:')) {
      return url;
    }

    return this.imageService.getImageUrl(url);
  }

  clearQueue(): void {
    this.imageQueue = [];
    this.loadingImages.clear();
    this.currentLoads = 0;
  }
}
