import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CarbrandService } from '../../../../core/services/carbrand.service';
import { CarBrand } from '../../../../shared/models/carBrand';

@Component({
  selector: 'app-quick-brand-add',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './quick-brand-add.component.html',
  styleUrl: './quick-brand-add.component.scss',
})
export class QuickBrandAddComponent {
  @Output() brandAdded = new EventEmitter<CarBrand>();

  quickBrandForm: FormGroup;
  isSubmitting = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly carBrandService: CarbrandService,
    public dialogRef: MatDialogRef<QuickBrandAddComponent>
  ) {
    this.quickBrandForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  onSubmit(): void {
    if (this.quickBrandForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    const formData = new FormData();
    formData.append('name', this.quickBrandForm.get('name')?.value);

    this.carBrandService.createCarBrand(formData).subscribe({
      next: (newBrandId) => {
        this.carBrandService.getCarBrandById(newBrandId).subscribe({
          next: (brand) => {
            this.brandAdded.emit(brand);
            this.dialogRef.close(brand);
          },
          error: (err) => {
            console.error('Error fetching new brand details', err);
            this.isSubmitting = false;
          },
        });
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Error creating brand', err);
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
