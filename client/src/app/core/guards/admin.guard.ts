import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';
import { catchError, of, switchMap } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const snack = inject(SnackbarService);

  if (accountService.isAdmin()) {
    return true;
  } else {
    return accountService.refreshToken().pipe(
      switchMap((response) => {
        if (accountService.isAdmin()) {
          return of(true);
        } else {
          snack.error('Error: you do not have admin rights');
          router.navigateByUrl('/shop');
          return of(false);
        }
      }),
      catchError((error) => {
        snack.error('Error: response status is 401');
        router.navigateByUrl('/shop');
        return of(false);
      })
    );
  }
};
