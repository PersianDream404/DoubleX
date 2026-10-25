import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  UnitDto,
  UnitListRequest,
  UnitListResult,
  UnitRequest,
  UpdateUnitRequest,
} from "./unit.types";

export class UnitService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Inventory/Units";

  getAll(
    query?: UnitListRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<UnitListResult>> {
    return this.api.get<UnitListResult>(this.path, query, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<UnitDto>> {
    return this.api.get<UnitDto>(`${this.path}/${id}`, undefined, config);
  }

  create(request: UnitRequest): Promise<ApiResponse<void>> {
    return this.api.post<void, UnitRequest>(this.path, request);
  }

  update(request: UpdateUnitRequest): Promise<ApiResponse<void>> {
    return this.api.put<void, UpdateUnitRequest>(
      `${this.path}/${request.id}`,
      request,
    );
  }

  delete(id: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  async setActive(id: number, isActive: boolean): Promise<ApiResponse<void>> {
    if (!isActive) {
      const response = await this.api.put<void, UpdateUnitRequest>(
        `${this.path}/${id}`,
        { ...(await this.requireUnit(id)), isActive: false },
      );
      return response;
    }

    return this.api.put<void, UpdateUnitRequest>(`${this.path}/${id}`, {
      ...(await this.requireUnit(id)),
      isActive: true,
    });
  }

  private async requireUnit(id: number): Promise<UpdateUnitRequest> {
    const response = await this.getById(id);
    if (!response.data) {
      throw new Error(response.message ?? "اطلاعات واحد دریافت نشد.");
    }

    const { id: unitId, name, symbol, unitType, isActive } = response.data;
    return { id: unitId, name, symbol, unitType, isActive };
  }
}
