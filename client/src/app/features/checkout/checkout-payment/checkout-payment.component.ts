import { NgIf } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-checkout-payment',
  imports: [MatRadioModule, NgIf, ReactiveFormsModule],
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss',
})
export class CheckoutPaymentComponent {
  paymentMethodControl: FormControl = new FormControl('', Validators.required);
  @Output() paymentMethodChanged: EventEmitter<string> = new EventEmitter();
  @Output() formValidityChanged: EventEmitter<boolean> = new EventEmitter();
  selectedPaymentMethod: string = '';

  onPaymentMethodChange(method: string) {
    this.selectedPaymentMethod = method;
    this.paymentMethodChanged.emit(this.selectedPaymentMethod);
    this.emitFormValidity();
  }

  private emitFormValidity() {
    this.formValidityChanged.emit(this.paymentMethodControl.valid);
  }

  get isFormValid() {
    return this.paymentMethodControl.valid;
  }
}
