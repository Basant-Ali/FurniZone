export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  imageUrl: string;
}

export interface ProductsResponse {
  data: Product[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface ApiResponse {
  success: boolean;
  message: string;
  data: ProductsResponse;
  errors: string[];
}
