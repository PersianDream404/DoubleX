"use client";

import { useEffect, useState } from "react";

import { PersonnelService } from "@/features/personnel/list/personnel.service";
import { handleApiError } from "@/lib/api/handle-api-error";
import { FinanceService } from "./finance.service";
import type {
  FinancePageData,
  PersonnelEarningsReportDto,
  PersonnelSettlementDto,
} from "./finance.types";

const PAGE_SIZE = 10;

type UseFinanceOptions = {
  personnelId: number | null;
  page: number;
  refreshKey: number;
  earningsFrom?: string;
  earningsTo?: string;
  earningsRangeValid: boolean;
};

type EarningsState = {
  personnelId: number;
  data: PersonnelEarningsReportDto | null;
  loading: boolean;
  error: string | null;
};

export function useFinance({
  personnelId,
  page,
  refreshKey,
  earningsFrom,
  earningsTo,
  earningsRangeValid,
}: UseFinanceOptions): FinancePageData {
  const [personnel, setPersonnel] = useState<FinancePageData["personnel"]>([]);
  const [personnelLoading, setPersonnelLoading] = useState(true);
  const [personnelError, setPersonnelError] = useState<string | null>(null);
  const [settlements, setSettlements] = useState<FinancePageData["settlements"] | null>(null);
  const [settlementsLoading, setSettlementsLoading] = useState(true);
  const [settlementsError, setSettlementsError] = useState<string | null>(null);
  const [earningsState, setEarningsState] = useState<EarningsState | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    const personnelService = new PersonnelService();

    Promise.resolve()
      .then(() =>
        personnelService.getAll(
          { PageNumber: 1, PageSize: 100 },
          { signal: controller.signal },
        ),
      )
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست پرسنل دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setPersonnel(response.data.list.filter((item) => item.status));
          setPersonnelError(null);
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setPersonnelError(handleApiError(error));
          setPersonnel([]);
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setPersonnelLoading(false);
      });

    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    const service = new FinanceService();

    Promise.resolve()
      .then(() => {
        if (controller.signal.aborted) return null;
        setSettlementsLoading(true);
        return service.getSettlements(
          {
            pageNumber: page,
            pageSize: PAGE_SIZE,
            personnelId: personnelId ?? undefined,
          },
          { signal: controller.signal },
        );
      })
      .then((response) => {
        if (!response) return;
        if (!response.data) {
          throw new Error(response.message ?? "فهرست تسویه‌ها دریافت نشد.");
        }
        setSettlements(response.data);
        setSettlementsError(null);
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setSettlementsError(handleApiError(error));
          setSettlements(null);
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setSettlementsLoading(false);
      });

    return () => controller.abort();
  }, [personnelId, page, refreshKey]);

  useEffect(() => {
    if (personnelId === null || !earningsRangeValid) return;

    const controller = new AbortController();
    Promise.resolve()
      .then(() => {
        if (controller.signal.aborted) return null;
        setEarningsState({
          personnelId,
          data: null,
          loading: true,
          error: null,
        });
        return new FinanceService().getPersonnelEarnings(personnelId, {
          from: earningsFrom,
          to: earningsTo,
          signal: controller.signal,
        });
      })
      .then((response) => {
        if (!response) return;
        if (!response.data) {
          throw new Error(response.message ?? "گزارش مطالبات دریافت نشد.");
        }
        setEarningsState({
          personnelId,
          data: response.data,
          loading: false,
          error: null,
        });
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setEarningsState({
            personnelId,
            data: null,
            loading: false,
            error: handleApiError(error),
          });
        }
      })

    return () => controller.abort();
  }, [personnelId, refreshKey, earningsFrom, earningsTo, earningsRangeValid]);

  const currentEarnings =
    personnelId !== null && earningsState?.personnelId === personnelId
      ? earningsState
      : null;

  return {
    personnel,
    settlements: settlements ?? {
      list: [] as PersonnelSettlementDto[],
      pagination: { totalCount: 0, totalPages: 0, pageNumber: page, pageSize: PAGE_SIZE },
      extraData: null,
    },
    earnings: currentEarnings?.data ?? null,
    personnelLoading,
    settlementsLoading,
    earningsLoading: personnelId !== null && (currentEarnings?.loading ?? true),
    personnelError,
    settlementsError,
    earningsError: currentEarnings?.error ?? null,
  };
}
