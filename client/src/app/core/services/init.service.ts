import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, switchMap } from 'rxjs';
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
    return this.accountService.checkAuth().pipe(
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
