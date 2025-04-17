import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
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
import { MatCardModule } from '@angular/material/card';
import { ShopParams } from '../../shared/models/shopParams';
import { SelectedCar } from '../../shared/models/carModel';

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

  shopParams = new ShopParams();
  pageSize = 10;
  pageNumber = 1;

  private subscriptions: Subscription[] = [];

  ngOnInit() {
    this.shopParams.pageSize = this.pageSize;
    this.shopParams.pageNumber = this.pageNumber;
    this.categoryService
      .getParentCategories(this.shopParams)
      .subscribe((categories) => {
        this.categories = categories;
      });

    this.loadSelectedCar();
    this.subscriptions.push(
      this.carModelService.carSelectionChanged.subscribe(() => {
        this.loadSelectedCar();
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  loadSelectedCar() {
    const carId = this.carModelService.getStoredCarId();
    if (carId) {
      this.carModelService.getSelectedCarDetails(carId).subscribe({
        next: (carModel) => {
          this.selectedCar = {
            brand: carModel.brand,
            model: carModel.model,
          };
        },
        error: (err) => {
          console.error('Failed to load selected car details', err);
          this.clearCarSelection(); // Clear invalid selection
        },
      });
    } else {
      this.selectedCar = null;
    }
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
