import { Injectable, inject } from '@angular/core';
import { CarModel, CarSelectionFilter } from '../../shared/models/carModel';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { PaginationParams } from '../../shared/models/paginationParams';

@Injectable({
  providedIn: 'root',
})
export class CarmodelService {
  baseUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);
  private readonly carModelsSubject = new BehaviorSubject<CarModel[]>([]);
  carModels$ = this.carModelsSubject.asObservable();
  private readonly totalItemsSubject = new BehaviorSubject<number>(0);
  totalItems$ = this.totalItemsSubject.asObservable();

  private readonly carIdsSubject = new BehaviorSubject<string[]>([]);
  private readonly brandsSubject = new BehaviorSubject<string[]>([]);
  private readonly modelsSubject = new BehaviorSubject<string[]>([]);
  private readonly yearsSubject = new BehaviorSubject<number[]>([]);
  private readonly bodyTypesSubject = new BehaviorSubject<string[]>([]);
  private readonly engineSpecsSubject = new BehaviorSubject<string[]>([]);

  private readonly allBrandsSubject = new BehaviorSubject<string[]>([]);

  public readonly carSelectionChanged = new BehaviorSubject<boolean>(false);

  public carIds$ = this.carIdsSubject.asObservable();
  public brands$ = this.brandsSubject.asObservable();
  public models$ = this.modelsSubject.asObservable();
  public years$ = this.yearsSubject.asObservable();
  public bodyTypes$ = this.bodyTypesSubject.asObservable();
  public engineSpecs$ = this.engineSpecsSubject.asObservable();
  public allBrands$ = this.allBrandsSubject.asObservable();

  getCarModels(paginationParams: PaginationParams) {
    let params = new HttpParams();
    params = params.append('pageSize', paginationParams.pageSize);
    params = params.append('pageNumber', paginationParams.pageNumber);

    this.http
      .get<{ count: number; data: CarModel[] }>(this.baseUrl + '/carModels', {
        params,
      })
      .subscribe({
        next: (response) => {
          this.carModelsSubject.next(response.data);
          this.totalItemsSubject.next(response.count);
        },
        error: (err) => console.error('Failed to load carModels', err),
      });
  }

  getCarModelById(id: string): Observable<CarModel> {
    return this.http.get<CarModel>(`${this.baseUrl}/carModels/${id}`, {
      withCredentials: true,
    });
  }

  getAllBrands(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/carModels/brands`).pipe(
      map((brands) => {
        this.allBrandsSubject.next(brands);
        return brands;
      })
    );
  }

  getCarSelectionWithIds(filter?: CarSelectionFilter): Observable<{
    ids: string[];
    brands: string[];
    models: string[];
    years: number[];
    bodyTypes: string[];
    engineSpecs: string[];
  }> {
    let params = new HttpParams();

    if (filter) {
      if (filter.brand) params = params.append('brand', filter.brand);
      if (filter.model) params = params.append('model', filter.model);
      if (filter.startYear)
        params = params.append('startYear', filter.startYear.toString());
      if (filter.bodyType) params = params.append('bodyType', filter.bodyType);
    }

    return this.http
      .get<{
        ids: string[];
        brands: string[];
        models: string[];
        years: number[];
        bodyTypes: string[];
        engineSpecs: string[];
      }>(`${this.baseUrl}/carModels/selection`, { params })
      .pipe(
        map((response) => {
          this.carIdsSubject.next(response.ids);
          this.brandsSubject.next(response.brands);
          this.modelsSubject.next(response.models);
          this.yearsSubject.next(response.years);
          this.bodyTypesSubject.next(response.bodyTypes);
          this.engineSpecsSubject.next(response.engineSpecs);
          return response;
        })
      );
  }

  createCarModel(formData: FormData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/carModels`, formData, {
      withCredentials: true,
    });
  }

  updateCarModel(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/carModels/${id}`, formData, {
      withCredentials: true,
    });
  }

  deleteCarModel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/carModels/${id}`, {
      withCredentials: true,
    });
  }

  saveCarSelection(carSelectionId: string): void {
    localStorage.setItem('selectedCarId', carSelectionId);
    this.carSelectionChanged.next(true);
  }

  getStoredCarId(): string | null {
    const storedCarId = localStorage.getItem('selectedCarId');
    return storedCarId ?? null;
  }

  getSelectedCarDetails(carId: string): Observable<CarModel> {
    return this.getCarModelById(carId);
  }

  clearCarSelection(): void {
    localStorage.removeItem('selectedCarId');
    this.carSelectionChanged.next(true);
  }
}
