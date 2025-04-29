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
export class AppComponent implements OnDestroy {
  title = 'EuroMotors';
  private readonly dialog = inject(MatDialog);
  notification = inject(NotificationService);
  router = inject(Router);

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
}
