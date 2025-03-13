import { Component, Input, Output, EventEmitter, output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCheckbox, MatCheckboxChange } from '@angular/material/checkbox';
import { NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

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
export class CheckoutInformationComponent {
  @Input() form!: FormGroup;
  @Input() saveInformation: boolean = false;
  @Output() saveInformationChange = new EventEmitter<MatCheckboxChange>();
  informationComplete = output<boolean>();

  onCheckboxChange(event: MatCheckboxChange) {
    this.saveInformationChange.emit(event);
    this.informationComplete.emit(true);
  }
}
