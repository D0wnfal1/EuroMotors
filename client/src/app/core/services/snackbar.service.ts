import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  private readonly snackbar = inject(MatSnackBar);

  error(message: string) {
    this.snackbar.open(message, 'Закрити', {
      duration: 5000,
      panelClass: ['snack-error'],
    });
  }

  success(message: string) {
    this.snackbar.open(message, 'Закрити', {
      duration: 5000,
      panelClass: ['snack-success'],
    });
  }
}
