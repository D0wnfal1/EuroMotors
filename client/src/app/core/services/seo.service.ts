import { Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

export interface SeoConfig {
  title: string;
  description: string;
  keywords?: string;
  ogTitle?: string;
  ogDescription?: string;
  ogImage?: string;
}

@Injectable({
  providedIn: 'root',
})
export class SeoService {
  constructor(private readonly meta: Meta, private readonly title: Title) {}

  updateSeoTags(config: SeoConfig) {
    this.title.setTitle(config.title);

    this.meta.updateTag({ name: 'description', content: config.description });

    if (config.keywords) {
      this.meta.updateTag({ name: 'keywords', content: config.keywords });
    }

    this.meta.updateTag({
      property: 'og:title',
      content: config.ogTitle ?? config.title,
    });
    this.meta.updateTag({
      property: 'og:description',
      content: config.ogDescription ?? config.description,
    });

    if (config.ogImage) {
      this.meta.updateTag({ property: 'og:image', content: config.ogImage });
    }

    this.meta.updateTag({
      name: 'twitter:card',
      content: 'summary_large_image',
    });
    this.meta.updateTag({
      name: 'twitter:title',
      content: config.ogTitle ?? config.title,
    });
    this.meta.updateTag({
      name: 'twitter:description',
      content: config.ogDescription ?? config.description,
    });

    if (config.ogImage) {
      this.meta.updateTag({ name: 'twitter:image', content: config.ogImage });
    }
  }

  setCanonicalLink(url?: string) {
    const canURL = url ?? window.location.href;
    const link: HTMLLinkElement = document.createElement('link');
    link.setAttribute('rel', 'canonical');
    link.setAttribute('href', canURL);

    const existingLink = document.head.querySelector('link[rel="canonical"]');
    if (existingLink) {
      document.head.removeChild(existingLink);
    }
    document.head.appendChild(link);
  }
}
