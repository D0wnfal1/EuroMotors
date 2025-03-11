import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Payment } from '../../shared/models/payment';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  constructor(private http: HttpClient) {}

  createPayment(paymentRequest: any): Observable<Payment> {
    return this.http.post<Payment>('/api/payments', paymentRequest);
  }

  getPaymentById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`/api/payments/${id}`);
  }
}
