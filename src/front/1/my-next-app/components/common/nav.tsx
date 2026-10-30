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
    label: "پذیرش و نوبت",
    href: "/receptions",
    children: [
      { label: "فهرست پذیرش‌ها", href: "/receptions" },
      { label: "ثبت پذیرش", href: "/receptions/new" },
      { label: "گزارش پذیرش", href: "/receptions/reports" },
      { label: "رزرو و نوبت", href: "/bookings" },
    ],
  },
  {
    label: "مشتری و پرسنل",
    href: "/customers",
    children: [
      { label: "لیست مشتریان", href: "/customers" },
      { label: "ثبت مشتری", href: "/customers/new" },
      { label: "لیست پرسنل", href: "/personnel" },
      { label: "ثبت پرسنل", href: "/personnel/new" },
    ],
  },
  {
    label: "خدمات و انبار",
    href: "/services",
    children: [
      { label: "فهرست خدمات", href: "/services" },
      { label: "دسته‌بندی خدمات", href: "/services/categories" },
      { label: "تعرفه خدمات", href: "/services/tariffs" },
      { label: "واحدهای اندازه‌گیری", href: "/inventory/units" },
      { label: "مواد و کالاها", href: "/inventory/materials" },
    ],
  },
  {
    label: "مالی و صندوق",
    href: "/finance",
    children: [
      { label: "حقوق و تسویه پرسنل", href: "/finance" },
      { label: "فهرست و گردش صندوق‌ها", href: "/cash-registers" },
    ],
  },
];

export function Navbar() {
  const pathname = usePathname();
  const isPathActive = (href: string) =>
    pathname === href || pathname.startsWith(`${href}/`);

  if (pathname === "/login") {
    return null;
  }

  return (
    <header className="flex h-16 items-center justify-between gap-2 bg-black px-3 text-white sm:px-6">
      <div className="flex min-w-0 flex-1 items-center gap-4 sm:gap-8">
        <Link href="/" className="text-2xl font-bold text-fuchsia-500">
          Beauty
        </Link>

        <nav
          aria-label="منوی اصلی"
          className="min-w-0 flex-1 overflow-x-auto whitespace-nowrap [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
        >
          <div className="flex w-max items-center gap-1">
          {NAV_ITEMS.map((item) => {
            const active = item.children
              ? item.children.some((child) => isPathActive(child.href))
              : isPathActive(item.href);
            const className = cn(
              "flex items-center gap-1 rounded-md px-3 py-2 text-sm transition-colors hover:bg-white/10",
              active && "bg-fuchsia-700/30 text-fuchsia-300",
            );

            if (!item.children) {
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={className}
                  aria-current={active ? "page" : undefined}
                >
                  {item.label}
                </Link>
              );
            }

            return (
              <DropdownMenu key={item.href}>
                <DropdownMenuTrigger render={<button className={className} />}>
                  {item.label}
                  <ChevronDown aria-hidden="true" className="size-3.5" />
                </DropdownMenuTrigger>
                <DropdownMenuContent align="start">
                  {item.children.map((child) => {
                    const childActive = isPathActive(child.href);

                    return (
                      <DropdownMenuItem
                        key={child.href}
                        className={cn(
                          childActive && "bg-accent text-accent-foreground",
                        )}
                        render={
                          <Link
                            href={child.href}
                            aria-current={childActive ? "page" : undefined}
                          />
                        }
                      >
                        {child.label}
                      </DropdownMenuItem>
                    );
                  })}
                </DropdownMenuContent>
              </DropdownMenu>
            );
          })}
          </div>
        </nav>
      </div>

      <div className="flex shrink-0 items-center gap-1">
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
