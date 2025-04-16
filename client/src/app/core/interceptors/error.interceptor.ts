import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 400) {
        const isJson = err.headers
          ?.get('content-type')
          ?.includes('application/json');

        let errorBody;

        if (isJson) {
          errorBody = err.error;
        } else if (typeof err.error === 'string') {
          errorBody = { description: err.error };
        } else {
          errorBody = { description: 'Bad Request' };
        }

        const errorDescription =
          errorBody.description || 'Something went wrong';

        snackbar.error(`${errorDescription}`);
      } else if (err.status === 401) {
        snackbar.error(err.error?.description || 'Unauthorized');
      } else if (err.status === 403) {
        snackbar.error('Forbidden');
      } else if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }

      return throwError(() => err);
    })
  );
};
