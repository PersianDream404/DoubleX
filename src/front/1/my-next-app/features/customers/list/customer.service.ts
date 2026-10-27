import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  CustomerDetailsDto,
  CustomerListResult,
  CustomerRequest,
  GetAllCustomersRequest,
  UpdateCustomerRequest,
} from "./customer.types";

export class CustomerService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Customer";

  getAll(
    query?: GetAllCustomersRequest,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CustomerListResult>> {
    return this.api.get<CustomerListResult>(this.path, query, config);
  }

  getById(
    id: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<CustomerDetailsDto>> {
    return this.api.get<CustomerDetailsDto>(`${this.path}/${id}`, undefined, config);
  }

  create(request: CustomerRequest): Promise<ApiResponse<{ message: string }>> {
    return this.api.post<{ message: string }, CustomerRequest>(this.path, request);
  }

  update(
    request: UpdateCustomerRequest,
  ): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, UpdateCustomerRequest>(
      `${this.path}/${request.id}`,
      request,
    );
  }

  delete(id: number): Promise<ApiResponse<{ message: string }>> {
    return this.api.delete<{ message: string }>(`${this.path}/${id}`);
  }

  toggle(id: number): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, Record<string, never>>(
      `${this.path}/${id}/Toggle`,
      {},
    );
  }
}
