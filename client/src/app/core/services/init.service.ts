import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, switchMap } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  init() {
    return this.accountService.getAuthState().pipe(
      switchMap((auth) => {
        if (auth.isAuthenticated) {
          return forkJoin({
            cart: this.cartService.getCart(
              localStorage.getItem('cart_id') ?? ''
            ),
            user: this.accountService.getUserInfo(),
          });
        } else {
          return of({ cart: null, user: null });
        }
      })
    );
  }
}
