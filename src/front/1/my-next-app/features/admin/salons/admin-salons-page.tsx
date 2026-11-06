"use client";

import Link from "next/link";
import { useState, type FormEvent } from "react";
import { KeyRound, LoaderCircle, Plus, Store } from "lucide-react";

import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";

import { CreateSalonWithAdminSchema } from "./admin-salon.schema";
import { AdminSalonService } from "./admin-salon.service";
import type { CreateSalonWithAdminResponse } from "./admin-salon.types";

const INITIAL_FORM = {
  salonName: "",
  adminFullName: "",
  adminMobile: "",
  adminPassword: "",
};

export function AdminSalonsPage() {
  const [form, setForm] = useState(INITIAL_FORM);
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [created, setCreated] = useState<CreateSalonWithAdminResponse | null>(
    null,
  );

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const parsed = CreateSalonWithAdminSchema.safeParse(form);
    if (!parsed.success) {
      setError(parsed.error.issues[0]?.message ?? "اطلاعات فرم را بررسی کنید.");
      return;
    }

    setPending(true);
    setError(null);
    setCreated(null);
    try {
      const response = await new AdminSalonService().createWithAdmin(parsed.data);
      if (!response.data) {
        throw new Error(response.message ?? "اطلاعات ایجادشده از سرور دریافت نشد.");
      }

      setCreated(response.data);
      setForm(INITIAL_FORM);
    } catch (requestError: unknown) {
      setError(handleApiError(requestError));
    } finally {
      setPending(false);
    }
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[166px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/admin/salons">مدیریت سامانه</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">ثبت سالن و مدیر</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-5 flex items-center gap-3 text-white">
            <Store className="size-6 text-fuchsia-300" aria-hidden="true" />
            <h1 className="text-xl font-semibold">افزودن سالن و حساب مدیر</h1>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-7 w-[92%] max-w-[920px] space-y-5 pb-10 sm:w-[84%]">
        <Card className="rounded-2xl border border-fuchsia-100 bg-white">
          <CardHeader>
            <CardTitle>اطلاعات سالن و مدیر اولیه</CardTitle>
            <CardDescription>
              حساب با نقش پایهٔ «مدیر سالن» ساخته می‌شود. مجوزهای اختصاصی آن را
              می‌توانید پس از ثبت، در صفحهٔ مدیریت دسترسی تغییر دهید.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={submit} className="space-y-5">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2 sm:col-span-2">
                  <Label htmlFor="salon-name">نام سالن</Label>
                  <Input
                    id="salon-name"
                    autoComplete="organization"
                    maxLength={200}
                    value={form.salonName}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, salonName: event.target.value }))
                    }
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="admin-full-name">نام مدیر سالن</Label>
                  <Input
                    id="admin-full-name"
                    autoComplete="name"
                    maxLength={200}
                    value={form.adminFullName}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, adminFullName: event.target.value }))
                    }
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="admin-mobile">شماره همراه مدیر</Label>
                  <Input
                    id="admin-mobile"
                    type="tel"
                    autoComplete="tel"
                    dir="ltr"
                    value={form.adminMobile}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, adminMobile: event.target.value }))
                    }
                    required
                  />
                </div>
                <div className="space-y-2 sm:col-span-2">
                  <Label htmlFor="admin-password">رمز عبور اولیه</Label>
                  <Input
                    id="admin-password"
                    type="password"
                    autoComplete="new-password"
                    minLength={3}
                    value={form.adminPassword}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, adminPassword: event.target.value }))
                    }
                    required
                  />
                  <p className="text-xs text-muted-foreground">
                    رمز را امن در اختیار مدیر سالن قرار دهید.
                  </p>
                </div>
              </div>

              {error && (
                <div
                  className="rounded-xl border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive"
                  role="alert"
                >
                  {error}
                </div>
              )}

              <Button
                type="submit"
                disabled={pending}
                className="bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              >
                {pending ? (
                  <LoaderCircle className="size-4 animate-spin" aria-hidden="true" />
                ) : (
                  <Plus className="size-4" aria-hidden="true" />
                )}
                ساخت سالن و حساب مدیر
              </Button>
            </form>
          </CardContent>
        </Card>

        {created && (
          <Card className="border-emerald-200 bg-emerald-50">
            <CardHeader className="flex flex-row items-center gap-2 pb-2">
              <KeyRound className="size-4 text-emerald-700" aria-hidden="true" />
              <CardTitle className="text-base text-emerald-900">
                سالن و حساب مدیر با موفقیت ساخته شدند.
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 text-sm text-emerald-900">
              <p>
                شناسه سالن: <bdi dir="ltr">{created.salonId}</bdi> · شناسه حساب:
                {" "}<bdi dir="ltr">{created.adminUserId}</bdi> · نقش: {created.role}
              </p>
              <Button
                render={
                  <Link
                    href={`/personnel/permissions?userId=${created.adminUserId}`}
                  />
                }
                size="sm"
                variant="outline"
                className="border-emerald-700 text-emerald-800"
              >
                تنظیم مجوزهای اختصاصی مدیر
              </Button>
            </CardContent>
          </Card>
        )}
      </main>
    </div>
  );
}
