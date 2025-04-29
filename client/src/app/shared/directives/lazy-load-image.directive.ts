import {
  AfterViewInit,
  Directive,
  ElementRef,
  HostBinding,
  Input,
  OnDestroy,
  inject,
} from '@angular/core';

@Directive({
  selector: 'img[appLazyLoad]',
  standalone: true,
})
export class LazyLoadImageDirective implements AfterViewInit, OnDestroy {
  private readonly el = inject(ElementRef);
  private observer: IntersectionObserver | null = null;

  @Input() lazyLoadSrc: string = '';
  @HostBinding('attr.loading') loading = 'lazy';

  @Input() lazyLoadPlaceholder: string = '';

  ngAfterViewInit(): void {
    if ('loading' in HTMLImageElement.prototype) {
      this.loadImage();
    } else {
      this.setupIntersectionObserver();
    }
  }

  private setupIntersectionObserver(): void {
    const options = {
      root: null,
      rootMargin: '100px',
      threshold: 0.1,
    };

    this.observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          this.loadImage();
          this.observer?.disconnect();
          this.observer = null;
        }
      });
    }, options);

    this.observer.observe(this.el.nativeElement);
  }

  private loadImage(): void {
    if (this.lazyLoadPlaceholder) {
      this.el.nativeElement.src = this.lazyLoadPlaceholder;
    }

    if (this.lazyLoadSrc) {
      const img = new Image();
      img.src = this.lazyLoadSrc;
      img.onload = () => {
        this.el.nativeElement.src = this.lazyLoadSrc;
      };
    }
  }

  ngOnDestroy(): void {
    if (this.observer) {
      this.observer.disconnect();
      this.observer = null;
    }
  }
}
