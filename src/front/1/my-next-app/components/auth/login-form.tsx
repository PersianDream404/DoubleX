"use client";

import { useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import {
  Eye,
  EyeOff,
  LoaderCircle,
  LockKeyhole,
  Phone,
  Sparkles,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { loginSchema } from "@/app/schemas/auth.schema";
import { loginWithApi } from "@/actions/auths/api-login.action";

export function LoginForm() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isPending, setIsPending] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (isPending) return;

    const formData = new FormData(event.currentTarget);
    const result = loginSchema.safeParse({
      username: formData.get("username"),
      password: formData.get("password"),
    });

    if (!result.success) {
      setError(result.error.issues[0]?.message ?? "اطلاعات ورود را بررسی کنید.");
      return;
    }

    setIsPending(true);
    setError(null);

    const loginResult = await loginWithApi(result.data);
    if (!loginResult.success) {
      setError(loginResult.error);
      setIsPending(false);
      return;
    }

    window.localStorage.setItem("access_token", loginResult.token);
    router.replace("/personnel");
  }

  return (
    <Card className="gap-0 rounded-3xl border border-fuchsia-100 bg-white py-0 shadow-[0_24px_80px_-32px_rgba(43,0,43,0.42)]">
      <CardHeader className="items-center px-6 pb-5 pt-8 text-center sm:px-10 sm:pt-10">
        <div className="mb-5 flex size-14 items-center justify-center rounded-2xl bg-gradient-to-br from-fuchsia-700 to-fuchsia-950 text-white shadow-lg shadow-fuchsia-900/20">
          <Sparkles className="size-7" aria-hidden="true" />
        </div>
        <p className="mb-2 text-xs font-semibold tracking-wide text-fuchsia-700">
          سامانه مدیریت سالن زیبایی
        </p>
        <CardTitle className="text-2xl font-bold text-slate-900">
          خوش آمدید
        </CardTitle>
        <p className="mt-2 text-sm leading-6 text-slate-500">
          برای ورود به پنل، شماره همراه و رمز عبور خود را وارد کنید.
        </p>
      </CardHeader>

      <CardContent className="px-6 pb-8 sm:px-10 sm:pb-10">
        <form onSubmit={handleSubmit} className="space-y-5">
          <div className="space-y-2">
            <Label htmlFor="username" className="text-sm text-slate-700">
              شماره همراه
            </Label>
            <div className="relative">
              <Phone
                className="pointer-events-none absolute start-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
                aria-hidden="true"
              />
              <Input
                id="username"
                name="username"
                type="tel"
                inputMode="tel"
                placeholder="مثال: 09123456789"
                autoComplete="tel"
                dir="ltr"
                className="h-12 rounded-xl border-slate-200 bg-slate-50/70 pe-10 ps-3 text-left placeholder:text-left focus-visible:border-fuchsia-500 focus-visible:ring-fuchsia-500/20"
                required
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="password" className="text-sm text-slate-700">
              رمز عبور
            </Label>
            <div className="relative">
              <LockKeyhole
                className="pointer-events-none absolute start-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
                aria-hidden="true"
              />
              <Input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                placeholder="رمز عبور خود را وارد کنید"
                autoComplete="current-password"
                className="h-12 rounded-xl border-slate-200 bg-slate-50/70 pe-10 ps-11 focus-visible:border-fuchsia-500 focus-visible:ring-fuchsia-500/20"
                required
              />
              <button
                type="button"
                onClick={() => setShowPassword((current) => !current)}
                aria-label={showPassword ? "پنهان کردن رمز عبور" : "نمایش رمز عبور"}
                aria-pressed={showPassword}
                className="absolute end-3 top-1/2 -translate-y-1/2 rounded-md p-1 text-slate-400 transition-colors hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
              >
                {showPassword ? (
                  <EyeOff className="size-4" aria-hidden="true" />
                ) : (
                  <Eye className="size-4" aria-hidden="true" />
                )}
              </button>
            </div>
          </div>

          {error && (
            <p
              className="rounded-xl border border-red-100 bg-red-50 px-3 py-2 text-sm leading-6 text-destructive"
              role="alert"
            >
              {error}
            </p>
          )}

          <Button
            type="submit"
            className="h-12 w-full rounded-xl bg-fuchsia-800 text-base font-semibold text-white shadow-md shadow-fuchsia-900/15 transition-colors hover:bg-fuchsia-900 focus-visible:ring-fuchsia-600/30"
            disabled={isPending}
          >
            {isPending && (
              <LoaderCircle className="size-4 animate-spin" aria-hidden="true" />
            )}
            {isPending ? "در حال بررسی اطلاعات..." : "ورود به پنل"}
          </Button>
        </form>
        <p className="mt-6 text-center text-xs leading-5 text-slate-400">
          با ورود به پنل، به ابزارهای مدیریت مجموعه دسترسی خواهید داشت.
        </p>
      </CardContent>
    </Card>
  );
}
