import type { PageParams } from "@/lib/api/types/paged-result";

export interface SalonDto {
  id: number;
  name: string;
  status: boolean;
}

export type SalonDetailsDto = Pick<SalonDto, "id" | "name">;

export interface CreateSalonRequest {
  name: string;
}

export interface UpdateSalonRequest extends CreateSalonRequest {
  id: number;
}

export type GetAllSalonRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
};

export type GetSelectListSalonResponse = Pick<
  SalonDto,
  "id" | "name" | "status"
>;
