import {
  Component,
  OnInit,
  OnDestroy,
  Input,
  OnChanges,
  SimpleChanges,
  inject,
  HostListener,
} from '@angular/core';
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
import { CarBrand } from '../../../shared/models/carBrand';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CarSelectionResponse } from '../../../shared/models/carModel';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

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
export class CarSelectionComponent implements OnInit, OnDestroy, OnChanges {
  @Input() preSelectedBrand?: CarBrand;

  carModelService = inject(CarmodelService);
  carBrands: CarBrand[] = [];
  models: string[] = [];
  years: number[] = [];
  bodyTypes: string[] = [];
  engineSpecs: string[] = [];
  availableCarIds: string[] = [];
  isSmallScreen = false;

  carSelectionForm: FormGroup;
  private subscriptions: Subscription[] = [];
  private isInitialLoad = true;
  private isFormSetupComplete = false;

  constructor(
    private carBrandService: CarbrandService,
    private fb: FormBuilder,
    private router: Router,
    private breakpointObserver: BreakpointObserver
  ) {
    this.carSelectionForm = this.fb.group({
      brand: [null, Validators.required],
      model: ['', Validators.required],
      year: [null, Validators.required],
      bodyType: ['', Validators.required],
      engineSpec: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.observeScreenSize();
    this.carBrandService.getAllCarBrands();

    this.subscriptions.push(
      this.carBrandService.availableBrands$.subscribe((brands) => {
        this.carBrands = brands;
        if (!this.isInitialLoad) {
          this.preSelectBrandIfAvailable();
        }
      })
    );

    this.subscriptions.push(
      this.carModelService.carSelectionChanged.subscribe(() => {
        if (!this.isInitialLoad) {
          this.carSelectionForm.reset();
          this.loadCarSelectionData();
        }
      })
    );

    this.loadCarSelectionData();
    this.loadFromLocalStorage();

    setTimeout(() => {
      this.setupFormValueChanges();
      this.isFormSetupComplete = true;
    }, 500);
  }

  observeScreenSize(): void {
    this.subscriptions.push(
      this.breakpointObserver
        .observe([Breakpoints.XSmall, Breakpoints.Small])
        .subscribe((result) => {
          this.isSmallScreen = result.matches;
        })
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['preSelectedBrand'] &&
      this.preSelectedBrand &&
      !this.isInitialLoad
    ) {
      this.preSelectBrandIfAvailable();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  loadCarSelectionData(filter?: any): void {
    this.carModelService.getCarSelectionWithIds(filter).subscribe({
      next: (response: CarSelectionResponse) => {
        this.availableCarIds = response.ids;

        if (!this.carSelectionForm.get('brand')?.value || filter) {
          this.models = response.models;
        }

        this.years = response.years;
        this.bodyTypes = response.bodyTypes;
        this.engineSpecs = response.engineSpecs;

        const currentModel = this.carSelectionForm.get('model')?.value;
        if (
          currentModel &&
          this.models.length > 0 &&
          !this.models.includes(currentModel)
        ) {
          this.carSelectionForm.patchValue({ model: '' }, { emitEvent: false });
        }
      },
      error: (err) => {
        console.error('Error loading car selection data', err);
      },
    });
  }

  setupFormValueChanges(): void {
    this.subscriptions.push(
      this.carSelectionForm
        .get('brand')
        ?.valueChanges.subscribe((brand: CarBrand) => {
          if (brand && this.isFormSetupComplete && !this.isInitialLoad) {
            this.carSelectionForm.patchValue(
              {
                model: '',
                year: null,
                bodyType: '',
                engineSpec: '',
              },
              { emitEvent: false }
            );

            this.updateFilters();
          }
        }) as Subscription,

      this.carSelectionForm.get('model')?.valueChanges.subscribe((model) => {
        if (model && this.isFormSetupComplete && !this.isInitialLoad) {
          this.updateFilters();
        }
      }) as Subscription,

      this.carSelectionForm.get('year')?.valueChanges.subscribe((year) => {
        if (year && this.isFormSetupComplete && !this.isInitialLoad) {
          this.updateFilters();
        }
      }) as Subscription,

      this.carSelectionForm
        .get('bodyType')
        ?.valueChanges.subscribe((bodyType) => {
          if (bodyType && this.isFormSetupComplete && !this.isInitialLoad) {
            this.updateFilters();
          }
        }) as Subscription
    );
  }

  updateFilters(): void {
    if (this.isInitialLoad) {
      return;
    }

    const selectedBrand = this.carSelectionForm.get('brand')?.value as CarBrand;

    const filter = {
      brand: selectedBrand ? selectedBrand.name : undefined,
      model: this.carSelectionForm.get('model')?.value,
      startYear: this.carSelectionForm.get('year')?.value,
      bodyType: this.carSelectionForm.get('bodyType')?.value,
    };

    if (selectedBrand && !this.carSelectionForm.get('model')?.value) {
      this.models = [];
    }

    this.loadCarSelectionData(filter);
  }

  saveSelection(): void {
    if (this.isInitialLoad) {
      return;
    }

    if (this.carSelectionForm.valid) {
      let selectedCarId: string | null = null;

      if (this.availableCarIds.length === 1) {
        selectedCarId = this.availableCarIds[0];
        this.carModelService.clearCarSelection();
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
              this.carModelService.clearCarSelection();
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
    } else {
    }
  }

  navigateToShop(carId: string): void {
    this.router.navigate(['/shop'], {
      queryParams: {
        carModelId: carId,
        t: new Date().getTime(),
      },
    });
  }

  private loadFromLocalStorage(): void {
    const savedCarId = this.carModelService.getStoredCarId();
    if (savedCarId) {
      this.isInitialLoad = true;

      forkJoin({
        carModel: this.carModelService.getSelectedCarDetails(savedCarId),
        carBrands: this.carBrandService.availableBrands$,
      }).subscribe({
        next: ({ carModel, carBrands }) => {
          const engineSpecStr = `${carModel.volumeLiters}L ${carModel.fuelType}`;

          const selectedBrand = carBrands.find(
            (brand) => brand.id === carModel.carBrandId
          );

          if (selectedBrand) {
            this.carSelectionForm
              .get('brand')
              ?.setValue(selectedBrand, { emitEvent: false });
          }

          setTimeout(() => {
            this.carSelectionForm.patchValue(
              {
                model: carModel.modelName,
                year: carModel.startYear,
                bodyType: carModel.bodyType,
                engineSpec: engineSpecStr,
              },
              { emitEvent: false }
            );

            setTimeout(() => {
              this.isInitialLoad = false;
            }, 500);
          }, 300);
        },
        error: (err) => {
          console.error('Failed to load selected car details', err);
          this.carSelectionForm.reset();
          this.carModelService.clearCarSelection();
          this.isInitialLoad = false;
        },
      });
    } else {
      setTimeout(() => {
        this.isInitialLoad = false;
      }, 500);
    }
  }

  private preSelectBrandIfAvailable(): void {
    if (this.isInitialLoad) {
      return;
    }

    if (this.preSelectedBrand && this.carBrands.length > 0) {
      const brandToSelect = this.carBrands.find(
        (brand) =>
          brand.id === this.preSelectedBrand?.id ||
          brand.name === this.preSelectedBrand?.name
      );

      const currentBrand = this.carSelectionForm.get('brand')?.value;

      if (
        brandToSelect &&
        (!currentBrand || currentBrand.id !== brandToSelect.id)
      ) {
        this.carSelectionForm.patchValue(
          {
            model: '',
            year: null,
            bodyType: '',
            engineSpec: '',
          },
          { emitEvent: false }
        );

        this.carSelectionForm
          .get('brand')
          ?.setValue(brandToSelect, { emitEvent: false });
        this.updateFilters();
      }
    }
  }
}
