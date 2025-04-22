import { Component, OnInit, OnDestroy, Inject, inject } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatStepperModule } from '@angular/material/stepper';
import { CarbrandService } from '../../../core/services/carbrand.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription, forkJoin } from 'rxjs';
import { Router } from '@angular/router';
import { SelectedCar } from '../../../shared/models/carModel';
import { CarBrand } from '../../../shared/models/carBrand';
import { CarmodelService } from '../../../core/services/carmodel.service';

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
  carModelService = inject(CarmodelService);
  carBrands: CarBrand[] = [];
  models: string[] = [];
  years: number[] = [];
  bodyTypes: string[] = [];
  engineSpecs: string[] = [];
  availableCarIds: string[] = [];

  carSelectionForm: FormGroup;
  selectedCar: SelectedCar | null = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private carBrandService: CarbrandService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.carSelectionForm = this.fb.group({
      brand: [null, Validators.required],
      model: ['', Validators.required],
      year: [null, Validators.required],
      bodyType: ['', Validators.required],
      engineSpec: ['', Validators.required],
    });

    this.loadFromLocalStorage();
  }

  ngOnInit(): void {
    this.carBrandService.getAllCarBrands();

    this.subscriptions.push(
      this.carBrandService.availableBrands$.subscribe((brands) => {
        this.carBrands = brands;
      })
    );

    this.subscriptions.push(
      this.carModelService.getCarSelectionWithIds().subscribe((resp) => {
        this.availableCarIds = resp.ids;
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
          this.models = resp.models;
          this.years = resp.years;
          this.bodyTypes = resp.bodyTypes;
          this.engineSpecs = resp.engineSpecs;
        });
      })
    );

    this.subscriptions.push(
      this.carSelectionForm
        .get('brand')
        ?.valueChanges.subscribe((brand: CarBrand) => {
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
    const selectedBrand = this.carSelectionForm.get('brand')?.value as CarBrand;

    const filter = {
      brand: selectedBrand ? selectedBrand.name : undefined,
      model: this.carSelectionForm.get('model')?.value,
      startYear: this.carSelectionForm.get('year')?.value,
      bodyType: this.carSelectionForm.get('bodyType')?.value,
    };

    this.carModelService.getCarSelectionWithIds(filter).subscribe({
      next: (response: CarSelectionResponse) => {
        this.availableCarIds = response.ids;
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
      let selectedCarId: string | null = null;

      if (this.availableCarIds.length === 1) {
        selectedCarId = this.availableCarIds[0];
        this.carModelService.saveCarSelection(selectedCarId);
        this.navigateToShop(selectedCarId);
      } else if (this.availableCarIds.length > 1) {
        const formValues = this.carSelectionForm.value;
        const selectedBrand = formValues.brand as CarBrand;

        const filter = {
          brand: selectedBrand ? selectedBrand.name : undefined,
          model: formValues.model,
          startYear: formValues.year,
          bodyType: formValues.bodyType,
          engineSpec: formValues.engineSpec,
        };

        this.carModelService.getCarSelectionWithIds(filter).subscribe({
          next: (response: CarSelectionResponse) => {
            if (response.ids.length === 1) {
              selectedCarId = response.ids[0];
              this.carModelService.saveCarSelection(selectedCarId);
              this.navigateToShop(selectedCarId);
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

  navigateToShop(carId: string): void {
    this.router.navigate(['/shop'], { queryParams: { carModelId: carId } });
  }

  clearSelection(): void {
    this.carSelectionForm.reset();
    this.selectedCar = null;
    this.carModelService.clearCarSelection();
  }

  private loadFromLocalStorage(): void {
    const savedCarId = this.carModelService.getStoredCarId();
    if (savedCarId) {
      forkJoin({
        carModel: this.carModelService.getSelectedCarDetails(savedCarId),
        carBrands: this.carBrandService.availableBrands$,
      }).subscribe({
        next: ({ carModel, carBrands }) => {
          const engineSpecStr = `${carModel.volumeLiters}L ${carModel.fuelType}`;

          const selectedBrand = carBrands.find(
            (brand) => brand.id === carModel.carBrandId
          );

          this.selectedCar = {
            brand: carModel.brandName || '',
            model: carModel.modelName,
            startYear: carModel.startYear,
            bodyType: carModel.bodyType,
            engineSpec: engineSpecStr,
          };

          this.carSelectionForm.patchValue({
            brand: selectedBrand,
            model: carModel.modelName,
            year: carModel.startYear,
            bodyType: carModel.bodyType,
            engineSpec: engineSpecStr,
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
