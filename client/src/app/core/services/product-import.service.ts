import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ImportResult {
  totalProcessed: number;
  successfullyImported: number;
  failed: number;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ProductImportService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) { }

  importExcelProducts(priceFile: File, mappingFile?: File): Observable<ImportResult> {
    const formData = new FormData();
    formData.append('priceFile', priceFile);

    if (mappingFile) {
      formData.append('mappingFile', mappingFile);
    }

    return this.http.post<ImportResult>(`${this.baseUrl}/product-import/excel`, formData);
  }
} 