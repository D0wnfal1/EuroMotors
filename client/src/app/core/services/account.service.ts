import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { CookieService } from 'ngx-cookie-service';
import { catchError, map, of, tap } from 'rxjs';

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
        map((token: string) => {
          if (token) {
            console.log('Token received:', token); // Добавляем лог
            this.cookieService.set('AuthToken', token, {
              secure: true,
              sameSite: 'Strict',
              expires: 30,
            });
            return this.getUserInfo();
          }
          return '';
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return [];
        })
      );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + '/users/register', values);
  }

  getUserInfo() {
    const token = this.cookieService.get('AuthToken');

    if (!token) {
      console.error('Token is missing!');
      this.currentUser.set(null);
      return of(null);
    }

    return this.http
      .get<User>(this.baseUrl + '/users/email', {
        headers: { Authorization: `Bearer ${token}` },
      })
      .pipe(
        tap((user) => {
          this.currentUser.set(user);
        }),
        catchError((error) => {
          console.error('Error fetching user info', error);
          this.currentUser.set(null);
          return of(null);
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
    this.cookieService.delete('AuthToken');
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
