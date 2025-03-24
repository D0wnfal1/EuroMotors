import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { IsAdminDirective } from '../../shared/directives/is-admin.directive';

@Component({
  selector: 'app-footer',
  imports: [RouterLink, RouterLinkActive, IsAdminDirective],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {}
