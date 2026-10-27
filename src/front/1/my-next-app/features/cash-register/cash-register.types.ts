import type { PagedResult } from "@/lib/api/types/paged-result";

export interface CashRegisterOption {
  id: number;
  salonId: number;
  name: string;
  code: string;
  description: string | null;
  isActive: boolean;
  activeSessionId: number | null;
}

export type CashRegisterListResult = PagedResult<CashRegisterOption>;

export interface CashRegisterSession {
  id: number;
  salonId: number;
  cashRegisterId: number;
  openedByUserId: number;
  openedAt: string;
  openingBalance: number;
  closedByUserId: number | null;
  closedAt: string | null;
  expectedClosingBalance: number | null;
  actualClosingBalance: number | null;
  difference: number | null;
  status: 1 | 2;
}

export interface CashRegisterBalance {
  cashRegisterId: number;
  sessionId: number | null;
  expectedBalance: number;
}

export interface CashRegisterTransaction {
  id: number;
  cashRegisterId: number;
  sessionId: number;
  type: 1 | 2 | 3 | 4;
  amount: number;
  occurredAt: string;
  createdByUserId: number;
  description: string | null;
  referenceType: string | null;
  referenceId: number | null;
}

export interface CashRegisterCreateRequest {
  name: string;
  code: string;
  description: string | null;
}

export type CashRegisterUpdateRequest = CashRegisterCreateRequest;

export interface CashMovementRequest {
  amount: number;
  idempotencyKey: string;
  description: string | null;
}
