import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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
import { MatSnackBar } from '@angular/material/snack-bar';
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
export class QuickBrandAddComponent implements OnInit {
  @Output() brandAdded = new EventEmitter<CarBrand>();

  quickBrandForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private carBrandService: CarbrandService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<QuickBrandAddComponent>
  ) {
    this.quickBrandForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.quickBrandForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    const formData = new FormData();
    formData.append('name', this.quickBrandForm.get('name')?.value);

    this.carBrandService.createCarBrand(formData).subscribe({
      next: (newBrandId) => {
        this.snackBar.open('Car brand created successfully!', 'Close', {
          duration: 3000,
        });

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
        this.snackBar.open('Failed to create brand', 'Close', {
          duration: 3000,
        });
        this.isSubmitting = false;
        console.error('Error creating brand', err);
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
