import { NgFor, CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { RouterLink } from '@angular/router';
import { ShopParams } from '../../../shared/models/shopParams';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CarModel } from '../../../shared/models/carModel';

@Component({
  selector: 'app-admin-carmodels',
  imports: [
    ReactiveFormsModule,
    MatPaginator,
    RouterLink,
    NgFor,
    MatButton,
    CommonModule,
    FormsModule,
  ],
  templateUrl: './admin-carmodels.component.html',
  styleUrl: './admin-carmodels.component.scss',
})
export class AdminCarmodelsComponent implements OnInit {
  private carModelService = inject(CarmodelService);
  carModels: CarModel[] = [];
  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  count = 0;
  ngOnInit() {
    this.getCarModels();
  }

  getCarModels() {
    this.carModelService.getCarModels(this.shopParams);
    this.carModelService.carModels$.subscribe((response) => {
      this.carModels = response;
    });

    this.carModelService.totalItems$.subscribe((count) => {
      this.totalItems = count;
    });
  }

  getCarModelName(carModelId: string): string {
    return this.carModels.find((c) => c.id === carModelId)?.brand || '—';
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getCarModels();
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getCarModels();
    }
  }

  deleteCarModel(carModelId: string): void {
    this.carModelService.deleteCarModel(carModelId).subscribe(() => {
      if (this.carModels) {
        this.carModels = this.carModels.filter(
          (p: CarModel) => p.id !== carModelId
        );
      }
    });
  }
}
