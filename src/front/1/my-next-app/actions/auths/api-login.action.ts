"use server";

import { cookies } from "next/headers";

import { loginSchema } from "@/app/schemas/auth.schema";
import { LoginService } from "@/features/auth/login/login.service";
import { handleApiError } from "@/lib/api/handle-api-error";

export type ApiLoginResult =
  | { success: true; token: string }
  | { success: false; error: string };

export async function loginWithApi(input: {
  username: string;
  password: string;
}): Promise<ApiLoginResult> {
  const result = loginSchema.safeParse(input);
  if (!result.success) {
    return {
      success: false,
      error: result.error.issues[0]?.message ?? "اطلاعات ورود را بررسی کنید.",
    };
  }

  try {
    const tokenResult = await new LoginService().login(result.data);
    const expires = new Date(tokenResult.expireDate);
    if (!Number.isFinite(expires.getTime()) || expires <= new Date()) {
      return {
        success: false,
        error: "توکن ورود دریافتی از سرور معتبر نیست.",
      };
    }

    const cookieStore = await cookies();
    cookieStore.set("access_token", tokenResult.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      expires,
      path: "/",
    });

    return { success: true, token: tokenResult.token };
  } catch (error: unknown) {
    return { success: false, error: handleApiError(error) };
  }
}
