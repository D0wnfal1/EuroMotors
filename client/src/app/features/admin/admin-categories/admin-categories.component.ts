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
import { finalize, takeUntil, tap } from 'rxjs/operators';
import { CategoryService } from '../../../core/services/category.service';
import { ImageService } from '../../../core/services/image.service';
import {
  Category,
  CategoryNode,
  FlatCategoryNode,
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

  private destroy$ = new Subject<void>();

  totalItems = 0;
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  private subcategoriesCache: { [parentId: string]: Category[] } = {};

  transformer = (node: CategoryNode, level: number): FlatCategoryNode => {
    return {
      id: node.id,
      name: node.name,
      isAvailable: node.isAvailable,
      imagePath: node.imagePath,
      parentCategoryId: node.parentCategoryId,
      level: level,
      expandable:
        level === 0 ? true : !!node.children && node.children.length > 0,
      isLoading: node.isLoading,
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

  loadedNodes = new Set<string>();

  loading = false;

  ngOnInit() {
    this.loadCategories();

    this.categoryService.categories$
      .pipe(takeUntil(this.destroy$))
      .subscribe((categories) => {
        if (categories && categories.length > 0) {
          this.updateCategoriesCache(categories);
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateCategoriesCache(categories: Category[]): void {
    categories.forEach((category) => {
      if (category.parentCategoryId) {
        if (!this.subcategoriesCache[category.parentCategoryId]) {
          this.subcategoriesCache[category.parentCategoryId] = [];
        }
        if (
          !this.subcategoriesCache[category.parentCategoryId].find(
            (c) => c.id === category.id
          )
        ) {
          this.subcategoriesCache[category.parentCategoryId].push(category);
        }
      }
    });
  }

  hasChild = (_: number, node: FlatCategoryNode) => {
    return node.level === 0 || node.expandable;
  };

  isLoading = (_: number, node: FlatCategoryNode) => node.isLoading === true;

  loadCategories(): void {
    this.categoryService
      .getParentCategories(this.shopParams)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          const treeData: CategoryNode[] = categories.map((category) => ({
            ...category,
            level: 0,
            expandable: true,
            children: [],
          }));

          this.dataSource.data = treeData;
        },
      });

    this.categoryService.totalItems$
      .pipe(takeUntil(this.destroy$))
      .subscribe((count) => {
        this.totalItems = count;
      });
  }

  toggleNode(node: FlatCategoryNode): void {
    if (this.treeControl.isExpanded(node)) {
      this.treeControl.collapse(node);
      return;
    }

    if (this.loadedNodes.has(node.id)) {
      this.treeControl.expand(node);
      return;
    }

    const index = this.treeControl.dataNodes.findIndex((n) => n.id === node.id);
    if (index >= 0) {
      this.treeControl.dataNodes[index].isLoading = true;
    }

    if (
      this.subcategoriesCache[node.id] &&
      this.subcategoriesCache[node.id].length > 0
    ) {
      this.updateTreeWithSubcategories(node, this.subcategoriesCache[node.id]);

      if (index >= 0) {
        this.treeControl.dataNodes[index].isLoading = false;
      }
      return;
    }

    this.categoryService
      .getSubcategories(node.id)
      .pipe(
        takeUntil(this.destroy$),
        tap((subcategories) => {
          this.subcategoriesCache[node.id] = subcategories;
        }),
        finalize(() => {
          if (index >= 0) {
            this.treeControl.dataNodes[index].isLoading = false;
          }
        })
      )
      .subscribe({
        next: (subcategories) => {
          this.updateTreeWithSubcategories(node, subcategories);
        },
        error: (err) => {
          console.error('Failed to load subcategories', err);
        },
      });
  }

  private updateTreeWithSubcategories(
    node: FlatCategoryNode,
    subcategories: Category[]
  ): void {
    this.loadedNodes.add(node.id);

    const updatedData = [...this.dataSource.data];
    const targetNode = this.findNodeById(updatedData, node.id);

    if (targetNode) {
      targetNode.children = subcategories.map((subcategory) => ({
        ...subcategory,
        level: node.level + 1,
        expandable: false,
        children: [],
      }));

      this.dataSource.data = updatedData;

      this.treeControl.expand(node);
    }
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

          this.loadedNodes.delete(categoryId);
          delete this.subcategoriesCache[categoryId];

          Object.keys(this.subcategoriesCache).forEach((parentId) => {
            this.subcategoriesCache[parentId] = this.subcategoriesCache[
              parentId
            ].filter((cat) => cat.id !== categoryId);
          });
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

  toggleAvailability(node: FlatCategoryNode): void {
    const newAvailability = !node.isAvailable;
    this.categoryService
      .setCategoryAvailability(node.id, newAvailability)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          const index = this.treeControl.dataNodes.findIndex(
            (n) => n.id === node.id
          );
          if (index >= 0) {
            this.treeControl.dataNodes[index].isAvailable = newAvailability;

            const updatedData = [...this.dataSource.data];
            const targetNode = this.findNodeById(updatedData, node.id);
            if (targetNode) {
              targetNode.isAvailable = newAvailability;
              this.dataSource.data = updatedData;
            }
          }

          if (
            node.parentCategoryId &&
            this.subcategoriesCache[node.parentCategoryId]
          ) {
            const cached = this.subcategoriesCache[node.parentCategoryId].find(
              (cat) => cat.id === node.id
            );
            if (cached) {
              cached.isAvailable = newAvailability;
            }
          }
        },
        error: (err) => {
          console.error('Failed to update category availability', err);
        },
      });
  }
}
