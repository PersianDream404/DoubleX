import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  AddTariffServicesRequest,
  ServiceTariffDto,
  ServiceTariffRequest,
  UpdateTariffServiceRequest,
} from "./service-tariff.types";

export class ServiceTariffService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Services/Tariffs";

  getAll(config?: { signal?: AbortSignal }): Promise<ApiResponse<ServiceTariffDto[]>> {
    return this.api.get<ServiceTariffDto[]>(this.path, undefined, config);
  }

  getById(id: number, config?: { signal?: AbortSignal }): Promise<ApiResponse<ServiceTariffDto>> {
    return this.api.get<ServiceTariffDto>(`${this.path}/${id}`, undefined, config);
  }

  create(request: ServiceTariffRequest): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, ServiceTariffRequest>(this.path, request);
  }

  update(id: number, request: ServiceTariffRequest): Promise<ApiResponse<unknown>> {
    return this.api.put<unknown, ServiceTariffRequest>(`${this.path}/${id}`, request);
  }

  delete(id: number): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(`${this.path}/${id}`);
  }

  addServices(tariffId: number, request: AddTariffServicesRequest): Promise<ApiResponse<unknown>> {
    return this.api.post<unknown, AddTariffServicesRequest>(
      `${this.path}/${tariffId}/Services`,
      request,
    );
  }

  updateService(
    tariffId: number,
    serviceId: number,
    request: UpdateTariffServiceRequest,
  ): Promise<ApiResponse<unknown>> {
    return this.api.put<unknown, UpdateTariffServiceRequest>(
      `${this.path}/${tariffId}/Services/${serviceId}`,
      request,
    );
  }

  removeService(tariffId: number, serviceId: number): Promise<ApiResponse<unknown>> {
    return this.api.delete<unknown>(
      `${this.path}/${tariffId}/Services/${serviceId}`,
    );
  }
}
