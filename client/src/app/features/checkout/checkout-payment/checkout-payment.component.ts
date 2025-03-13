import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-checkout-payment',
  imports: [MatRadioModule, NgIf, ReactiveFormsModule],
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss',
})
export class CheckoutPaymentComponent implements OnInit {
  paymentForm!: FormGroup;
  isFormValid: boolean = true;

  ngOnInit() {
    this.paymentForm = new FormGroup({
      paymentMethod: new FormControl('', Validators.required),
    });

    this.paymentForm.statusChanges.subscribe((status) => {
      this.isFormValid = status === 'VALID';
    });
  }
}
