import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';
import { NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AccountService } from '../../../core/services/account.service';

@Component({
  selector: 'app-checkout-information',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatCheckbox,
    NgIf,
  ],
  templateUrl: './checkout-information.component.html',
  styleUrls: ['./checkout-information.component.scss'],
})
export class CheckoutInformationComponent implements OnInit {
  checkoutForm!: FormGroup;
  isFormFilled = false;
  @Output() cityChange: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.checkoutForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /^\+?[0-9]{1,4}?[-. ]?(\(?\d{1,3}?\)?[-. ]?)?\d{1,4}[-. ]?\d{1,4}[-. ]?\d{1,4}$/
          ),
        ],
      ],
      city: ['', Validators.required],
      saveAsDefault: [false],
    });

    this.accountService.getUserInfo().subscribe((user) => {
      if (user) {
        this.checkoutForm.patchValue({
          email: user.email,
          firstName: user.firstName,
          lastName: user.lastName,
          phoneNumber: user.phoneNumber,
          city: user.city,
        });
      }
    });

    this.checkoutForm.valueChanges.subscribe(() => {
      this.isFormFilled = this.checkoutForm.valid;
    });

    this.checkoutForm.get('city')?.valueChanges.subscribe((city: string) => {
      this.cityChange.emit(city);
    });

    this.checkoutForm
      .get('saveAsDefault')
      ?.valueChanges.subscribe((save: boolean) => {
        if (save) {
          this.onSubmit();
        }
      });
  }

  onSubmit(): void {
    if (this.checkoutForm.valid) {
      const formValues = this.checkoutForm.value;
      this.accountService.updateUserInfo(formValues).subscribe({});
    } else {
    }
  }
}
