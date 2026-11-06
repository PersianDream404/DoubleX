"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";
import { CalendarDays, ChevronDown, Menu, Settings } from "lucide-react";

import { Button } from "@/components/ui/button";
import { LogoutButton } from "@/components/auth/logout-button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
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
      { label: "مدیریت دسترسی پرسنل", href: "/personnel/permissions" },
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
  const activeSection = NAV_ITEMS.find((item) =>
    item.children?.some((child) => isPathActive(child.href)),
  );
  const [mobileOpen, setMobileOpen] = useState(false);
  const [sectionState, setSectionState] = useState({
    pathname,
    href: activeSection?.href ?? null,
  });
  const expandedSection =
    sectionState.pathname === pathname
      ? sectionState.href
      : activeSection?.href ?? null;

  if (pathname === "/login") {
    return null;
  }

  return (
    <header className="flex h-15 items-center justify-between gap-2 border-b border-fuchsia-700 bg-[linear-gradient(105deg,#15051f_0%,#24102e_55%,#15051f_100%)] px-3 text-white sm:px-6">
      <div className="flex min-w-0 flex-1 items-center gap-4 sm:gap-8">
        <Link href="/" className="text-2xl font-bold text-fuchsia-500">
          Beauty
        </Link>

        <nav
          aria-label="منوی اصلی"
          className="hidden min-w-0 flex-1 overflow-x-auto whitespace-nowrap [scrollbar-width:none] md:block [&::-webkit-scrollbar]:hidden"
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

        <Sheet open={mobileOpen} onOpenChange={setMobileOpen}>
          <SheetTrigger
            render={
              <Button
                variant="ghost"
                size="icon"
                aria-label="باز کردن منوی اصلی"
                className="shrink-0 hover:bg-white/10 hover:text-white md:hidden"
              />
            }
          >
            <Menu aria-hidden="true" className="size-5" />
          </SheetTrigger>
          <SheetContent
            side="right"
            className="w-[min(20rem,85vw)] border-fuchsia-800 bg-[#1b0a25] text-white md:hidden"
          >
            <SheetHeader className="border-b border-white/10 pr-14">
              <SheetTitle className="text-right text-fuchsia-300">
                منوی اصلی
              </SheetTitle>
            </SheetHeader>
            <nav aria-label="منوی موبایل" className="overflow-y-auto px-4 py-3">
              <ul className="space-y-1">
                {NAV_ITEMS.map((item) => {
                  const active = item.children
                    ? item.children.some((child) => isPathActive(child.href))
                    : isPathActive(item.href);

                  return (
                    <li key={item.href}>
                      {item.children ? (
                        <>
                          <button
                            type="button"
                            aria-expanded={expandedSection === item.href}
                            onClick={() => {
                              setSectionState({
                                pathname,
                                href:
                                  expandedSection === item.href
                                    ? null
                                    : item.href,
                              });
                            }}
                            className={cn(
                              "flex min-h-11 w-full items-center justify-between rounded-md px-3 text-right text-sm transition-colors hover:bg-white/10",
                              active && "text-fuchsia-300",
                            )}
                          >
                            {item.label}
                            <ChevronDown
                              aria-hidden="true"
                              className={cn(
                                "size-4 transition-transform",
                                expandedSection === item.href && "rotate-180",
                              )}
                            />
                          </button>
                          {expandedSection === item.href && (
                            <ul className="mr-3 border-r border-fuchsia-700/50 py-1 pr-3">
                              {item.children.map((child) => {
                                const childActive = isPathActive(child.href);

                                return (
                                  <li key={child.href}>
                                    <Link
                                      href={child.href}
                                      onClick={() => setMobileOpen(false)}
                                      aria-current={
                                        childActive ? "page" : undefined
                                      }
                                      className={cn(
                                        "flex min-h-10 items-center rounded-md px-3 text-sm text-white/80 transition-colors hover:bg-white/10 hover:text-white",
                                        childActive &&
                                          "bg-fuchsia-700/30 text-fuchsia-200",
                                      )}
                                    >
                                      {child.label}
                                    </Link>
                                  </li>
                                );
                              })}
                            </ul>
                          )}
                        </>
                      ) : (
                        <Link
                          href={item.href}
                          onClick={() => setMobileOpen(false)}
                          aria-current={active ? "page" : undefined}
                          className={cn(
                            "flex min-h-11 items-center rounded-md px-3 text-sm transition-colors hover:bg-white/10",
                            active && "bg-fuchsia-700/30 text-fuchsia-300",
                          )}
                        >
                          {item.label}
                        </Link>
                      )}
                    </li>
                  );
                })}
              </ul>
            </nav>
          </SheetContent>
        </Sheet>
      </div>

      <div className="flex shrink-0 items-center gap-1">
        <Button
          variant="ghost"
          size="icon"
          aria-label="تقویم"
          className="hidden hover:bg-white/10 hover:text-white sm:inline-flex"
        >
          <CalendarDays className="size-5" />
        </Button>
        <Button
          variant="ghost"
          size="icon"
          aria-label="تنظیمات"
          className="hidden hover:bg-white/10 hover:text-white sm:inline-flex"
        >
          <Settings className="size-5" />
        </Button>
        <LogoutButton />
      </div>
    </header>
  );
}
