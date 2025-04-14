import { Component, OnInit } from '@angular/core';
import { CarmodelService } from '../../../../core/services/carmodel.service';
import { BodyType, FuelType } from '../../../../shared/models/carModel';
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
  imageInvalid: boolean = false;
  selectedImage: File | null = null;
  bodyTypes = Object.values(BodyType);
  fuelTypes = Object.values(FuelType);
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
      startYear: [
        '',
        [
          Validators.required,
          Validators.min(1900),
          Validators.max(new Date().getFullYear()),
        ],
      ],
      endYear: ['', [Validators.max(new Date().getFullYear())]],
      bodyType: ['', Validators.required],
      volumeLiters: ['', [Validators.required, Validators.min(0.1)]],
      fuelType: ['', Validators.required],
    });
  }

  loadCarModelData() {
    if (this.carModelId) {
      this.carModelService.getCarModelById(this.carModelId).subscribe({
        next: (carModel) => {
          this.carModelForm.patchValue({
            brand: carModel.brand,
            model: carModel.model,
            startYear: carModel.startYear,
            endYear: carModel.endYear,
            bodyType: carModel.bodyType,
            volumeLiters: carModel.volumeLiters,
            fuelType: carModel.fuelType,
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

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      this.selectedImage = file;
      this.imageInvalid = false;
    } else {
      this.selectedImage = null;
      this.imageInvalid = true;
    }
  }

  onSubmit() {
    if (this.carModelForm.invalid) {
      return;
    }

    const formData = new FormData();
    formData.append('brand', this.carModelForm.get('brand')?.value);
    formData.append('model', this.carModelForm.get('model')?.value);
    formData.append('startYear', this.carModelForm.get('startYear')?.value);
    formData.append('endYear', this.carModelForm.get('endYear')?.value);
    formData.append('bodyType', this.carModelForm.get('bodyType')?.value);
    formData.append(
      'volumeLiters',
      this.carModelForm.get('volumeLiters')?.value
    );
    formData.append('fuelType', this.carModelForm.get('fuelType')?.value);

    if (this.selectedImage) {
      formData.append('image', this.selectedImage, this.selectedImage.name);
    }

    if (this.isEditMode && this.carModelId) {
      this.carModelService.updateCarModel(this.carModelId, formData).subscribe({
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
      this.carModelService.createCarModel(formData).subscribe({
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
