"use client";

import { useMemo, useState, type FormEvent } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { jalaaliMonthLength, toGregorian } from "jalaali-js";
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

const EMPTY_FIXED_SALARY = {
  periodStart: "",
  periodEnd: "",
};

type EarningsPeriodPreset = "all" | "daily" | "monthly" | "custom";
type EarningsDateRange = {
  from: string;
  to: string;
  preset: EarningsPeriodPreset;
};

const EMPTY_EARNINGS_DATE_RANGE: EarningsDateRange = {
  from: "",
  to: "",
  preset: "all",
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

function currentJalaliDate() {
  const parts = new Intl.DateTimeFormat("en-US-u-ca-persian-nu-latn", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(new Date());
  const getPart = (type: Intl.DateTimeFormatPartTypes) =>
    parts.find((part) => part.type === type)?.value ?? "";
  return `${getPart("year")}/${getPart("month")}/${getPart("day")}`;
}

function toApiDateTime(value: string, endOfDay: boolean) {
  const normalized = normalizeDigits(value);
  const [year, month, day] = normalized.split("/").map(Number);
  const gregorian = toGregorian(year, month, day);
  const date = `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(gregorian.gd).padStart(2, "0")}`;
  const offsetParts = new Intl.DateTimeFormat("en-US", {
    timeZone: "Asia/Tehran",
    timeZoneName: "longOffset",
  }).formatToParts(new Date(Date.UTC(gregorian.gy, gregorian.gm - 1, gregorian.gd, 12)));
  const offsetLabel = offsetParts.find((part) => part.type === "timeZoneName")?.value ?? "GMT+03:30";
  const offset = offsetLabel === "GMT" ? "+00:00" : offsetLabel.replace(/^GMT/, "");
  return `${date}T${endOfDay ? "23:59:59.999" : "00:00:00"}${offset}`;
}

function normalizeJalaliDate(value: string) {
  return normalizeDigits(value);
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
    <div className={`flex min-w-0 items-center justify-between gap-2 rounded-lg border p-3 ${tone}`}>
      <div className="min-w-0">
        <p className="truncate text-[11px] font-medium opacity-80">{title}</p>
        <p className="mt-1 truncate text-sm font-bold">{value}</p>
      </div>
      <span className="flex size-8 shrink-0 items-center justify-center rounded-full bg-white/80">
        <Icon aria-hidden="true" className="size-4" />
      </span>
    </div>
  );
}

export function FinancePage() {
  const service = useMemo(() => new FinanceService(), []);
  const [selectedPersonnelId, setSelectedPersonnelId] = useState<number | null>(null);
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [earningsDateRange, setEarningsDateRange] = useState<EarningsDateRange>(
    EMPTY_EARNINGS_DATE_RANGE,
  );
  const [fixedSalary, setFixedSalary] = useState(EMPTY_FIXED_SALARY);
  const [receptionId, setReceptionId] = useState("");
  const [settlementAmount, setSettlementAmount] = useState("");
  const [earningType, setEarningType] = useState<PersonnelEarningType>(1);
  const [paymentMethod, setPaymentMethod] = useState("");
  const [settlementDescription, setSettlementDescription] = useState("");
  const [submitting, setSubmitting] = useState<"fixed" | "reception" | "settlement" | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const earningsRangeValid =
    !earningsDateRange.from ||
    !earningsDateRange.to ||
    normalizeJalaliDate(earningsDateRange.from) <= normalizeJalaliDate(earningsDateRange.to);
  const earningsFrom = earningsDateRange.from
    ? toApiDateTime(earningsDateRange.from, false)
    : undefined;
  const earningsTo = earningsDateRange.to
    ? toApiDateTime(earningsDateRange.to, true)
    : undefined;

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
    earningsFrom,
    earningsTo,
    earningsRangeValid,
  });

  const personnelNames = useMemo(
    () => new Map(personnel.map((item) => [item.id, item.fullName])),
    [personnel],
  );
  const totalPages = Math.max(1, settlements.pagination.totalPages);
  const selectedPersonnel = personnel.find((item) => item.id === selectedPersonnelId);
  const latestSettlement = settlements.list.reduce<
    (typeof settlements.list)[number] | null
  >((latest, settlement) => {
    if (!latest || Date.parse(settlement.paidAt) > Date.parse(latest.paidAt)) {
      return settlement;
    }
    return latest;
  }, null);

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

  function setDailyEarningsRange() {
    const today = currentJalaliDate();
    setEarningsDateRange({ from: today, to: today, preset: "daily" });
  }

  function setMonthlyEarningsRange() {
    const [year, month] = currentJalaliDate().split("/").map(Number);
    setEarningsDateRange({
      from: `${year}/${String(month).padStart(2, "0")}/01`,
      to: `${year}/${String(month).padStart(2, "0")}/${String(jalaaliMonthLength(year, month)).padStart(2, "0")}`,
      preset: "monthly",
    });
  }

  return (
    <div className="min-h-screen bg-[#f6f8fd]">
      <div className="border-b border-fuchsia-800 bg-[#32103c]">
        <div className="mx-auto flex h-7 w-[94%] max-w-[1480px] items-center">
          <Breadcrumb>
            <BreadcrumbList className="text-[10px] text-white/75">
              <BreadcrumbItem>
                <BreadcrumbLink href="/finance">حقوق و تسویه</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">حقوق و پورسانت</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </div>
      </div>

      <div className="mx-auto w-[94%] max-w-[1480px] py-4 sm:py-5">
        <div className="grid items-start gap-3 lg:grid-cols-[320px_minmax(0,1fr)]">
          <aside className="space-y-3">
            <section className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
              <header className="flex items-center justify-between border-b border-slate-100 px-4 py-3">
                <h2 className="flex items-center gap-2 text-sm font-bold text-slate-800">
                  <ReceiptText aria-hidden="true" className="size-4 text-fuchsia-600" />
                  خلاصه وضعیت
                </h2>
                <span className="size-2 rounded-full bg-emerald-500" aria-label="گزارش مالی" />
              </header>
              <dl className="divide-y divide-slate-100 px-3">
                <div className="flex items-center justify-between gap-3 py-3">
                  <dt className="text-xs text-slate-500">پرسنل</dt>
                  <dd className="truncate text-xs font-semibold text-slate-800">
                    {selectedPersonnel?.fullName ?? "انتخاب نشده"}
                  </dd>
                </div>
                <div className="flex items-center justify-between gap-3 py-3">
                  <dt className="text-xs text-slate-500">آخرین پرداخت</dt>
                  <dd className="text-xs font-semibold text-slate-800" dir="ltr">
                    {latestSettlement ? paidAtLabel(latestSettlement.paidAt) : "—"}
                  </dd>
                </div>
                <div className="flex items-center justify-between gap-3 py-3">
                  <dt className="text-xs text-slate-500">روش پرداخت</dt>
                  <dd className="truncate text-xs font-semibold text-slate-800">
                    {latestSettlement?.paymentMethod ?? "—"}
                  </dd>
                </div>
              </dl>
            </section>

            <form
              onSubmit={submitSettlement}
              className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
            >
              <header className="mb-4 border-b border-slate-100 pb-3">
                <h2 className="flex items-center gap-2 text-sm font-bold text-slate-800">
                  <Banknote aria-hidden="true" className="size-4 text-fuchsia-600" />
                  ثبت پرداخت پرسنل
                </h2>
                <p className="mt-1 text-[11px] leading-5 text-slate-500">
                  برای پرداخت مرحله‌ای، مبلغ همان پرداخت را ثبت کنید.
                </p>
              </header>
              <div className="grid gap-3">
                <div className="grid gap-1.5">
                  <Label htmlFor="finance-settlement-type">نوع مطالبات</Label>
                  <select
                    id="finance-settlement-type"
                    value={earningType}
                    onChange={(event) => setEarningType(Number(event.target.value) as PersonnelEarningType)}
                    className="h-9 rounded-lg border border-slate-200 bg-white px-3 text-xs outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
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
                    placeholder="۰"
                    className="h-9 rounded-lg text-xs"
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
                    className="h-9 rounded-lg text-xs"
                  />
                </div>
                <div className="grid gap-1.5">
                  <Label htmlFor="finance-settlement-description">توضیحات (اختیاری)</Label>
                  <Input
                    id="finance-settlement-description"
                    value={settlementDescription}
                    onChange={(event) => setSettlementDescription(event.target.value)}
                    maxLength={1000}
                    placeholder="توضیحات را وارد کنید..."
                    className="h-9 rounded-lg text-xs"
                  />
                </div>
              </div>
              {selectedPersonnelId && earnings && (
                <p className="mt-3 rounded-lg border border-blue-100 bg-blue-50/70 px-3 py-2 text-[10px] leading-5 text-blue-800">
                  مانده کل قابل‌پرداخت: {amountLabel(earnings.remainingAmount)}. کنترل مانده نوع مطالبات در سرور انجام می‌شود.
                </p>
              )}
              {!selectedPersonnelId && (
                <p className="mt-3 rounded-lg bg-amber-50 px-3 py-2 text-[10px] leading-5 text-amber-800">
                  ابتدا پرسنل را از بخش فیلتر انتخاب کنید.
                </p>
              )}
              <Button
                type="submit"
                disabled={!selectedPersonnelId || submitting !== null}
                className="mt-4 h-9 w-full rounded-lg bg-fuchsia-700 text-xs text-white hover:bg-fuchsia-800"
              >
                {submitting === "settlement" ? (
                  <LoaderCircle className="size-4 animate-spin" />
                ) : (
                  <Banknote className="size-4" />
                )}
                ثبت پرداخت
              </Button>
            </form>
          </aside>

          <main className="min-w-0 overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <header className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 bg-gradient-to-l from-white to-fuchsia-50/30 px-4 py-3 sm:px-5">
              <div>
                <h1 className="flex items-center gap-2 text-base font-bold text-fuchsia-800">
                  <ReceiptText aria-hidden="true" className="size-5 text-fuchsia-600" />
                  حقوق و پورسانت
                </h1>
                <p className="mt-1 text-[11px] text-slate-500">
                  محاسبه حقوق ثابت، پورسانت و ثبت پرداخت‌های مرحله‌ای
                </p>
              </div>
              <Button
                type="button"
                variant="outline"
                onClick={() => setRefreshKey((current) => current + 1)}
                className="h-8 rounded-lg border-fuchsia-200 px-3 text-xs text-fuchsia-700 hover:bg-fuchsia-50"
              >
                <RefreshCw className="size-3.5" />
                به‌روزرسانی
              </Button>
            </header>

            <div className="space-y-3 p-3 sm:p-4">
              <section className="rounded-lg border border-slate-200 p-3">
                <div className="grid gap-3 lg:grid-cols-[minmax(220px,1fr)_minmax(0,1.5fr)] lg:items-end">
                  <div className="grid gap-1.5">
                    <Label htmlFor="finance-personnel">پرسنل</Label>
                    <select
                      id="finance-personnel"
                      value={selectedPersonnelId ?? ""}
                      onChange={(event) => onPersonnelChange(event.target.value)}
                      disabled={personnelLoading || personnel.length === 0}
                      className="h-9 w-full rounded-lg border border-slate-200 bg-white px-3 text-xs outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 disabled:opacity-60"
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
                    {personnelError && (
                      <p role="alert" className="text-xs text-destructive">{personnelError}</p>
                    )}
                    {!personnelLoading && !personnelError && personnel.length === 0 && (
                      <p className="text-xs text-slate-500">پرسنل فعالی برای این سالن ثبت نشده است.</p>
                    )}
                  </div>

                  <div className="grid gap-2 sm:grid-cols-[auto_minmax(0,1fr)] sm:items-end">
                    <div className="flex overflow-x-auto rounded-lg border border-slate-200 bg-slate-50/70">
                      {([
                        { value: "monthly", label: "ماهانه", onClick: setMonthlyEarningsRange },
                        { value: "daily", label: "روزانه", onClick: setDailyEarningsRange },
                        {
                          value: "all",
                          label: "همه تاریخ‌ها",
                          onClick: () => setEarningsDateRange(EMPTY_EARNINGS_DATE_RANGE),
                        },
                      ] as const).map((preset) => (
                        <button
                          key={preset.value}
                          type="button"
                          aria-pressed={earningsDateRange.preset === preset.value}
                          onClick={preset.onClick}
                          className={`shrink-0 border-l border-slate-200 px-3 py-2 text-[11px] transition last:border-l-0 ${
                            earningsDateRange.preset === preset.value
                              ? "bg-fuchsia-50 font-semibold text-fuchsia-800"
                              : "text-slate-600 hover:bg-white"
                          }`}
                        >
                          {preset.label}
                        </button>
                      ))}
                    </div>
                    <div className="grid gap-2 sm:grid-cols-2">
                      <JalaliDateInput
                        id="finance-earnings-from"
                        label="از تاریخ"
                        value={earningsDateRange.from}
                        onChange={(from) =>
                          setEarningsDateRange({ from, to: earningsDateRange.to, preset: "custom" })
                        }
                      />
                      <JalaliDateInput
                        id="finance-earnings-to"
                        label="تا تاریخ"
                        value={earningsDateRange.to}
                        onChange={(to) =>
                          setEarningsDateRange({ from: earningsDateRange.from, to, preset: "custom" })
                        }
                      />
                    </div>
                  </div>
                </div>
                {!earningsRangeValid && (
                  <p role="alert" className="mt-3 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-xs text-rose-700">
                    تاریخ پایان باید مساوی یا بعد از تاریخ شروع باشد.
                  </p>
                )}
              </section>

              {(actionError || successMessage) && (
                <p
                  role={actionError ? "alert" : "status"}
                  className={`rounded-lg border px-3 py-2 text-xs ${
                    actionError
                      ? "border-rose-200 bg-rose-50 text-rose-800"
                      : "border-emerald-200 bg-emerald-50 text-emerald-800"
                  }`}
                >
                  {actionError ?? successMessage}
                </p>
              )}

              <section
                aria-label="خلاصه مطالبات پرسنل"
                className="grid grid-cols-2 gap-2 sm:grid-cols-3 xl:grid-cols-5"
              >
                <SummaryCard
                  title="حقوق ثابت"
                  value={!selectedPersonnelId || !earningsRangeValid ? "—" : earningsLoading ? "..." : amountLabel(earnings?.fixedSalaryAmount ?? 0)}
                  icon={Wallet}
                  tone="border-fuchsia-100 bg-fuchsia-50/70 text-fuchsia-800"
                />
                <SummaryCard
                  title="پورسانت خدمات"
                  value={!selectedPersonnelId || !earningsRangeValid ? "—" : earningsLoading ? "..." : amountLabel(earnings?.serviceCommissionAmount ?? 0)}
                  icon={CircleDollarSign}
                  tone="border-blue-100 bg-blue-50/70 text-blue-800"
                />
                <SummaryCard
                  title="مانده بدهی"
                  value={!selectedPersonnelId || !earningsRangeValid ? "—" : earningsLoading ? "..." : amountLabel(earnings?.remainingAmount ?? 0)}
                  icon={BadgeDollarSign}
                  tone="border-pink-100 bg-pink-50/70 text-pink-800"
                />
                <SummaryCard
                  title="پرداخت‌شده"
                  value={!selectedPersonnelId || !earningsRangeValid ? "—" : earningsLoading ? "..." : amountLabel(earnings?.settledAmount ?? 0)}
                  icon={Banknote}
                  tone="border-emerald-100 bg-emerald-50/70 text-emerald-800"
                />
                <SummaryCard
                  title="کل مطالبات"
                  value={!selectedPersonnelId || !earningsRangeValid ? "—" : earningsLoading ? "..." : amountLabel(earnings?.totalEarned ?? 0)}
                  icon={ReceiptText}
                  tone="border-violet-100 bg-violet-50/70 text-violet-800"
                />
              </section>

              {earningsError && (
                <p role="alert" className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-xs text-rose-800">
                  {earningsError}
                </p>
              )}

              <section aria-label="محاسبه مطالبات" className="grid items-stretch gap-3 lg:grid-cols-2">
                <form
                  onSubmit={submitFixedSalary}
                  className="overflow-hidden rounded-lg border border-slate-200"
                >
                  <header className="flex items-start gap-2 border-b border-slate-100 bg-gradient-to-l from-white to-fuchsia-50/50 px-3 py-3">
                    <Wallet aria-hidden="true" className="mt-0.5 size-4 shrink-0 text-fuchsia-600" />
                    <div>
                      <h2 className="text-xs font-bold text-slate-800">محاسبه حقوق ثابت</h2>
                      <p className="mt-1 text-[10px] leading-5 text-slate-500">
                        حقوق ثبت‌شده در قرارداد پرسنل را برای بازه انتخابی ثبت کنید.
                      </p>
                    </div>
                  </header>
                  <div className="grid gap-3 p-3 sm:grid-cols-2">
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
                    <Button
                      type="submit"
                      disabled={!selectedPersonnelId || submitting !== null}
                      className="h-8 rounded-lg bg-fuchsia-700 text-xs text-white hover:bg-fuchsia-800 sm:col-span-2 sm:w-fit"
                    >
                      {submitting === "fixed" && <LoaderCircle className="size-4 animate-spin" />}
                      محاسبه حقوق ثابت
                    </Button>
                  </div>
                </form>

                <form
                  onSubmit={submitReceptionEarnings}
                  className="overflow-hidden rounded-lg border border-slate-200"
                >
                  <header className="flex items-start gap-2 border-b border-slate-100 bg-gradient-to-l from-white to-blue-50/50 px-3 py-3">
                    <CircleDollarSign aria-hidden="true" className="mt-0.5 size-4 shrink-0 text-fuchsia-600" />
                    <div>
                      <h2 className="text-xs font-bold text-slate-800">محاسبه پورسانت</h2>
                      <p className="mt-1 text-[10px] leading-5 text-slate-500">
                        پورسانت خدمات تکمیل‌شده و دارای پرسنل از روی قرارداد محاسبه می‌شود.
                      </p>
                    </div>
                  </header>
                  <div className="grid gap-3 p-3 sm:grid-cols-[minmax(0,1fr)_auto] sm:items-end">
                    <div className="grid gap-1.5">
                      <Label htmlFor="finance-reception-id">شناسه پذیرش</Label>
                      <Input
                        id="finance-reception-id"
                        value={receptionId}
                        onChange={(event) => setReceptionId(normalizeDigits(event.target.value))}
                        inputMode="numeric"
                        dir="ltr"
                        placeholder="مثلاً ۱۲۳"
                        className="h-9 rounded-lg text-xs"
                      />
                    </div>
                    <Button
                      type="submit"
                      disabled={submitting !== null}
                      className="h-8 rounded-lg bg-fuchsia-700 text-xs text-white hover:bg-fuchsia-800"
                    >
                      {submitting === "reception" && <LoaderCircle className="size-4 animate-spin" />}
                      محاسبه پورسانت
                    </Button>
                    <p className="text-[10px] leading-5 text-slate-500 sm:col-span-2">
                      محاسبه برای همه پرسنل واجد شرایطِ پذیرش انجام می‌شود.
                    </p>
                  </div>
                </form>
              </section>

              <section className="overflow-hidden rounded-lg border border-slate-200">
                <header className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 px-3 py-3">
                  <h2 className="flex items-center gap-2 text-xs font-bold text-slate-800">
                    <CalendarDays aria-hidden="true" className="size-4 text-fuchsia-600" />
                    تاریخچه پرداخت‌ها
                  </h2>
                  {settlements.pagination.totalCount > 0 && (
                    <Badge variant="secondary" className="text-[10px]">
                      {new Intl.NumberFormat("fa-IR").format(settlements.pagination.totalCount)} پرداخت
                    </Badge>
                  )}
                </header>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow className="bg-slate-50 hover:bg-slate-50">
                        {!selectedPersonnelId && <TableHead className="h-9 px-3 text-start text-[10px]">پرسنل</TableHead>}
                        <TableHead className="h-9 px-3 text-start text-[10px]">تاریخ پرداخت</TableHead>
                        <TableHead className="h-9 px-3 text-start text-[10px]">روش پرداخت</TableHead>
                        <TableHead className="h-9 px-3 text-start text-[10px]">مبلغ</TableHead>
                        <TableHead className="h-9 px-3 text-start text-[10px]">نوع مطالبات</TableHead>
                        <TableHead className="h-9 px-3 text-start text-[10px]">توضیحات</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {settlementsError ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 5 : 6} className="py-8 text-center text-xs text-destructive" role="alert">
                            {settlementsError}
                          </TableCell>
                        </TableRow>
                      ) : settlementsLoading ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 5 : 6} className="py-8 text-center text-xs text-slate-500">
                            در حال دریافت تسویه‌ها...
                          </TableCell>
                        </TableRow>
                      ) : settlements.list.length === 0 ? (
                        <TableRow>
                          <TableCell colSpan={selectedPersonnelId ? 5 : 6} className="py-8 text-center text-xs text-slate-500">
                            <ReceiptText aria-hidden="true" className="mx-auto mb-2 size-7 text-slate-300" />
                            پرداختی برای نمایش ثبت نشده است.
                          </TableCell>
                        </TableRow>
                      ) : (
                        settlements.list.map((settlement) => (
                          <TableRow key={settlement.id} className="h-10">
                            {!selectedPersonnelId && (
                              <TableCell className="px-3 py-2 text-xs">
                                {personnelNames.get(settlement.personnelId) ?? `پرسنل ${settlement.personnelId}`}
                              </TableCell>
                            )}
                            <TableCell className="whitespace-nowrap px-3 py-2 text-xs" dir="ltr">
                              {paidAtLabel(settlement.paidAt)}
                            </TableCell>
                            <TableCell className="px-3 py-2 text-xs">{settlement.paymentMethod}</TableCell>
                            <TableCell className="whitespace-nowrap px-3 py-2 text-xs font-semibold">
                              {amountLabel(settlement.amount)}
                            </TableCell>
                            <TableCell className="px-3 py-2 text-xs">{earningTypeLabel(settlement.earningType)}</TableCell>
                            <TableCell className="max-w-40 truncate px-3 py-2 text-xs text-slate-500">
                              {settlement.description || "—"}
                            </TableCell>
                          </TableRow>
                        ))
                      )}
                    </TableBody>
                  </Table>
                </div>
                {totalPages > 1 && (
                  <div className="flex items-center justify-between border-t border-slate-100 px-3 py-2 text-[10px] text-slate-600">
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      disabled={page <= 1 || settlementsLoading}
                      onClick={() => setPage((current) => Math.max(1, current - 1))}
                      className="h-7 rounded-lg px-2 text-[10px]"
                    >
                      قبلی
                    </Button>
                    <span>
                      صفحه {new Intl.NumberFormat("fa-IR").format(page)} از {new Intl.NumberFormat("fa-IR").format(totalPages)}
                    </span>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      disabled={page >= totalPages || settlementsLoading}
                      onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
                      className="h-7 rounded-lg px-2 text-[10px]"
                    >
                      بعدی
                    </Button>
                  </div>
                )}
              </section>

              <p className="rounded-lg border border-fuchsia-100 bg-fuchsia-50/60 px-3 py-2 text-[10px] leading-5 text-slate-600">
                مطالبات متفرقه در API فعلی Finance ثبت نمی‌شود؛ این صفحه فقط حقوق ثابت و پورسانت خدمات را بر اساس قرارداد و پذیرش محاسبه می‌کند.
              </p>
            </div>
          </main>
        </div>
      </div>
    </div>
  );
}
