import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { CarBrand } from '../../shared/models/carBrand';
import { PaginationParams } from '../../shared/models/paginationParams';

@Injectable({
  providedIn: 'root',
})
export class CarbrandService {
  baseUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);
  private readonly carBrandsSubject = new BehaviorSubject<CarBrand[]>([]);
  carBrands$ = this.carBrandsSubject.asObservable();
  private readonly totalItemsSubject = new BehaviorSubject<number>(0);
  totalItems$ = this.totalItemsSubject.asObservable();

  private readonly availableBrandsSubject = new BehaviorSubject<CarBrand[]>([]);
  availableBrands$ = this.availableBrandsSubject.asObservable();

  getCarBrands(paginationParams: PaginationParams) {
    let params = new HttpParams();
    params = params.append('pageSize', paginationParams.pageSize);
    params = params.append('pageNumber', paginationParams.pageNumber);

    this.http
      .get<{ count: number; data: CarBrand[] }>(this.baseUrl + '/carBrands', {
        params,
      })
      .subscribe({
        next: (response) => {
          this.carBrandsSubject.next(response.data);
          this.totalItemsSubject.next(response.count);
        },
        error: (err) => console.error('Failed to load car brands', err),
      });
  }

  getAllCarBrands() {
    const params = new HttpParams()
      .append('pageSize', '0')
      .append('pageNumber', '1');

    this.http
      .get<{ count: number; data: CarBrand[] }>(this.baseUrl + '/carBrands', {
        params,
      })
      .subscribe({
        next: (response) => {
          this.availableBrandsSubject.next(response.data);
        },
        error: (err) => console.error('Failed to load all car brands', err),
      });
  }

  getCarBrandById(id: string): Observable<CarBrand> {
    return this.http.get<CarBrand>(`${this.baseUrl}/carBrands/${id}`, {
      withCredentials: true,
    });
  }

  getCarBrandByName(name: string): Observable<CarBrand> {
    const params = new HttpParams().append('name', name);
    return this.http.get<CarBrand>(`${this.baseUrl}/carBrands/by-name`, {
      params,
      withCredentials: true,
    });
  }

  getCarBrandsByPattern(searchPattern: string): Observable<CarBrand[]> {
    const params = new HttpParams().append('pattern', searchPattern);
    return this.http.get<CarBrand[]>(`${this.baseUrl}/carBrands/search`, {
      params,
      withCredentials: true,
    });
  }

  createCarBrand(formData: FormData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/carBrands`, formData, {
      withCredentials: true,
    });
  }

  updateCarBrand(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/carBrands/${id}`, formData, {
      withCredentials: true,
    });
  }

  updateCarBrandName(id: string, name: string): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/carBrands/${id}`,
      { name },
      { withCredentials: true }
    );
  }

  deleteCarBrand(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/carBrands/${id}`, {
      withCredentials: true,
    });
  }

  clearCache(): void {
    this.carBrandsSubject.next([]);
    this.availableBrandsSubject.next([]);
  }
}
