import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  DashboardAlert,
  DashboardCharts,
  DashboardPeriod,
  DashboardSummary,
  DashboardTodayAppointments,
} from "./dashboard.types";

export class DashboardService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Dashboard";

  getSummary(
    period: DashboardPeriod,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<DashboardSummary>> {
    return this.api.get<DashboardSummary>(
      `${this.path}/summary`,
      { period },
      config,
    );
  }

  getCharts(
    period: DashboardPeriod,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<DashboardCharts>> {
    return this.api.get<DashboardCharts>(
      `${this.path}/charts`,
      { period },
      config,
    );
  }

  getTodayAppointments(
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<DashboardTodayAppointments>> {
    return this.api.get<DashboardTodayAppointments>(
      `${this.path}/appointments/today`,
      undefined,
      config,
    );
  }

  getAlerts(
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<DashboardAlert[]>> {
    return this.api.get<DashboardAlert[]>(
      `${this.path}/alerts`,
      undefined,
      config,
    );
  }
}
