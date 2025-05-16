import { Injectable } from '@angular/core';
import { Product } from '../../shared/models/product';
import { Category } from '../../shared/models/category';

interface ProductSchema {
  '@context': string;
  '@type': string;
  name: string;
  description?: string;
  sku: string;
  brand: {
    '@type': string;
    name: string;
  };
  offers: {
    '@type': string;
    url: string;
    priceCurrency: string;
    price: number;
    availability: string;
  };
  image?: string;
}

interface OrganizationSchema {
  '@context': string;
  '@type': string;
  name: string;
  description: string;
  url: string;
  address: {
    '@type': string;
    addressCountry: string;
  };
}

interface CategorySchema {
  '@context': string;
  '@type': string;
  name: string;
  description?: string;
  url: string;
  image?: string;
  parentCategory?: {
    '@type': string;
    name: string;
    url: string;
  };
}

@Injectable({
  providedIn: 'root',
})
export class SchemaService {
  private addJsonLd(
    schema: ProductSchema | OrganizationSchema | CategorySchema
  ) {
    let script = document.querySelector('script[type="application/ld+json"]');
    if (script) {
      document.head.removeChild(script);
    }

    script = document.createElement('script');
    script.setAttribute('type', 'application/ld+json');
    script.textContent = JSON.stringify(schema);
    document.head.appendChild(script);
  }

  addProductSchema(product: Product) {
    const schema: ProductSchema = {
      '@context': 'https://schema.org',
      '@type': 'Product',
      name: product.name,
      description: product.specifications
        ?.map((spec) => `${spec.specificationName}: ${spec.specificationValue}`)
        .join(', '),
      sku: product.vendorCode,
      brand: {
        '@type': 'Brand',
        name: 'AutoRSD',
      },
      offers: {
        '@type': 'Offer',
        url: window.location.href,
        priceCurrency: 'UAH',
        price: product.price,
        availability: product.isAvailable
          ? 'https://schema.org/InStock'
          : 'https://schema.org/OutOfStock',
      },
    };

    if (product.images?.[0]?.path) {
      schema.image = product.images[0].path;
    }

    this.addJsonLd(schema);
  }

  addOrganizationSchema() {
    const schema: OrganizationSchema = {
      '@context': 'https://schema.org',
      '@type': 'AutoPartsStore',
      name: 'AutoRSD',
      description: 'Магазин автозапчастей для європейських автомобілів',
      url: window.location.origin,
      address: {
        '@type': 'PostalAddress',
        addressCountry: 'UA',
      },
    };

    this.addJsonLd(schema);
  }

  addCategorySchema(category: Category, parentCategory?: Category) {
    const schema: CategorySchema = {
      '@context': 'https://schema.org',
      '@type': 'CollectionPage',
      name: category.name,
      description: `Автозапчастини категорії ${category.name} в магазині AutoRSD`,
      url: window.location.href,
    };

    if (category.imagePath) {
      schema.image = category.imagePath;
    }

    if (parentCategory) {
      schema.parentCategory = {
        '@type': 'CollectionPage',
        name: parentCategory.name,
        url: `${window.location.origin}/category/${parentCategory.id}`,
      };
    }

    this.addJsonLd(schema);
  }
}
