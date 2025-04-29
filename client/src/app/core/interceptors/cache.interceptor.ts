import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, share } from 'rxjs/operators';

interface CacheEntry {
  response: HttpResponse<any>;
  timestamp: number;
}

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  private readonly cache = new Map<string, CacheEntry>();
  private readonly cacheDuration = 5 * 60 * 1000;

  private readonly excludedUrls = [
    '/users/login',
    '/users/register',
    '/users/logout',
    '/orders',
    '/cart',
  ];

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    if (request.method !== 'GET' || this.isExcludedUrl(request.url)) {
      return next.handle(request);
    }

    const cacheKey = request.url + request.params.toString();
    const cached = this.cache.get(cacheKey);

    const now = Date.now();
    if (cached && now - cached.timestamp < this.cacheDuration) {
      return of(cached.response.clone());
    }

    return next.handle(request).pipe(
      tap((event) => {
        if (event instanceof HttpResponse) {
          this.cache.set(cacheKey, {
            response: event.clone(),
            timestamp: Date.now(),
          });
        }
      }),
      share()
    );
  }

  private isExcludedUrl(url: string): boolean {
    return this.excludedUrls.some((excludedUrl) => url.includes(excludedUrl));
  }

  clearCache(): void {
    this.cache.clear();
  }

  clearCacheItem(url: string): void {
    const keys = Array.from(this.cache.keys());
    keys.forEach((key) => {
      if (key.includes(url)) {
        this.cache.delete(key);
      }
    });
  }
}
