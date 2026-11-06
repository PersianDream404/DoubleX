import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  CreateSalonWithAdminRequest,
  CreateSalonWithAdminResponse,
} from "./admin-salon.types";

export class AdminSalonService {
  private readonly api = new ApiClient();

  createWithAdmin(
    request: CreateSalonWithAdminRequest,
  ): Promise<ApiResponse<CreateSalonWithAdminResponse>> {
    return this.api.post<CreateSalonWithAdminResponse, CreateSalonWithAdminRequest>(
      "/api/Identity/admin/salons",
      request,
    );
  }
}
