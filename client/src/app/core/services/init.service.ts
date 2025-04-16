import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, switchMap, catchError } from 'rxjs';
import { AccountService } from './account.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private http = inject(HttpClient);

  init() {
    return this.tryRefreshToken().pipe(
      switchMap(() => this.accountService.getAuthState()),
      switchMap((auth: { isAuthenticated: boolean }) => {
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

  private tryRefreshToken() {
    return this.accountService.refreshToken();
  }
}
