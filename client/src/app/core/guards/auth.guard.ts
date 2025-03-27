import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { of, switchMap, map, catchError } from 'rxjs';
import { AccountService } from '../services/account.service';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.currentUser()) {
    return of(true);
  } else {
    return accountService.getAuthState().pipe(
      switchMap((auth) => {
        if (auth.isAuthenticated) {
          return of(true);
        } else {
          return accountService.refreshToken().pipe(
            switchMap(() => {
              return accountService.getAuthState().pipe(
                map((authAfterRefresh) => {
                  if (authAfterRefresh.isAuthenticated) {
                    return true;
                  } else {
                    router.navigate(['/account/login'], {
                      queryParams: { returnUrl: state.url },
                    });
                    return false;
                  }
                })
              );
            }),
            catchError(() => {
              router.navigate(['/account/login'], {
                queryParams: { returnUrl: state.url },
              });
              return of(false);
            })
          );
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
