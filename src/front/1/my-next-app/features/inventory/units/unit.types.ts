import type { PageParams, PagedResult } from "@/lib/api/types/paged-result";

export type UnitType = 1 | 2 | 3;

export interface UnitDto {
  id: number;
  name: string;
  symbol: string;
  unitType: UnitType;
  isActive: boolean;
}

export interface UnitRequest {
  name: string;
  symbol: string;
  unitType: UnitType;
}

export interface UpdateUnitRequest extends UnitRequest {
  id: number;
  isActive: boolean;
}

export type UnitListResult = PagedResult<UnitDto>;
export type UnitListRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
  IsActive?: boolean;
};
