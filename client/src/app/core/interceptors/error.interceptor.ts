import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

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

        console.error('Error 400:', errorDescription);
      } else if (err.status === 401) {
        console.error('Unauthorized');
      } else if (err.status === 403) {
        console.error('Forbidden');
      } else if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }

      return throwError(() => err);
    })
  );
};
