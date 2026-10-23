import type { PagedResult, PageParams } from "@/lib/api/types/paged-result";

export interface PersonnelDto {
  id: number;
  fullName: string;
  nationalCode: string;
  jobTitle: string;
  cardNumber: string;
  birthDate: string;
  mobilePhone: string;
}

export type GetAllPersonnelRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
};

export type PersonnelListResult = PagedResult<PersonnelDto>;
