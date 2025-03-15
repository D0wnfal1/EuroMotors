import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Payment } from '../../shared/models/payment';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  createPayment(orderId: string) {
    return this.http.post<{ data: string; signature: string }>(
      `${this.baseUrl}payments?orderId=${orderId}`,
      {}
    );
  }

  getPaymentById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`${this.baseUrl}payments/${id}`);
  }
}
