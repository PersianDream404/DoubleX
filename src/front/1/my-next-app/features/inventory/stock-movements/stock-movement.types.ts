export interface RegisterStockMovementRequest {
  materialId: number;
  type: 1 | 2;
  quantity: number;
  purchaseQuantity: number | null;
  purchaseUnitPrice: number | null;
  occurredAt: string;
  referenceType: null;
  referenceId: null;
  description: string | null;
  idempotencyKey: string;
}

export interface QuoteStockMovementCostsRequest {
  items: {
    key: string;
    materialId: number;
    quantity: number;
  }[];
}

export interface StockMovementCostQuote {
  items: {
    key: string;
    unitCost: number;
    totalCost: number;
  }[];
  totalCost: number;
}
