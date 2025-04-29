import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LazyLoadImageDirective } from '../directives/lazy-load-image.directive';

@NgModule({
  declarations: [],
  imports: [CommonModule, LazyLoadImageDirective],
  exports: [LazyLoadImageDirective],
})
export class ImageOptimizationModule {}
