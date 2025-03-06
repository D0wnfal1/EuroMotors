import { Component, inject } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { MatDivider } from '@angular/material/divider';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { Category } from '../../../shared/models/category';
import { CarModel } from '../../../shared/models/carModel';

@Component({
  selector: 'app-filters-dialog',
  standalone: true,
  imports: [
    MatDivider,
    MatSelectionList,
    MatListOption,
    MatButton,
    FormsModule
  ],
  templateUrl: './filters-dialog.component.html',
  styleUrl: './filters-dialog.component.scss'
})
export class FiltersDialogComponent {
  shopService = inject(ShopService);
  private dialogRef = inject(MatDialogRef<FiltersDialogComponent>);
  data = inject(MAT_DIALOG_DATA);

  selectedCategoryNames: string[] = this.data.selectedCategoriesNames;
  selectedCarModelBrands: string[] = this.data.selectedCarModelBrands;
  selectedCarModelModels: string[] = this.data.selectedCarModelModels;

  applyFilters() {
    this.dialogRef.close({
      selectedCategoryNames: this.selectedCategoryNames,
      selectedCarModelBrands: this.selectedCarModelBrands,
      selectedCarModelModels: this.selectedCarModelModels
    })
  }

}
