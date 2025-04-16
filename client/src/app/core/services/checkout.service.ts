import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { WarehousesResponse, Warehouse } from '../../shared/models/warehouse';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getWarehouses(city: string, query: string): Observable<Warehouse[]> {
    const payload = { city, query };
    return this.http
      .post<WarehousesResponse>(
        this.baseUrl + '/deliveries/warehouses',
        payload
      )
      .pipe(
        map((response) => {
          if (response.success) {
            return response.data;
          }
        })
      );
  }

  requestCallback(name: string, phone: string): Observable<any> {
    const request = { name, phone };
    return this.http.post(this.baseUrl + '/callback/request', request);
  }
}
