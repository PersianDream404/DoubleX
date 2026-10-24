import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type { LoginRequest, LoginResponse } from "./login.types";

export class LoginService {
  private readonly api: ApiClient;

  constructor() {
    this.api = new ApiClient();
  }

  async login(credentials: {
    username: string;
    password: string;
  }): Promise<{ token: string; expireDate: string }> {
    const response: ApiResponse<LoginResponse> = await this.api.post<
      LoginResponse,
      LoginRequest
    >("/api/Account/Login", {
      userName: credentials.username,
      password: credentials.password,
    });

    const tokenResult = response.data?.tokenResult;
    if (!tokenResult?.token || !tokenResult.expireDate) {
      throw new Error("پاسخ ورود از سرور معتبر نیست.");
    }

    return tokenResult;
  }
}
