import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createOrder(orderData: any): Observable<{ orderId: string }> {
    return this.http.post<{ orderId: string }>(
      `${this.baseUrl}orders`,
      orderData
    );
  }

  getOrderById(orderId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}orders/${orderId}`);
  }

  changeOrderStatus(orderId: string, status: string): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}orders?id=${orderId}&status=${status}`,
      {}
    );
  }

  deleteOrder(orderId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}orders?id=${orderId}`);
  }
}
