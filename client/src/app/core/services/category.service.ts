import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category } from '../../shared/models/category';
import { PaginationParams } from '../../shared/models/paginationParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private categoriesSubject = new BehaviorSubject<Category[]>([]);
  categories$ = this.categoriesSubject.asObservable();
  private totalItemsSubject = new BehaviorSubject<number>(0);
  totalItems$ = this.totalItemsSubject.asObservable();

  getCategories(paginationParams: PaginationParams) {
    let params = new HttpParams();
    params = params.append('pageSize', paginationParams.pageSize);
    params = params.append('pageNumber', paginationParams.pageNumber);

    this.http
      .get<{ count: number; data: Category[] }>(this.baseUrl + '/categories', {
        params,
      })
      .subscribe({
        next: (response) => {
          this.categoriesSubject.next(response.data);
          this.totalItemsSubject.next(response.count);
        },
        error: (err) => console.error('Failed to load categories', err),
      });
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/categories/${id}`, {
      withCredentials: true,
    });
  }

  createCategory(formData: FormData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/categories`, formData, {
      withCredentials: true,
    });
  }

  updateCategory(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/categories/${id}`, formData, {
      withCredentials: true,
    });
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/categories/${id}`, {
      withCredentials: true,
    });
  }
}
