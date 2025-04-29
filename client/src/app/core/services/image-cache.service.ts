import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ImageCacheService {
  private readonly cache: Map<string, string> = new Map<string, string>();
  private readonly preloadedImages: Map<string, HTMLImageElement> = new Map<
    string,
    HTMLImageElement
  >();

  async cacheImage(url: string): Promise<string> {
    if (this.cache.has(url)) {
      return this.cache.get(url)!;
    }

    try {
      const response = await fetch(url);
      const blob = await response.blob();

      return new Promise<string>((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
          const dataUrl = reader.result as string;
          this.cache.set(url, dataUrl);
          resolve(dataUrl);
        };
        reader.onerror = reject;
        reader.readAsDataURL(blob);
      });
    } catch (error) {
      console.error('Failed to cache image:', url, error);
      return url;
    }
  }

  getImage(url: string): string {
    return this.cache.get(url) ?? url;
  }

  preloadImages(urls: string[]): void {
    urls.forEach((url) => {
      if (!this.preloadedImages.has(url)) {
        const img = new Image();
        img.src = url;
        this.preloadedImages.set(url, img);

        img.onload = () => {
          this.cacheImage(url).catch((err) =>
            console.error('Error preloading image:', url, err)
          );
        };
      }
    });
  }

  clearCache(): void {
    this.cache.clear();
    this.preloadedImages.clear();
  }
}
