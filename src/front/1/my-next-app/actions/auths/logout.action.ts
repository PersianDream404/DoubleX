"use server";

import { deleteSession } from "@/app/services/auth.service";
import { redirect } from "next/navigation";
import { cookies } from "next/headers";

export async function logoutAction() {
  await deleteSession();
  const cookieStore = await cookies();
  cookieStore.delete("access_token");

  redirect("/login");
}
