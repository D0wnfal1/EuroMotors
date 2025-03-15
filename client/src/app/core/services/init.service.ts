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
        const cartId = localStorage.getItem('cart_id');
        const cart$ = cartId
          ? this.cartService.getCart(cartId)
          : of(this.cartService.createCart());

        const user$ = auth.isAuthenticated
          ? this.accountService.getUserInfo()
          : of(null);

        return forkJoin({ cart: cart$, user: user$ });
      })
    );
  }
}
