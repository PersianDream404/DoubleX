"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import type { PersonnelListResult } from "./personnel.types";
import { PersonnelService } from "./personnel.service";
import { PersonnelListQuerySchema } from "./personnel.schema";

const PAGE_SIZE = 50;

type PersonnelsState = {
  requestKey: string;
  data: PersonnelListResult | null;
  error: string | null;
  loading: boolean;
};

export function usePersonnels(
  query: string,
  page: number,
  refreshKey = 0,
) {
  const requestKey = JSON.stringify([query, page, refreshKey]);
  const [state, setState] = useState<PersonnelsState>({
    requestKey: "",
    data: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    const controller = new AbortController();

    Promise.resolve()
      .then(() => {
        const parsedQuery = PersonnelListQuerySchema.safeParse({
          q: query,
          pageNumber: page,
        });

        if (!parsedQuery.success) {
          throw new Error("پارامترهای جستجوی پرسنل معتبر نیست.");
        }

        const service = new PersonnelService();
        const { q, pageNumber } = parsedQuery.data;

        return service.getAll(
          {
            Q: q || undefined,
            PageNumber: pageNumber,
            PageSize: PAGE_SIZE,
          },
          { signal: controller.signal },
        );
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "پاسخ فهرست پرسنل خالی است.");
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
  }, [query, page, refreshKey, requestKey]);

  if (state.requestKey !== requestKey) {
    return { data: null, error: null, loading: true };
  }

  return state;
}
