import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  GetAllPersonnelRequest,
  PersonnelListResult,
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
}
