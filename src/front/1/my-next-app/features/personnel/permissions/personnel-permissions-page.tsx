"use client";

import { useMemo, useState, type FormEvent } from "react";
import { KeyRound, LoaderCircle, RotateCcw, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { IdentityUserIdSchema } from "./personnel-permissions.schema";
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
}: {
  initialUserId?: number;
}) {
  const [userIdInput, setUserIdInput] = useState(
    initialUserId === undefined ? "" : String(initialUserId),
  );
  const [userId, setUserId] = useState<number | null>(initialUserId ?? null);
  const [formError, setFormError] = useState<string | null>(null);
  const {
    permissions,
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

  function submitUserId(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const parsed = IdentityUserIdSchema.safeParse(userIdInput);
    if (!parsed.success) {
      setFormError(parsed.error.issues[0]?.message ?? "شناسه حساب معتبر نیست.");
      setUserId(null);
      return;
    }

    setFormError(null);
    setUserId(parsed.data);
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[150px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/personnel">مدیریت پرسنل</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">دسترسی حساب پرسنل</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-4 flex items-center gap-3 text-white">
            <KeyRound className="size-6 text-fuchsia-300" aria-hidden="true" />
            <h1 className="text-xl font-semibold">مدیریت دسترسی اختصاصی</h1>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-7 w-[92%] max-w-[1200px] space-y-5 pb-10 sm:w-[84%]">
        <Card className="rounded-2xl border border-fuchsia-100 bg-white">
          <CardHeader>
            <CardTitle>انتخاب حساب کاربری</CardTitle>
            <CardDescription>
              شناسهٔ حساب Identity را وارد کنید؛ این شناسه با شناسهٔ رکورد پرسنل
              متفاوت است.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={submitUserId} className="flex flex-col gap-3 sm:flex-row sm:items-end">
              <div className="w-full max-w-sm space-y-2">
                <Label htmlFor="identity-user-id">شناسه حساب</Label>
                <Input
                  id="identity-user-id"
                  inputMode="numeric"
                  dir="ltr"
                  value={userIdInput}
                  onChange={(event) => setUserIdInput(event.target.value)}
                  aria-invalid={Boolean(formError)}
                  aria-describedby={formError ? "identity-user-id-error" : undefined}
                  placeholder="مثلاً ۱۲۳"
                />
                {formError && (
                  <p id="identity-user-id-error" className="text-sm text-destructive" role="alert">
                    {formError}
                  </p>
                )}
              </div>
              <Button type="submit" className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                <Search className="size-4" aria-hidden="true" />
                دریافت دسترسی‌ها
              </Button>
            </form>
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
        ) : userId !== null && loadingAccess ? (
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
        ) : userId !== null && !error ? (
          <p className="py-8 text-center text-sm text-muted-foreground">
            دسترسی‌های این حساب نمایش داده نمی‌شود.
          </p>
        ) : null}
      </main>
    </div>
  );
}
