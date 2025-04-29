import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CarSelectionComponent } from './car-selection/car-selection.component';
import { ProductSliderComponent } from './product-slider/product-slider.component';
import { CarmodelService } from '../../core/services/carmodel.service';
import { ImageService } from '../../core/services/image.service';
import { HierarchicalCategory } from '../../shared/models/category';
import { Product } from '../../shared/models/product';
import { Subscription, Observable, shareReplay } from 'rxjs';
import { CarbrandService } from '../../core/services/carbrand.service';
import { CarBrand } from '../../shared/models/carBrand';
import { Router, RouterModule } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { HomePageService } from '../../core/services/home-page.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CarSelectionComponent,
    ProductSliderComponent,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTooltipModule,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit, OnDestroy {
  uniqueBrands: string[] = [];
  allBrands: CarBrand[] = [];
  displayedBrands: CarBrand[] = [];
  showAllBrands: boolean = false;

  mainCategories: HierarchicalCategory[] = [];
  displayedCategories: HierarchicalCategory[] = [];
  showAllCategories: boolean = false;
  totalCategories: number = 0;
  displayedCategoriesCount: number = 10;

  popularProducts: Product[] = [];
  newProducts: Product[] = [];
  discountedProducts: Product[] = [];

  private readonly subscriptions: Subscription[] = [];
  private readonly breakpointObserver = inject(BreakpointObserver);
  private readonly homePageService = inject(HomePageService);

  private cachedBrands$: Observable<CarBrand[]> | null = null;

  hasSelectedCar: boolean = false;
  selectedCarId: string | null = null;
  isMobile: boolean = false;
  isTablet: boolean = false;
  displayedBrandsCount: number = 16;

  constructor(
    private readonly carmodelService: CarmodelService,
    private readonly carBrandService: CarbrandService,
    private readonly imageService: ImageService,
    private readonly router: Router
  ) {
    this.subscriptions.push(
      this.breakpointObserver
        .observe([Breakpoints.XSmall, Breakpoints.Small, Breakpoints.Medium])
        .subscribe((result) => {
          this.isMobile = result.breakpoints[Breakpoints.XSmall];
          this.isTablet = result.breakpoints[Breakpoints.Small];

          if (this.isMobile) {
            this.displayedBrandsCount = 4;
            this.displayedCategoriesCount = 5;
          } else if (this.isTablet) {
            this.displayedBrandsCount = 8;
            this.displayedCategoriesCount = 6;
          } else {
            this.displayedBrandsCount = 16;
            this.displayedCategoriesCount = 10;
          }

          if (!this.showAllBrands) {
            this.displayedBrands = this.allBrands.slice(
              0,
              this.displayedBrandsCount
            );
          }

          if (!this.showAllCategories) {
            this.displayedCategories = this.mainCategories.slice(
              0,
              this.displayedCategoriesCount
            );
          }
        })
    );
  }

  ngOnInit(): void {
    this.checkSelectedCar();
    this.loadAllBrands();
    this.loadHierarchicalCategories();
    this.loadPopularProducts();
    this.loadNewProducts();
    this.loadDiscountedProducts();
  }

  loadHierarchicalCategories(): void {
    const categoriesSub = this.homePageService.getMainCategories().subscribe({
      next: (categories) => {
        this.mainCategories = categories;
        this.totalCategories = categories.length;
        this.displayedCategories = this.mainCategories.slice(
          0,
          this.displayedCategoriesCount
        );
      },
      error: (err) =>
        console.error('Failed to load hierarchical categories', err),
    });

    this.subscriptions.push(categoriesSub);
  }

  loadAllBrands(): void {
    if (!this.cachedBrands$) {
      this.cachedBrands$ = this.carBrandService.availableBrands$.pipe(
        shareReplay({
          bufferSize: 1,
          refCount: false,
          windowTime: 10 * 60 * 1000,
        })
      );
    }

    const brandsSub = this.cachedBrands$.subscribe((brands) => {
      this.allBrands = brands;
      this.displayedBrands = this.allBrands.slice(0, this.displayedBrandsCount);
    });

    this.subscriptions.push(brandsSub);

    if (this.allBrands.length === 0) {
      this.carBrandService.getAllCarBrands();
    }
  }

  loadPopularProducts(): void {
    const productsSub = this.homePageService.getPopularProducts(10).subscribe({
      next: (products) => {
        this.popularProducts = products;
      },
      error: (err) => console.error('Failed to load popular products', err),
    });

    this.subscriptions.push(productsSub);
  }

  loadNewProducts(): void {
    const productsSub = this.homePageService.getNewProducts(10).subscribe({
      next: (products) => {
        this.newProducts = products;
      },
      error: (err) => console.error('Failed to load new products', err),
    });

    this.subscriptions.push(productsSub);
  }

  loadDiscountedProducts(): void {
    const productsSub = this.homePageService
      .getDiscountedProducts(10)
      .subscribe({
        next: (products) => {
          this.discountedProducts = products;
        },
        error: (err) =>
          console.error('Failed to load discounted products', err),
      });

    this.subscriptions.push(productsSub);
  }

  getBrandLogo(brand: CarBrand): string {
    return brand.logoPath
      ? this.imageService.getImageUrl(brand.logoPath)
      : '/images/no-image.jpeg';
  }

  viewAllBrands(): void {
    this.toggleShowAllBrands();
  }

  toggleShowAllBrands(): void {
    this.showAllBrands = !this.showAllBrands;
    if (this.showAllBrands) {
      this.displayedBrands = this.allBrands;
    } else {
      this.displayedBrands = this.allBrands.slice(0, this.displayedBrandsCount);
    }
  }

  viewAllCategories(): void {
    this.toggleShowAllCategories();
  }

  toggleShowAllCategories(): void {
    this.showAllCategories = !this.showAllCategories;
    if (this.showAllCategories) {
      this.displayedCategories = this.mainCategories;
    } else {
      this.displayedCategories = this.mainCategories.slice(
        0,
        this.displayedCategoriesCount
      );
    }
  }

  checkSelectedCar(): void {
    const savedCarId = this.carmodelService.getStoredCarId();
    this.hasSelectedCar = !!savedCarId;
  }

  clearCarSelection(): void {
    this.carmodelService.clearCarSelection();
    this.hasSelectedCar = false;
  }

  exploreCategory(categoryId: string): void {
    this.router.navigate(['/shop/category', categoryId], {
      queryParams: {
        pageNumber: 1,
        pageSize: 10,
      },
    });
  }

  clearCaches() {
    this.cachedBrands$ = null;
  }

  ngOnDestroy(): void {
    this.clearCaches();
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }
}
