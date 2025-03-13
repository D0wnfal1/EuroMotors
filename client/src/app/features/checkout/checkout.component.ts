import {
  Component,
  inject,
  Input,
  OnChanges,
  OnInit,
  signal,
  SimpleChanges,
} from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
} from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../core/services/account.service';
import { CartService } from '../../core/services/cart.service';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutInformationComponent } from './checkout-information/checkout-information.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { CheckoutPaymentComponent } from './checkout-payment/checkout-payment.component';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    CheckoutDeliveryComponent,
    CheckoutInformationComponent,
    MatStepperModule,
    CheckoutReviewComponent,
    MatProgressSpinner,
    CheckoutPaymentComponent,
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit, OnChanges {
  isFormValid: boolean = false;
  onFormValidityChanged(isValid: boolean) {
    this.isFormValid = isValid;
  }
  cartService = inject(CartService);
  accountService = inject(AccountService);

  @Input() form!: FormGroup;
  @Input() selectedCity: string = '';

  saveInformation = false;
  cityControl = new FormControl('');
  deliveryMethod: any = null;
  paymentMethodControl: FormControl = new FormControl('', Validators.required);
  paymentMethod: any;
  constructor(private fb: FormBuilder) {
    if (!this.form) {
      this.form = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        phoneNumber: [
          '',
          [
            Validators.required,
            Validators.pattern(
              '^\\+?[0-9]{1,4}?[-. ]?(\\(?\\d{1,3}?\\)?[-. ]?)?\\d{1,4}[-. ]?\\d{1,4}[-. ]?\\d{1,4}$'
            ),
          ],
        ],
        city: ['', Validators.required],
        saveAsDefault: [false],
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['form'] && this.form) {
      this.selectedCity = this.form.get('city')?.value || '';
    }
  }

  ngOnInit(): void {
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

  async onStepChange(event: { selectedIndex: number }): Promise<void> {
    const { selectedIndex } = event;

    if (this.saveInformation) {
      await firstValueFrom(this.accountService.updateUserInfo(this.form.value));
    }

    switch (selectedIndex) {
      case 0:
        if (this.form.valid) {
          this.updateCompletionStatus('information');
        }
        break;
      case 1:
        const city = this.form.get('city')?.value;
        this.cityControl.setValue(city);
        this.updateCompletionStatus('delivery');
        break;
      case 2:
        this.updateCompletionStatus('payment');
        break;
      default:
        break;
    }
  }

  private updateCompletionStatus(
    _step: 'information' | 'delivery' | 'payment'
  ): void {}

  onCityChange(city: string): void {
    this.selectedCity = city;
  }

  onSaveInformationCheckboxChange(event: { checked: boolean }): void {
    this.saveInformation = event.checked;
  }
}
