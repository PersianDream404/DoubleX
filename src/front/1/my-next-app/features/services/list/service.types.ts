import type { PagedResult } from "@/lib/api/types/paged-result";

export interface ServiceDto {
  id: number;
  salonId: number;
  categoryId: number;
  code: string;
  name: string;
  description: string | null;
  durationMinutes: number;
  isActive: boolean;
}

export interface ServiceCategoryDto {
  id: number;
  salonId: number;
  name: string;
  description: string | null;
  isActive: boolean;
}

export interface ServiceMaterialDto {
  id: number;
  salonId: number;
  serviceId: number;
  materialId: number;
  materialName: string;
  baseUnitName: string;
  baseUnitSymbol: string;
  quantityPerService: number;
  description: string | null;
}

export interface ServiceMaterialRequest {
  salonId: number;
  serviceId: number;
  materialId: number;
  quantityPerService: number;
  description: string | null;
}

export interface ServiceRequest {
  salonId: number;
  categoryId: number;
  code: string;
  name: string;
  description: string | null;
  durationMinutes: number;
}

export interface UpdateServiceRequest extends ServiceRequest {
  isActive: boolean;
}

export interface ServiceListQuery {
  salonId: number;
  q?: string;
}

export type ServiceListResult = PagedResult<ServiceDto>;

export interface ServiceMaterialOption {
  id: number;
  name: string;
  code: string;
  baseUnitName: string;
  baseUnitSymbol: string;
  isActive: boolean;
}
