import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
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

  getCategories(
    paginationParams: PaginationParams
  ): Observable<{ count: number; data: Category[] }> {
    const params = new HttpParams()
      .append('pageSize', paginationParams.pageSize.toString())
      .append('pageNumber', paginationParams.pageNumber.toString());

    return this.http.get<{ count: number; data: Category[] }>(
      `${this.baseUrl}/categories`,
      { params }
    );
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/categories/${id}`, {
      withCredentials: true,
    });
  }

  getSubcategories(parentCategoryId: string): Observable<Category[]> {
    return this.http.get<Category[]>(
      `${this.baseUrl}/categories/${parentCategoryId}/subcategories`
    );
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
