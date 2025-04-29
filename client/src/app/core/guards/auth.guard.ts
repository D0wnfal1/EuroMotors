import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  const redirectToLogin = () => {
    router.navigate(['/account/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  };

  if (accountService.currentUser()) {
    return true;
  }

  return accountService.checkAuth().pipe(
    map((state) => {
      if (state.isAuthenticated && state.user) {
        return true;
      }
      return redirectToLogin();
    }),
    catchError(() => {
      return accountService.refreshToken().pipe(
        map(() => true),
        catchError(() => of(redirectToLogin()))
      );
    })
  );
};
