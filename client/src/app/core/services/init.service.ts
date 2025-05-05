import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, switchMap, catchError } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private readonly cartService = inject(CartService);
  private readonly accountService = inject(AccountService);

  init() {
    return this.accountService.refreshToken().pipe(
      catchError(() => {
        return this.accountService.checkAuth();
      }),
      switchMap((auth) => {
        const cartId = localStorage.getItem('cart_id');
        const cart$ = cartId
          ? this.cartService.getCart(cartId)
          : of(this.cartService.createCart());

        return forkJoin({
          cart: cart$,
          user: of(auth.user),
        });
      })
    );
  }
}
