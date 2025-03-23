import { Injectable, inject } from '@angular/core';
import { CarModel } from '../../shared/models/carModel';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { PaginationParams } from '../../shared/models/paginationParams';

@Injectable({
  providedIn: 'root',
})
export class CarmodelService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private carModelsSubject = new BehaviorSubject<CarModel[]>([]);
  carModels$ = this.carModelsSubject.asObservable();
  private totalItemsSubject = new BehaviorSubject<number>(0);
  totalItems$ = this.totalItemsSubject.asObservable();

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
    return this.http.get<CarModel>(`${this.baseUrl}/carModels/${id}`);
  }

  createCarModel(formData: FormData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/carModels`, formData);
  }

  updateCarModel(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/carModels/${id}`, formData);
  }

  deleteCarModel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/carModels/${id}`);
  }
}
