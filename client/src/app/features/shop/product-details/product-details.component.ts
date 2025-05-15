import {
  Component,
  ElementRef,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { ImageService } from '../../../core/services/image.service';
import { RelatedProductsComponent } from '../related-products/related-products.component';
import { MatTabGroup, MatTabsModule } from '@angular/material/tabs';
import { CarModel } from '../../../shared/models/carModel';
import { CarmodelService } from '../../../core/services/carmodel.service';
import { CarBrand } from '../../../shared/models/carBrand';
import { CarbrandService } from '../../../core/services/carbrand.service';
import { CallbackComponent } from '../../../layout/callback/callback.component';
import { MatDialog } from '@angular/material/dialog';
import { SeoService } from '../../../core/services/seo.service';
import { SchemaService } from '../../../core/services/schema.service';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    FormsModule,
    RelatedProductsComponent,
    MatTabsModule,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly imageService = inject(ImageService);
  private readonly cartService = inject(CartService);
  private readonly carModelService = inject(CarmodelService);
  private readonly carBrandService = inject(CarbrandService);
  private readonly dialog = inject(MatDialog);
  private readonly seoService = inject(SeoService);
  private readonly schemaService = inject(SchemaService);

  product?: Product;
  activeIndex: number = 0;
  quantityInCart = 0;
  quantity = 1;
  carModels: CarModel[] = [];
  carBrands: CarBrand[] = [];

  showZoom = false;
  zoomStyle: any = {};
  zoomScale = 2.5;

  @ViewChild('tabGroup') tabGroup!: MatTabGroup;
  @ViewChild('productImg') productImg!: ElementRef<HTMLImageElement>;

  getImageUrl(imagePath: string): string {
    return this.imageService.getImageUrl(imagePath);
  }

  nextImage() {
    if (!this.product || !this.product.images.length) return;

    if (this.activeIndex < this.product.images.length - 1) {
      this.activeIndex++;
    } else {
      this.activeIndex = 0;
    }
  }

  previousImage() {
    if (!this.product || !this.product.images.length) return;

    if (this.activeIndex > 0) {
      this.activeIndex--;
    } else {
      this.activeIndex = this.product.images.length - 1;
    }
  }

  onImageZoom(event: MouseEvent) {
    if (!this.productImg || !this.product?.images.length) return;

    const img = this.productImg.nativeElement;
    const rect = img.getBoundingClientRect();

    const mouseX = event.clientX - rect.left;
    const mouseY = event.clientY - rect.top;

    const xPercent = mouseX / rect.width;
    const yPercent = mouseY / rect.height;

    const zoomWidth = 200;
    const zoomHeight = 200;

    const zoomLeft = mouseX + 40;
    const zoomTop = mouseY - zoomHeight / 2;

    const bgPosX = xPercent * 100;
    const bgPosY = yPercent * 100;

    this.zoomStyle = {
      width: `${zoomWidth}px`,
      height: `${zoomHeight}px`,
      left: `${zoomLeft}px`,
      top: `${zoomTop}px`,
      backgroundImage: `url('${this.getImageUrl(
        this.product.images[this.activeIndex].path
      )}')`,
      backgroundPosition: `${bgPosX}% ${bgPosY}%`,
      backgroundSize: `${this.zoomScale * 100}%`,
      backgroundRepeat: 'no-repeat',
      zIndex: 10,
    };
  }

  ngOnInit(): void {
    this.loadProduct();
    this.loadCarModels();
    this.loadCarBrands();
  }

  private loadProduct() {
    const productId = this.activatedRoute.snapshot.paramMap.get('id');
    if (productId) {
      this.productService.getProductById(productId).subscribe((product) => {
        this.product = product;
        this.updateSeoMetadata();
        this.updateStructuredData();
        this.updateQuantityInCart();
      });
    }
  }

  private loadCarModels() {
    this.carModelService.carModels$.subscribe((carModels) => {
      this.carModels = carModels;
    });

    this.carModelService.getCarModels({ pageNumber: 1, pageSize: 0 });
  }

  private loadCarBrands() {
    this.carBrandService.carBrands$.subscribe((carBrands) => {
      this.carBrands = carBrands;
    });

    this.carBrandService.getCarBrands({ pageNumber: 1, pageSize: 0 });
  }

  private updateSeoMetadata() {
    if (this.product) {
      const specifications = this.product.specifications
        ?.map((spec) => `${spec.specificationName}: ${spec.specificationValue}`)
        .join(', ');

      this.seoService.updateSeoTags({
        title: `${this.product.name} | EuroMotors`,
        description: `${this.product.name} - ${
          specifications || 'Автозапчастини'
        }. Ціна: ${this.product.price}₴`,
        keywords: `${this.product.name}, автозапчастини, ${
          specifications || ''
        }`,
        ogTitle: this.product.name,
        ogDescription: `Купити ${this.product.name} в магазині EuroMotors`,
        ogImage: this.product.images?.[0]?.path,
      });

      this.seoService.setCanonicalLink();
    }
  }

  private updateStructuredData() {
    if (this.product) {
      this.schemaService.addProductSchema(this.product);
    }
  }

  updateCart() {
    if (!this.product) return;
    if (this.quantity > this.quantityInCart) {
      const itemsToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart += itemsToAdd;
      this.cartService.addItemToCart(this.product.id, itemsToAdd);
    }
    if (this.quantity < this.quantityInCart) {
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart = this.quantity;
      if (this.quantity === 0) {
        this.cartService.removeItemFromCart(this.product.id);
      } else {
        this.cartService.decrementItemQuantity(this.product.id, itemsToRemove);
      }
    }
  }

  updateQuantityInCart() {
    this.quantityInCart =
      this.cartService
        .cart()
        ?.cartItems.find((x) => x.productId === this.product?.id)?.quantity ??
      0;
    this.quantity = this.quantityInCart || 1;
  }

  getButtonText() {
    return this.quantityInCart > 0 ? 'Оновити кошик' : 'Додати до кошика';
  }

  getCompatibleCarModels(): string {
    if (
      !this.product ||
      !this.product.carModelIds ||
      this.product.carModelIds.length === 0
    ) {
      return 'Цей товар не підходить для конкретної моделі автомобіля';
    }

    return this.product.carModelIds
      .map((id) => {
        const model = this.carModels.find((cm) => cm.id === id);
        if (!model) return '';

        const brandName =
          model.carBrand?.name ??
          this.carBrands.find((b) => b.id === model.carBrandId)?.name ??
          '';

        return `${brandName} ${model.modelName} (${model.startYear})`;
      })
      .filter((name) => name !== '')
      .join(', ');
  }

  openCallbackDialog() {
    this.dialog.open(CallbackComponent, {
      width: '400px',
      panelClass: 'callback-dialog',
      disableClose: false,
    });
  }
}
