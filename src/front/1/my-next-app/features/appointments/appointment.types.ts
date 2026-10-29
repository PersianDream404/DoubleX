import type { PagedResult } from "@/lib/api/types/paged-result";
import type { CustomerDto } from "@/features/customers/list/customer.types";
import type { PersonnelDto } from "@/features/personnel/list/personnel.types";
import type { ServiceDto, ServicePersonnelDto } from "@/features/services/list/service.types";
import type { ServiceTariffDto } from "@/features/services/tariffs/service-tariff.types";

export type AppointmentStatus = 1 | 2 | 3 | 4 | 5;
export type AppointmentNotificationChannel = 0 | 1 | 2;

export interface AppointmentListItemDto {
  id: number;
  customerId: number;
  customerName: string;
  serviceId: number;
  serviceName: string;
  personnelId: number;
  personnelName: string;
  startsAt: string;
  endsAt: string;
  finalAmount: number;
  status: AppointmentStatus;
}

export interface AppointmentDetailsDto extends AppointmentListItemDto {
  salonId: number;
  serviceTariffId: number;
  durationMinutes: number;
  notes: string | null;
  baseAmount: number;
  discountAmount: number;
  notificationChannel: AppointmentNotificationChannel;
  createdAt: string;
  updatedAt: string;
}

export interface AppointmentAvailabilityDto {
  isAvailable: boolean;
  startsAt: string;
  endsAt: string;
  durationMinutes: number;
  baseAmount: number;
}

export interface CreateAppointmentRequest {
  idempotencyKey: string;
  customerId: number;
  serviceId: number;
  serviceTariffId: number;
  personnelId: number;
  startsAt: string;
  discountAmount: number;
  notes: string | null;
  notificationChannel: AppointmentNotificationChannel;
}

export interface UpdateAppointmentRequest {
  startsAt: string;
  personnelId: number;
  notes: string | null;
}

export interface AppointmentOptions {
  customers: CustomerDto[];
  services: ServiceDto[];
  tariffs: ServiceTariffDto[];
  personnel: PersonnelDto[];
  servicePersonnel: ServicePersonnelDto[];
}

export type AppointmentListResult = PagedResult<AppointmentListItemDto>;

export interface ChangeAppointmentStatusRequest {
  status: AppointmentStatus;
}
