import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type { RegisterStockMovementRequest } from "./stock-movement.types";

export class StockMovementService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Inventory/Movements";

  registerMovement(
    request: RegisterStockMovementRequest,
  ): Promise<ApiResponse<void>> {
    return this.api.post<void, RegisterStockMovementRequest>(this.path, request);
  }
}
