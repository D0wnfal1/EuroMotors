import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-empty-cart-state',
  standalone: true,
  imports: [MatIcon, MatButton, RouterLink],
  templateUrl: './empty-cart-state.component.html',
  styleUrl: './empty-cart-state.component.scss',
})
export class EmptyCartStateComponent {}
