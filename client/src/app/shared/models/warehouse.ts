export type Warehouse = {
  description: any;
  Ref: string;
  // При необходимости можно добавить дополнительные поля, например:
  // Address?: string;
  // City?: string;
  // Phone?: string;
};

export interface WarehousesResponse {
  errors(arg0: string, errors: any): unknown;
  success: any;
  data: any;
  warehouses: Warehouse[];
}
