import { Component, OnInit, inject } from '@angular/core';
import { CarSelectionComponent } from '../../home/car-selection/car-selection.component';
import { MatCardModule } from '@angular/material/card';
import { SelectedCar } from '../../../shared/models/carModel';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CommonModule, NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { Subscription } from 'rxjs';
import { CompatibleProductsComponent } from './compatible-products/compatible-products.component';

@Component({
  selector: 'app-customer-car',
  standalone: true,
  imports: [
    CarSelectionComponent,
    MatCardModule,
    CommonModule,
    NgIf,
    MatButtonModule,
    CompatibleProductsComponent,
  ],
  templateUrl: './customer-car.component.html',
  styleUrls: ['./customer-car.component.scss'],
})
export class CustomerCarComponent implements OnInit {
  private carModelService = inject(CarmodelService);
  selectedCar: SelectedCar | null = null;
  selectedCarId: string | null = null;
  private subs = new Subscription();

  ngOnInit(): void {
    this.loadSelectedCar();

    this.subs.add(
      this.carModelService.carSelectionChanged.subscribe((changed) => {
        if (changed) {
          this.loadSelectedCar();
        }
      })
    );
  }

  clearSelection(): void {
    this.selectedCar = null;
    this.selectedCarId = null;
    this.carModelService.clearCarSelection();
  }

  private loadSelectedCar(): void {
    const savedId = this.carModelService.getStoredCarId();
    if (!savedId) {
      return;
    }

    this.selectedCarId = savedId;

    this.carModelService.getSelectedCarDetails(savedId).subscribe({
      next: (carModel) => {
        this.selectedCar = {
          brand: carModel.brandName || '',
          model: carModel.modelName,
          startYear: carModel.startYear,
          bodyType: carModel.bodyType,
          engineSpec: carModel.volumeLiters + 'L ' + carModel.fuelType,
        };
      },
      error: (err) => {
        console.error('Failed to load data for the selected machine', err);
        this.carModelService.clearCarSelection();
      },
    });
  }
}
