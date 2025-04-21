import { NgFor, CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { RouterLink } from '@angular/router';
import { ShopParams } from '../../../shared/models/shopParams';
import { CarbrandService } from '../../../core/services/carbrand.service';
import { CarBrand } from '../../../shared/models/carBrand';
import { ImageService } from '../../../core/services/image.service';
import { MatTableModule } from '@angular/material/table';
import { CarmodelService } from '../../../core/services/carmodel.service';

@Component({
  selector: 'app-admin-carbrands',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatPaginator,
    RouterLink,
    MatButton,
    CommonModule,
    FormsModule,
    MatTableModule,
  ],
  templateUrl: './admin-carbrands.component.html',
  styleUrl: './admin-carbrands.component.scss',
})
export class AdminCarbrandsComponent implements OnInit {
  private carBrandService = inject(CarbrandService);
  private carModelService = inject(CarmodelService);
  private imageService = inject(ImageService);
  carBrands: CarBrand[] = [];
  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  brandModelsCount: { [brandId: string]: number } = {};

  displayedColumns: string[] = ['logo', 'name', 'models', 'actions'];

  ngOnInit() {
    this.getCarBrands();
    this.loadModelsCounts();
  }

  getCarBrands() {
    this.carBrandService.getCarBrands(this.shopParams);
    this.carBrandService.carBrands$.subscribe((response) => {
      this.carBrands = response;
    });

    this.carBrandService.totalItems$.subscribe((count) => {
      this.totalItems = count;
    });
  }

  loadModelsCounts() {
    const params = new ShopParams();
    params.pageSize = 1000;
    params.pageNumber = 1;

    this.carModelService.getCarModels(params);
    this.carModelService.carModels$.subscribe((models) => {
      const counts: { [brandId: string]: number } = {};

      models.forEach((model) => {
        if (model.carBrandId) {
          counts[model.carBrandId] = (counts[model.carBrandId] || 0) + 1;
        }
      });

      this.brandModelsCount = counts;
    });
  }

  getModelsCount(brandId: string): number {
    return this.brandModelsCount[brandId] || 0;
  }

  getCarBrandLogo(logoPath?: string): string {
    return logoPath
      ? this.imageService.getImageUrl(logoPath)
      : '/images/no-image.jpeg';
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getCarBrands();
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sortOrder = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getCarBrands();
    }
  }

  deleteCarBrand(carBrandId: string): void {
    this.carBrandService.deleteCarBrand(carBrandId).subscribe(() => {
      if (this.carBrands) {
        this.carBrands = this.carBrands.filter(
          (b: CarBrand) => b.id !== carBrandId
        );
      }
    });
  }
}
