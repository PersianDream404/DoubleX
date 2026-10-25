export interface RegisterStockMovementRequest {
  materialId: number;
  type: 1 | 2;
  quantity: number;
  occurredAt: string;
  referenceType: null;
  referenceId: null;
  description: string | null;
  idempotencyKey: string;
}
