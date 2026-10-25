import type { PageParams, PagedResult } from "@/lib/api/types/paged-result";

export interface MaterialDto {
  id: number;
  name: string;
  code: string;
  brand: string | null;
  baseUnitId: number;
  baseUnitName: string;
  baseUnitSymbol: string;
  purchaseUnitId: number;
  purchaseUnitName: string;
  purchaseUnitSymbol: string;
  purchaseConversionFactor: number;
  minimumStock: number;
  currentStock: number;
  isActive: boolean;
}

export interface MaterialRequest {
  name: string;
  code: string;
  brand: string | null;
  baseUnitId: number;
  purchaseUnitId: number;
  purchaseConversionFactor: number;
  minimumStock: number;
}

export interface UpdateMaterialRequest extends MaterialRequest {
  id: number;
  isActive: boolean;
}

export type MaterialListResult = PagedResult<MaterialDto>;
export type MaterialListRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
  IsActive?: boolean;
};
