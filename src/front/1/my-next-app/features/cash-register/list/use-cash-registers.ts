"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import type { PagedResult } from "@/lib/api/types/paged-result";

import { CashRegisterListQuerySchema } from "../cash-register.schema";
import { CashRegisterService } from "../cash-register.service";
import type { CashRegisterOption } from "../cash-register.types";

const PAGE_SIZE = 20;

type CashRegistersState = {
  requestKey: string;
  data: PagedResult<CashRegisterOption> | null;
  error: string | null;
  loading: boolean;
};

export function useCashRegisters(query: string, page: number, refreshKey: number) {
  const requestKey = JSON.stringify([query, page, refreshKey]);
  const [state, setState] = useState<CashRegistersState>({
    requestKey: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();
    const service = new CashRegisterService();

    Promise.resolve()
      .then(() => {
        const parsedQuery = CashRegisterListQuerySchema.safeParse({
          pageNumber: page,
          pageSize: PAGE_SIZE,
          q: query.trim() || undefined,
        });
        if (!parsedQuery.success) {
          throw new Error("پارامترهای فهرست صندوق معتبر نیست.");
        }
        return service.getAll(
          {
            pageNumber: parsedQuery.data.pageNumber,
            pageSize: parsedQuery.data.pageSize,
            q: parsedQuery.data.q,
          },
          { signal: controller.signal },
        );
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "پاسخ فهرست صندوق‌ها خالی است.");
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
  }, [page, query, requestKey]);

  if (state.requestKey !== requestKey) {
    return { data: null, error: null, loading: true };
  }
  return state;
}
