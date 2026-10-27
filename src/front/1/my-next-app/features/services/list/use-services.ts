"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import { ServiceCatalogService } from "./service.service";
import type { ServiceDto } from "./service.types";

type ServicesState = {
  key: string;
  data: ServiceDto[] | null;
  error: string | null;
  loading: boolean;
};

export function useServices(salonId: number, refreshKey: number) {
  const key = JSON.stringify([salonId, refreshKey]);
  const [state, setState] = useState<ServicesState>({
    key: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    if (!salonId) return;

    const controller = new AbortController();
    Promise.resolve()
      .then(() => {
        return new ServiceCatalogService().getAll({ signal: controller.signal });
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست خدمات دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setState({ key, data: response.data, error: null, loading: false });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            key,
            data: null,
            error: handleApiError(error),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [key, salonId, refreshKey]);

  if (!salonId) {
    return { data: [], error: null, loading: false };
  }
  if (state.key !== key) {
    return { data: null, error: null, loading: true };
  }
  return state;
}
