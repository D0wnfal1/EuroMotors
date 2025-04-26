export class ShopParams {
  categoryIds: string[] = [];
  carModelIds: string[] = [];
  sortOrder = '';
  searchTerm = '';
  isDiscounted?: boolean;
  isNew?: boolean;
  isPopular?: boolean;
  pageNumber = 1;
  pageSize = 10;
}
