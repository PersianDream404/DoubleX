"use client";

import { useEffect, useMemo, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import {
  ReceptionReportService,
  type ReceptionReportTab,
} from "./reception-report.service";
import type {
  ReceptionReportDebt,
  ReceptionReportDebtTotals,
  ReceptionReportDiscount,
  ReceptionReportDiscountTotals,
  ReceptionReportFilters,
  ReceptionReportMaterial,
  ReceptionReportMaterialTotals,
  ReceptionReportPage,
  ReceptionReportPersonnel,
  ReceptionReportReception,
  ReceptionReportServiceItem,
  ReceptionReportSummary,
  ReceptionReportTotals,
} from "./reception-report.types";

export type ReceptionReportData =
  | {
      kind: "receptions";
      result: ReceptionReportPage<ReceptionReportReception, ReceptionReportTotals>;
    }
  | {
      kind: "personnel";
      result: ReceptionReportPage<ReceptionReportPersonnel, ReceptionReportTotals>;
    }
  | {
      kind: "services";
      result: ReceptionReportPage<ReceptionReportServiceItem, ReceptionReportTotals>;
    }
  | {
      kind: "debts";
      result: ReceptionReportPage<ReceptionReportDebt, ReceptionReportDebtTotals>;
    }
  | {
      kind: "discounts";
      result: ReceptionReportPage<ReceptionReportDiscount, ReceptionReportDiscountTotals>;
    }
  | {
      kind: "materials";
      result: ReceptionReportPage<ReceptionReportMaterial, ReceptionReportMaterialTotals>;
    };

interface AsyncState<T> {
  key: string;
  data: T | null;
  error: string | null;
  loading: boolean;
}

export function useReceptionReports(
  tab: ReceptionReportTab,
  filters: ReceptionReportFilters,
  pageNumber: number,
  refreshKey: number,
) {
  const service = useMemo(() => new ReceptionReportService(), []);
  const [summaryState, setSummaryState] = useState<AsyncState<ReceptionReportSummary> | null>(null);
  const [reportState, setReportState] = useState<AsyncState<ReceptionReportData> | null>(null);
  const summaryKey = `${filters.fromDate}|${filters.toDateExclusive}|${refreshKey}`;
  const reportKey = JSON.stringify([tab, filters, pageNumber, refreshKey]);

  useEffect(() => {
    const controller = new AbortController();
    void service
      .getSummary(
        {
          fromDate: filters.fromDate,
          toDateExclusive: filters.toDateExclusive,
        },
        { signal: controller.signal },
      )
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "خلاصه گزارش دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setSummaryState({
            key: summaryKey,
            data: response.data,
            error: null,
            loading: false,
          });
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) {
          setSummaryState({
            key: summaryKey,
            data: null,
            error: handleApiError(cause),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [filters.fromDate, filters.toDateExclusive, service, summaryKey]);

  useEffect(() => {
    if (tab === "summary") return;
    const controller = new AbortController();

    async function loadReport() {
      const config = { signal: controller.signal };
      switch (tab) {
        case "receptions": {
          const response = await service.getReceptions(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش پذیرش دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
        case "personnel": {
          const response = await service.getPersonnel(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش پرسنل دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
        case "services": {
          const response = await service.getServices(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش خدمات دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
        case "debts": {
          const response = await service.getDebts(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش بدهی دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
        case "discounts": {
          const response = await service.getDiscounts(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش تخفیف دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
        case "materials": {
          const response = await service.getMaterials(filters, pageNumber, config);
          if (!response.data) throw new Error(response.message ?? "گزارش مواد مصرفی دریافت نشد.");
          return { kind: tab, result: response.data } satisfies ReceptionReportData;
        }
      }
    }

    void loadReport()
      .then((data) => {
        if (!controller.signal.aborted && data) {
          setReportState({
            key: reportKey,
            data,
            error: null,
            loading: false,
          });
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) {
          setReportState({
            key: reportKey,
            data: null,
            error: handleApiError(cause),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [filters, pageNumber, reportKey, service, tab]);

  const currentSummary = summaryState?.key === summaryKey ? summaryState : null;
  const currentReport = reportState?.key === reportKey ? reportState : null;

  return {
    summary: currentSummary?.data ?? null,
    summaryLoading: currentSummary === null || currentSummary.loading,
    summaryError: currentSummary?.error ?? null,
    report: currentReport?.data ?? null,
    reportLoading:
      tab !== "summary" && (currentReport === null || currentReport.loading),
    reportError: currentReport?.error ?? null,
  };
}
