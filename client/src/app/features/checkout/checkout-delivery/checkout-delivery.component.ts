import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  inject,
} from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import {
  AbstractControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
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

function deliveryMethodValidator(
  group: AbstractControl
): { [key: string]: any } | null {
  const method = group.get('method')?.value;
  const warehouse = group.get('warehouse')?.value;
  if (method === 'delivery' && !warehouse) {
    return { warehouseRequired: true };
  }
  return null;
}

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
export class CheckoutDeliveryComponent implements OnInit, OnChanges {
  checkoutService = inject(CheckoutService);
  @Input() city: string = '';
  deliveryGroup: FormGroup;

  cityControl = new FormControl<string>('', Validators.required);
  queryControl = new FormControl<string>('');

  filteredWarehouses: Observable<Warehouse[]> = of([]);
  isLoading = false;
  isWarehouseSelected = false;
  @Output() warehouseSelected: EventEmitter<string> =
    new EventEmitter<string>();
  selectedWarehouseDescription: string = '';

  @Output() deliveryMethodChanged: EventEmitter<string> =
    new EventEmitter<string>();

  constructor() {
    this.deliveryGroup = new FormGroup(
      {
        method: new FormControl<string>('pickup', Validators.required),
        warehouse: new FormControl<Warehouse | null>(null),
      },
      { validators: deliveryMethodValidator }
    );
  }

  ngOnInit(): void {
    const methodControl = this.deliveryGroup.get('method');
    if (methodControl) {
      methodControl.valueChanges.subscribe((value: string | null) => {
        const method = value ?? 'pickup';
        this.deliveryMethodChanged.emit(method);
        // if (method === 'delivery') {
        //   const city = this.cityControl.value ?? '';
        //   const query = this.queryControl.value ?? '';
        //   this.loadWarehouses(city, query);
        // }
      });
    }

    combineLatest([
      this.cityControl.valueChanges,
      this.queryControl.valueChanges,
    ])
      .pipe(
        debounceTime(1000),
        switchMap(([city, query]) => {
          if (!city) {
            return of([]);
          } else if (this.isWarehouseSelected) {
            return of([]);
          } else {
            this.isLoading = true;
            return this.checkoutService.getWarehouses(city, query ?? '');
          }
        }),
        tap(() => {
          this.isLoading = false;
        })
      )
      .subscribe((warehouses: Warehouse[]) => {
        this.filteredWarehouses = of(warehouses);
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['city'] && changes['city'].currentValue) {
      this.cityControl.setValue(changes['city'].currentValue);
      const query = this.queryControl.value ?? '';
      // setTimeout(() => {
      //   this.loadWarehouses(changes['city'].currentValue, query);
      // }, 1000);
    }
  }

  onDeliveryMethodChange(method: string | null): void {
    if (method === 'delivery') {
      const city = this.cityControl.value ?? '';
      const query = this.queryControl.value ?? '';
      this.loadWarehouses(city, query);
    }
  }

  loadWarehouses(city: string, query: string): void {
    if (city && !this.isWarehouseSelected) {
      this.isLoading = true;
      this.checkoutService
        .getWarehouses(city, query)
        .subscribe((warehouses: Warehouse[]) => {
          this.filteredWarehouses = of(warehouses);
          this.isLoading = false;
        });
    }
  }

  onPickupSelected(warehouse: Warehouse): void {
    const warehouseControl = this.deliveryGroup.get('warehouse');
    if (warehouseControl) {
      warehouseControl.setValue(warehouse);
      this.isWarehouseSelected = true;
      this.selectedWarehouseDescription = warehouse.description;
      // Emit the selected warehouse name to parent component
      this.warehouseSelected.emit(warehouse.description);
    }
  }

  displayWarehouse(warehouse: Warehouse | null): string {
    return warehouse ? warehouse.description : '';
  }
}
