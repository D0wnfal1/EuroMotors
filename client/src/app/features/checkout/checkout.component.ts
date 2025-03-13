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
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';
import { CheckoutInformationComponent } from './checkout-information/checkout-information.component';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { RouterLink } from '@angular/router';

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
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit, OnChanges {
  cartService = inject(CartService);
  accountService = inject(AccountService);
  saveInformation = false;
  @Input() form!: FormGroup; // To receive form from the parent
  @Input() selectedCity: string = '';

  // Статус завершения шагов
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
      // Переходимо до Shipping і передаємо місто
      const city = this.form.controls['city'].value;
      this.form.controls['city'].setValue(city); // Це дозволить зберегти місто в Delivery
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
    this.selectedCity = city; // Обновляем город
  }

  onSaveAddressCheckboxChange(event: any) {
    this.saveInformation = (event as MatCheckboxChange).checked;
  }
}
