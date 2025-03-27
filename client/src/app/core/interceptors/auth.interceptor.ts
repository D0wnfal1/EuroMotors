import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AccountService } from '../services/account.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        return accountService.refreshToken().pipe(
          switchMap(() => {
            return next(req.clone({ withCredentials: true }));
          })
        );
      }
      return throwError(() => error);
    })
  );
};
