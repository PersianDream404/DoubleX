"use client";

import { useMemo, useState, type FormEvent } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { toGregorian } from "jalaali-js";
import {
  BadgeDollarSign,
  Banknote,
  CalendarDays,
  CircleDollarSign,
  LoaderCircle,
  ReceiptText,
  RefreshCw,
  Wallet,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { handleApiError } from "@/lib/api/handle-api-error";
import { FinanceService } from "./finance.service";
import {
  FixedSalarySchema,
  PersonnelSettlementSchema,
  ReceptionEarningsSchema,
} from "./finance.schema";
import type { PersonnelEarningType } from "./finance.types";
import { useFinance } from "./use-finance";

type FinanceTab = "earnings" | "settlements";

const EMPTY_FIXED_SALARY = {
  periodStart: "",
  periodEnd: "",
};

function normalizeDigits(value: string) {
  return value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));
}

function toApiDate(value: string) {
  const [year, month, day] = normalizeDigits(value).split("/").map(Number);
  const date = toGregorian(year, month, day);
  return `${date.gy}-${String(date.gm).padStart(2, "0")}-${String(date.gd).padStart(2, "0")}`;
}

function persianDigits(value: string) {
  return value.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]);
}

function amountLabel(value: number) {
  return `${new Intl.NumberFormat("fa-IR").format(value)} تومان`;
}

function paidAtLabel(value: string) {
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

function earningTypeLabel(type: PersonnelEarningType) {
  return type === 1 ? "حقوق ثابت" : "پورسانت خدمات";
}

function JalaliDateInput({
  id,
  label,
  value,
  onChange,
  required = false,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}) {
  return (
    <div className="grid gap-1.5">
      <Label htmlFor={id}>{label}{required ? " *" : ""}</Label>
      <DatePicker
        calendar={persian}
        locale={persian_fa}
        digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
        format="YYYY/MM/DD"
        value={value || null}
        editable={false}
        calendarPosition="bottom-start"
        portal
        zIndex={150}
        onChange={(date: DateObject | null) =>
          onChange(date?.format("YYYY/MM/DD") ?? "")
        }
        render={(_displayValue, openCalendar) => (
          <div className="relative">
            <Input
              id={id}
              value={persianDigits(value)}
              placeholder="۱۴۰۵/۰۱/۰۱"
              readOnly
              dir="ltr"
              inputMode="none"
              aria-haspopup="dialog"
              className="cursor-pointer pe-9 text-start"
              onClick={openCalendar}
              onKeyDown={(event) => {
                if (event.key === "Enter" || event.key === " ") {
                  event.preventDefault();
                  openCalendar();
                }
              }}
            />
            <CalendarDays
              aria-hidden="true"
              className="pointer-events-none absolute end-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
            />
          </div>
        )}
      />
    </div>
  );
}

function SummaryCard({
  title,
  value,
  icon: Icon,
  tone,
}: {
  title: string;
  value: string;
  icon: typeof Wallet;
  tone: string;
}) {
  return (
    <div className="flex min-w-0 items-center gap-3 rounded-xl border border-fuchsia-100 bg-white p-4 shadow-sm">
      <span className={`flex size-10 shrink-0 items-center justify-center rounded-full ${tone}`}>
        <Icon aria-hidden="true" className="size-5" />
      </span>
      <div className="min-w-0">
        <p className="text-xs text-slate-500">{title}</p>
        <p className="mt-1 truncate text-base font-semibold text-slate-900">
          {value}
        </p>
      </div>
    </div>
  );
}

export function FinancePage() {
  const service = useMemo(() => new FinanceService(), []);
  const [tab, setTab] = useState<FinanceTab>("earnings");
  const [selectedPersonnelId, setSelectedPersonnelId] = useState<number | null>(null);
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [fixedSalary, setFixedSalary] = useState(EMPTY_FIXED_SALARY);
  const [receptionId, setReceptionId] = useState("");
  const [settlementAmount, setSettlementAmount] = useState("");
  const [earningType, setEarningType] = useState<PersonnelEarningType>(1);
  const [paymentMethod, setPaymentMethod] = useState("");
  const [settlementDescription, setSettlementDescription] = useState("");
  const [submitting, setSubmitting] = useState<"fixed" | "reception" | "settlement" | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const {
    personnel,
    settlements,
    earnings,
    personnelLoading,
    settlementsLoading,
    earningsLoading,
    personnelError,
    settlementsError,
    earningsError,
  } = useFinance({
    personnelId: selectedPersonnelId,
    page,
    refreshKey,
  });

  const personnelNames = useMemo(
    () => new Map(personnel.map((item) => [item.id, item.fullName])),
    [personnel],
  );
  const totalPages = Math.max(1, settlements.pagination.totalPages);

  function showSuccess(message: string) {
    setSuccessMessage(message);
    setActionError(null);
    setRefreshKey((current) => current + 1);
  }

  async function submitFixedSalary(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;

    const parsed = FixedSalarySchema.safeParse({
      personnelId: selectedPersonnelId,
      ...fixedSalary,
    });
    if (!parsed.success) {
      setActionError(parsed.error.issues[0]?.message ?? "اطلاعات محاسبه حقوق معتبر نیست.");
      return;
    }

    setSubmitting("fixed");
    setActionError(null);
    setSuccessMessage(null);
    try {
      const response = await service.calculateFixedSalary({
        personnelId: parsed.data.personnelId,
        periodStart: toApiDate(parsed.data.periodStart),
        periodEnd: toApiDate(parsed.data.periodEnd),
      });
      if (!response.data) {
        throw new Error(response.message ?? "محاسبه حقوق ثابت انجام نشد.");
      }
      showSuccess("حقوق ثابت دوره با موفقیت محاسبه و ثبت شد.");
    } catch (error: unknown) {
      setActionError(handleApiError(error));
    } finally {
      setSubmitting(null);
    }
  }

  async function submitReceptionEarnings(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;

    const parsed = ReceptionEarningsSchema.safeParse({ receptionId });
    if (!parsed.success) {
      setActionError(parsed.error.issues[0]?.message ?? "شناسه پذیرش معتبر نیست.");
      return;
    }

    setSubmitting("reception");
    setActionError(null);
    setSuccessMessage(null);
    try {
      const response = await service.calculateReceptionEarnings(parsed.data.receptionId);
      if (!response.data) {
        throw new Error(response.message ?? "محاسبه پورسانت انجام نشد.");
      }
      showSuccess(`${new Intl.NumberFormat("fa-IR").format(response.data.createdCount)} پورسانت برای پذیرش محاسبه و ثبت شد.`);
    } catch (error: unknown) {
      setActionError(handleApiError(error));
    } finally {
      setSubmitting(null);
    }
  }

  async function submitSettlement(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;

    const parsed = PersonnelSettlementSchema.safeParse({
      personnelId: selectedPersonnelId,
      earningType,
      amount: settlementAmount,
      paymentMethod,
      description: settlementDescription,
    });
    if (!parsed.success) {
      setActionError(parsed.error.issues[0]?.message ?? "اطلاعات تسویه معتبر نیست.");
      return;
    }

    setSubmitting("settlement");
    setActionError(null);
    setSuccessMessage(null);
    try {
      const response = await service.registerSettlement({
        ...parsed.data,
        idempotencyKey: `settlement:${crypto.randomUUID()}`,
        description: parsed.data.description || null,
      });
      if (!response.data) {
        throw new Error(response.message ?? "ثبت تسویه انجام نشد.");
      }
      setSettlementAmount("");
      setPaymentMethod("");
      setSettlementDescription("");
      setPage(1);
      showSuccess("پرداخت پرسنل با موفقیت ثبت شد.");
    } catch (error: unknown) {
      setActionError(handleApiError(error));
    } finally {
      setSubmitting(null);
    }
  }

  function onPersonnelChange(value: string) {
    setSelectedPersonnelId(value ? Number(value) : null);
    setPage(1);
    setActionError(null);
    setSuccessMessage(null);
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] max-w-[1200px] pt-2">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/finance">مدیریت مالی</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">حقوق و تسویه پرسنل</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-3 flex items-center justify-between gap-3">
            <div>
              <h1 className="text-lg font-semibold text-white">حقوق و تسویه پرسنل</h1>
              <p className="mt-1 text-xs text-white/70">
                محاسبه حقوق ثابت، پورسانت و ثبت پرداخت‌های مرحله‌ای
              </p>
            </div>
            <Button
              type="button"
              variant="outline"
              className="h-8 border-white/40 bg-white/10 text-white hover:bg-white/20 hover:text-white"
              onClick={() => setRefreshKey((current) => current + 1)}
            >
              <RefreshCw className="size-4" />
              به‌روزرسانی
            </Button>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[94px] w-[92%] max-w-[1200px] space-y-4 pb-10">
        <section className="rounded-xl border border-fuchsia-100 bg-white p-4 shadow-sm">
          <div className="grid items-end gap-3 md:grid-cols-[minmax(0,1fr)_minmax(0,2fr)]">
            <div className="grid gap-1.5">
              <Label htmlFor="finance-personnel">پرسنل</Label>
              <select
                id="finance-personnel"
                value={selectedPersonnelId ?? ""}
                onChange={(event) => onPersonnelChange(event.target.value)}
                disabled={personnelLoading || personnel.length === 0}
                className="h-10 w-full rounded-md border border-input bg-background px-3 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 disabled:opacity-60"
              >
                <option value="">
                  {personnelLoading ? "در حال دریافت فهرست پرسنل..." : "انتخاب پرسنل"}
                </option>
                {personnel.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.fullName} - {item.jobTitle}
                  </option>
                ))}
              </select>
              {personnelError && <p role="alert" className="text-xs text-destructive">{personnelError}</p>}
              {!personnelLoading && !personnelError && personnel.length === 0 && (
                <p className="text-xs text-slate-500">پرسنل فعالی برای این سالن ثبت نشده است.</p>
              )}
            </div>
            <div className="flex flex-wrap gap-2" role="tablist" aria-label="بخش مالی پرسنل">
              <Button
                type="button"
                role="tab"
                aria-selected={tab === "earnings"}
                className={tab === "earnings" ? "bg-fuchsia-700 text-white hover:bg-fuchsia-800" : "bg-fuchsia-50 text-fuchsia-800 hover:bg-fuchsia-100"}
                onClick={() => setTab("earnings")}
              >
                <BadgeDollarSign className="size-4" />
                حقوق و پورسانت
              </Button>
              <Button
                type="button"
                role="tab"
                aria-selected={tab === "settlements"}
                className={tab === "settlements" ? "bg-fuchsia-700 text-white hover:bg-fuchsia-800" : "bg-fuchsia-50 text-fuchsia-800 hover:bg-fuchsia-100"}
                onClick={() => setTab("settlements")}
              >
                <Banknote className="size-4" />
                تسویه‌حساب
              </Button>
            </div>
          </div>

          {actionError && (
            <p role="alert" className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-800">
              {actionError}
            </p>
          )}
          {successMessage && (
            <p role="status" className="mt-4 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-800">
              {successMessage}
            </p>
          )}
        </section>

        {tab === "earnings" ? (
          <section role="tabpanel" className="space-y-4">
            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
              <SummaryCard
                title="حقوق ثابت"
                value={earningsLoading ? "..." : amountLabel(earnings?.fixedSalaryAmount ?? 0)}
                icon={Wallet}
                tone="bg-blue-50 text-blue-700"
              />
              <SummaryCard
                title="پورسانت خدمات"
                value={earningsLoading ? "..." : amountLabel(earnings?.serviceCommissionAmount ?? 0)}
                icon={CircleDollarSign}
                tone="bg-fuchsia-50 text-fuchsia-700"
              />
              <SummaryCard
                title="کل مطالبات"
                value={earningsLoading ? "..." : amountLabel(earnings?.totalEarned ?? 0)}
                icon={ReceiptText}
                tone="bg-violet-50 text-violet-700"
              />
              <SummaryCard
                title="پرداخت‌شده"
                value={earningsLoading ? "..." : amountLabel(earnings?.settledAmount ?? 0)}
                icon={Banknote}
                tone="bg-emerald-50 text-emerald-700"
              />
              <SummaryCard
                title="مانده بدهی"
                value={earningsLoading ? "..." : amountLabel(earnings?.remainingAmount ?? 0)}
                icon={BadgeDollarSign}
                tone="bg-amber-50 text-amber-700"
              />
            </div>
            {earningsError && (
              <p role="alert" className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-800">
                {earningsError}
              </p>
            )}

            <div className="grid items-start gap-4 lg:grid-cols-2">
              <form onSubmit={submitFixedSalary} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                <div className="mb-4 flex items-center gap-2">
                  <Wallet className="size-5 text-fuchsia-700" />
                  <div>
                    <h2 className="text-sm font-semibold">محاسبه حقوق ثابت</h2>
                    <p className="text-xs text-slate-500">حقوق ثبت‌شده در قرارداد پرسنل را برای بازه انتخابی ثبت کنید.</p>
                  </div>
                </div>
                <div className="grid gap-3 sm:grid-cols-2">
                  <JalaliDateInput
                    id="finance-period-start"
                    label="شروع دوره"
                    value={fixedSalary.periodStart}
                    onChange={(periodStart) => setFixedSalary((current) => ({ ...current, periodStart }))}
                    required
                  />
                  <JalaliDateInput
                    id="finance-period-end"
                    label="پایان دوره"
                    value={fixedSalary.periodEnd}
                    onChange={(periodEnd) => setFixedSalary((current) => ({ ...current, periodEnd }))}
                    required
                  />
                </div>
                <Button
                  type="submit"
                  disabled={!selectedPersonnelId || submitting !== null}
                  className="mt-4 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
                >
                  {submitting === "fixed" && <LoaderCircle className="size-4 animate-spin" />}
                  محاسبه و ثبت حقوق
                </Button>
              </form>

              <form onSubmit={submitReceptionEarnings} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                <div className="mb-4 flex items-center gap-2">
                  <CircleDollarSign className="size-5 text-fuchsia-700" />
                  <div>
                    <h2 className="text-sm font-semibold">محاسبه پورسانت پذیرش</h2>
                    <p className="text-xs text-slate-500">پورسانت خدمات تکمیل‌شده و دارای پرسنل را از روی قرارداد محاسبه می‌کند.</p>
                  </div>
                </div>
                <div className="grid gap-1.5">
                  <Label htmlFor="finance-reception-id">شناسه پذیرش</Label>
                  <Input
                    id="finance-reception-id"
                    value={receptionId}
                    onChange={(event) => setReceptionId(normalizeDigits(event.target.value))}
                    inputMode="numeric"
                    dir="ltr"
                    placeholder="مثلاً 123"
                  />
                  <p className="text-xs text-slate-500">محاسبه برای همه پرسنل واجد شرایطِ پذیرش انجام می‌شود.</p>
                </div>
                <Button
                  type="submit"
                  disabled={submitting !== null}
                  className="mt-4 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
                >
                  {submitting === "reception" && <LoaderCircle className="size-4 animate-spin" />}
                  محاسبه پورسانت
                </Button>
              </form>
            </div>
            <p className="rounded-lg border border-amber-200 bg-amber-50 px-3 py-2 text-xs leading-6 text-amber-900">
              مطالبات متفرقه به‌عنوان نوع مستقل در API فعلی Finance ثبت نمی‌شود؛ این صفحه فقط حقوق ثابت و پورسانت خدمات را بر اساس قرارداد و پذیرش محاسبه می‌کند.
            </p>
            {!selectedPersonnelId && (
              <p className="rounded-lg border border-slate-200 bg-slate-50 p-4 text-center text-sm text-slate-600">
                برای مشاهده مانده مطالبات، یک پرسنل انتخاب کنید.
              </p>
            )}
          </section>
        ) : (
          <section role="tabpanel" className="space-y-4">
            <div className="grid items-start gap-4 lg:grid-cols-[minmax(0,1fr)_minmax(0,1.35fr)]">
              <form onSubmit={submitSettlement} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                <div className="mb-4 flex items-center gap-2">
                  <Banknote className="size-5 text-fuchsia-700" />
                  <div>
                    <h2 className="text-sm font-semibold">ثبت پرداخت پرسنل</h2>
                    <p className="text-xs text-slate-500">برای پرداخت مرحله‌ای، مبلغ همان پرداخت را ثبت کنید.</p>
                  </div>
                </div>
                <div className="grid gap-3">
                  <div className="grid gap-1.5">
                    <Label htmlFor="finance-settlement-type">نوع مطالبات</Label>
                    <select
                      id="finance-settlement-type"
                      value={earningType}
                      onChange={(event) => setEarningType(Number(event.target.value) as PersonnelEarningType)}
                      className="h-10 rounded-md border border-input bg-background px-3 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                    >
                      <option value={1}>حقوق ثابت</option>
                      <option value={2}>پورسانت خدمات</option>
                    </select>
                  </div>
                  <div className="grid gap-1.5">
                    <Label htmlFor="finance-settlement-amount">مبلغ پرداخت (تومان)</Label>
                    <Input
                      id="finance-settlement-amount"
                      value={settlementAmount}
                      onChange={(event) => setSettlementAmount(normalizeDigits(event.target.value))}
                      inputMode="decimal"
                      dir="ltr"
                      placeholder="0"
                    />
                  </div>
                  <div className="grid gap-1.5">
                    <Label htmlFor="finance-payment-method">روش پرداخت</Label>
                    <Input
                      id="finance-payment-method"
                      value={paymentMethod}
                      onChange={(event) => setPaymentMethod(event.target.value)}
                      placeholder="مثلاً کارت‌به‌کارت یا نقدی"
                      maxLength={50}
                    />
                  </div>
                  <div className="grid gap-1.5">
                    <Label htmlFor="finance-settlement-description">توضیحات (اختیاری)</Label>
                    <Input
                      id="finance-settlement-description"
                      value={settlementDescription}
                      onChange={(event) => setSettlementDescription(event.target.value)}
                      maxLength={1000}
                    />
                  </div>
                </div>
                {selectedPersonnelId && earnings && (
                  <p className="mt-3 rounded-md bg-amber-50 px-3 py-2 text-xs leading-5 text-amber-900">
                    مانده کل قابل‌پرداخت: {amountLabel(earnings.remainingAmount)}. کنترل مانده جداگانه برای نوع مطالبات در سرور انجام می‌شود.
                  </p>
                )}
                {!selectedPersonnelId && (
                  <p className="mt-3 text-xs text-amber-700">ابتدا پرسنل را از بالای صفحه انتخاب کنید.</p>
                )}
                <Button
                  type="submit"
                  disabled={!selectedPersonnelId || submitting !== null}
                  className="mt-4 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
                >
                  {submitting === "settlement" && <LoaderCircle className="size-4 animate-spin" />}
                  ثبت پرداخت
                </Button>
              </form>

              <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                <div className="mb-3 flex items-center justify-between gap-3">
                  <div>
                    <h2 className="text-sm font-semibold">تاریخچه پرداخت‌ها</h2>
                    <p className="text-xs text-slate-500">
                      {selectedPersonnelId ? "تسویه‌های پرسنل انتخاب‌شده" : "تسویه‌های ثبت‌شده در سالن"}
                    </p>
                  </div>
                  {settlements.pagination.totalCount > 0 && (
                    <Badge variant="secondary">{new Intl.NumberFormat("fa-IR").format(settlements.pagination.totalCount)} پرداخت</Badge>
                  )}
                </div>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow className="bg-muted/60 hover:bg-muted/60">
                        {!selectedPersonnelId && <TableHead className="h-9 px-2 text-start">پرسنل</TableHead>}
                        <TableHead className="h-9 px-2 text-start">نوع مطالبات</TableHead>
                        <TableHead className="h-9 px-2 text-start">مبلغ</TableHead>
                        <TableHead className="h-9 px-2 text-start">روش پرداخت</TableHead>
                        <TableHead className="h-9 px-2 text-start">تاریخ پرداخت</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {settlementsError ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 4 : 5} className="py-8 text-center text-sm text-destructive" role="alert">
                            {settlementsError}
                          </TableCell>
                        </TableRow>
                      ) : settlementsLoading ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 4 : 5} className="py-8 text-center text-sm">در حال دریافت تسویه‌ها...</TableCell>
                        </TableRow>
                      ) : settlements.list.length === 0 ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 4 : 5} className="py-8 text-center text-sm">پرداختی برای نمایش ثبت نشده است.</TableCell>
                        </TableRow>
                      ) : (
                        settlements.list.map((settlement) => (
                          <TableRow key={settlement.id} className="h-10">
                            {!selectedPersonnelId && (
                              <TableCell className="px-2 py-2 text-sm">
                                {personnelNames.get(settlement.personnelId) ?? `پرسنل ${settlement.personnelId}`}
                              </TableCell>
                            )}
                            <TableCell className="px-2 py-2 text-sm">{earningTypeLabel(settlement.earningType)}</TableCell>
                            <TableCell className="whitespace-nowrap px-2 py-2 text-sm">{amountLabel(settlement.amount)}</TableCell>
                            <TableCell className="px-2 py-2 text-sm">{settlement.paymentMethod}</TableCell>
                            <TableCell className="whitespace-nowrap px-2 py-2 text-sm" dir="ltr">{paidAtLabel(settlement.paidAt)}</TableCell>
                          </TableRow>
                        ))
                      )}
                    </TableBody>
                  </Table>
                </div>
                {totalPages > 1 && (
                  <div className="mt-3 flex items-center justify-between text-xs text-slate-600">
                    <Button type="button" variant="outline" size="sm" disabled={page <= 1 || settlementsLoading} onClick={() => setPage((current) => Math.max(1, current - 1))}>
                      قبلی
                    </Button>
                    <span>صفحه {new Intl.NumberFormat("fa-IR").format(page)} از {new Intl.NumberFormat("fa-IR").format(totalPages)}</span>
                    <Button type="button" variant="outline" size="sm" disabled={page >= totalPages || settlementsLoading} onClick={() => setPage((current) => Math.min(totalPages, current + 1))}>
                      بعدی
                    </Button>
                  </div>
                )}
              </div>
            </div>
          </section>
        )}
      </main>
    </div>
  );
}
