import prisma from "@/lib/prisma";
import { cookies } from "next/headers";

const SESSION_DURATION = 1000 * 60 * 60 * 24 * 7;

export async function createSession(userId: number) {
  const sessionId = crypto.randomUUID();

  const expiresAt = new Date(Date.now() + SESSION_DURATION);

  await prisma.session.create({
    data: {
      id: sessionId,
      userId,
      expiresAt,
    },
  });

  return {
    id: sessionId,
    expiresAt,
  };
}

export async function getCurrentSession() {
  const cookieStore = await cookies();

  const sessionId =
    cookieStore.get("session")?.value;

  if (!sessionId) {
    return null;
  }

  const session = await prisma.session.findUnique({
    where: {
      id: sessionId,
    },
    include: {
      user: {
        include: {
          userRoles: {
            include: {
              role: true,
            },
          },
        },
      },
    },
  });

  if (!session) {
    return null;
  }

  if (session.expiresAt <= new Date()) {
    await prisma.session.delete({
      where: {
        id: session.id,
      },
    });

    return null;
  }

  return session;
}