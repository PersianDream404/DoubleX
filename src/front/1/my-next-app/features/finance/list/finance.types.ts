import type { PagedResult } from "@/lib/api/types/paged-result";
import type { PersonnelDto } from "@/features/personnel/list/personnel.types";

export type PersonnelEarningType = 1 | 2;

export interface PersonnelSettlementDto {
  id: number;
  personnelId: number;
  earningType: PersonnelEarningType;
  amount: number;
  paidAt: string;
  paymentMethod: string;
  createdBy: number;
  description: string | null;
}

export interface PersonnelEarningsReportDto {
  personnelId: number;
  fixedSalaryAmount: number;
  serviceCommissionAmount: number;
  totalEarned: number;
  settledAmount: number;
  remainingAmount: number;
}

export interface FinancePageData {
  personnel: PersonnelDto[];
  settlements: PagedResult<PersonnelSettlementDto>;
  earnings: PersonnelEarningsReportDto | null;
  personnelLoading: boolean;
  settlementsLoading: boolean;
  earningsLoading: boolean;
  personnelError: string | null;
  settlementsError: string | null;
  earningsError: string | null;
}

export interface PersonnelSettlementsRequest {
  [key: string]: string | number | boolean | null | undefined;
  pageNumber: number;
  pageSize: number;
  personnelId?: number;
}

export interface CalculateFixedSalaryRequest {
  personnelId: number;
  periodStart: string;
  periodEnd: string;
}

export interface RegisterPersonnelSettlementRequest {
  personnelId: number;
  earningType: PersonnelEarningType;
  amount: number;
  paymentMethod: string;
  idempotencyKey: string;
  description: string | null;
}
