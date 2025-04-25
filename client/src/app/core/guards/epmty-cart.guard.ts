import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';

export const emptyCartGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const router = inject(Router);

  if (!cartService.cart() || cartService.cart()?.cartItems.length === 0) {
    router.navigateByUrl('/cart');
    return false;
  }
  return true;
};
