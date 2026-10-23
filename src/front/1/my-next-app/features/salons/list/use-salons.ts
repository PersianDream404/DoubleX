"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import type { PagedResult } from "@/lib/api/types/paged-result";

import { SalonService } from "./salon.service";
import type { SalonDto } from "./salon.types";

const PAGE_SIZE = 50;

type SalonsState = {
  requestKey: string;
  data: PagedResult<SalonDto> | null;
  error: string | null;
  loading: boolean;
};

export function useSalons(query: string, page: number) {
  const requestKey = JSON.stringify([query, page]);
  const [state, setState] = useState<SalonsState>({
    requestKey: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();
    const service = new SalonService();

    Promise.resolve()
      .then(() =>
        service.getAll(
          {
            Q: query.trim() || undefined,
            PageNumber: page,
            PageSize: PAGE_SIZE,
          },
          { signal: controller.signal },
        ),
      )
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "پاسخ فهرست سالن‌ها خالی است.");
        }

        if (!controller.signal.aborted) {
          setState({
            requestKey,
            data: response.data,
            error: null,
            loading: false,
          });
        }
      })
      .catch((error: unknown) => {
        if (controller.signal.aborted) {
          return;
        }

        setState({
          requestKey,
          data: null,
          error: handleApiError(error),
          loading: false,
        });
      });

    return () => controller.abort();
  }, [query, page, requestKey]);

  if (state.requestKey !== requestKey) {
    return { data: null, error: null, loading: true };
  }

  return state;
}
