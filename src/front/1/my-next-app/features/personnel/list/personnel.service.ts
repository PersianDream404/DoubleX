import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  GetAllPersonnelRequest,
  CreatePersonnelRequest,
  PersonnelDetailsDto,
  PersonnelListResult,
  UpdatePersonnelRequest,
} from "./personnel.types";

export class PersonnelService {
  private readonly api: ApiClient;

  constructor() {
    this.api = new ApiClient();
  }

  getAll(
    query?: GetAllPersonnelRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PersonnelListResult>> {
    return this.api.get<PersonnelListResult>(
      "/api/Personnel",
      query,
      config,
    );
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<PersonnelDetailsDto>> {
    return this.api.get<PersonnelDetailsDto>(
      `/api/Personnel/${id}`,
      undefined,
      config,
    );
  }

  create(request: CreatePersonnelRequest): Promise<ApiResponse<string>> {
    return this.api.post<string, CreatePersonnelRequest>(
      "/api/Personnel",
      request,
    );
  }

  update(id: number, request: UpdatePersonnelRequest): Promise<ApiResponse<string>> {
    return this.api.put<string, UpdatePersonnelRequest>(
      `/api/Personnel/${id}`,
      request,
    );
  }

  delete(id: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(`/api/Personnel/${id}`);
  }

  toggle(id: number): Promise<ApiResponse<string>> {
    return this.api.put<string, undefined>(
      `/api/Personnel/${id}/Toggle`,
      undefined,
    );
  }
}
