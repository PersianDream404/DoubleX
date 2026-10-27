import type { ServiceDto } from "@/features/services/list/service.types";

export interface ServiceTariffItemDto {
  serviceId: number;
  serviceName: string;
  amount: number;
}

export interface ServiceTariffDto {
  id: number;
  salonId: number;
  name: string;
  validFrom: string;
  validTo: string | null;
  services: ServiceTariffItemDto[];
}

export interface ServiceTariffRequest {
  name: string;
  validFrom: string;
  validTo: string | null;
}

export interface AddTariffServicesRequest {
  services: { serviceId: number; amount: number }[];
}

export interface UpdateTariffServiceRequest {
  amount: number;
}

export type TariffServiceOption = Pick<ServiceDto, "id" | "name" | "code" | "isActive">;
