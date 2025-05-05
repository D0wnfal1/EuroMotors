import { Component, OnInit } from '@angular/core';
import { CarbrandService } from '../../../../core/services/carbrand.service';
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
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { SnackbarService } from '../../../../core/services/snackbar.service';

@Component({
  selector: 'app-carbrand-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    RouterLink,
  ],
  templateUrl: './carbrand-form.component.html',
  styleUrl: './carbrand-form.component.scss',
})
export class CarbrandFormComponent implements OnInit {
  carBrandForm: FormGroup = new FormGroup({});
  isEditMode: boolean = false;
  carBrandId: string | null = null;
  logoInvalid: boolean = false;
  selectedLogo: File | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly carBrandService: CarbrandService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly snackbar: SnackbarService
  ) {}

  ngOnInit(): void {
    this.initializeForm();

    this.carBrandId = this.activatedRoute.snapshot.paramMap.get('id');
    if (this.carBrandId) {
      this.isEditMode = true;
      this.loadCarBrandData();
    }
  }

  initializeForm() {
    this.carBrandForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  loadCarBrandData() {
    if (this.carBrandId) {
      this.carBrandService.getCarBrandById(this.carBrandId).subscribe({
        next: (carBrand) => {
          this.carBrandForm.patchValue({
            name: carBrand.name,
          });
        },
        error: (error) => {
          console.error('Error loading car brand data:', error);
        },
      });
    }
  }

  onLogoSelected(event: any) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      this.selectedLogo = file;
      this.logoInvalid = false;
    } else {
      this.selectedLogo = null;
      this.logoInvalid = true;
    }
  }

  onSubmit() {
    if (this.carBrandForm.invalid) {
      this.snackbar.error('Please fix the errors in the form');
      return;
    }

    const formData = new FormData();
    formData.append('name', this.carBrandForm.get('name')?.value);

    if (this.selectedLogo) {
      formData.append('logo', this.selectedLogo, this.selectedLogo.name);
    }

    if (this.isEditMode && this.carBrandId) {
      this.carBrandService.updateCarBrand(this.carBrandId, formData).subscribe({
        next: () => {
          this.carBrandService.clearCache();
          this.snackbar.success('Car brand updated successfully');
          this.router.navigate(['/admin/carbrands']);
        },
        error: (error) => {
          this.snackbar.error('Failed to update car brand');
          console.error('Error updating car brand:', error);
        },
      });
    } else {
      this.carBrandService.createCarBrand(formData).subscribe({
        next: (newCarBrandId) => {
          this.carBrandService.clearCache();
          this.snackbar.success('Car brand created successfully');
          this.router.navigate(['/admin/carbrands']);
        },
        error: (error) => {
          this.snackbar.error('Failed to create car brand');
          console.error('Error creating car brand:', error);
        },
      });
    }
  }
}
