import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";

import type {
  CreateSalonRequest,
  GetAllSalonRequest,
  GetSelectListSalonResponse,
  SalonDto,
  SalonDetailsDto,
  UpdateSalonRequest,
} from "./salon.types";

export class SalonService {
  private readonly api: ApiClient;

  constructor() {
    this.api = new ApiClient();
  }

  getAll(
    query?: GetAllSalonRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PagedResult<SalonDto>>> {
    return this.api.get<PagedResult<SalonDto>>("/api/Salon", query, config);
  }

  getById(id: number): Promise<ApiResponse<SalonDetailsDto>> {
    return this.api.get<SalonDetailsDto>(`/api/Salon/${id}`);
  }

  create(request: CreateSalonRequest): Promise<ApiResponse<void>> {
    return this.api.post<void, CreateSalonRequest>("/api/Salon", request);
  }

  update(request: UpdateSalonRequest): Promise<ApiResponse<void>> {
    return this.api.put<void, UpdateSalonRequest>(
      `/api/Salon/${request.id}`,
      request,
    );
  }

  delete(id: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(`/api/Salon/${id}`);
  }

  toggle(id: number): Promise<ApiResponse<void>> {
    return this.api.put<void, Record<string, never>>(
      `/api/Salon/${id}/Toggle`,
      {},
    );
  }

  getSelectList(
    query?: GetAllSalonRequest,
  ): Promise<ApiResponse<PagedResult<GetSelectListSalonResponse>>> {
    return this.api.get<PagedResult<GetSelectListSalonResponse>>(
      "/api/Salon/SelectList",
      query,
    );
  }
}
