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
    const subscription = this.carModelService.getCarSelection().subscribe({
      next: () => {
        this.subscriptions.push(
          this.carModelService.brands$.subscribe((brands) => {
            this.brands = brands;
          }),
          this.carModelService.models$.subscribe((models) => {
            this.models = models;
          }),
          this.carModelService.years$.subscribe((years) => {
            this.years = years;
          }),
          this.carModelService.bodyTypes$.subscribe((bodyTypes) => {
            this.bodyTypes = bodyTypes;
          }),
          this.carModelService.engineSpecs$.subscribe((engineSpecs) => {
            this.engineSpecs = engineSpecs;
          })
        );
      },
      error: (err) => {
        console.error('Error fetching car selection', err);
      },
    });

    this.subscriptions.push(subscription);

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

    this.carModelService.getCarSelection(filter).subscribe({
      error: (err) => {
        console.error('Error updating filters', err);
      },
    });
  }

  saveSelection(): void {
    if (this.carSelectionForm.valid) {
      this.selectedCar = {
        brand: this.carSelectionForm.value.brand,
        model: this.carSelectionForm.value.model,
        year: this.carSelectionForm.value.year,
        bodyType: this.carSelectionForm.value.bodyType,
        engineSpec: this.carSelectionForm.value.engineSpec,
      };

      this.carModelService.saveCarSelection(this.selectedCar);

      this.carModelService.carSelectionChanged.next(true);
    }
  }

  clearSelection(): void {
    this.carSelectionForm.reset();
    this.selectedCar = null;
    this.carModelService.clearCarSelection();

    this.carModelService.getCarSelection().subscribe();
  }

  private loadFromLocalStorage(): void {
    const savedCar = this.carModelService.getStoredCarSelection();
    if (savedCar) {
      this.selectedCar = savedCar;
      this.carSelectionForm.patchValue({
        brand: this.selectedCar?.brand,
        model: this.selectedCar?.model,
        year: this.selectedCar?.year,
        bodyType: this.selectedCar?.bodyType,
        engineSpec: this.selectedCar?.engineSpec,
      });
    }
  }
}
