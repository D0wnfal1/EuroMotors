import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatInput,
    MatLabel,
    MatButton,
    MatError,
    NgIf,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);
  returnUrl = '/shop';
  errorMessage: string | null = null;

  constructor() {
    const url = this.activatedRoute.snapshot.queryParams['returnUrl'];
    if (url) this.returnUrl = url;
  }

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  onSubmit() {
    this.errorMessage = null;
    if (this.loginForm.valid) {
      const formValue = this.loginForm.value;

      const loginData = {
        email: formValue.email!,
        password: formValue.password!,
      };

      this.accountService.login(loginData).subscribe({
        next: () => {
          this.router.navigateByUrl('/');
        },
        error: (error) => {
          console.error('Login failed:', error);
          let errorMsg: string;

          if (error.error?.message) {
            errorMsg = error.error.message;
          } else if (error.error) {
            errorMsg = error.error;
          } else {
            errorMsg = 'Failed to login. Please try again.';
          }

          this.errorMessage = errorMsg;
          this.snackBar.open(errorMsg, 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar'],
            horizontalPosition: 'center',
            verticalPosition: 'top',
          });
        },
      });
    }
  }
}
