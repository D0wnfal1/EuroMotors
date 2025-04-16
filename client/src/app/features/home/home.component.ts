import { Component } from '@angular/core';
import { CarSelectionComponent } from './car-selection/car-selection.component';

@Component({
  selector: 'app-home',
  imports: [CarSelectionComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {}
