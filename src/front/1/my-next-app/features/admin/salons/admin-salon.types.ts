export interface CreateSalonWithAdminRequest {
  salonName: string;
  adminFullName: string;
  adminMobile: string;
  adminPassword: string;
}

export interface CreateSalonWithAdminResponse {
  salonId: number;
  adminUserId: number;
  role: string;
}
