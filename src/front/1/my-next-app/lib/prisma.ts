import { PrismaMssql } from "@prisma/adapter-mssql";
import { PrismaClient } from "@/prisma/generated/prisma/client";

const adapter = new PrismaMssql({
  server: "db71273.public.databaseasp.net",
  database: "db71273",

  user: "db71273",
  password: process.env.DATABASE_PASSWORD,

  options: {
    encrypt: true,
    trustServerCertificate: true,
  },
});

const prisma = new PrismaClient({
  adapter,
});

export default prisma;
