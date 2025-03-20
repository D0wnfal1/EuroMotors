import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  MatListOption,
  MatSelectionList,
  MatSelectionListChange,
} from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../models/pagination';
import { Product } from '../../models/product';
import { ShopParams } from '../../models/shopParams';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-product-list',
  imports: [
    MatMenu,
    MatIcon,
    MatSelectionList,
    MatPaginator,
    MatListOption,
    NgIf,
    NgFor,
    ReactiveFormsModule,
    FormsModule,
    MatMenuTrigger,
    MatButton,
  ],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent {
  @Input() products?: Pagination<Product>;
  @Input() shopParams!: ShopParams;
  @Input() pageSizeOptions: number[] = [5, 10, 15, 20];
  @Input() sortOptions: { name: string; value: string }[] = [];

  @Output() pageChanged = new EventEmitter<PageEvent>();
  @Output() searchChanged = new EventEmitter<void>();
  @Output() sortChanged = new EventEmitter<MatSelectionListChange>();

  handlePageEvent(event: PageEvent): void {
    this.pageChanged.emit(event);
  }

  onSearchChange(): void {
    this.searchChanged.emit();
  }

  onSortChange(event: MatSelectionListChange): void {
    this.sortChanged.emit(event);
  }
}
