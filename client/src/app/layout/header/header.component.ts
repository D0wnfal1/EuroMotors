import {
  Component,
  OnInit,
  OnDestroy,
  inject,
  EventEmitter,
  Output,
} from '@angular/core';
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
import { Category, HierarchicalCategory } from '../../shared/models/category';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { CarmodelService } from '../../core/services/carmodel.service';
import { Subscription } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { ShopParams } from '../../shared/models/shopParams';
import { SelectedCar } from '../../shared/models/carModel';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';

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
    FormsModule,
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
  productService = inject(ProductService);
  private router = inject(Router);

  categories: Category[] = [];
  hierarchicalCategories: HierarchicalCategory[] = [];
  activeCategory: Category | null = null;
  subcategories: Category[] = [];
  subcategoriesVisible = false;
  selectedCar: SelectedCar | null = null;
  selectedCarId: string | null = null;

  shopParams = new ShopParams();
  pageSize = 0;
  pageNumber = 0;

  private subscriptions: Subscription[] = [];

  ngOnInit() {
    this.shopParams.pageSize = this.pageSize;
    this.shopParams.pageNumber = this.pageNumber;
    this.loadCategories();
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

  loadCategories() {
    this.categoryService.getHierarchicalCategories(this.shopParams).subscribe({
      next: (response) => {
        this.hierarchicalCategories = response.data;
        this.categories = this.hierarchicalCategories.map((cat) => ({
          id: cat.id,
          name: cat.name,
          isAvailable: cat.isAvailable,
          imagePath: cat.imagePath,
          slug: cat.slug,
          products: [],
          subcategoryNames: cat.subCategories?.map((sub) => sub.name) || [],
        }));
      },
      error: (err) => {
        console.error('Failed to load categories', err);
      },
    });
  }

  loadSelectedCar() {
    const carId = this.carModelService.getStoredCarId();
    this.selectedCarId = carId;
    if (carId) {
      this.carModelService.getSelectedCarDetails(carId).subscribe({
        next: (carModel) => {
          this.selectedCar = {
            brand: carModel.brandName || '',
            model: carModel.modelName,
          };
        },
        error: (err) => {
          console.error('Failed to load selected car details', err);
          this.clearCarSelection();
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
      this.loadSubcategoriesFromHierarchy(category.id);
    }
  }

  loadSubcategoriesFromHierarchy(categoryId: string) {
    const hierarchicalCategory = this.hierarchicalCategories.find(
      (cat) => cat.id === categoryId
    );

    if (hierarchicalCategory && hierarchicalCategory.subCategories) {
      this.subcategories = hierarchicalCategory.subCategories.map((sub) => ({
        id: sub.id,
        name: sub.name,
        isAvailable: sub.isAvailable,
        imagePath: sub.imagePath,
        slug: sub.slug,
        products: [],
        subcategoryNames: [],
      }));
    } else {
      this.subcategories = [];
    }
  }

  closeSubcategories() {
    this.subcategoriesVisible = false;
    this.activeCategory = null;
  }

  navigateToShop(carId: string): void {
    this.router.navigate(['/shop'], { queryParams: { carModelId: carId } });
  }

  onSearchChange(): void {
    this.shopParams.pageNumber = 1;

    let queryParams: any = {};

    if (
      this.shopParams.searchTerm &&
      this.shopParams.searchTerm.trim() !== ''
    ) {
      queryParams.search = this.shopParams.searchTerm;
    }

    if (this.router.url.includes('/shop')) {
      const urlTree = this.router.parseUrl(this.router.url);
      const currentQueryParams = urlTree.queryParams;

      if (currentQueryParams['carModelId']) {
        queryParams.carModelId = currentQueryParams['carModelId'];
      }

      queryParams.pageNumber = 1;
    }

    this.router.navigate(['/shop'], { queryParams });
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
