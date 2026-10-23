import prisma from "@/lib/prisma";
import { UserProfileDto } from "../DTOs/Auth/user.dto";
import bcrypt from "bcryptjs";
import { cookies } from "next/headers";
import { isRoleName } from "@/lib/roles";

export async function getUserByUsername(
  username: string,
): Promise<UserProfileDto | null> {
  const user = await prisma.user.findUnique({
    where: {
      username,
    },
    include: {
      userRoles: {
        include: {
          role: true,
        },
      },
    },
  });

  if (!user) {
    return null;
  }

  return {
    id: user.id,
    username: user.username,
    roles: user.userRoles.map((userRole) => userRole.role.name),
  };
}

export async function verifyUserPassword(username: string, password: string) {
  const user = await prisma.user.findUnique({
    where: { username },
    include: {
      userRoles: {
        include: {
          role: true,
        },
      },
    },
  });

  if (!user) {
    return null;
  }

  const isPasswordValid = await bcrypt.compare(password, user.passwordHash);

  if (!isPasswordValid) {
    return null;
  }

  return {
    id: user.id,
    username: user.username,
    roles: user.userRoles
      .map((userRole) => userRole.role.name)
      .filter(isRoleName),
  };
}
export async function deleteSession() {
  const cookieStore = await cookies();

  const sessionId = cookieStore.get("session")?.value;

  if (!sessionId) {
    return;
  }

  await prisma.session.deleteMany({
    where: {
      id: sessionId,
    },
  });

  cookieStore.delete("session");
}
