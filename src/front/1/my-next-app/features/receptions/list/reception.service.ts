import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";

import type {
  AddReceptionServiceRequest,
  CreateReceptionRequest,
  CustomerSelectItem,
  ReceptionDetails,
  ReceptionListQuery,
  ReceptionListResult,
  RegisterReceptionMaterialRequest,
  RegisterReceptionPaymentRequest,
} from "./reception.types";

export type ReceptionStatus = 1 | 2 | 3 | 4;

export class ReceptionService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Receptions";

  getAll(
    query: ReceptionListQuery,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionListResult>> {
    return this.api.get<ReceptionListResult>(this.path, query, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionDetails>> {
    return this.api.get<ReceptionDetails>(`${this.path}/${id}`, undefined, config);
  }

  create(
    request: CreateReceptionRequest,
  ): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, CreateReceptionRequest>(
      this.path,
      request,
    );
  }

  changeStatus(
    id: number,
    salonId: number,
    status: ReceptionStatus,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, { salonId: number; status: ReceptionStatus }>(
      `${this.path}/${id}/status`,
      { salonId, status },
    );
  }

  addService(
    receptionId: number,
    request: AddReceptionServiceRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<{ message: string }, AddReceptionServiceRequest>(
      `${this.path}/${receptionId}/services`,
      request,
    );
  }

  registerMaterial(
    receptionId: number,
    serviceId: number,
    request: RegisterReceptionMaterialRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<{ message: string }, RegisterReceptionMaterialRequest>(
      `${this.path}/${receptionId}/services/${serviceId}/materials`,
      request,
    );
  }

  registerPayment(
    receptionId: number,
    request: RegisterReceptionPaymentRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<{ message: string }, RegisterReceptionPaymentRequest>(
      `${this.path}/${receptionId}/payments`,
      request,
    );
  }

  getCustomers(
    query: { q?: string; pageNumber: number; pageSize: number },
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PagedResult<CustomerSelectItem>>> {
    return this.api.get<PagedResult<CustomerSelectItem>>(
      "/api/Customer/SelectList",
      {
        Q: query.q || undefined,
        PageNumber: query.pageNumber,
        PageSize: query.pageSize,
      },
      config,
    );
  }
}
