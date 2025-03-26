import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';
import { PaymentMethod } from '../../../shared/models/order';

@Component({
  selector: 'app-checkout-payment',
  imports: [MatRadioModule, NgIf, ReactiveFormsModule],
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss',
})
export class CheckoutPaymentComponent implements OnInit {
  paymentForm!: FormGroup;
  PaymentMethod = PaymentMethod;

  ngOnInit() {
    this.paymentForm = new FormGroup({
      paymentMethod: new FormControl(
        PaymentMethod.Postpaid,
        Validators.required
      ),
    });
  }
}
