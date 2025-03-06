import { Product } from "./product";


export type ProductImage = {
    id: string;
    url: string;
    productId: string;
    product?: Product;
}
