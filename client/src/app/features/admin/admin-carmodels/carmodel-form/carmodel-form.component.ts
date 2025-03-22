import { Component, OnInit } from '@angular/core';
import { CarmodelService } from '../../../../core/services/carmodel.service';
import { CarModel } from '../../../../shared/models/carModel';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatFormFieldModule, MatError } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-carmodel-form',
  imports: [
    ReactiveFormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    RouterLink,
  ],
  templateUrl: './carmodel-form.component.html',
  styleUrl: './carmodel-form.component.scss',
})
export class CarmodelFormComponent implements OnInit {
  carModelForm: FormGroup = new FormGroup({});
  isEditMode: boolean = false;
  carModelId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private carModelService: CarmodelService,
    private snackBar: MatSnackBar,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();

    this.carModelId = this.activatedRoute.snapshot.paramMap.get('id');
    if (this.carModelId) {
      this.isEditMode = true;
      this.loadCarModelData();
    }
  }

  initializeForm() {
    this.carModelForm = this.fb.group({
      brand: ['', [Validators.required, Validators.maxLength(100)]],
      model: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  loadCarModelData() {
    if (this.carModelId) {
      this.carModelService.getCarModelById(this.carModelId).subscribe({
        next: (carModel) => {
          this.carModelForm.patchValue({
            brand: carModel.brand,
            model: carModel.model,
          });
        },
        error: (error) => {
          this.snackBar.open('Failed to load car model data', 'Close', {
            duration: 3000,
            panelClass: ['snack-error'],
          });
        },
      });
    }
  }

  onSubmit() {
    if (this.carModelForm.invalid) {
      return;
    }

    const carModelData: CarModel = this.carModelForm.value;

    if (this.isEditMode && this.carModelId) {
      this.carModelService
        .updateCarModel(this.carModelId, carModelData)
        .subscribe({
          next: () => {
            this.snackBar.open('CarModel updated successfully!', 'Close', {
              duration: 3000,
              panelClass: ['snack-success'],
            });
            this.router.navigate(['/admin/carmodels']);
          },
          error: () => {
            this.snackBar.open('Failed to update car model', 'Close', {
              duration: 3000,
              panelClass: ['snack-error'],
            });
          },
        });
    } else {
      this.carModelService.createCarModel(carModelData).subscribe({
        next: (newCarModelId) => {
          this.snackBar.open('CarModel created successfully!', 'Close', {
            duration: 3000,
            panelClass: ['snack-success'],
          });
          this.router.navigate(['/admin/carmodels']);
        },
        error: () => {
          this.snackBar.open('Failed to create car model', 'Close', {
            duration: 3000,
            panelClass: ['snack-error'],
          });
        },
      });
    }
  }
}
