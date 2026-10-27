"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceTariffService } from "./service-tariff.service";
import type { ServiceTariffDto } from "./service-tariff.types";

type TariffsState = {
  key: string;
  data: ServiceTariffDto[] | null;
  error: string | null;
  loading: boolean;
};

export function useServiceTariffs(salonId: number, refreshKey: number) {
  const key = `${salonId}:${refreshKey}`;
  const [state, setState] = useState<TariffsState>({
    key: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    if (!salonId) return;
    const controller = new AbortController();
    new ServiceTariffService()
      .getAll({ signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست تعرفه‌ها دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setState({ key, data: response.data, error: null, loading: false });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setState({ key, data: null, error: handleApiError(reason), loading: false });
        }
      });
    return () => controller.abort();
  }, [key, salonId]);

  if (!salonId) return { data: [], error: null, loading: false };
  if (state.key !== key) return { data: null, error: null, loading: true };
  return state;
}
