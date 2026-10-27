"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { CustomerListQuerySchema } from "./customer.schema";
import { CustomerService } from "./customer.service";
import type { CustomerListResult } from "./customer.types";

const PAGE_SIZE = 20;

type CustomersState = {
  requestKey: string;
  data: CustomerListResult | null;
  error: string | null;
  loading: boolean;
};

export function useCustomers(query: string, page: number, refreshKey: number) {
  const requestKey = JSON.stringify([query, page, refreshKey]);
  const [state, setState] = useState<CustomersState>({
    requestKey: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();
    const service = new CustomerService();

    Promise.resolve()
      .then(() => {
        const parsed = CustomerListQuerySchema.safeParse({
          pageNumber: page,
          pageSize: PAGE_SIZE,
          q: query.trim() || undefined,
        });
        if (!parsed.success) {
          throw new Error("پارامترهای فهرست مشتری معتبر نیست.");
        }
        return service.getAll(
          {
            PageNumber: parsed.data.pageNumber,
            PageSize: parsed.data.pageSize,
            Q: parsed.data.q,
          },
          { signal: controller.signal },
        );
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "پاسخ فهرست مشتریان خالی است.");
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
