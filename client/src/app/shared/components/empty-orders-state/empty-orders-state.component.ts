import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-empty-orders-state',
  imports: [MatIcon, MatButton, RouterLink],
  templateUrl: './empty-orders-state.component.html',
  styleUrl: './empty-orders-state.component.scss',
})
export class EmptyOrdersStateComponent {}
