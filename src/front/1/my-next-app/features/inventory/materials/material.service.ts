import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  MaterialDto,
  MaterialListRequest,
  MaterialListResult,
  MaterialRequest,
  UpdateMaterialRequest,
} from "./material.types";

export class MaterialService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Inventory/Materials";

  getAll(
    query?: MaterialListRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<MaterialListResult>> {
    return this.api.get<MaterialListResult>(this.path, query, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<MaterialDto>> {
    return this.api.get<MaterialDto>(`${this.path}/${id}`, undefined, config);
  }

  create(request: MaterialRequest): Promise<ApiResponse<void>> {
    return this.api.post<void, MaterialRequest>(this.path, request);
  }

  update(request: UpdateMaterialRequest): Promise<ApiResponse<void>> {
    return this.api.put<void, UpdateMaterialRequest>(
      `${this.path}/${request.id}`,
      request,
    );
  }

  delete(id: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  async setActive(id: number, isActive: boolean): Promise<ApiResponse<void>> {
    const response = await this.getById(id);
    if (!response.data) {
      throw new Error(response.message ?? "اطلاعات کالا دریافت نشد.");
    }

    const {
      name,
      code,
      brand,
      baseUnitId,
      purchaseUnitId,
      purchaseConversionFactor,
      minimumStock,
    } = response.data;

    return this.update({
      id,
      name,
      code,
      brand,
      baseUnitId,
      purchaseUnitId,
      purchaseConversionFactor,
      minimumStock,
      isActive,
    });
  }
}
