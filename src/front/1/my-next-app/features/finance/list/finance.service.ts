import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";
import type {
  CalculateFixedSalaryRequest,
  PersonnelEarningsReportDto,
  PersonnelSettlementDto,
  PersonnelSettlementsRequest,
  RegisterPersonnelSettlementRequest,
} from "./finance.types";

export class FinanceService {
  private readonly api = new ApiClient();

  calculateFixedSalary(
    request: CalculateFixedSalaryRequest,
  ): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, CalculateFixedSalaryRequest>(
      "/api/Finance/earnings/fixed-salary",
      request,
    );
  }

  calculateReceptionEarnings(
    receptionId: number,
  ): Promise<ApiResponse<{ createdCount: number }>> {
    return this.api.post<{ createdCount: number }, Record<string, never>>(
      `/api/Finance/earnings/receptions/${receptionId}/calculate`,
      {},
    );
  }

  getPersonnelEarnings(
    personnelId: number,
    request?: { from?: string; to?: string; signal?: AbortSignal },
  ): Promise<ApiResponse<PersonnelEarningsReportDto>> {
    return this.api.get<PersonnelEarningsReportDto>(
      `/api/Finance/reports/personnel/${personnelId}/earnings`,
      { from: request?.from, to: request?.to },
      { signal: request?.signal },
    );
  }

  getSettlements(
    request: PersonnelSettlementsRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PagedResult<PersonnelSettlementDto>>> {
    return this.api.get<PagedResult<PersonnelSettlementDto>>(
      "/api/Finance/settlements",
      request,
      config,
    );
  }

  registerSettlement(
    request: RegisterPersonnelSettlementRequest,
  ): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, RegisterPersonnelSettlementRequest>(
      "/api/Finance/settlements",
      request,
    );
  }
}
