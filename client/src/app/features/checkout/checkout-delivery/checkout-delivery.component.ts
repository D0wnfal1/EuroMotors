import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  inject,
  output,
} from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { CheckoutService } from '../../../core/services/checkout.service';
import { Warehouse } from '../../../shared/models/warehouse';
import { FormControl } from '@angular/forms';
import {
  Observable,
  combineLatest,
  debounceTime,
  of,
  switchMap,
  tap,
} from 'rxjs';

@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    NgIf,
    NgFor,
    AsyncPipe,
    MatButtonModule,
    MatRadioModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatInputModule,
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss'],
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutService = inject(CheckoutService);
  deliveryMethod = '';
  deliveryFeeText: string = '';
  warehouseControl = new FormControl();
  queryControl = new FormControl('');
  @Input() cityControl!: FormControl;
  filteredWarehouses: Observable<Warehouse[]> = new Observable();
  isLoading = false;
  private isWarehouseSelected = false;
  deliveryForm = new FormControl('', Validators.required);
  @Output() deliveryMethodChanged: EventEmitter<string> = new EventEmitter();
  selectedDeliveryMethod: string = '';

  ngOnInit() {
    combineLatest([
      this.cityControl.valueChanges,
      this.queryControl.valueChanges,
    ])
      .pipe(
        debounceTime(1000),
        switchMap(([city, query]) => {
          if (!city) {
            this.warehouseControl.disable();
            return of([]);
          } else if (this.isWarehouseSelected) {
            return of([]);
          } else {
            this.warehouseControl.enable();
            const queryString = query ?? '';
            this.isLoading = true;
            return this.checkoutService.getWarehouses(city, queryString);
          }
        }),
        tap(() => {
          this.isLoading = false;
        })
      )
      .subscribe((warehouses) => {
        this.filteredWarehouses = of(warehouses);
      });
  }

  onDeliveryMethodChange(method: string) {
    this.selectedDeliveryMethod = method;
    this.deliveryMethodChanged.emit(this.selectedDeliveryMethod);

    if (this.selectedDeliveryMethod === 'delivery') {
      const city = this.cityControl.value;
      const queryString = this.queryControl.value ?? '';
      this.loadWarehouses(city, queryString);
    }
  }

  loadWarehouses(city: string, query: string) {
    if (city && !this.isWarehouseSelected) {
      this.isLoading = true;
      this.checkoutService
        .getWarehouses(city, query)
        .subscribe((warehouses) => {
          this.filteredWarehouses = of(warehouses);
          this.isLoading = false;
        });
    }
  }

  onPickupSelected(warehouse: Warehouse) {
    this.warehouseControl.setValue(warehouse);
    this.isWarehouseSelected = true;
  }

  displayWarehouse(warehouse: Warehouse): string {
    return warehouse ? warehouse.description : '';
  }

  get isFormValid() {
    return (
      this.deliveryMethod &&
      (this.deliveryMethod !== 'delivery' ||
        (this.cityControl.valid && this.queryControl.valid))
    );
  }
}
