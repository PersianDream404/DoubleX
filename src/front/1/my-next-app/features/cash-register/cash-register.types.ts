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
