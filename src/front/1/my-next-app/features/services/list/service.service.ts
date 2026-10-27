import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  ServiceCategoryDto,
  ServiceCategoryRequest,
  ServiceDto,
  ServiceMaterialDto,
  ServiceMaterialRequest,
  ServicePersonnelDto,
  ServicePersonnelRequest,
  ServiceRequest,
  UpdateServiceRequest,
  UpdateServiceCategoryRequest,
} from "./service.types";

export class ServiceCatalogService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Services";

  getAll(config?: { signal?: AbortSignal }): Promise<ApiResponse<ServiceDto[]>> {
    return this.api.get<ServiceDto[]>(this.path, undefined, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceDto>> {
    return this.api.get<ServiceDto>(`${this.path}/${id}`, undefined, config);
  }

  getCategories(config?: { signal?: AbortSignal }): Promise<ApiResponse<ServiceCategoryDto[]>> {
    return this.api.get<ServiceCategoryDto[]>(
      `${this.path}/Categories`,
      undefined,
      config,
    );
  }

  getCategoryById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceCategoryDto>> {
    return this.api.get<ServiceCategoryDto>(`${this.path}/Categories/${id}`, undefined, config);
  }

  createCategory(request: ServiceCategoryRequest): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, ServiceCategoryRequest>(`${this.path}/Categories`, request);
  }

  updateCategory(id: number, request: UpdateServiceCategoryRequest): Promise<ApiResponse<unknown>> {
    return this.api.put<unknown, UpdateServiceCategoryRequest>(
      `${this.path}/Categories/${id}`,
      request,
    );
  }

  deactivateCategory(id: number): Promise<ApiResponse<unknown>> {
    return this.api.patch<unknown, Record<string, never>>(
      `${this.path}/Categories/${id}/Deactivate`,
      {},
    );
  }

  deleteCategory(id: number): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(`${this.path}/Categories/${id}`);
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

  deactivate(id: number): Promise<ApiResponse<unknown>> {
    return this.api.patch<unknown, Record<string, never>>(
      `${this.path}/${id}/Deactivate`,
      {},
    );
  }

  getMaterials(
    serviceId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServiceMaterialDto[]>> {
    return this.api.get<ServiceMaterialDto[]>(
      `${this.path}/Materials`,
      { serviceId },
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

  removeMaterial(id: number): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(`${this.path}/Materials/${id}`);
  }

  getServicePersonnels(
    serviceId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServicePersonnelDto[]>> {
    return this.api.get<ServicePersonnelDto[]>(
      `${this.path}/Personnels`,
      { serviceId },
      config,
    );
  }

  getPersonnelServiceAssignments(
    personnelId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ServicePersonnelDto[]>> {
    return this.api.get<ServicePersonnelDto[]>(
      `${this.path}/Personnels/ByPersonnel`,
      { personnelId },
      config,
    );
  }

  addPersonnelToService(
    request: ServicePersonnelRequest,
  ): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, ServicePersonnelRequest>(
      `${this.path}/Personnels`,
      request,
    );
  }

  removePersonnelFromService(
    id: number,
    serviceId: number,
  ): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(`${this.path}/Personnels/${id}`, {
      params: { serviceId },
    });
  }
}
