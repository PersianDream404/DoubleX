import "dotenv/config";
import bcrypt from "bcryptjs";

import prisma from "@/lib/prisma";

async function main() {
  console.log("🌱 Seeding database...");

  // -------------------------
  // Roles
  // -------------------------

  const adminRole = await prisma.role.upsert({
    where: {
      name: "Admin",
    },
    update: {},
    create: {
      name: "Admin",
    },
  });

  const employerRole = await prisma.role.upsert({
    where: {
      name: "Employer",
    },
    update: {},
    create: {
      name: "Employer",
    },
  });

  const jobSeekerRole = await prisma.role.upsert({
    where: {
      name: "JobSeeker",
    },
    update: {},
    create: {
      name: "JobSeeker",
    },
  });

  // -------------------------
  // Users
  // -------------------------

  const passwordHash = await bcrypt.hash("123456", 10);

  const admin = await prisma.user.upsert({
    where: {
      username: "admin",
    },
    update: {},
    create: {
      username: "admin",
      passwordHash,
    },
  });

  const employer = await prisma.user.upsert({
    where: {
      username: "employer",
    },
    update: {},
    create: {
      username: "employer",
      passwordHash,
    },
  });

  const jobSeeker = await prisma.user.upsert({
    where: {
      username: "jobseeker",
    },
    update: {},
    create: {
      username: "jobseeker",
      passwordHash,
    },
  });

  await prisma.userRole.upsert({
    where: {
      userId_roleId: {
        userId: admin.id,
        roleId: adminRole.id,
      },
    },
    update: {},
    create: {
      userId: admin.id,
      roleId: adminRole.id,
    },
  });

  await prisma.userRole.upsert({
    where: {
      userId_roleId: {
        userId: employer.id,
        roleId: employerRole.id,
      },
    },
    update: {},
    create: {
      userId: employer.id,
      roleId: employerRole.id,
    },
  });

  await prisma.userRole.upsert({
    where: {
      userId_roleId: {
        userId: jobSeeker.id,
        roleId: jobSeekerRole.id,
      },
    },
    update: {},
    create: {
      userId: jobSeeker.id,
      roleId: jobSeekerRole.id,
    },
  });

  // -------------------------
  // Categories
  // -------------------------

  const laptopCategory = await prisma.category.create({
    data: {
      name: "لپ‌تاپ",
    },
  });

  const phoneCategory = await prisma.category.create({
    data: {
      name: "موبایل",
    },
  });

  const accessoryCategory = await prisma.category.create({
    data: {
      name: "لوازم جانبی",
    },
  });

  // -------------------------
  // Products
  // -------------------------

  const laptop = await prisma.product.create({
    data: {
      name: "Dell Latitude 5540",
      price: 45000000,
      status: true,
      categoryId: laptopCategory.id,
    },
  });

  const phone = await prisma.product.create({
    data: {
      name: "iPhone 15",
      price: 65000000,
      status: true,
      categoryId: phoneCategory.id,
    },
  });

  const mouse = await prisma.product.create({
    data: {
      name: "Logitech Mouse",
      price: 2500000,
      status: true,
      categoryId: accessoryCategory.id,
    },
  });

  const keyboard = await prisma.product.create({
    data: {
      name: "Mechanical Keyboard",
      price: 4500000,
      status: false,
      categoryId: accessoryCategory.id,
    },
  });

  // -------------------------
  // Companies
  // -------------------------

  const companyA = await prisma.company.create({
    data: {
      name: "شرکت فناوری آریا",
      phone: "02111111111",
      email: "info@arya.test",
      status: true,
    },
  });

  const companyB = await prisma.company.create({
    data: {
      name: "شرکت داده گستر",
      phone: "02122222222",
      email: "info@datagostar.test",
      status: true,
    },
  });

  // -------------------------
  // Customers
  // -------------------------

  const customer1 = await prisma.customer.create({
    data: {
      name: "علی رضایی",
      phone: "09120000001",
      email: "ali@test.com",
      companyId: companyA.id,
    },
  });

  const customer2 = await prisma.customer.create({
    data: {
      name: "رضا محمدی",
      phone: "09120000002",
      email: "reza@test.com",
      companyId: companyA.id,
    },
  });

  const customer3 = await prisma.customer.create({
    data: {
      name: "مهدی کریمی",
      phone: "09120000003",
      email: "mehdi@test.com",
      companyId: companyB.id,
    },
  });

  const customer4 = await prisma.customer.create({
    data: {
      name: "سارا احمدی",
      phone: "09120000004",
      email: "sara@test.com",
    },
  });

  // -------------------------
  // Orders
  // -------------------------

  const order1 = await prisma.order.create({
    data: {
      customerId: customer1.id,
      status: "Completed",
      totalPrice: 70000000,
      items: {
        create: [
          {
            productId: laptop.id,
            quantity: 1,
            price: 45000000,
          },
          {
            productId: mouse.id,
            quantity: 2,
            price: 2500000,
          },
        ],
      },
    },
  });

  const order2 = await prisma.order.create({
    data: {
      customerId: customer2.id,
      status: "Completed",
      totalPrice: 65000000,
      items: {
        create: [
          {
            productId: phone.id,
            quantity: 1,
            price: 65000000,
          },
        ],
      },
    },
  });

  const order3 = await prisma.order.create({
    data: {
      customerId: customer3.id,
      status: "Pending",
      totalPrice: 9000000,
      items: {
        create: [
          {
            productId: keyboard.id,
            quantity: 2,
            price: 4500000,
          },
        ],
      },
    },
  });

  const order4 = await prisma.order.create({
    data: {
      customerId: customer4.id,
      status: "Completed",
      totalPrice: 2500000,
      items: {
        create: [
          {
            productId: mouse.id,
            quantity: 1,
            price: 2500000,
          },
        ],
      },
    },
  });

  // -------------------------
  // Payments
  // -------------------------

  await prisma.payment.createMany({
    data: [
      {
        orderId: order1.id,
        amount: 70000000,
        status: "Paid",
        method: "Card",
      },
      {
        orderId: order2.id,
        amount: 65000000,
        status: "Paid",
        method: "Online",
      },
      {
        orderId: order3.id,
        amount: 9000000,
        status: "Pending",
        method: "Online",
      },
      {
        orderId: order4.id,
        amount: 2500000,
        status: "Paid",
        method: "Card",
      },
    ],
  });

  console.log("✅ Seed completed.");
}

main()
  .catch((error) => {
    console.error("❌ Seed failed:", error);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });