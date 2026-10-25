"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { MaterialListQuerySchema } from "./material.schema";
import { MaterialService } from "./material.service";
import type { MaterialListResult } from "./material.types";

const PAGE_SIZE = 50;

type MaterialsState = {
  requestKey: string;
  data: MaterialListResult | null;
  error: string | null;
  loading: boolean;
};

export function useMaterials(query: string, page: number, refreshKey = 0) {
  const requestKey = JSON.stringify([query, page, refreshKey]);
  const [state, setState] = useState<MaterialsState>({
    requestKey: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();

    Promise.resolve()
      .then(() => {
        const parsed = MaterialListQuerySchema.safeParse({
          q: query,
          pageNumber: page,
        });
        if (!parsed.success) {
          throw new Error("پارامترهای جستجوی کالا معتبر نیست.");
        }

        return new MaterialService().getAll(
          {
            Q: parsed.data.q || undefined,
            PageNumber: parsed.data.pageNumber,
            PageSize: PAGE_SIZE,
          },
          { signal: controller.signal },
        );
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "پاسخ فهرست کالاها خالی است.");
        }

        if (!controller.signal.aborted) {
          setState({ requestKey, data: response.data, error: null, loading: false });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            requestKey,
            data: null,
            error: handleApiError(error),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [query, page, refreshKey, requestKey]);

  if (state.requestKey !== requestKey) {
    return { data: null, error: null, loading: true };
  }

  return state;
}
