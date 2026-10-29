"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { ReceptionListQuerySchema } from "./reception.schema";
import { ReceptionService } from "./reception.service";
import type { ReceptionListItem } from "./reception.types";

const PAGE_SIZE = 100;

type ReceptionState = {
  requestKey: number;
  data: ReceptionListItem[] | null;
  error: string | null;
  loading: boolean;
};

export function useReceptions(refreshKey = 0) {
  const [state, setState] = useState<ReceptionState>({
    requestKey: -1,
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();
    const service = new ReceptionService();

    async function load() {
      try {
        const firstQuery = ReceptionListQuerySchema.safeParse({
          pageNumber: 1,
          pageSize: PAGE_SIZE,
        });
        if (!firstQuery.success) {
          throw new Error("پارامترهای فهرست پذیرش معتبر نیست.");
        }

        const firstResponse = await service.getAll(
          {
            PageNumber: firstQuery.data.pageNumber,
            PageSize: firstQuery.data.pageSize,
          },
          { signal: controller.signal },
        );
        if (!firstResponse.data) {
          throw new Error(firstResponse.message ?? "فهرست پذیرش دریافت نشد.");
        }

        const items = [...firstResponse.data.list];
        for (
          let pageNumber = 2;
          pageNumber <= firstResponse.data.pagination.totalPages;
          pageNumber += 1
        ) {
          const parsed = ReceptionListQuerySchema.safeParse({
            pageNumber,
            pageSize: PAGE_SIZE,
          });
          if (!parsed.success) {
            throw new Error("شماره صفحه فهرست پذیرش معتبر نیست.");
          }

          const response = await service.getAll(
            {
              PageNumber: parsed.data.pageNumber,
              PageSize: parsed.data.pageSize,
            },
            { signal: controller.signal },
          );
          if (!response.data) {
            throw new Error(response.message ?? "بخشی از فهرست پذیرش دریافت نشد.");
          }
          items.push(...response.data.list);
        }

        if (!controller.signal.aborted) {
          setState({
            requestKey: refreshKey,
            data: items,
            error: null,
            loading: false,
          });
        }
      } catch (error: unknown) {
        if (!controller.signal.aborted) {
          setState({
            requestKey: refreshKey,
            data: null,
            error: handleApiError(error),
            loading: false,
          });
        }
      }
    }

    void load();
    return () => controller.abort();
  }, [refreshKey]);

  if (state.requestKey !== refreshKey) {
    return { data: null, error: null, loading: true };
  }

  return state;
}
