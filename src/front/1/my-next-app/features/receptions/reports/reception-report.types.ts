import type { PagedResult } from "@/lib/api/types/paged-result";

export interface ReceptionReportDateFilters {
  fromDate: string;
  toDateExclusive: string;
}

export interface ReceptionReportSelectOption {
  id: number;
  label: string;
  description: string;
}

export interface ReceptionReportSelectPage {
  list: ReceptionReportSelectOption[];
  hasMore: boolean;
}

export interface ReceptionReportFilters extends ReceptionReportDateFilters {
  customerSearch?: string;
  customerId?: number;
  receptionNumber?: number;
  personnelId?: number;
  serviceId?: number;
  materialId?: number;
  receptionStatus?: number;
  financialStatus?: number;
  serviceStatus?: number;
  hasAppointment?: boolean;
  minimumRemainingAmount?: number;
  maximumRemainingAmount?: number;
}

export interface ReceptionReportSummary {
  receptionCount: number;
  completedReceptionCount: number;
  inProgressReceptionCount: number;
  cancelledReceptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  materialChargeAmount: number;
  trend: ReceptionReportTrend[];
  paymentMethods: ReceptionReportPaymentMethod[];
}

export interface ReceptionReportTrend {
  date: string;
  receptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
}

export interface ReceptionReportPaymentMethod {
  paymentMethod: string;
  amount: number;
}

export interface ReceptionReportReception {
  id: number;
  receptionNumber: number;
  customerId: number;
  customerName: string;
  customerMobilePhone: string | null;
  receptionistId: number;
  receptionDate: string;
  status: number;
  financialStatus: number;
  serviceCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
  remainingAmount: number;
}

export interface ReceptionReportPersonnel {
  personnelId: number;
  personnelName: string | null;
  completedServiceCount: number;
  receptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
}

export interface ReceptionReportServiceItem {
  serviceId: number;
  serviceName: string;
  completedServiceCount: number;
  receptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  averageFinalAmount: number;
}

export interface ReceptionReportDebt {
  receptionId: number;
  receptionNumber: number;
  customerId: number;
  customerName: string;
  customerMobilePhone: string | null;
  receptionDate: string;
  status: number;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
}

export interface ReceptionReportDiscount {
  receptionId: number;
  receptionNumber: number;
  customerId: number;
  customerName: string;
  customerMobilePhone: string | null;
  receptionDate: string;
  serviceId: number;
  serviceName: string;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
}

export interface ReceptionReportMaterial {
  materialId: number;
  serviceId: number;
  serviceName: string;
  materialName: string | null;
  baseUnitName: string | null;
  baseUnitSymbol: string | null;
  usageCount: number;
  quantity: number;
  totalCost: number | null;
}

export interface ReceptionReportTotals {
  receptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
  remainingAmount: number;
}

export interface ReceptionReportDebtTotals {
  receptionCount: number;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
}

export interface ReceptionReportDiscountTotals {
  serviceCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
}

export interface ReceptionReportMaterialTotals {
  usageCount: number;
  quantity: number;
  totalCost: number;
}

export interface ReceptionReportPage<TItem, TTotals> {
  page: PagedResult<TItem>;
  totals: TTotals;
}
