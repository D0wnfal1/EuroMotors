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
        let errorBody;

        try {
          errorBody =
            typeof err.error === 'string' ? JSON.parse(err.error) : err.error;
        } catch (e) {
          console.error('Error parsing response:', e);
          snackbar.error('Bad Request');
          return throwError(() => err);
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
