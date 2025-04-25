import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { catchError, of, switchMap } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.isAdmin()) {
    return true;
  } else {
    return accountService.refreshToken().pipe(
      switchMap((response) => {
        if (accountService.isAdmin()) {
          return of(true);
        } else {
          console.error('User is not an admin');
          router.navigateByUrl('/shop');
          return of(false);
        }
      }),
      catchError((error) => {
        console.error('Error refreshing token', error);
        router.navigateByUrl('/shop');
        return of(false);
      })
    );
  }
};
