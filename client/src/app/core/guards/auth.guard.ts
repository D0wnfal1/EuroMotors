import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.currentUser()) {
    return of(true);
  } else {
    return accountService.getAuthState().pipe(
      map((auth) => {
        if (auth.isAuthenticated) {
          return true;
        } else {
          router.navigate(['/account/login'], {
            queryParams: { returnUrl: state.url },
          });
          return false;
        }
      }),
      catchError(() => {
        router.navigate(['/account/login'], {
          queryParams: { returnUrl: state.url },
        });
        return of(false);
      })
    );
  }
};
