import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CarSelectionComponent } from './car-selection/car-selection.component';
import { CarmodelService } from '../../core/services/carmodel.service';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { ImageService } from '../../core/services/image.service';
import { Category } from '../../shared/models/category';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CarSelectionComponent,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit, OnDestroy {
  // Car brands properties
  uniqueBrands: string[] = [];
  allBrands: string[] = [];
  displayedBrands: string[] = [];
  showAllBrands: boolean = false;

  // Categories properties
  mainCategories: Category[] = [];

  // Products properties
  shopParams = new ShopParams();
  allProducts: Product[] = [];
  displayedProducts: Product[] = [];
  newProducts: Product[] = [];
  discountProducts: Product[] = [];
  popularProducts: Product[] = [];
  selectedProductTab: string = 'new';

  // Slider properties
  slidePosition: number = 0;
  itemsPerView: number = 4;
  slideWidth: number = 290; // 280px item width + 2*5px margins

  private subscriptions: Subscription[] = [];

  constructor(
    private carmodelService: CarmodelService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private imageService: ImageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Load car brands - используем новый метод для загрузки всех брендов
    this.loadAllBrands();

    // Load main categories
    this.loadMainCategories();

    // Load products
    this.loadProducts();

    // Set responsive items per view based on window width
    this.setItemsPerView();
    window.addEventListener('resize', this.setItemsPerView.bind(this));
  }

  ngOnDestroy(): void {
    // Clean up subscriptions to prevent memory leaks
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    window.removeEventListener('resize', this.setItemsPerView.bind(this));
  }

  setItemsPerView(): void {
    if (window.innerWidth < 640) {
      this.itemsPerView = 1;
    } else if (window.innerWidth < 768) {
      this.itemsPerView = 2;
    } else if (window.innerWidth < 1024) {
      this.itemsPerView = 3;
    } else {
      this.itemsPerView = 4;
    }
    this.slidePosition = 0; // Reset position when resizing
  }

  // Заменяем метод loadUniqueBrands на новый loadAllBrands
  loadAllBrands(): void {
    const brandsSub = this.carmodelService.allBrands$.subscribe((brands) => {
      this.allBrands = brands;
      this.displayedBrands = this.allBrands.slice(0, 16);
    });

    this.subscriptions.push(brandsSub);
    // Вызываем новый метод для получения всех брендов
    this.carmodelService.getAllBrands().subscribe();
  }

  loadMainCategories(): void {
    this.shopParams.pageNumber = 1;
    this.shopParams.pageSize = 10;
    const categoriesSub = this.categoryService
      .getParentCategories(this.shopParams)
      .subscribe({
        next: (categories) => {
          this.mainCategories = categories;

          // For each main category, load its subcategories
          categories.forEach((category) => {
            if (category.id) {
              const subCatSub = this.categoryService
                .getSubcategories(category.id)
                .subscribe({
                  next: (subcategories) => {
                    const index = this.mainCategories.findIndex(
                      (c) => c.id === category.id
                    );
                    if (index !== -1) {
                      this.mainCategories[index].subcategoryNames =
                        subcategories.map((sc) => sc.name);
                    }
                  },
                  error: (err) =>
                    console.error(
                      `Failed to load subcategories for ${category.name}`,
                      err
                    ),
                });
              this.subscriptions.push(subCatSub);
            }
          });
        },
        error: (err) => console.error('Failed to load main categories', err),
      });

    this.subscriptions.push(categoriesSub);
  }

  loadProducts(): void {
    // Create shop params for fetching products
    this.shopParams.pageNumber = 1;
    this.shopParams.pageSize = 20;

    const productsSub = this.productService
      .getProducts(this.shopParams)
      .subscribe({
        next: (response) => {
          this.allProducts = response.data;

          // Filter products for each tab
          this.newProducts = [...this.allProducts]
            .sort((a, b) =>
              // This is a placeholder sort - in a real app, you might sort by date added
              b.id.localeCompare(a.id)
            )
            .slice(0, 10);

          this.discountProducts = [...this.allProducts]
            .filter((p) => p.discount > 0)
            .sort((a, b) => b.discount - a.discount)
            .slice(0, 10);

          this.popularProducts = [...this.allProducts]
            // In a real app, you might sort by sales count or views
            .slice(0, 10);

          // Set initial displayed products
          this.changeProductTab(this.selectedProductTab);
        },
        error: (err) => console.error('Failed to load products', err),
      });

    this.subscriptions.push(productsSub);
  }

  viewAllBrands(): void {
    if (!this.showAllBrands) {
      this.displayedBrands = this.allBrands;
      this.showAllBrands = true;
    } else {
      this.displayedBrands = this.allBrands.slice(0, 16);
      this.showAllBrands = false;
    }
  }

  changeProductTab(tab: string): void {
    this.selectedProductTab = tab;
    this.slidePosition = 0; // Reset slide position

    switch (tab) {
      case 'new':
        this.displayedProducts = this.newProducts;
        break;
      case 'discounts':
        this.displayedProducts = this.discountProducts;
        break;
      case 'popular':
        this.displayedProducts = this.popularProducts;
        break;
      default:
        this.displayedProducts = this.newProducts;
    }
  }

  nextProduct(): void {
    const maxSlides = Math.max(
      0,
      this.displayedProducts.length - this.itemsPerView
    );
    const nextPosition = Math.max(
      Math.min(this.slidePosition - this.slideWidth, 0),
      -maxSlides * this.slideWidth
    );
    this.slidePosition = nextPosition;
  }

  prevProduct(): void {
    const maxSlides = Math.max(
      0,
      this.displayedProducts.length - this.itemsPerView
    );
    const prevPosition = Math.min(
      Math.max(
        this.slidePosition + this.slideWidth,
        -maxSlides * this.slideWidth
      ),
      0
    );
    this.slidePosition = prevPosition;
  }

  calculateDiscountedPrice(product: Product): number {
    if (product.discount && product.discount > 0) {
      return product.price * (1 - product.discount / 100);
    }
    return product.price;
  }

  getImageUrl(path: string): string {
    return this.imageService.getImageUrl(path);
  }
}
