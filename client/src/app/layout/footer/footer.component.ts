import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { IsAdminDirective } from '../../shared/directives/is-admin.directive';
import { MatDialog } from '@angular/material/dialog';
import { CallbackComponent } from '../callback/callback.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-footer',
  imports: [
    RouterLink,
    RouterLinkActive,
    IsAdminDirective,
    MatIconModule,
    MatButton,
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  private dialog = inject(MatDialog);

  openCallbackDialog() {
    this.dialog.open(CallbackComponent, {
      width: '400px',
      panelClass: 'callback-dialog',
      disableClose: false,
    });
  }
}
