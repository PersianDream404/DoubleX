import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

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

export type ReceptionReportTab =
  | "summary"
  | "receptions"
  | "personnel"
  | "services"
  | "debts"
  | "discounts"
  | "materials";

const PAGE_SIZE = 15;

export class ReceptionReportService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Receptions/reports";

  getSummary(
    filters: Pick<ReceptionReportFilters, "fromDate" | "toDateExclusive">,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportSummary>> {
    return this.api.get<ReceptionReportSummary>(`${this.path}/summary`, filters, config);
  }

  getReceptions(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportReception, ReceptionReportTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportReception, ReceptionReportTotals>>(
      `${this.path}/receptions`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }

  getPersonnel(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportPersonnel, ReceptionReportTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportPersonnel, ReceptionReportTotals>>(
      `${this.path}/personnel`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }

  getServices(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportServiceItem, ReceptionReportTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportServiceItem, ReceptionReportTotals>>(
      `${this.path}/services`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }

  getDebts(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportDebt, ReceptionReportDebtTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportDebt, ReceptionReportDebtTotals>>(
      `${this.path}/debts`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }

  getDiscounts(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportDiscount, ReceptionReportDiscountTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportDiscount, ReceptionReportDiscountTotals>>(
      `${this.path}/discounts`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }

  getMaterials(
    filters: ReceptionReportFilters,
    pageNumber: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ReceptionReportPage<ReceptionReportMaterial, ReceptionReportMaterialTotals>>> {
    return this.api.get<ReceptionReportPage<ReceptionReportMaterial, ReceptionReportMaterialTotals>>(
      `${this.path}/materials`,
      { ...filters, PageNumber: pageNumber, PageSize: PAGE_SIZE },
      config,
    );
  }
}
