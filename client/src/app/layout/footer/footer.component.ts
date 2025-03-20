import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDivider } from '@angular/material/divider';
import { MatIcon } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { IsAdminDirective } from '../../shared/directives/is-admin.directive';

@Component({
  selector: 'app-footer',
  imports: [
    MatButton,
    MatIcon,
    MatDivider,
    RouterLink,
    RouterLinkActive,
    IsAdminDirective,
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
