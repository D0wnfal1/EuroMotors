import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import {
  MatTreeModule,
  MatTreeFlatDataSource,
  MatTreeFlattener,
} from '@angular/material/tree';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { CategoryService } from '../../../core/services/category.service';
import { ImageService } from '../../../core/services/image.service';
import {
  CategoryNode,
  FlatCategoryNode,
  HierarchicalCategory,
} from '../../../shared/models/category';
import { ShopParams } from '../../../shared/models/shopParams';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatPaginator,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatTreeModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.scss',
})
export class AdminCategoriesComponent implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly imageService = inject(ImageService);

  private readonly destroy$ = new Subject<void>();

  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  transformer = (node: CategoryNode, level: number): FlatCategoryNode => {
    return {
      id: node.id,
      name: node.name,
      isAvailable: node.isAvailable,
      imagePath: node.imagePath,
      parentCategoryId: node.parentCategoryId,
      level: level,
      expandable: !!node.children && node.children.length > 0,
      isLoading: false,
    };
  };

  treeControl = new FlatTreeControl<FlatCategoryNode>(
    (node) => node.level,
    (node) => node.expandable
  );

  treeFlattener = new MatTreeFlattener(
    this.transformer,
    (node) => node.level,
    (node) => node.expandable,
    (node) => node.children
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  loading = false;

  ngOnInit() {
    this.loadCategories();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  hasChild = (_: number, node: FlatCategoryNode) => node.expandable;

  isLoading = (_: number, node: FlatCategoryNode) => node.isLoading === true;

  loadCategories(): void {
    this.loading = true;
    this.categoryService
      .getHierarchicalCategories(this.shopParams)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          const treeData = this.convertToTreeNodes(response);
          this.dataSource.data = treeData;
          this.totalItems = response.length;
        },
        error: (err) => {
          console.error('Failed to load categories', err);
        },
      });
  }

  private convertToTreeNodes(
    categories: HierarchicalCategory[]
  ): CategoryNode[] {
    return categories.map((cat) => this.mapToTreeNode(cat, null));
  }

  private mapToTreeNode(
    category: HierarchicalCategory,
    parentId: string | null
  ): CategoryNode {
    return {
      id: category.id,
      name: category.name,
      isAvailable: category.isAvailable,
      imagePath: category.imagePath,
      parentCategoryId: parentId as string | undefined,
      level: 0,
      expandable: category.subCategories && category.subCategories.length > 0,
      children: category.subCategories?.map((subCat) =>
        this.mapToTreeNode(subCat, category.id)
      ),
    };
  }

  toggleNode(node: FlatCategoryNode): void {
    if (this.treeControl.isExpanded(node)) {
      this.treeControl.collapse(node);
    } else {
      this.treeControl.expand(node);
    }
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.loadCategories();
  }

  getCategoryImage(imagePath?: string): string {
    return imagePath
      ? this.imageService.getImageUrl(imagePath)
      : '/images/no-image.jpeg';
  }

  deleteCategory(categoryId: string): void {
    this.categoryService
      .deleteCategory(categoryId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          const updatedData = this.removeNodeFromTree(
            [...this.dataSource.data],
            categoryId
          );
          this.dataSource.data = updatedData;
        },
        error: (err) => {
          console.error('Failed to delete category', err);
        },
      });
  }

  removeNodeFromTree(nodes: CategoryNode[], id: string): CategoryNode[] {
    return nodes.filter((node) => {
      if (node.id === id) {
        return false;
      }

      if (node.children && node.children.length > 0) {
        node.children = this.removeNodeFromTree(node.children, id);
      }

      return true;
    });
  }

  findNodeById(nodes: CategoryNode[], id: string): CategoryNode | null {
    for (const node of nodes) {
      if (node.id === id) {
        return node;
      }
      if (node.children && node.children.length > 0) {
        const found = this.findNodeById(node.children, id);
        if (found) {
          return found;
        }
      }
    }
    return null;
  }
}
