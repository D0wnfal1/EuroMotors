import {
  EnvironmentProviders,
  makeEnvironmentProviders,
  inject,
} from '@angular/core';
import {
  HttpHandlerFn,
  HttpRequest,
  HttpResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';
import { HttpCacheService } from './http-cache.service';

export function provideHttpCache(): EnvironmentProviders {
  return makeEnvironmentProviders([HttpCacheService]);
}

export const httpCacheInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> => {
  if (req.method !== 'GET') {
    return next(req);
  }

  if (
    req.url.includes('/auth/') ||
    req.url.includes('/carts/') ||
    req.url.includes('/users/')
  ) {
    return next(req);
  }

  const cacheService = inject(HttpCacheService);
  const cachedResponse = cacheService.get(req.url);

  if (cachedResponse) {
    return of(cachedResponse);
  }

  return next(req).pipe(
    tap((event) => {
      if (event instanceof HttpResponse) {
        cacheService.set(req.url, event);
      }
    })
  );
};
