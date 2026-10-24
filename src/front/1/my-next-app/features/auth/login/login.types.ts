export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  tokenResult: {
    token: string;
    expireDate: string;
  };
  info: {
    id: number;
    salonId: number;
    roles: string[];
  };
}
