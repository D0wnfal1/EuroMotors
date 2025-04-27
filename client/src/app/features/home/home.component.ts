import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CarSelectionComponent } from './car-selection/car-selection.component';
import { ProductSliderComponent } from './product-slider/product-slider.component';
import { CarmodelService } from '../../core/services/carmodel.service';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { ImageService } from '../../core/services/image.service';
import { HierarchicalCategory } from '../../shared/models/category';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';
import { Subscription } from 'rxjs';
import { CarbrandService } from '../../core/services/carbrand.service';
import { CarBrand } from '../../shared/models/carBrand';
import { CartService } from '../../core/services/cart.service';
import { Router } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

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
  totalCategories: number = 0;
  categoryParams = new ShopParams();

  private subscriptions: Subscription[] = [];
  private breakpointObserver = inject(BreakpointObserver);

  hasSelectedCar: boolean = false;
  isMobile: boolean = false;
  isTablet: boolean = false;
  displayedBrandsCount: number = 16;

  constructor(
    private carmodelService: CarmodelService,
    private carBrandService: CarbrandService,
    private categoryService: CategoryService,
    private imageService: ImageService,
    private cartService: CartService,
    private router: Router
  ) {
    this.subscriptions.push(
      this.breakpointObserver
        .observe([Breakpoints.XSmall, Breakpoints.Small, Breakpoints.Medium])
        .subscribe((result) => {
          this.isMobile = result.breakpoints[Breakpoints.XSmall];
          this.isTablet = result.breakpoints[Breakpoints.Small];

          if (this.isMobile) {
            this.displayedBrandsCount = 4;
          } else if (this.isTablet) {
            this.displayedBrandsCount = 8;
          } else {
            this.displayedBrandsCount = 16;
          }

          if (!this.showAllBrands) {
            this.displayedBrands = this.allBrands.slice(
              0,
              this.displayedBrandsCount
            );
          }
        })
    );
  }

  ngOnInit(): void {
    this.checkSelectedCar();
    this.loadAllBrands();
    this.loadHierarchicalCategories();
  }

  loadHierarchicalCategories(): void {
    this.categoryParams.pageNumber = 0;
    this.categoryParams.pageSize = 0;

    const categoriesSub = this.categoryService
      .getHierarchicalCategories(this.categoryParams)
      .subscribe({
        next: (response) => {
          this.mainCategories = response.data;
          this.totalCategories = response.count;
        },
        error: (err) =>
          console.error('Failed to load hierarchical categories', err),
      });

    this.subscriptions.push(categoriesSub);
  }

  loadAllBrands(): void {
    const brandsSub = this.carBrandService.availableBrands$.subscribe(
      (brands) => {
        this.allBrands = brands;
        this.displayedBrands = this.allBrands.slice(
          0,
          this.displayedBrandsCount
        );
      }
    );

    this.subscriptions.push(brandsSub);
    this.carBrandService.getAllCarBrands();
  }

  viewAllBrands(): void {
    if (!this.showAllBrands) {
      this.displayedBrands = this.allBrands;
      this.showAllBrands = true;
    } else {
      this.displayedBrands = this.allBrands.slice(0, this.displayedBrandsCount);
      this.showAllBrands = false;
    }
  }

  getImageUrl(path: string): string {
    return this.imageService.getImageUrl(path);
  }

  getBrandLogo(brand: CarBrand): string {
    return brand.logoPath
      ? this.imageService.getImageUrl(brand.logoPath)
      : '/images/no-image.jpeg';
  }

  trackByProductId(index: number, item: Product): string {
    return item.id;
  }

  addToCart(productId: string, event: Event): void {
    event.stopPropagation();
    this.cartService.addItemToCart(productId);
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

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }
}
