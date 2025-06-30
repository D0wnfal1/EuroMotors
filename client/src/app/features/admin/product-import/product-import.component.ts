import { Component } from '@angular/core';
import { ProductImportService, ImportResult } from '../../../core/services/product-import.service';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-product-import',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatChipsModule,
    MatListModule
  ],
  templateUrl: './product-import.component.html',
  styleUrls: ['./product-import.component.scss']
})
export class ProductImportComponent {
  priceFile?: File;
  mappingFile?: File;
  isImporting = false;
  result?: ImportResult;
  errorMessage?: string;

  constructor(private productImportService: ProductImportService) {}

  onPriceFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.priceFile = input.files[0];
    }
  }

  onMappingFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.mappingFile = input.files[0];
    }
  }

  clearPriceFile(): void {
    this.priceFile = undefined;
  }

  clearMappingFile(): void {
    this.mappingFile = undefined;
  }

  importProducts(): void {
    if (!this.priceFile) {
      this.errorMessage = 'Файл прайсу є обов\'язковим';
      return;
    }

    this.isImporting = true;
    this.errorMessage = undefined;
    this.result = undefined;

    this.productImportService.importExcelProducts(this.priceFile, this.mappingFile)
      .pipe(finalize(() => this.isImporting = false))
      .subscribe({
        next: (result) => {
          this.result = result;
        },
        error: (error) => {
          console.error('Помилка імпорту товарів:', error);
          this.errorMessage = 'Не вдалося імпортувати товари. Будь ласка, спробуйте ще раз.';
        }
      });
  }
} 