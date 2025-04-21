import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category } from '../../shared/models/category';
import { PaginationParams } from '../../shared/models/paginationParams';

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

  getCategories(): void {
    this.http
      .get<Category[]>(`${this.baseUrl}/categories`)
      .subscribe((categories) => {
        this.categoriesSubject.next(categories);
      });
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/categories/${id}`, {
      withCredentials: true,
    });
  }

  getParentCategories(
    paginationParams: PaginationParams
  ): Observable<Category[]> {
    let params = new HttpParams()
      .set('pageSize', paginationParams.pageSize)
      .set('pageNumber', paginationParams.pageNumber);

    return this.http
      .get<{ data: Category[]; count: number }>(
        `${this.baseUrl}/categories/parentCategories`,
        { params }
      )
      .pipe(
        map((res) => {
          this.categoriesSubject.next(res.data);
          this.totalItemsSubject.next(res.count);
          return res.data;
        })
      );
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

  setCategoryAvailability(id: string, isAvailable: boolean): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/categories/${id}`,
      { isAvailable },
      { withCredentials: true }
    );
  }
}
