import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, MatCard, MatButton, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  validationErrors?: string[];

  registerForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
  });

  onSubmit() {
    this.validationErrors = undefined;
    if (this.registerForm.valid) {
      const formValue = this.registerForm.value;

      const registerData = {
        email: formValue.email!,
        password: formValue.password!,
        firstName: formValue.firstName!,
        lastName: formValue.lastName!,
      };

      this.accountService.register(registerData).subscribe({
        next: () => {
          this.router.navigateByUrl('/account/login');
        },
        error: (error) => {
          console.error('Registration failed:', error);
          if (error.error?.errors) {
            // Handle validation errors from API
            this.validationErrors = [];
            for (const key in error.error.errors) {
              if (error.error.errors[key]) {
                this.validationErrors.push(...error.error.errors[key]);
              }
            }
          } else if (error.error) {
            this.validationErrors = [
              typeof error.error === 'string'
                ? error.error
                : 'Registration failed',
            ];
          } else {
            this.validationErrors = ['Registration failed. Please try again.'];
          }

          // Display first error in snackbar
          if (this.validationErrors && this.validationErrors.length > 0) {
            this.snackBar.open(this.validationErrors[0], 'Close', {
              duration: 5000,
              panelClass: ['error-snackbar'],
              horizontalPosition: 'center',
              verticalPosition: 'top',
            });
          }
        },
      });
    }
  }
}
