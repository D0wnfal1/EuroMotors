import { Component, OnInit } from '@angular/core';
import { CarmodelService } from '../../../../core/services/carmodel.service';
import { CarbrandService } from '../../../../core/services/carbrand.service';
import { BodyType, FuelType } from '../../../../shared/models/carModel';
import { CarBrand } from '../../../../shared/models/carBrand';
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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { QuickBrandAddComponent } from '../quick-brand-add/quick-brand-add.component';

@Component({
  selector: 'app-carmodel-form',
  imports: [
    ReactiveFormsModule,
    MatSelectModule,
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    RouterLink,
    MatDialogModule,
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
  availableBrands: CarBrand[] = [];

  constructor(
    private fb: FormBuilder,
    private carModelService: CarmodelService,
    private carBrandService: CarbrandService,
    private snackBar: MatSnackBar,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadCarBrands();
    this.initializeForm();

    this.carModelId = this.activatedRoute.snapshot.paramMap.get('id');
    if (this.carModelId) {
      this.isEditMode = true;
      this.loadCarModelData();
    }
  }

  loadCarBrands() {
    this.carBrandService.getAllCarBrands();
    this.carBrandService.availableBrands$.subscribe((brands) => {
      this.availableBrands = brands;
    });
  }

  openQuickAddBrandDialog() {
    const dialogRef = this.dialog.open(QuickBrandAddComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.availableBrands = [...this.availableBrands, result];
        this.carModelForm.get('carBrandId')?.setValue(result.id);

        this.carBrandService.getAllCarBrands();
      }
    });
  }

  initializeForm() {
    this.carModelForm = this.fb.group({
      carBrandId: ['', [Validators.required]],
      modelName: ['', [Validators.required, Validators.maxLength(100)]],
      startYear: [
        '',
        [
          Validators.required,
          Validators.min(1900),
          Validators.max(new Date().getFullYear()),
        ],
      ],
      bodyType: ['', Validators.required],
      engineSpec: this.fb.group({
        volumeLiters: ['', [Validators.required, Validators.min(0.1)]],
        fuelType: ['', Validators.required],
      }),
    });
  }

  loadCarModelData() {
    if (this.carModelId) {
      this.carModelService.getCarModelById(this.carModelId).subscribe({
        next: (carModel) => {
          this.carModelForm.patchValue({
            carBrandId: carModel.carBrandId,
            modelName: carModel.modelName,
            startYear: carModel.startYear,
            bodyType: carModel.bodyType,
            engineSpec: {
              volumeLiters: carModel.volumeLiters,
              fuelType: carModel.fuelType,
            },
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

  onBrandChange(event: any) {
    const selectedBrandId = event.value;
    if (selectedBrandId) {
      console.log('Selected brand ID:', selectedBrandId);
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
    formData.append('carBrandId', this.carModelForm.get('carBrandId')?.value);
    formData.append('modelName', this.carModelForm.get('modelName')?.value);
    formData.append('startYear', this.carModelForm.get('startYear')?.value);
    formData.append('bodyType', this.carModelForm.get('bodyType')?.value);
    formData.append(
      'volumeLiters',
      this.carModelForm.get('engineSpec.volumeLiters')?.value
    );
    formData.append(
      'fuelType',
      this.carModelForm.get('engineSpec.fuelType')?.value
    );

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
