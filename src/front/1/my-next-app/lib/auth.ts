import { getCurrentSession } from "@/app/services/session.service";
import {
  isRoleName,
  type RoleName,
} from "@/lib/roles";

export async function getCurrentUser() {
  const session = await getCurrentSession();

  if (!session) {
    return null;
  }

  const roles = session.user.userRoles
    .map((userRole) => userRole.role.name)
    .filter(isRoleName);

  return {
    id: session.user.id,
    username: session.user.username,
    roles,
  };
}