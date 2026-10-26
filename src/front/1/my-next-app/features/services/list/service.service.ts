import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  ServiceCategoryDto,
  ServiceDto,
  ServiceMaterialDto,
  ServiceMaterialRequest,
  ServiceRequest,
  UpdateServiceRequest,
} from "./service.types";

export class ServiceCatalogService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Services";

  getAll(
    salonId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceDto[]>> {
    return this.api.get<ServiceDto[]>(this.path, { salonId }, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceDto>> {
    return this.api.get<ServiceDto>(`${this.path}/${id}`, undefined, config);
  }

  getCategories(
    salonId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceCategoryDto[]>> {
    return this.api.get<ServiceCategoryDto[]>(
      `${this.path}/Categories`,
      { salonId },
      config,
    );
  }

  create(request: ServiceRequest): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, ServiceRequest>(this.path, request);
  }

  update(id: number, request: UpdateServiceRequest): Promise<ApiResponse<unknown>> {
    return this.api.put<unknown, UpdateServiceRequest>(
      `${this.path}/${id}`,
      request,
    );
  }

  deactivate(id: number, salonId: number): Promise<ApiResponse<unknown>> {
    return this.api.patch<unknown, Record<string, never>>(
      `${this.path}/${id}/Deactivate`,
      {},
      { params: { salonId } },
    );
  }

  getMaterials(
    salonId: number,
    serviceId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceMaterialDto[]>> {
    return this.api.get<ServiceMaterialDto[]>(
      `${this.path}/Materials`,
      { salonId, serviceId },
      config,
    );
  }

  addMaterial(request: ServiceMaterialRequest): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, ServiceMaterialRequest>(
      `${this.path}/Materials`,
      request,
    );
  }

  updateMaterial(
    id: number,
    request: ServiceMaterialRequest,
  ): Promise<ApiResponse<unknown>> {
    return this.api.put<unknown, ServiceMaterialRequest>(
      `${this.path}/Materials/${id}`,
      request,
    );
  }

  removeMaterial(
    id: number,
    salonId: number,
  ): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(
      `${this.path}/Materials/${id}`,
      { params: { salonId } },
    );
  }
}
