import { MatPaginatorIntl } from '@angular/material/paginator';

export class UkrainianPaginatorIntl extends MatPaginatorIntl {
  override itemsPerPageLabel = 'Елементів на сторінці:';
  override nextPageLabel = 'Наступна сторінка';
  override previousPageLabel = 'Попередня сторінка';
  override firstPageLabel = 'Перша сторінка';
  override lastPageLabel = 'Остання сторінка';

  override getRangeLabel = (page: number, pageSize: number, length: number) => {
    if (length === 0) {
      return 'Сторінка 1 з 1';
    }
    const amountPages = Math.ceil(length / pageSize);
    return `Сторінка ${page + 1} з ${amountPages}`;
  };
}
