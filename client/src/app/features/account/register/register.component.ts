import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { MatSnackBar } from '@angular/material/snack-bar';

function passwordStrengthValidator(): ValidationErrors | null {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (!value) {
      return null;
    }

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumeric = /[0-9]/.test(value);
    const hasSpecialChar = /[^a-zA-Z0-9]/.test(value);

    const passwordValid =
      hasUpperCase && hasLowerCase && hasNumeric && hasSpecialChar;

    return !passwordValid ? { passwordStrength: true } : null;
  };
}

interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

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
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        passwordStrengthValidator(),
      ],
    ],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
  });

  onSubmit() {
    this.validationErrors = undefined;

    if (this.registerForm.invalid) {
      const passwordControl = this.registerForm.get('password');

      if (passwordControl?.invalid) {
        if (passwordControl.errors?.['required']) {
          this.showError('Введіть пароль');
        } else if (passwordControl.errors?.['minlength']) {
          this.showError('Пароль повинен містити мінімум 8 символів');
        } else if (passwordControl.errors?.['passwordStrength']) {
          this.showError(
            'Пароль повинен містити великі літери, малі літери, цифри та спеціальні символи'
          );
        }
      } else if (this.registerForm.get('email')?.invalid) {
        this.showError('Введіть правильну електронну адресу');
      } else if (this.registerForm.get('firstName')?.invalid) {
        this.showError("Введіть ім'я");
      } else if (this.registerForm.get('lastName')?.invalid) {
        this.showError('Введіть прізвище');
      }

      return;
    }

    const values = {
      email: String(this.registerForm.get('email')?.value),
      password: String(this.registerForm.get('password')?.value),
      firstName: String(this.registerForm.get('firstName')?.value),
      lastName: String(this.registerForm.get('lastName')?.value),
    };

    this.accountService.register(values).subscribe({
      next: () => {
        this.snackBar.open(
          'Реєстрація успішна! Увійдіть у свій обліковий запис.',
          'Закрити',
          {
            duration: 5000,
            panelClass: ['success-snackbar'],
            horizontalPosition: 'center',
            verticalPosition: 'top',
          }
        );
        this.router.navigateByUrl('/account/login');
      },
      error: (error) => {
        console.error('Registration failed:', error);
        if (error.error?.errors) {
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
              : 'Помилка реєстрації',
          ];
        } else {
          this.validationErrors = ['Помилка реєстрації. Спробуйте ще раз.'];
        }

        if (this.validationErrors && this.validationErrors.length > 0) {
          this.showError(this.validationErrors[0]);
        }
      },
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Закрити', {
      duration: 5000,
      panelClass: ['error-snackbar'],
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }
}
