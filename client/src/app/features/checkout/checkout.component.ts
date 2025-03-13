import {
  Component,
  inject,
  Input,
  OnChanges,
  OnInit,
  signal,
  SimpleChanges,
} from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { MatStepperModule } from '@angular/material/stepper';
import {
  MatCheckboxChange,
  MatCheckboxModule,
} from '@angular/material/checkbox';
import { AccountService } from '../../core/services/account.service';
import { CartService } from '../../core/services/cart.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
  FormControl,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';
import { CheckoutInformationComponent } from './checkout-information/checkout-information.component';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { RouterLink } from '@angular/router';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatInputModule,
    ReactiveFormsModule,
    CheckoutInformationComponent,
    CheckoutDeliveryComponent,
    RouterLink,
    CheckoutReviewComponent,
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit, OnChanges {
  cartService = inject(CartService);
  accountService = inject(AccountService);
  saveInformation = false;
  deliveryForm = new FormControl('', Validators.required);
  @Input() form!: FormGroup;
  @Input() selectedCity: string = '';
  cityControl = new FormControl();

  deliveryMethod: any = null;
  completionStatus = signal<{
    information: boolean;
    card: boolean;
    delivery: boolean;
  }>({ information: false, card: false, delivery: false });

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(
            '^\\+?[0-9]{1,4}?[-. ]?(\\(?\\d{1,3}?\\)?[-. ]?)?\\d{1,4}[-. ]?\\d{1,4}[-. ]?\\d{1,4}$'
          ),
        ],
      ],
      city: ['', [Validators.required]],
      saveAsDefault: [false],
    });
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['form'] && this.form) {
      this.selectedCity = this.form.get('city')?.value || '';
    }
  }

  ngOnInit() {
    this.accountService.getUserInfo().subscribe((user) => {
      if (user) {
        this.form.patchValue({
          email: user.email,
          firstName: user.firstName || '',
          lastName: user.lastName || '',
          phoneNumber: user.phoneNumber || '',
          city: user.city || '',
        });

        this.cityControl.setValue(user.city || '');
      }
    });
  }

  async onStepChange(event: any) {
    const stepIndex = event.selectedIndex;

    if (this.saveInformation) {
      await firstValueFrom(this.accountService.updateUserInfo(this.form.value));
    }

    if (stepIndex === 0 && this.form.valid) {
      this.completionStatus.update((status) => ({
        ...status,
        information: true,
      }));
    } else if (stepIndex === 1) {
      const city = this.form.controls['city'].value;
      this.cityControl.setValue(city);
      this.completionStatus.update((status) => ({
        ...status,
        delivery: true,
      }));
    } else if (stepIndex === 2) {
      this.completionStatus.update((status) => ({
        ...status,
        card: true,
      }));
    }
  }

  onCityChange(city: string) {
    this.selectedCity = city;
  }

  onSaveAddressCheckboxChange(event: any) {
    this.saveInformation = (event as MatCheckboxChange).checked;
  }
}
