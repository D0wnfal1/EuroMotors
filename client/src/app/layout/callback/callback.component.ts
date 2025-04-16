import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CheckoutService } from '../../core/services/checkout.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-callback',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './callback.component.html',
  styleUrl: './callback.component.scss',
})
export class CallbackComponent implements OnInit {
  callbackForm: FormGroup = new FormGroup({});

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CallbackComponent>,
    private checkoutService: CheckoutService
  ) {}

  ngOnInit() {
    this.callbackForm = this.fb.group({
      phone: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /^\+?[0-9]{1,4}?[-. ]?(\(?\d{1,3}?\)?[-. ]?)?\d{1,4}[-. ]?\d{1,4}[-. ]?\d{1,4}$/
          ),
        ],
      ],
      name: ['', [Validators.required]],
    });
  }

  onSubmit() {
    if (this.callbackForm.valid) {
      const { name, phone } = this.callbackForm.value;
      this.checkoutService.requestCallback(name, phone).subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Помилка при відправці запиту:', err);
        },
      });
    }
  }

  close() {
    this.dialogRef.close();
  }
}
