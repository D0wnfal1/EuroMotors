import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  const redirectToShop = () => {
    router.navigateByUrl('/shop');
    return false;
  };

  if (accountService.isAdmin()) {
    return true;
  }

  return accountService.checkAuth().pipe(
    map((state) => {
      if (state.isAuthenticated && state.user) {
        return accountService.isAdmin() || redirectToShop();
      }
      return redirectToShop();
    }),
    catchError(() => of(redirectToShop()))
  );
};
