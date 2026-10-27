"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceCatalogService } from "../list/service.service";
import type { ServiceCategoryDto } from "../list/service.types";

type CategoriesState = {
  key: string;
  data: ServiceCategoryDto[] | null;
  error: string | null;
  loading: boolean;
};

export function useServiceCategories(salonId: number, refreshKey: number) {
  const key = `${salonId}:${refreshKey}`;
  const [state, setState] = useState<CategoriesState>({
    key: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    if (!salonId) return;
    const controller = new AbortController();
    new ServiceCatalogService()
      .getCategories({ signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست دسته‌بندی‌ها دریافت نشد.");
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
