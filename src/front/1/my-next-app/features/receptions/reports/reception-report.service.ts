import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";

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
  ReceptionReportSelectOption,
  ReceptionReportSelectPage,
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

export type ReceptionReportSelectKind =
  | "customer"
  | "personnel"
  | "service"
  | "material";

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

  async searchCustomers(
    query: string,
    pageNumber: number,
    signal: AbortSignal,
  ): Promise<ReceptionReportSelectPage> {
    const response = await this.api.get<PagedResult<{
      id: number;
      fullName: string;
      mobilePhone: string;
      status: boolean;
    }>>(
      "/api/Customer",
      { Q: query || undefined, PageNumber: pageNumber, PageSize: 15 },
      { signal },
    );
    if (!response.data) throw new Error(response.message ?? "فهرست مشتریان دریافت نشد.");
    return {
      list: response.data.list.map((item) => ({
        id: item.id,
        label: item.fullName,
        description: `${item.mobilePhone}${item.status ? "" : " • غیرفعال"}`,
      })),
      hasMore: pageNumber < response.data.pagination.totalPages,
    };
  }

  async searchPersonnel(
    query: string,
    pageNumber: number,
    signal: AbortSignal,
  ): Promise<ReceptionReportSelectPage> {
    const response = await this.api.get<PagedResult<{
      id: number;
      fullName: string;
      jobTitle: string;
      status: boolean;
    }>>(
      "/api/Personnel",
      { Q: query || undefined, PageNumber: pageNumber, PageSize: 15 },
      { signal },
    );
    if (!response.data) throw new Error(response.message ?? "فهرست پرسنل دریافت نشد.");
    return {
      list: response.data.list.map((item) => ({
        id: item.id,
        label: item.fullName,
        description: `${item.jobTitle || `شناسه ${new Intl.NumberFormat("fa-IR").format(item.id)}`}${item.status ? "" : " • غیرفعال"}`,
      })),
      hasMore: pageNumber < response.data.pagination.totalPages,
    };
  }

  async searchServices(
    query: string,
    _pageNumber: number,
    signal: AbortSignal,
  ): Promise<ReceptionReportSelectPage> {
    const response = await this.api.get<{
      id: number;
      name: string;
      code: string;
      isActive: boolean;
    }[]>("/api/Services", { Q: query || undefined }, { signal });
    if (!response.data) throw new Error(response.message ?? "فهرست خدمات دریافت نشد.");
    return {
      list: response.data.map((item): ReceptionReportSelectOption => ({
        id: item.id,
        label: item.name,
        description: `${item.code}${item.isActive ? "" : " • غیرفعال"}`,
      })),
      hasMore: false,
    };
  }

  async searchMaterials(
    query: string,
    pageNumber: number,
    signal: AbortSignal,
  ): Promise<ReceptionReportSelectPage> {
    const response = await this.api.get<PagedResult<{
      id: number;
      name: string;
      code: string;
      baseUnitName: string;
      isActive: boolean;
    }>>(
      "/api/Inventory/Materials",
      { Q: query || undefined, PageNumber: pageNumber, PageSize: 15 },
      { signal },
    );
    if (!response.data) throw new Error(response.message ?? "فهرست مواد مصرفی دریافت نشد.");
    return {
      list: response.data.list.map((item) => ({
        id: item.id,
        label: item.name,
        description: `${item.code} • ${item.baseUnitName}${item.isActive ? "" : " • غیرفعال"}`,
      })),
      hasMore: pageNumber < response.data.pagination.totalPages,
    };
  }
}
