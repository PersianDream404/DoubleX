import "dotenv/config";

import bcrypt from "bcryptjs";
import prisma from "@/lib/prisma";

async function main() {
  const username = "admin";
  const password = "admin";

  const adminRole = await prisma.role.upsert({
    where: {
      name: "Admin",
    },
    update: {},
    create: {
      name: "Admin",
    },
  });

  const passwordHash = await bcrypt.hash(password, 12);

  const adminUser = await prisma.user.upsert({
    where: {
      username,
    },
    update: {},
    create: {
      username,
      passwordHash,
    },
  });

  await prisma.userRole.upsert({
    where: {
      userId_roleId: {
        userId: adminUser.id,
        roleId: adminRole.id,
      },
    },
    update: {},
    create: {
      userId: adminUser.id,
      roleId: adminRole.id,
    },
  });

  console.log("Admin user created successfully.");
  console.log("Username:", username);
  console.log("Password:", password);
}

main()
  .catch((error) => {
    console.error(error);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });