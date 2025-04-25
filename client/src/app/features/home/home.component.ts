import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CarSelectionComponent } from './car-selection/car-selection.component';
import { CarmodelService } from '../../core/services/carmodel.service';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { ImageService } from '../../core/services/image.service';
import { HierarchicalCategory } from '../../shared/models/category';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';
import { Subscription, interval } from 'rxjs';
import { CarbrandService } from '../../core/services/carbrand.service';
import { CarBrand } from '../../shared/models/carBrand';
import { ProductItemComponent } from '../shop/product-item/product-item.component';
import { CartService } from '../../core/services/cart.service';

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
    ProductItemComponent,
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

  shopParams = new ShopParams();
  allProducts: Product[] = [];
  displayedProducts: Product[] = [];
  newProducts: Product[] = [];
  discountProducts: Product[] = [];
  popularProducts: Product[] = [];
  selectedProductTab: string = 'new';

  slidePosition: number = 0;
  itemsPerView: number = 4;
  slideWidth: number = 284;
  maxSlidePosition: number = 0;
  currentSlideIndex: number = 0;
  totalSlides: number = 0;
  autoplayInterval = 5000;
  autoplaySubscription?: Subscription;

  private subscriptions: Subscription[] = [];

  hasSelectedCar: boolean = false;

  constructor(
    private carmodelService: CarmodelService,
    private carBrandService: CarbrandService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private imageService: ImageService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.checkSelectedCar();
    this.loadAllBrands();
    this.loadHierarchicalCategories();
    this.loadProducts();

    this.setItemsPerView();
    window.addEventListener('resize', this.setItemsPerView.bind(this));

    this.startAutoplay();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    window.removeEventListener('resize', this.setItemsPerView.bind(this));
    this.stopAutoplay();
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

  startAutoplay(): void {
    this.stopAutoplay();
    this.autoplaySubscription = interval(this.autoplayInterval).subscribe(
      () => {
        this.nextProduct();
      }
    );
  }

  stopAutoplay(): void {
    if (this.autoplaySubscription) {
      this.autoplaySubscription.unsubscribe();
      this.autoplaySubscription = undefined;
    }
  }

  pauseAutoplay(): void {
    this.stopAutoplay();
  }

  resumeAutoplay(): void {
    this.startAutoplay();
  }

  calculateItemsPerView(): number {
    if (window.innerWidth < 640) {
      return 1;
    } else if (window.innerWidth < 768) {
      return 2;
    } else if (window.innerWidth < 1024) {
      return 3;
    } else {
      return 4;
    }
  }

  setItemsPerView(): void {
    this.itemsPerView = this.calculateItemsPerView();
    this.slidePosition = 0;
    this.currentSlideIndex = 0;
    this.updateMaxSlidePosition();
  }

  updateMaxSlidePosition(): void {
    const productsPerSlide = this.itemsPerView;
    const totalProducts = this.displayedProducts.length;

    const fullSlides = Math.floor(totalProducts / productsPerSlide);

    const remainingProducts = totalProducts % productsPerSlide;

    this.totalSlides = remainingProducts > 0 ? fullSlides + 1 : fullSlides;

    if (fullSlides > 0) {
      this.maxSlidePosition =
        productsPerSlide * this.slideWidth * (fullSlides - 1);

      if (remainingProducts > 0) {
        this.maxSlidePosition += remainingProducts * this.slideWidth;
      }
    } else {
      this.maxSlidePosition = 0;
    }
  }

  loadAllBrands(): void {
    const brandsSub = this.carBrandService.availableBrands$.subscribe(
      (brands) => {
        this.allBrands = brands;
        this.displayedBrands = this.allBrands.slice(0, 16);
      }
    );

    this.subscriptions.push(brandsSub);
    this.carBrandService.getAllCarBrands();
  }

  loadProducts(): void {
    this.shopParams.pageNumber = 1;
    this.shopParams.pageSize = 20;

    const productsSub = this.productService
      .getProducts(this.shopParams)
      .subscribe({
        next: (response) => {
          this.allProducts = response.data;

          this.newProducts = [...this.allProducts]
            .sort((a, b) => b.id.localeCompare(a.id))
            .slice(0, 10);

          this.discountProducts = [...this.allProducts]
            .filter((p) => p.discount > 0)
            .sort((a, b) => b.discount - a.discount)
            .slice(0, 10);

          this.popularProducts = [...this.allProducts].slice(0, 10);

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
    this.slidePosition = 0;
    this.currentSlideIndex = 0;

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

    this.updateMaxSlidePosition();

    this.startAutoplay();
  }

  nextProduct(): void {
    const productsPerSlide = this.itemsPerView;
    this.currentSlideIndex++;

    if (this.currentSlideIndex >= this.totalSlides) {
      this.currentSlideIndex = 0;
      this.slidePosition = 0;
    } else {
      this.slidePosition =
        -this.currentSlideIndex * productsPerSlide * this.slideWidth;

      const totalProducts = this.displayedProducts.length;
      const productsOnLastSlide =
        totalProducts % productsPerSlide || productsPerSlide;

      if (
        this.currentSlideIndex === this.totalSlides - 1 &&
        productsOnLastSlide < productsPerSlide
      ) {
        const fullSlidesCount = Math.floor(totalProducts / productsPerSlide);
        this.slidePosition =
          -fullSlidesCount * productsPerSlide * this.slideWidth;
      }
    }
  }

  prevProduct(): void {
    const productsPerSlide = this.itemsPerView;
    this.currentSlideIndex--;

    if (this.currentSlideIndex < 0) {
      this.currentSlideIndex = this.totalSlides - 1;

      const totalProducts = this.displayedProducts.length;
      const fullSlidesCount = Math.floor(totalProducts / productsPerSlide);
      const productsOnLastSlide = totalProducts % productsPerSlide;

      if (productsOnLastSlide > 0) {
        this.slidePosition =
          -fullSlidesCount * productsPerSlide * this.slideWidth;
      } else {
        this.slidePosition =
          -(this.totalSlides - 1) * productsPerSlide * this.slideWidth;
      }
    } else {
      this.slidePosition =
        -this.currentSlideIndex * productsPerSlide * this.slideWidth;
    }
  }

  goToSlide(index: number): void {
    if (index >= 0 && index < this.totalSlides) {
      const productsPerSlide = this.itemsPerView;
      this.currentSlideIndex = index;

      if (index === this.totalSlides - 1) {
        const totalProducts = this.displayedProducts.length;
        const fullSlidesCount = Math.floor(totalProducts / productsPerSlide);
        const productsOnLastSlide = totalProducts % productsPerSlide;

        if (productsOnLastSlide > 0) {
          this.slidePosition =
            -fullSlidesCount * productsPerSlide * this.slideWidth;
        } else {
          this.slidePosition = -index * productsPerSlide * this.slideWidth;
        }
      } else {
        this.slidePosition = -index * productsPerSlide * this.slideWidth;
      }

      if (this.autoplaySubscription) {
        this.stopAutoplay();
        this.startAutoplay();
      }
    }
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
}
