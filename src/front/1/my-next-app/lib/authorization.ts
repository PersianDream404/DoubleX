import { redirect } from "next/navigation";

import { getCurrentUser } from "@/lib/auth";
import type { RoleName } from "@/lib/roles";

export async function requireRole(roles: RoleName | RoleName[]) {
  const user = await getCurrentUser();

  if (!user) {
    redirect("/login");
  }

  const requiredRoles = typeof roles === "string" ? [roles] : roles;

  const hasRole = requiredRoles.some((role) => user.roles.includes(role));

  if (!hasRole) {
    redirect("/403");
  }

  return user;
}
