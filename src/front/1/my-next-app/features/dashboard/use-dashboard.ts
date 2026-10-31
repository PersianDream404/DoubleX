"use client";

import { useCallback, useEffect, useMemo, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import { MaterialService } from "@/features/inventory/materials/material.service";

import { DashboardService } from "./dashboard.service";
import type { DashboardData, DashboardPeriod } from "./dashboard.types";

interface DashboardState {
  key: string;
  data: DashboardData | null;
  error: string | null;
  loading: boolean;
}

function requireData<T>(data: T | null, message: string): T {
  if (data === null) throw new Error(message);
  return data;
}

export function useDashboard(period: DashboardPeriod) {
  const service = useMemo(() => new DashboardService(), []);
  const materialService = useMemo(() => new MaterialService(), []);
  const [refreshKey, setRefreshKey] = useState(0);
  const [state, setState] = useState<DashboardState | null>(null);
  const requestKey = `${period}-${refreshKey}`;

  useEffect(() => {
    const controller = new AbortController();
    const config = { signal: controller.signal };

    async function loadDashboard() {
      try {
        const lowStockRequest = materialService
          .getAll(
            {
              IsActive: true,
              LowStockOnly: true,
              PageNumber: 1,
              PageSize: 50,
            },
            config,
          )
          .then((response) => ({
            data: requireData(
              response.data,
              response.message ?? "فهرست موجودی مواد دریافت نشد.",
            ),
            error: null,
          }))
          .catch((cause: unknown) => ({
            data: null,
            error: handleApiError(cause),
          }));

        const [
          summaryResponse,
          chartsResponse,
          appointmentsResponse,
          alertsResponse,
          lowStockResult,
        ] =
          await Promise.all([
            service.getSummary(period, config),
            service.getCharts(period, config),
            service.getTodayAppointments(config),
            service.getAlerts(config),
            lowStockRequest,
          ]);

        const data: DashboardData = {
          summary: requireData(
            summaryResponse.data,
            summaryResponse.message ?? "خلاصه داشبورد دریافت نشد.",
          ),
          charts: requireData(
            chartsResponse.data,
            chartsResponse.message ?? "نمودارهای داشبورد دریافت نشد.",
          ),
          appointments: requireData(
            appointmentsResponse.data,
            appointmentsResponse.message ?? "نوبت‌های امروز دریافت نشد.",
          ),
          alerts: requireData(
            alertsResponse.data,
            alertsResponse.message ?? "هشدارهای داشبورد دریافت نشد.",
          ),
          lowStockMaterials: lowStockResult.data,
          lowStockError: lowStockResult.error,
        };

        if (!controller.signal.aborted) {
          setState({ key: requestKey, data, error: null, loading: false });
        }
      } catch (cause: unknown) {
        if (!controller.signal.aborted) {
          setState({
            key: requestKey,
            data: null,
            error: handleApiError(cause),
            loading: false,
          });
        }
      }
    }

    void loadDashboard();
    return () => controller.abort();
  }, [materialService, period, requestKey, service]);

  const refresh = useCallback(() => setRefreshKey((key) => key + 1), []);
  const current = state?.key === requestKey ? state : null;

  return {
    data: current?.data ?? null,
    error: current?.error ?? null,
    loading: current === null || current.loading,
    refresh,
  };
}
