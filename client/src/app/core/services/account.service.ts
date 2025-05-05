import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { User } from '../../shared/models/user';
import {
  BehaviorSubject,
  catchError,
  filter,
  map,
  Observable,
  of,
  switchMap,
  take,
  tap,
  throwError,
} from 'rxjs';
import {
  LoginRequest,
  RegisterRequest,
  UpdateUserRequest,
  AuthenticationResponse,
  AuthState,
} from '../../shared/models/account';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private readonly baseUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);

  readonly currentUser = signal<User | null>(null);
  private readonly isRefreshing = signal<boolean>(false);
  private readonly refreshTokenSubject = new BehaviorSubject<boolean | null>(
    null
  );

  readonly isAdmin = computed(() => {
    const roles = this.currentUser()?.roles;
    return Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin';
  });

  readonly isAuthenticated = computed(() => {
    return this.currentUser() !== null;
  });

  login(values: LoginRequest): Observable<User> {
    return this.http
      .post<AuthenticationResponse>(this.baseUrl + '/users/login', values, {
        withCredentials: true,
      })
      .pipe(
        switchMap(() => this.checkAuth()),
        map((state) => {
          if (!state.user) {
            throw new Error('Login failed');
          }
          return state.user;
        }),
        catchError(this.handleError)
      );
  }

  refreshToken(): Observable<AuthState> {
    if (this.isRefreshing()) {
      return this.refreshTokenSubject.pipe(
        filter((result): result is boolean => result !== null),
        take(1),
        switchMap((success) => {
          if (!success) {
            return throwError(
              () => new Error('Previous refresh attempt failed')
            );
          }
          return this.checkAuth();
        })
      );
    }

    this.isRefreshing.set(true);
    return this.http
      .post<AuthenticationResponse>(
        `${this.baseUrl}/auth/refresh`,
        {},
        { withCredentials: true }
      )
      .pipe(
        tap((response) => {
          this.isRefreshing.set(false);
          this.refreshTokenSubject.next(true);
        }),
        switchMap(() => this.checkAuth()),
        catchError((error) => {
          console.error('Token refresh failed:', error);
          this.isRefreshing.set(false);
          this.refreshTokenSubject.next(false);
          return throwError(() => error);
        })
      );
  }

  checkAuth(): Observable<AuthState> {
    if (this.currentUser()) {
      return of({
        isAuthenticated: true,
        user: this.currentUser(),
      });
    }

    return this.http
      .get<AuthState>(this.baseUrl + '/auth/status', { withCredentials: true })
      .pipe(
        tap((state) => {
          this.currentUser.set(state.user);
        }),
        catchError((error) => {
          this.currentUser.set(null);
          return throwError(() => error);
        })
      );
  }

  register(values: RegisterRequest): Observable<any> {
    return this.http
      .post(this.baseUrl + '/users/register', values, {
        withCredentials: true,
      })
      .pipe(catchError(this.handleError));
  }

  updateUserInfo(values: UpdateUserRequest): Observable<User> {
    return this.http
      .put<void>(this.baseUrl + '/users/update', values, {
        withCredentials: true,
      })
      .pipe(
        switchMap(() => this.checkAuth()),
        map((state) => {
          if (!state.user) {
            throw new Error('Update failed');
          }
          return state.user;
        }),
        catchError(this.handleError)
      );
  }

  logout(): Observable<any> {
    return this.http
      .post(this.baseUrl + '/users/logout', {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this.currentUser.set(null);
          this.deleteCookie('AccessToken');
          this.deleteCookie('RefreshToken');
        }),
        catchError(this.handleError)
      );
  }

  private deleteCookie(name: string): void {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else {
      errorMessage = error.error?.error ?? error.message;
    }

    return throwError(() => new Error(errorMessage));
  }
}
