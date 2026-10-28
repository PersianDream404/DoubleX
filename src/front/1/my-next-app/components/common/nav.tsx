"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { CalendarDays, ChevronDown, Settings } from "lucide-react";

import { Button } from "@/components/ui/button";
import { LogoutButton } from "@/components/auth/logout-button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";

type NavItem = {
  label: string;
  href: string;
  children?: { label: string; href: string }[];
};

const NAV_ITEMS: NavItem[] = [
  { label: "مدیریت داشبورد", href: "/dashboard" },
  {
    label: "مدیریت پرسنل",
    href: "/personnel",
    children: [
      { label: "لیست پرسنل", href: "/personnel" },
      { label: "ثبت پرسنل", href: "/personnel/new" },
    ],
  },
  {
    label: "مدیریت مشتریان",
    href: "/customers",
    children: [
      { label: "لیست مشتریان", href: "/customers" },
      { label: "ثبت مشتری", href: "/customers/new" },
    ],
  },
  {
    label: "مدیریت خدمات",
    href: "/services",
    children: [
      { label: "فهرست خدمات", href: "/services" },
      { label: "دسته‌بندی خدمات", href: "/services/categories" },
      { label: "تعرفه خدمات", href: "/services/tariffs" },
    ],
  },
  {
    label: "مدیریت انبار",
    href: "/inventory",
    children: [
      { label: "واحدهای اندازه‌گیری", href: "/inventory/units" },
      { label: "مواد و کالاها", href: "/inventory/materials" },
    ],
  },
  {
    label: "مدیریت پذیرش",
    href: "/receptions",
    children: [
      { label: "فهرست پذیرش‌ها", href: "/receptions" },
      { label: "ثبت پذیرش", href: "/receptions/new" },
    ],
  },
  {
    label: "مدیریت صندوق",
    href: "/cash-registers",
    children: [{ label: "فهرست و گردش صندوق‌ها", href: "/cash-registers" }],
  },
  {
    label: "مدیریت مالی",
    href: "/finance",
    children: [{ label: "حقوق و تسویه پرسنل", href: "/finance" }],
  },
  { label: "رزرو و نوبت", href: "/bookings" },
];

export function Navbar() {
  const pathname = usePathname();

  if (pathname === "/login") {
    return null;
  }

  return (
    <header className="flex h-16 items-center justify-between bg-black px-6 text-white">
      {/* راست: لوگو + منو */}
      <div className="flex items-center gap-8">
        <Link href="/" className="text-2xl font-bold text-fuchsia-500">
          Beauty
        </Link>

        <nav className="flex items-center gap-1">
          {NAV_ITEMS.map((item) => {
            const active = pathname.startsWith(item.href);
            const className = cn(
              "flex items-center gap-1 rounded-md px-3 py-2 text-sm transition-colors hover:bg-white/10",
              active && "bg-fuchsia-700/30 text-fuchsia-300",
            );

            if (!item.children) {
              return (
                <Link key={item.href} href={item.href} className={className}>
                  {item.label}
                </Link>
              );
            }

            return (
              <DropdownMenu key={item.href}>
                <DropdownMenuTrigger render={<button className={className} />}>
                  {item.label}
                  <ChevronDown className="size-3.5" />
                </DropdownMenuTrigger>
                <DropdownMenuContent align="start">
                  {item.children.map((child) => (
                    <DropdownMenuItem
                      key={child.href}
                      render={<Link href={child.href} />}
                    >
                      {child.label}
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuContent>
              </DropdownMenu>
            );
          })}
        </nav>
      </div>

      {/* چپ: آیکون‌ها */}
      <div className="flex items-center gap-1">
        <Button
          variant="ghost"
          size="icon"
          aria-label="تقویم"
          className="hover:bg-white/10 hover:text-white"
        >
          <CalendarDays className="size-5" />
        </Button>
        <Button
          variant="ghost"
          size="icon"
          aria-label="تنظیمات"
          className="hover:bg-white/10 hover:text-white"
        >
          <Settings className="size-5" />
        </Button>
        <LogoutButton />
      </div>
    </header>
  );
}
