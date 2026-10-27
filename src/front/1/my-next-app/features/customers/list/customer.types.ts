import type { PageParams, PagedResult } from "@/lib/api/types/paged-result";

export interface CustomerDto {
  id: number;
  salonId: number;
  fullName: string;
  birthDate: string;
  nationalCode: string;
  address: string;
  allergyDescription: string | null;
  jobTitle: string | null;
  landlinePhone: string | null;
  mobilePhone: string;
  status: boolean;
}

export type CustomerDetailsDto = CustomerDto;

export interface CustomerRequest {
  fullName: string;
  birthDate: string;
  nationalCode: string;
  address: string;
  allergyDescription: string | null;
  jobTitle: string | null;
  landlinePhone: string | null;
  mobilePhone: string;
}

export interface UpdateCustomerRequest extends CustomerRequest {
  id: number;
}

export type GetAllCustomersRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
};

export type CustomerListResult = PagedResult<CustomerDto>;
