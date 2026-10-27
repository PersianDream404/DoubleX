import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type { CashRegisterListResult } from "./cash-register.types";

export class CashRegisterService {
  private readonly api = new ApiClient();

  getAll(
    query: { pageNumber: number; pageSize: number; isActive?: boolean },
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CashRegisterListResult>> {
    return this.api.get<CashRegisterListResult>(
      "/api/CashRegisters",
      {
        PageNumber: query.pageNumber,
        PageSize: query.pageSize,
        IsActive: query.isActive,
      },
      config,
    );
  }
}
