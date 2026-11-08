"use client";

import { useMemo, useState } from "react";
import { KeyRound, LoaderCircle, RotateCcw } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { usePersonnelPermissions } from "./use-personnel-permissions";

const MODULE_NAMES: Record<string, string> = {
  Appointment: "نوبت‌دهی",
  CashRegister: "صندوق",
  Customer: "مشتریان",
  Dashboard: "داشبورد",
  Finance: "مالی",
  Inventory: "انبار",
  Personnel: "پرسنل",
  Reception: "پذیرش",
  Salon: "سالن",
  Service: "خدمات",
};

const PERMISSION_NAMES: Record<string, string> = {
  Create: "ایجاد",
  List: "مشاهده فهرست",
  Details: "مشاهده جزئیات",
  Update: "ویرایش",
  Delete: "حذف",
  Toggle: "تغییر وضعیت",
  Cancel: "لغو",
  Reports: "گزارش‌ها",
};

export function PersonnelPermissionsPage({
  initialUserId,
  adminMode = false,
}: {
  initialUserId?: number;
  adminMode?: boolean;
}) {
  const [userId, setUserId] = useState<number | null>(initialUserId ?? null);
  const {
    permissions,
    users,
    loadingUsers,
    usersError,
    selectedUserId,
    access,
    loadingPermissions,
    loadingAccess,
    error,
    pendingPermission,
    resetting,
    setOverride,
    removeOverride,
    resetOverrides,
  } = usePersonnelPermissions(userId);

  const groupedPermissions = useMemo(
    () =>
      permissions.reduce<Record<string, typeof permissions>>((groups, item) => {
        groups[item.module] ??= [];
        groups[item.module].push(item);
        return groups;
      }, {}),
    [permissions],
  );

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[150px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href={adminMode ? "/admin/salons" : "/personnel"}>
                  {adminMode ? "مدیریت سامانه" : "مدیریت پرسنل"}
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">
                  {adminMode ? "دسترسی کاربران سالن" : "دسترسی حساب پرسنل"}
                </BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-4 flex items-center gap-3 text-white">
            <KeyRound className="size-6 text-fuchsia-300" aria-hidden="true" />
            <h1 className="text-xl font-semibold">
              {adminMode ? "مدیریت دسترسی کاربران سالن" : "مدیریت دسترسی اختصاصی"}
            </h1>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-7 w-[92%] max-w-[1200px] space-y-5 pb-10 sm:w-[84%]">
        <Card className="rounded-2xl border border-fuchsia-100 bg-white">
          <CardHeader>
            <CardTitle>انتخاب حساب کاربری</CardTitle>
            <CardDescription>
              حساب را از فهرست کاربران مجاز انتخاب کنید.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="w-full max-w-xl space-y-2">
              <Label htmlFor="identity-user-select">حساب کاربر</Label>
              <Select
                value={selectedUserId === null ? "" : String(selectedUserId)}
                onValueChange={(value) => {
                  const parsed = Number(value);
                  setUserId(
                    Number.isSafeInteger(parsed) && parsed > 0 ? parsed : null,
                  );
                }}
                disabled={loadingUsers || Boolean(usersError) || users.length === 0}
              >
                <SelectTrigger
                  id="identity-user-select"
                  aria-label="انتخاب حساب کاربر"
                  className="w-full max-w-xl border-fuchsia-200 bg-white"
                >
                  <SelectValue
                    placeholder={
                      loadingUsers ? "در حال دریافت کاربران..." : "انتخاب حساب..."
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {users.map((user) => {
                    const displayName = user.fullName?.trim() || "بدون نام";
                    return (
                      <SelectItem
                        key={user.userId}
                        value={String(user.userId)}
                      >
                        {displayName} · {user.mobile}
                        {adminMode
                          ? ` · سالن ${user.salonId.toLocaleString("fa-IR")}`
                          : ""}
                      </SelectItem>
                    );
                  })}
                </SelectContent>
              </Select>
              {loadingUsers && (
                <p className="text-sm text-muted-foreground" role="status">
                  در حال بارگذاری فهرست کاربران...
                </p>
              )}
              {usersError && (
                <p className="text-sm text-destructive" role="alert">
                  {usersError}
                </p>
              )}
              {!loadingUsers && !usersError && users.length === 0 && (
                <p className="text-sm text-muted-foreground">
                  حساب قابل مدیریتی برای انتخاب وجود ندارد.
                </p>
              )}
              {userId !== null && selectedUserId === null && !loadingUsers && (
                <p className="text-sm text-destructive" role="alert">
                  این حساب در فهرست حساب‌های قابل مدیریت نیست.
                </p>
              )}
            </div>
          </CardContent>
        </Card>

        {error && (
          <div className="rounded-xl border border-destructive/30 bg-destructive/5 p-4 text-sm text-destructive" role="alert">
            {error}
          </div>
        )}

        {loadingPermissions ? (
          <div className="flex items-center justify-center gap-2 py-12 text-sm text-muted-foreground" role="status">
            <LoaderCircle className="size-4 animate-spin" aria-hidden="true" />
            در حال دریافت فهرست مجوزها...
          </div>
        ) : permissions.length === 0 && !error ? (
          <p className="py-8 text-center text-sm text-muted-foreground">مجوزی برای مدیریت ثبت نشده است.</p>
        ) : selectedUserId !== null && loadingAccess ? (
          <div className="flex items-center justify-center gap-2 py-12 text-sm text-muted-foreground" role="status">
            <LoaderCircle className="size-4 animate-spin" aria-hidden="true" />
            در حال دریافت وضعیت دسترسی حساب...
          </div>
        ) : access ? (
          <>
            <Card className="rounded-2xl border border-fuchsia-100 bg-white">
              <CardHeader className="flex flex-row items-start justify-between gap-4">
                <div>
                  <CardTitle>حساب {access.userId.toLocaleString("fa-IR")}</CardTitle>
                  <CardDescription className="mt-2 flex flex-wrap gap-2">
                    {access.roles.length > 0 ? access.roles.map((role) => (
                      <Badge key={role} variant="secondary">{role}</Badge>
                    )) : "این حساب نقشی ندارد."}
                  </CardDescription>
                </div>
                <Button
                  type="button"
                  variant="outline"
                  disabled={resetting || (access.grants.length === 0 && access.denies.length === 0)}
                  onClick={() => {
                    if (window.confirm("تمام تغییرات اختصاصی این حساب حذف شود؟")) {
                      void resetOverrides();
                    }
                  }}
                >
                  {resetting ? <LoaderCircle className="size-4 animate-spin" /> : <RotateCcw className="size-4" />}
                  بازنشانی همه
                </Button>
              </CardHeader>
              <CardContent className="flex flex-wrap gap-x-6 gap-y-2 text-sm text-muted-foreground">
                <span>مجوز نهایی: {access.effectivePermissions.length.toLocaleString("fa-IR")}</span>
                <span>اعطاشده: {access.grants.length.toLocaleString("fa-IR")}</span>
                <span>محدودشده: {access.denies.length.toLocaleString("fa-IR")}</span>
              </CardContent>
            </Card>

            {Object.entries(groupedPermissions).map(([module, modulePermissions]) => (
              <Card key={module} className="rounded-2xl border border-fuchsia-100 bg-white">
                <CardHeader className="border-b border-border/70">
                  <CardTitle>{MODULE_NAMES[module] ?? module}</CardTitle>
                </CardHeader>
                <CardContent className="divide-y divide-border/70 p-0">
                  {modulePermissions.map((permission) => {
                    const isRolePermission = access.rolePermissions.includes(permission.code);
                    const isGranted = access.grants.includes(permission.code);
                    const isDenied = access.denies.includes(permission.code);
                    const isEffective = access.effectivePermissions.includes(permission.code);
                    const busy = pendingPermission === permission.code || resetting;
                    const actionName = permission.code.split(".").pop() ?? "";

                    return (
                      <div key={permission.code} className="flex flex-col gap-3 px-4 py-4 sm:flex-row sm:items-center sm:justify-between">
                        <div className="min-w-0 space-y-1">
                          <p className="font-medium">{PERMISSION_NAMES[actionName] ?? permission.displayName}</p>
                          <p className="break-all text-xs text-muted-foreground" dir="ltr">{permission.code}</p>
                          <div className="flex flex-wrap gap-2 pt-1">
                            <Badge variant={isRolePermission ? "secondary" : "outline"}>
                              {isRolePermission ? "پیش‌فرض نقش" : "بدون پیش‌فرض نقش"}
                            </Badge>
                            {isGranted && <Badge className="bg-emerald-100 text-emerald-800 hover:bg-emerald-100">اعطای اختصاصی</Badge>}
                            {isDenied && <Badge variant="destructive">محدودشده</Badge>}
                            <Badge className={isEffective ? "bg-fuchsia-100 text-fuchsia-800 hover:bg-fuchsia-100" : "bg-slate-100 text-slate-700 hover:bg-slate-100"}>
                              {isEffective ? "دسترسی فعال" : "دسترسی غیرفعال"}
                            </Badge>
                            {!permission.isAssignable && (
                              <Badge variant="outline">قابل تخصیص نیست</Badge>
                            )}
                          </div>
                        </div>

                        <div className="flex shrink-0 flex-wrap gap-2">
                          <Button
                            type="button"
                            size="sm"
                            variant={isGranted ? "secondary" : "outline"}
                            disabled={!permission.isAssignable || busy || isGranted}
                            onClick={() => void setOverride(permission.code, "Grant")}
                          >
                            اعطا
                          </Button>
                          <Button
                            type="button"
                            size="sm"
                            variant={isDenied ? "destructive" : "outline"}
                            disabled={!permission.isAssignable || busy || isDenied}
                            onClick={() => void setOverride(permission.code, "Deny")}
                          >
                            محدودکردن
                          </Button>
                          {(isGranted || isDenied) && (
                            <Button
                              type="button"
                              size="sm"
                              variant="ghost"
                              disabled={busy}
                              onClick={() => void removeOverride(permission.code)}
                            >
                              حذف تغییر
                            </Button>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </CardContent>
              </Card>
            ))}
          </>
        ) : selectedUserId !== null && !error ? (
          <p className="py-8 text-center text-sm text-muted-foreground">
            دسترسی‌های این حساب نمایش داده نمی‌شود.
          </p>
        ) : null}
      </main>
    </div>
  );
}
