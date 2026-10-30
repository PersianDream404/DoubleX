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
