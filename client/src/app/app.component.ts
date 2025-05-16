import {
  Component,
  inject,
  HostListener,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { CallbackComponent } from './layout/callback/callback.component';
import { NotificationService } from './core/services/notification.service';
import { ImageOptimizationModule } from './shared/modules/image-optimization.module';
import { AccountService } from './core/services/account.service';
import { of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    MatIconModule,
    MatButtonModule,
    ImageOptimizationModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'AutoRSD';
  private readonly dialog = inject(MatDialog);
  notification = inject(NotificationService);
  router = inject(Router);
  private readonly accountService = inject(AccountService);

  ngOnInit() {
    const style = document.createElement('style');
    style.innerHTML = `
      * {
        touch-action: auto !important;
        -webkit-overflow-scrolling: touch !important;
      }
    `;
    document.head.appendChild(style);
    this.initializeAuth();
  }

  @HostListener('window:beforeunload', ['$event'])
  unloadHandler(event: Event) {
    this.notification.clearAllToasts();
  }

  ngOnDestroy() {
    this.notification.clearAllToasts();
  }

  openCallbackDialog() {
    this.dialog.open(CallbackComponent, {
      width: '500px',
    });
  }

  private initializeAuth(): void {
    this.accountService
      .checkAuth()
      .pipe(
        catchError(() => {
          return this.accountService.refreshToken().pipe(
            switchMap(() => this.accountService.checkAuth()),
            catchError((error) => {
              return of({ isAuthenticated: false, user: null });
            })
          );
        })
      )
      .subscribe();
  }
}
