import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    NgIf,
    MatButtonModule,
    MatRadioModule,
    MatSelectModule,
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss'],
})
export class CheckoutDeliveryComponent {
  deliveryMethod: any;
  onPickupSelected($event: MatSelectChange<any>) {
    throw new Error('Method not implemented.');
  }
}
