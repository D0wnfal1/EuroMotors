import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Order } from '../../shared/models/order';
import { OrderParams } from '../../shared/models/orderParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createOrder(orderData: any): Observable<{ orderId: string }> {
    return this.http.post<{ orderId: string }>(
      `${this.baseUrl}/orders`,
      orderData
    );
  }

  getOrders(orderParams: OrderParams) {
    let params = new HttpParams();
    params = params.append('pageSize', orderParams.pageSize);
    params = params.append('pageNumber', orderParams.pageNumber);
    if (orderParams.filter) {
      params = params.set('status', orderParams.filter);
    }
    return this.http.get<Pagination<Order>>(this.baseUrl + '/orders', {
      params,
    });
  }

  getOrderById(orderId: string): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/orders/${orderId}`);
  }

  getUserOrders(userId: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.baseUrl}/orders/${userId}/user`);
  }

  changeOrderStatus(orderId: string, status: string): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/orders?id=${orderId}&status=${status}`,
      {}
    );
  }

  deleteOrder(orderId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/orders?id=${orderId}`);
  }
}
