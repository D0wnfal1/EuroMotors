import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy.service';
import { MatProgressBar } from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';
import { NgIf } from '@angular/common';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../shared/models/category';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { CarmodelService } from '../../core/services/carmodel.service';
import { Subscription } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCard, MatCardModule } from '@angular/material/card';

interface SelectedCar {
  brand: string;
  model: string;
  year: number;
  bodyType: string;
  engineSpec: string;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem,
    NgIf,
    MatToolbarModule,
    MatTooltipModule,
    MatCardModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit, OnDestroy {
  busyService = inject(BusyService);
  cartService = inject(CartService);
  accountService = inject(AccountService);
  categoryService = inject(CategoryService);
  carModelService = inject(CarmodelService);
  private router = inject(Router);

  categories: Category[] = [];
  activeCategory: Category | null = null;
  subcategories: Category[] = [];
  subcategoriesVisible = false;
  selectedCar: SelectedCar | null = null;

  private subscriptions: Subscription[] = [];

  ngOnInit() {
    this.categoryService.getParentCategories().subscribe((categories) => {
      this.categories = categories;
    });

    // Подписываемся на изменения выбранного автомобиля
    this.loadSelectedCar();
    this.subscriptions.push(
      this.carModelService.carSelectionChanged.subscribe(() => {
        this.loadSelectedCar();
      })
    );
  }

  ngOnDestroy() {
    // Отписываемся от всех подписок при уничтожении компонента
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  loadSelectedCar() {
    this.selectedCar = this.carModelService.getStoredCarSelection();
  }

  clearCarSelection() {
    this.carModelService.clearCarSelection();
    this.selectedCar = null;
  }

  toggleSubcategories(category: Category) {
    if (this.activeCategory?.id === category.id) {
      this.subcategoriesVisible = !this.subcategoriesVisible;
      if (!this.subcategoriesVisible) {
        this.activeCategory = null;
      }
    } else {
      this.activeCategory = category;
      this.subcategoriesVisible = true;
      this.loadSubcategories(category);
    }
  }

  loadSubcategories(category: Category) {
    this.categoryService
      .getSubcategories(category.id)
      .subscribe((subcategories) => {
        this.subcategories = subcategories;
      });
  }

  closeSubcategories() {
    this.subcategoriesVisible = false;
    this.activeCategory = null;
  }

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        console.error('Logout failed', err);
      },
    });
  }
}
