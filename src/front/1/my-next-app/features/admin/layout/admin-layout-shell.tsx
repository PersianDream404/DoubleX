"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState, type ReactNode } from "react";
import { ArrowRight, KeyRound, Menu, Store } from "lucide-react";

import { LogoutButton } from "@/components/auth/logout-button";
import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { cn } from "@/lib/utils";

const ADMIN_ITEMS = [
  { label: "افزودن سالن", href: "/admin/salons", icon: Store },
  { label: "مدیریت دسترسی کاربران", href: "/admin/users/permissions", icon: KeyRound },
];

function AdminNavigation({
  pathname,
  onNavigate,
}: {
  pathname: string;
  onNavigate?: () => void;
}) {
  return (
    <nav aria-label="منوی پنل مدیریت" className="space-y-1">
      {ADMIN_ITEMS.map((item) => {
        const Icon = item.icon;
        const active =
          pathname === item.href ||
          (item.href !== "/admin/salons" && pathname.startsWith(`${item.href}/`));

        return (
          <Link
            key={item.label}
            href={item.href}
            onClick={onNavigate}
            aria-current={active ? "page" : undefined}
            className={cn(
              "flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm text-white/75 transition-colors hover:bg-white/10 hover:text-white focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-fuchsia-300",
              active && "bg-fuchsia-700/35 font-medium text-fuchsia-200",
            )}
          >
            <Icon className="size-4 shrink-0" aria-hidden="true" />
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}

function SidebarContent({
  pathname,
  mobile = false,
}: {
  pathname: string;
  mobile?: boolean;
}) {
  return (
    <div
      className={cn(
        "flex h-full flex-col bg-[linear-gradient(180deg,#1b0a25_0%,#120719_100%)] p-4 text-white",
        mobile && "pt-16",
      )}
    >
      <Link
        href="/admin/salons"
        className="mb-8 flex items-center gap-3 rounded-xl px-2 py-3 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-fuchsia-300"
      >
        <span className="grid size-10 place-items-center rounded-xl bg-fuchsia-700/30 text-fuchsia-200">
          <Store className="size-5" aria-hidden="true" />
        </span>
        <span>
          <span className="block text-lg font-bold">Beauty</span>
          <span className="block text-xs text-white/55">مدیریت سامانه</span>
        </span>
      </Link>

      <p className="mb-2 px-3 text-[11px] font-medium text-white/40">مدیریت</p>
      <AdminNavigation pathname={pathname} />

      <div className="mt-auto space-y-3 border-t border-white/10 pt-4">
        <Link
          href="/dashboard"
          className="flex min-h-10 items-center gap-2 rounded-lg px-3 text-sm text-white/65 hover:bg-white/10 hover:text-white"
        >
          <ArrowRight className="size-4" aria-hidden="true" />
          بازگشت به سامانه
        </Link>
        <LogoutButton />
      </div>
    </div>
  );
}

export function AdminLayoutShell({ children }: { children: ReactNode }) {
  const pathname = usePathname();
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <div className="min-h-screen bg-slate-50">
      <aside className="fixed inset-y-0 right-0 z-30 hidden w-64 border-l border-fuchsia-950/20 shadow-xl lg:block">
        <SidebarContent pathname={pathname} mobile />
      </aside>

      <div className="min-h-screen lg:pr-64">
        <header className="sticky top-0 z-20 flex h-14 items-center justify-between border-b border-fuchsia-100 bg-white/95 px-4 backdrop-blur lg:justify-end lg:px-8">
          <Sheet open={mobileOpen} onOpenChange={setMobileOpen}>
            <SheetTrigger
              render={
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="border-fuchsia-200 text-fuchsia-800 lg:hidden"
                  aria-label="باز کردن منوی مدیریت"
                />
              }
            >
              <Menu className="size-5" aria-hidden="true" />
            </SheetTrigger>
            <SheetContent
              side="right"
              showCloseButton
              className="w-[min(19rem,85vw)] border-fuchsia-900 bg-[#1b0a25] p-0 text-white"
            >
              <SheetHeader className="sr-only">
                <SheetTitle>منوی پنل مدیریت</SheetTitle>
              </SheetHeader>
              <SidebarContent pathname={pathname} />
            </SheetContent>
          </Sheet>

          <div className="text-right">
            <p className="text-sm font-semibold text-slate-900">پنل مدیریت سامانه</p>
            <p className="text-xs text-slate-500">مدیریت سالن‌ها و دسترسی کاربران</p>
          </div>
        </header>

        <main className="min-w-0 p-4 sm:p-6 lg:p-8">{children}</main>
      </div>
    </div>
  );
}
