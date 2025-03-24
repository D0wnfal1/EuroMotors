import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { CookieService } from 'ngx-cookie-service';
import { catchError, map, of, switchMap, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private cookieService = inject(CookieService);
  currentUser = signal<User | null>(null);
  isAdmin = computed(() => {
    const roles = this.currentUser()?.roles;
    return Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin';
  });

  login(values: any) {
    return this.http
      .post(this.baseUrl + '/users/login', values, { responseType: 'text' })
      .pipe(
        switchMap((token: string) => {
          if (token) {
            this.cookieService.set(
              'AuthToken',
              token,
              30,
              '/',
              undefined,
              true,
              'Strict'
            );
            return this.getUserInfo();
          }
          return of(null);
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => new Error(error));
        })
      );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + '/users/register', values);
  }

  getUserInfo() {
    return this.http.get<User>(this.baseUrl + '/users/email').pipe(
      map((user) => {
        this.currentUser.set(user);
        return user;
      })
    );
  }

  updateUserInfo(values: {
    email: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    city: string;
  }) {
    const body = {
      email: values.email,
      firstName: values.firstName,
      lastName: values.lastName,
      phoneNumber: values.phoneNumber,
      city: values.city,
    };

    return this.http.put(this.baseUrl + '/users/update', body).pipe(
      map(() => {
        this.getUserInfo().subscribe();
      })
    );
  }

  logout() {
    this.cookieService.delete('AuthToken', '/');
    this.currentUser.set(null);
  }

  getAuthState() {
    const token = this.cookieService.get('AuthToken');
    if (!token) {
      return of({ isAuthenticated: false });
    }
    return this.http.get<{ isAuthenticated: boolean }>(
      this.baseUrl + '/users/auth-status',
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
  }
}
