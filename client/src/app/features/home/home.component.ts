import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  categories: any[] = [];

  ngOnInit(): void {
    this.http.get<any>(`${this.baseUrl}categories`).subscribe({
      next: (response) => (this.categories = response),
      error: (error) => console.error(error),
      complete: () => {
        console.log('complete');
      },
    });
  }
}
