"use server";

import { cookies } from "next/headers";

import { loginSchema } from "@/app/schemas/auth.schema";
import { verifyUserPassword } from "@/app/services/auth.service";
import { createSession } from "@/app/services/session.service";
import { redirect } from "next/navigation";

export type LoginActionState = {
  success: boolean;
  error?: string;
};

export async function loginAction(
  prevState: LoginActionState,
  formData: FormData,
): Promise<LoginActionState> {
  const username = String(formData.get("username") ?? "");

  const password = String(formData.get("password") ?? "");

  const result = loginSchema.safeParse({
    username,
    password,
  });

  if (!result.success) {
    return {
      success: false,
      error: "نام کاربری و رمز عبور را وارد کنید",
    };
  }

  const user = await verifyUserPassword(
    result.data.username,
    result.data.password,
  );

  if (!user) {
    return {
      success: false,
      error: "نام کاربری یا رمز عبور اشتباه است",
    };
  }

  const session = await createSession(user.id);

  const cookieStore = await cookies();

  cookieStore.set("session", session.id, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    expires: session.expiresAt,
    path: "/",
  });

  redirect("/admin");
  return {
    success: true,
  };
}
