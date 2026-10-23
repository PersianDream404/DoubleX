"use server";

import { deleteSession } from "@/app/services/auth.service";
import { redirect } from "next/navigation";

export async function logoutAction() {
  await deleteSession();

  redirect("/login");
}
