export type Warehouse = {
  description: any;
  Ref: string;
};

export interface WarehousesResponse {
  errors(arg0: string, errors: any): unknown;
  success: any;
  data: any;
  warehouses: Warehouse[];
}
