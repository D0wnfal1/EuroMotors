import {
  HttpRequest,
  HttpHandlerFn,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<boolean | null>(null);

export const authInterceptor = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const accountService = inject(AccountService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status === 401 &&
        !request.url.includes('auth/refresh') &&
        !request.url.includes('auth/login')
      ) {
        return handle401Error(request, next, accountService);
      }
      return throwError(() => error);
    })
  );
};

function handle401Error(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
  accountService: AccountService
): Observable<HttpEvent<unknown>> {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    return accountService.refreshToken().pipe(
      switchMap(() => {
        isRefreshing = false;
        refreshTokenSubject.next(true);
        return next(request);
      }),
      catchError((error) => {
        isRefreshing = false;
        refreshTokenSubject.next(false);
        if (error.status === 401) {
          accountService.logout().subscribe();
        }
        return throwError(() => error);
      })
    );
  }

  return refreshTokenSubject.pipe(
    filter((result) => result !== null),
    take(1),
    switchMap((result) => {
      if (result) {
        return next(request);
      }
      return throwError(() => new Error('Token refresh failed'));
    })
  );
}
