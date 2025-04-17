import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatStepperModule } from '@angular/material/stepper';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

interface SelectedCar {
  brand: string;
  model: string;
  year: number;
  bodyType: string;
  engineSpec: string;
}

interface CarSelectionResponse {
  ids: string[];
  brands: string[];
  models: string[];
  years: number[];
  bodyTypes: string[];
  engineSpecs: string[];
}

@Component({
  selector: 'app-car-selection',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    FormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
    MatStepperModule,
  ],
  templateUrl: './car-selection.component.html',
  styleUrl: './car-selection.component.scss',
})
export class CarSelectionComponent implements OnInit, OnDestroy {
  brands: string[] = [];
  models: string[] = [];
  years: number[] = [];
  bodyTypes: string[] = [];
  engineSpecs: string[] = [];
  availableCarIds: string[] = [];

  carSelectionForm: FormGroup;
  selectedCar: SelectedCar | null = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private carModelService: CarmodelService,
    private fb: FormBuilder
  ) {
    this.carSelectionForm = this.fb.group({
      brand: ['', Validators.required],
      model: ['', Validators.required],
      year: [null, Validators.required],
      bodyType: ['', Validators.required],
      engineSpec: ['', Validators.required],
    });

    this.loadFromLocalStorage();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.carModelService.getCarSelectionWithIds().subscribe((resp) => {
        this.availableCarIds = resp.ids;
        this.brands = resp.brands;
        this.models = resp.models;
        this.years = resp.years;
        this.bodyTypes = resp.bodyTypes;
        this.engineSpecs = resp.engineSpecs;
      })
    );

    this.subscriptions.push(
      this.carModelService.carSelectionChanged.subscribe(() => {
        this.carSelectionForm.reset();
        this.selectedCar = null;
        this.carModelService.getCarSelectionWithIds().subscribe((resp) => {
          this.availableCarIds = resp.ids;
          this.brands = resp.brands;
          this.models = resp.models;
          this.years = resp.years;
          this.bodyTypes = resp.bodyTypes;
          this.engineSpecs = resp.engineSpecs;
        });
      })
    );

    this.subscriptions.push(
      this.carSelectionForm.get('brand')?.valueChanges.subscribe((brand) => {
        if (brand) {
          this.updateFilters();
        }
      }) as Subscription,

      this.carSelectionForm.get('model')?.valueChanges.subscribe((model) => {
        if (model) {
          this.updateFilters();
        }
      }) as Subscription,

      this.carSelectionForm.get('year')?.valueChanges.subscribe((year) => {
        if (year) {
          this.updateFilters();
        }
      }) as Subscription,

      this.carSelectionForm
        .get('bodyType')
        ?.valueChanges.subscribe((bodyType) => {
          if (bodyType) {
            this.updateFilters();
          }
        }) as Subscription
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  updateFilters(): void {
    const filter = {
      brand: this.carSelectionForm.get('brand')?.value,
      model: this.carSelectionForm.get('model')?.value,
      startYear: this.carSelectionForm.get('year')?.value,
      bodyType: this.carSelectionForm.get('bodyType')?.value,
    };

    this.carModelService.getCarSelectionWithIds(filter).subscribe({
      next: (response: CarSelectionResponse) => {
        this.availableCarIds = response.ids;
        this.brands = response.brands;
        this.models = response.models;
        this.years = response.years;
        this.bodyTypes = response.bodyTypes;
        this.engineSpecs = response.engineSpecs;
      },
      error: (err) => {
        console.error('Error updating filters', err);
      },
    });
  }

  saveSelection(): void {
    if (this.carSelectionForm.valid) {
      if (this.availableCarIds.length === 1) {
        this.carModelService.saveCarSelection(this.availableCarIds[0]);
      } else if (this.availableCarIds.length > 1) {
        const formValues = this.carSelectionForm.value;
        const filter = {
          brand: formValues.brand,
          model: formValues.model,
          startYear: formValues.year,
          bodyType: formValues.bodyType,
          engineSpec: formValues.engineSpec,
        };

        this.carModelService.getCarSelectionWithIds(filter).subscribe({
          next: (response: CarSelectionResponse) => {
            if (response.ids.length === 1) {
              this.carModelService.saveCarSelection(response.ids[0]);
            } else {
              console.error(
                'Multiple car IDs still match after all filters',
                response.ids
              );
            }
          },
          error: (err) => {
            console.error('Error finding car ID', err);
          },
        });
      } else {
        console.error('No car IDs match the current selection');
      }
    }
  }

  clearSelection(): void {
    this.carSelectionForm.reset();
    this.selectedCar = null;
    this.carModelService.clearCarSelection();
  }

  private loadFromLocalStorage(): void {
    const savedCarId = this.carModelService.getStoredCarId();
    if (savedCarId) {
      this.carModelService.getSelectedCarDetails(savedCarId).subscribe({
        next: (carModel) => {
          this.selectedCar = {
            brand: carModel.brand,
            model: carModel.model,
            year: carModel.startYear,
            bodyType: carModel.bodyType,
            engineSpec: carModel.horsePower + ' ' + carModel.fuelType,
          };

          this.carSelectionForm.patchValue({
            brand: carModel.brand,
            model: carModel.model,
            year: carModel.startYear,
            bodyType: carModel.bodyType,
            engineSpec: carModel.horsePower + ' ' + carModel.fuelType,
          });
        },
        error: (err) => {
          console.error('Failed to load selected car details', err);
          this.clearSelection();
        },
      });
    }
  }
}
