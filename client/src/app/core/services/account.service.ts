import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { catchError, map, switchMap, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);
  isAdmin = computed(() => {
    const roles = this.currentUser()?.roles;
    return Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin';
  });

  login(values: any) {
    return this.http
      .post(this.baseUrl + '/users/login', values, {
        responseType: 'text',
        withCredentials: true,
      })
      .pipe(
        switchMap(() => {
          return this.getUserInfo();
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => new Error(error));
        })
      );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + '/users/register', values, {
      withCredentials: true,
    });
  }

  getUserInfo() {
    return this.http
      .get<User>(this.baseUrl + '/users/email', { withCredentials: true })
      .pipe(
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

    return this.http
      .put(this.baseUrl + '/users/update', body, { withCredentials: true })
      .pipe(
        tap(() => {
          this.getUserInfo().subscribe();
        })
      );
  }

  logout() {
    return this.http
      .post(this.baseUrl + '/users/logout', {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this.currentUser.set(null);
          this.deleteCookie('AuthToken');
        }),
        catchError((error) => {
          console.error('Logout error:', error);
          return throwError(() => new Error(error));
        })
      );
  }

  private deleteCookie(name: string) {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  }

  getAuthState() {
    return this.http.get<{ isAuthenticated: boolean }>(
      this.baseUrl + '/users/auth-status',
      { withCredentials: true }
    );
  }
}
