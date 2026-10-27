import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";

import type {
  CashMovementRequest,
  CashRegisterBalance,
  CashRegisterCreateRequest,
  CashRegisterListResult,
  CashRegisterSession,
  CashRegisterTransaction,
  CashRegisterUpdateRequest,
} from "./cash-register.types";

export class CashRegisterService {
  private readonly api = new ApiClient();

  getAll(
    query: {
      pageNumber: number;
      pageSize: number;
      q?: string;
      isActive?: boolean;
    },
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CashRegisterListResult>> {
    return this.api.get<CashRegisterListResult>(
      "/api/CashRegisters",
      {
        PageNumber: query.pageNumber,
        PageSize: query.pageSize,
        Q: query.q,
        IsActive: query.isActive,
      },
      config,
    );
  }

  create(request: CashRegisterCreateRequest): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, CashRegisterCreateRequest>(
      "/api/CashRegisters",
      request,
    );
  }

  update(
    id: number,
    request: CashRegisterUpdateRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, CashRegisterUpdateRequest>(
      `/api/CashRegisters/${id}`,
      request,
    );
  }

  toggle(id: number): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, Record<string, never>>(
      `/api/CashRegisters/${id}/Toggle`,
      {},
    );
  }

  getActiveSession(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CashRegisterSession>> {
    return this.api.get<CashRegisterSession>(
      `/api/CashRegisters/${id}/sessions/active`,
      undefined,
      config,
    );
  }

  getBalance(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CashRegisterBalance>> {
    return this.api.get<CashRegisterBalance>(
      `/api/CashRegisters/${id}/balance`,
      undefined,
      config,
    );
  }

  getTransactions(
    id: number,
    query: { pageNumber: number; pageSize: number; sessionId?: number },
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PagedResult<CashRegisterTransaction>>> {
    return this.api.get<PagedResult<CashRegisterTransaction>>(
      `/api/CashRegisters/${id}/transactions`,
      {
        PageNumber: query.pageNumber,
        PageSize: query.pageSize,
        SessionId: query.sessionId,
      },
      config,
    );
  }

  openSession(
    id: number,
    openingBalance: number,
  ): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, { openingBalance: number }>(
      `/api/CashRegisters/${id}/sessions`,
      { openingBalance },
    );
  }

  closeSession(
    id: number,
    sessionId: number,
    actualClosingBalance: number,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<
      { message: string },
      { actualClosingBalance: number }
    >(
      `/api/CashRegisters/${id}/sessions/${sessionId}/close`,
      { actualClosingBalance },
    );
  }

  recordMovement(
    id: number,
    type: "cash-in" | "cash-out",
    request: CashMovementRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<{ message: string }, CashMovementRequest>(
      `/api/CashRegisters/${id}/${type}`,
      request,
    );
  }
}
