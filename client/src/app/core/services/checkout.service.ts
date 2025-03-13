import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getWarehouses(cityName: string, query: string): Observable<any> {
    return this.http.post<any>(this.baseUrl + 'deliveries/warehouses', {
      cityName,
      query,
    });
  }
}
