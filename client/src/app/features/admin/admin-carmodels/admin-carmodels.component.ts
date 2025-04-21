import { NgFor, CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { RouterLink } from '@angular/router';
import { ShopParams } from '../../../shared/models/shopParams';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CarbrandService } from '../../../core/services/carbrand.service';
import { CarModel } from '../../../shared/models/carModel';
import { ImageService } from '../../../core/services/image.service';
import { MatTableModule } from '@angular/material/table';
import { forkJoin } from 'rxjs';
import { CarBrand } from '../../../shared/models/carBrand';

@Component({
  selector: 'app-admin-carmodels',
  imports: [
    ReactiveFormsModule,
    MatPaginator,
    RouterLink,
    MatButton,
    CommonModule,
    FormsModule,
    MatTableModule,
  ],
  templateUrl: './admin-carmodels.component.html',
  styleUrl: './admin-carmodels.component.scss',
})
export class AdminCarmodelsComponent implements OnInit {
  private carModelService = inject(CarmodelService);
  private carBrandService = inject(CarbrandService);
  private imageService = inject(ImageService);
  carModels: CarModel[] = [];
  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  count = 0;

  brandCache: { [brandId: string]: CarBrand } = {};

  displayedColumns: string[] = [
    'brand',
    'model',
    'startYear',
    'bodyType',
    'engine',
    'actions',
  ];

  ngOnInit() {
    this.getCarModels();
    this.loadAllBrands();
  }

  getCarModels() {
    this.carModelService.getCarModels(this.shopParams);
    this.carModelService.carModels$.subscribe((response) => {
      this.carModels = response;

      this.loadMissingBrands();
    });

    this.carModelService.totalItems$.subscribe((count) => {
      this.totalItems = count;
    });
  }

  loadAllBrands() {
    const brandsParams = new ShopParams();
    brandsParams.pageSize = 1000;
    brandsParams.pageNumber = 1;

    this.carBrandService.getCarBrands(brandsParams);
    this.carBrandService.carBrands$.subscribe((brands) => {
      brands.forEach((brand) => {
        this.brandCache[brand.id] = brand;
      });
    });
  }

  loadMissingBrands() {
    const missingBrandIds = new Set<string>();

    this.carModels.forEach((model) => {
      if (
        model.carBrandId &&
        (!model.carBrand || !model.carBrand.name) &&
        !this.brandCache[model.carBrandId]
      ) {
        missingBrandIds.add(model.carBrandId);
      }
    });

    if (missingBrandIds.size > 0) {
      const requests = Array.from(missingBrandIds).map((id) =>
        this.carBrandService.getCarBrandById(id)
      );

      if (requests.length > 0) {
        forkJoin(requests).subscribe((brands) => {
          brands.forEach((brand) => {
            if (brand) {
              this.brandCache[brand.id] = brand;
            }
          });
        });
      }
    }
  }

  getBrandName(carModel: CarModel): string {
    if (!carModel) return '—';

    if (carModel.brandName) {
      return carModel.brandName;
    }

    if (carModel.carBrandId && this.brandCache[carModel.carBrandId]) {
      return this.brandCache[carModel.carBrandId].name;
    }

    return '—';
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

  getEngineInfo(carModel: CarModel): string {
    if (!carModel.volumeLiters && !carModel.fuelType) return '—';

    return `${carModel.volumeLiters}L ${carModel.fuelType}`;
  }
}
