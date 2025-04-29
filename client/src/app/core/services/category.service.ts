import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map, shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category, HierarchicalCategory } from '../../shared/models/category';
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

  private cachedHierarchicalCategories: {
    [key: string]: Observable<HierarchicalCategory[]>;
  } = {};
  private cachedCategories: Observable<Category[]> | null = null;

  getCategories(): void {
    this.cachedCategories ??= this.http
      .get<Category[]>(`${this.baseUrl}/categories`)
      .pipe(shareReplay(1));

    this.cachedCategories.subscribe((categories) => {
      this.categoriesSubject.next(categories);
    });
  }

  getHierarchicalCategories(
    params: PaginationParams
  ): Observable<HierarchicalCategory[]> {
    const cacheKey = `${params.pageNumber}_${params.pageSize}`;

    if (!this.cachedHierarchicalCategories[cacheKey]) {
      let httpParams = new HttpParams();
      httpParams = httpParams.append(
        'pageNumber',
        params.pageNumber.toString()
      );
      httpParams = httpParams.append('pageSize', params.pageSize.toString());

      this.cachedHierarchicalCategories[cacheKey] = this.http
        .get<any>(`${this.baseUrl}/categories/hierarchical`, {
          params: httpParams,
        })
        .pipe(
          map((response) => {
            this.totalItemsSubject.next(response.count);
            return response.data;
          }),
          shareReplay(1)
        );
    }

    return this.cachedHierarchicalCategories[cacheKey];
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http
      .get<Category>(`${this.baseUrl}/categories/${id}`)
      .pipe(shareReplay(1));
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
