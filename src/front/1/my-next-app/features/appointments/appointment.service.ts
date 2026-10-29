import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { CustomerDto, CustomerListResult } from "@/features/customers/list/customer.types";
import type { PersonnelDto, PersonnelListResult } from "@/features/personnel/list/personnel.types";
import type { ServiceDto, ServicePersonnelDto } from "@/features/services/list/service.types";
import type { ServiceTariffDto } from "@/features/services/tariffs/service-tariff.types";

import type {
  AppointmentAvailabilityDto,
  AppointmentDetailsDto,
  AppointmentListResult,
  AppointmentStatus,
  ChangeAppointmentStatusRequest,
  CreateAppointmentRequest,
  UpdateAppointmentRequest,
} from "./appointment.types";

const PAGE_SIZE = 100;

export class AppointmentService {
  private readonly api = new ApiClient();
  private readonly path = "/api/Appointments";

  getAppointments(
    query: Record<string, string | number | boolean | null | undefined>,
    signal?: AbortSignal,
  ): Promise<ApiResponse<AppointmentListResult>> {
    return this.api.get<AppointmentListResult>(this.path, query, { signal });
  }

  getById(id: number, signal?: AbortSignal): Promise<ApiResponse<AppointmentDetailsDto>> {
    return this.api.get<AppointmentDetailsDto>(`${this.path}/${id}`, undefined, { signal });
  }

  getAvailability(
    query: {
      serviceId: number;
      serviceTariffId: number;
      personnelId: number;
      startsAt: string;
    },
    signal?: AbortSignal,
  ): Promise<ApiResponse<AppointmentAvailabilityDto>> {
    return this.api.get<AppointmentAvailabilityDto>(`${this.path}/availability`, query, { signal });
  }

  create(request: CreateAppointmentRequest): Promise<ApiResponse<{ id: number }>> {
    return this.api.post<{ id: number }, CreateAppointmentRequest>(this.path, request);
  }

  update(id: number, request: UpdateAppointmentRequest): Promise<ApiResponse<{ message: string }>> {
    return this.api.put<{ message: string }, UpdateAppointmentRequest>(`${this.path}/${id}`, request);
  }

  changeStatus(
    id: number,
    status: AppointmentStatus,
  ): Promise<ApiResponse<{ message: string }>> {
    const request: ChangeAppointmentStatusRequest = { status };
    return this.api.put<{ message: string }, ChangeAppointmentStatusRequest>(
      `${this.path}/${id}/status`,
      request,
    );
  }

  async searchCustomers(query: string, signal?: AbortSignal): Promise<CustomerDto[]> {
    const response = await this.api.get<CustomerListResult>(
      "/api/Customer",
      { Q: query, PageNumber: 1, PageSize: 10 },
      { signal },
    );
    if (!response.data) throw new Error(response.message ?? "فهرست مشتریان دریافت نشد.");
    return response.data.list.filter((customer) => customer.status);
  }

  async getCustomerById(id: number, signal?: AbortSignal): Promise<ApiResponse<CustomerDto>> {
    return this.api.get<CustomerDto>(`/api/Customer/${id}`, undefined, { signal });
  }

  async getActiveServices(signal?: AbortSignal): Promise<ServiceDto[]> {
    const response = await this.api.get<ServiceDto[]>("/api/Services/SelectList", undefined, { signal });
    if (!response.data) throw new Error(response.message ?? "فهرست خدمات دریافت نشد.");
    return response.data.filter((service) => service.isActive);
  }

  async getTariffs(signal?: AbortSignal): Promise<ServiceTariffDto[]> {
    const response = await this.api.get<ServiceTariffDto[]>("/api/Services/Tariffs", undefined, { signal });
    if (!response.data) throw new Error(response.message ?? "فهرست تعرفه‌ها دریافت نشد.");
    return response.data;
  }

  async getServicePersonnel(
    serviceId: number,
    signal?: AbortSignal,
  ): Promise<ServicePersonnelDto[]> {
    const response = await this.api.get<ServicePersonnelDto[]>(
      "/api/Services/Personnels",
      { serviceId },
      { signal },
    );
    if (!response.data) throw new Error(response.message ?? "فهرست پرسنل خدمت دریافت نشد.");
    return response.data;
  }

  async getAllPersonnel(signal?: AbortSignal): Promise<PersonnelDto[]> {
    const first = await this.api.get<PersonnelListResult>(
      "/api/Personnel",
      { PageNumber: 1, PageSize: PAGE_SIZE },
      { signal },
    );
    if (!first.data) throw new Error(first.message ?? "فهرست پرسنل دریافت نشد.");
    const personnel = [...first.data.list];
    for (let pageNumber = 2; pageNumber <= first.data.pagination.totalPages; pageNumber += 1) {
      const response = await this.api.get<PersonnelListResult>(
        "/api/Personnel",
        { PageNumber: pageNumber, PageSize: PAGE_SIZE },
        { signal },
      );
      if (!response.data) throw new Error(response.message ?? "فهرست پرسنل کامل دریافت نشد.");
      personnel.push(...response.data.list);
    }
    return personnel.filter((person) => person.status);
  }

  async getAppointmentsForRange(
    from: string,
    to: string,
    serviceId: number | null,
    signal?: AbortSignal,
  ) {
    const pageSize = 100;
    const first = await this.getAppointments(
      { From: from, To: to, ServiceId: serviceId, PageNumber: 1, PageSize: pageSize },
      signal,
    );
    if (!first.data) throw new Error(first.message ?? "فهرست نوبت‌ها دریافت نشد.");
    const appointments = [...first.data.list];
    for (let pageNumber = 2; pageNumber <= first.data.pagination.totalPages; pageNumber += 1) {
      const response = await this.getAppointments(
        { From: from, To: to, ServiceId: serviceId, PageNumber: pageNumber, PageSize: pageSize },
        signal,
      );
      if (!response.data) throw new Error(response.message ?? "فهرست نوبت‌ها کامل دریافت نشد.");
      appointments.push(...response.data.list);
    }
    return appointments;
  }

}
