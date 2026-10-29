"use client";

import Link from "next/link";
import { useMemo, useState, type ReactNode } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import {
  Ban,
  Banknote,
  CalendarDays,
  CheckCircle2,
  Clock3,
  Eye,
  LoaderCircle,
  Plus,
  RefreshCw,
  Search,
  UsersRound,
} from "lucide-react";
import { toJalaali } from "jalaali-js";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";
import { useReceptions } from "./use-receptions";
import { ReceptionDetailsDialog } from "./reception-details-dialog";
import { ReceptionService, type ReceptionStatus } from "./reception.service";
import { handleApiError } from "@/lib/api/handle-api-error";
import type { ReceptionListItem } from "./reception.types";

const PAGE_SIZE = 10;

const STATUS_META: Record<ReceptionStatus, { label: string; tone: string }> = {
  1: { label: "در انتظار", tone: "amber" },
  2: { label: "در حال انجام", tone: "fuchsia" },
  3: { label: "تکمیل‌شده", tone: "emerald" },
  4: { label: "لغوشده", tone: "slate" },
};

function amountLabel(amount: number) {
  return `${new Intl.NumberFormat("fa-IR").format(amount)} تومان`;
}

function tehranDateKey(value: string) {
  const date = new Date(value);
  if (!Number.isFinite(date.getTime())) return "";
  const parts = new Intl.DateTimeFormat("en-US-u-ca-gregory", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(date);
  const part = (type: Intl.DateTimeFormatPartTypes) =>
    Number(parts.find((item) => item.type === type)?.value);
  const jalali = toJalaali(part("year"), part("month"), part("day"));
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`;
}

function todayJalali() {
  return tehranDateKey(new Date().toISOString());
}

function formatReceptionDate(value: string) {
  const date = new Date(value);
  if (!Number.isFinite(date.getTime())) return "—";
  return new Intl.DateTimeFormat("fa-IR", {
    timeZone: "Asia/Tehran",
    dateStyle: "short",
    timeStyle: "short",
  }).format(date);
}

function StatusBadge({ status }: { status: number }) {
  const meta = STATUS_META[status as ReceptionStatus] ?? { label: "نامشخص", tone: "slate" };
  const colors: Record<string, string> = {
    amber: "bg-amber-50 text-amber-700 ring-amber-200",
    fuchsia: "bg-fuchsia-50 text-fuchsia-700 ring-fuchsia-200",
    emerald: "bg-emerald-50 text-emerald-700 ring-emerald-200",
    slate: "bg-slate-100 text-slate-600 ring-slate-200",
  };
  return (
    <span className={`inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-1 text-[11px] font-medium ring-1 ring-inset ${colors[meta.tone]}`}>
      <span className={`size-1.5 rounded-full ${
        meta.tone === "amber" ? "bg-amber-500" :
          meta.tone === "fuchsia" ? "bg-fuchsia-500" :
            meta.tone === "emerald" ? "bg-emerald-500" : "bg-slate-400"
      }`} />
      {meta.label}
    </span>
  );
}

function ReceptionDatePicker({
  value,
  onChange,
}: {
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
      format="YYYY/MM/DD"
      value={value}
      editable={false}
      calendarPosition="bottom-start"
      portal
      zIndex={160}
      onChange={(date: DateObject | null) => onChange(date?.format("YYYY/MM/DD") ?? value)}
      render={(_displayValue, openCalendar) => (
        <Button
          type="button"
          variant="outline"
          onClick={openCalendar}
          className="h-10 min-w-[165px] justify-between rounded-xl border-slate-200 bg-white px-3 text-xs text-slate-700"
          aria-label="انتخاب تاریخ پذیرش"
        >
          <CalendarDays className="size-4 text-slate-500" />
          {value.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)])}
        </Button>
      )}
    />
  );
}

export function ReceptionsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [selectedDate, setSelectedDate] = useState(todayJalali);
  const [statusFilter, setStatusFilter] = useState<number | null>(null);
  const [receptionistFilter, setReceptionistFilter] = useState("");
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [detailsOpen, setDetailsOpen] = useState(false);
  const [busyId, setBusyId] = useState<number | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const { data, error, loading } = useReceptions(refreshKey);
  const isToday = selectedDate === todayJalali();

  const dateReceptions = useMemo(
    () => (data ?? []).filter((reception) => tehranDateKey(reception.receptionDate) === selectedDate),
    [data, selectedDate],
  );
  const statusCounts = useMemo(() => ({
    all: dateReceptions.length,
    open: dateReceptions.filter((item) => item.status === 1).length,
    inProgress: dateReceptions.filter((item) => item.status === 2).length,
    completed: dateReceptions.filter((item) => item.status === 3).length,
  }), [dateReceptions]);
  const receptionistIds = useMemo(
    () => [...new Set(dateReceptions.map((item) => item.receptionistId))].sort((a, b) => a - b),
    [dateReceptions],
  );
  const filtered = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase("fa");
    return dateReceptions.filter((reception) => {
      const matchesQuery = !normalizedQuery ||
        `${reception.receptionNumber} ${reception.customerName}`
          .toLocaleLowerCase("fa")
          .includes(normalizedQuery);
      const matchesStatus = statusFilter === null || reception.status === statusFilter;
      const matchesReceptionist = !receptionistFilter ||
        reception.receptionistId === Number(receptionistFilter);
      return matchesQuery && matchesStatus && matchesReceptionist;
    });
  }, [dateReceptions, query, receptionistFilter, statusFilter]);
  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const visibleItems = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);
  const paidTotal = dateReceptions.reduce((sum, item) => sum + item.paidAmount, 0);

  function resetPage() {
    setPage(1);
  }

  async function changeReceptionStatus(reception: ReceptionListItem, status: ReceptionStatus) {
    const label = STATUS_META[status].label;
    const confirmation = status === 4
      ? `پذیرش ${reception.receptionNumber} لغو شود؟ اگر پرداخت موفق یا گردش انبار برگشت‌ناپذیر ثبت شده باشد، سرور لغو را رد می‌کند.`
      : `وضعیت پذیرش ${reception.receptionNumber} به «${label}» تغییر کند؟`;
    if (!window.confirm(confirmation)) return;
    setBusyId(reception.id);
    setActionError(null);
    setNotice(null);
    try {
      const response = await new ReceptionService().changeStatus(
        reception.id,
        reception.salonId,
        status,
      );
      if (!response.data) throw new Error(response.message ?? "تغییر وضعیت پذیرش تأیید نشد.");
      setNotice(`وضعیت پذیرش ${reception.receptionNumber} تغییر کرد.`);
      setRefreshKey((key) => key + 1);
    } catch (cause: unknown) {
      setActionError(handleApiError(cause));
    } finally {
      setBusyId(null);
    }
  }

  function openDetails(reception: ReceptionListItem) {
    setDetailsId(reception.id);
    setDetailsOpen(true);
  }

  return (
    <div dir="rtl" className="min-h-screen bg-slate-50">
      <section className="h-[150px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)] sm:h-[165px]">
        <div className="mx-auto w-[94%] pt-3 sm:w-[90%] sm:max-w-[1320px]">
          <div className="flex items-center justify-between border-b border-fuchsia-900/70 pb-3 text-xs text-white/70">
            <span>مدیریت پذیرش / {isToday ? "پذیرش‌های امروز" : `پذیرش‌های ${selectedDate.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)])}`}</span>
            <span className="hidden sm:inline">سامانه مدیریت سالن زیبایی</span>
          </div>
          <div className="mt-4 flex items-center justify-between gap-3">
            <div>
              <p className="text-xs font-medium text-fuchsia-300">صف پذیرش</p>
              <h1 className="mt-1 text-xl font-bold text-white sm:text-2xl">
                {isToday ? "پذیرش‌های امروز" : `پذیرش‌های ${selectedDate.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)])}`}
              </h1>
            </div>
            <Button
              render={<Link href="/receptions/new" />}
              className="h-10 shrink-0 rounded-xl bg-fuchsia-700 px-4 text-white shadow-lg shadow-fuchsia-950/30 hover:bg-fuchsia-600"
            >
              <Plus className="size-4" />
              پذیرش جدید
            </Button>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[48px] w-[94%] max-w-[1320px] pb-10 sm:-mt-[46px] sm:w-[90%]">
        <section className="rounded-2xl border border-slate-200 bg-white p-3 shadow-[0_12px_40px_-24px_rgba(30,10,40,0.35)] sm:p-5">
          <div className="flex flex-col gap-3 xl:flex-row xl:items-center xl:justify-between">
            <div className="flex flex-wrap items-center gap-2">
              <div className="relative min-w-[220px] flex-1 sm:w-[300px] sm:flex-none">
                <Search className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-slate-400" />
                <Input
                  aria-label="جستجوی پذیرش"
                  value={query}
                  onChange={(event) => {
                    setQuery(event.target.value);
                    resetPage();
                  }}
                  placeholder="جستجوی نام مشتری یا کد پذیرش..."
                  className="h-10 rounded-xl border-slate-200 bg-white pe-3 ps-9 text-xs placeholder:text-slate-400"
                />
              </div>
              <select
                aria-label="فیلتر پذیرشگر"
                value={receptionistFilter}
                onChange={(event) => {
                  setReceptionistFilter(event.target.value);
                  resetPage();
                }}
                className="h-10 min-w-[135px] rounded-xl border border-slate-200 bg-white px-3 text-xs text-slate-700 outline-none focus-visible:border-fuchsia-400 focus-visible:ring-2 focus-visible:ring-fuchsia-100"
              >
                <option value="">همه پذیرشگرها</option>
                {receptionistIds.map((id) => (
                  <option key={id} value={id}>پذیرشگر {new Intl.NumberFormat("fa-IR").format(id)}</option>
                ))}
              </select>
              <Button
                variant="outline"
                onClick={() => setRefreshKey((key) => key + 1)}
                disabled={loading}
                className="h-10 rounded-xl border-slate-200 px-3 text-xs text-slate-600"
                aria-label="تازه‌سازی فهرست"
              >
                <RefreshCw className={`ml-1.5 size-3.5 ${loading ? "animate-spin" : ""}`} />
                تازه‌سازی
              </Button>
            </div>
            <div className="flex flex-wrap items-center gap-2">
              <span className="rounded-full bg-emerald-50 px-3 py-1.5 text-[11px] font-medium text-emerald-700">
                دریافت از سرور
                <span className="mr-2 inline-block size-1.5 rounded-full bg-emerald-500" />
              </span>
              <ReceptionDatePicker
                value={selectedDate}
                onChange={(date) => {
                  setSelectedDate(date);
                  resetPage();
                }}
              />
            </div>
          </div>

          <div className="mt-4 flex overflow-x-auto rounded-xl border border-slate-200 bg-slate-50/70 sm:w-fit">
            {([
              { value: null, label: "همه", count: statusCounts.all },
              { value: 1, label: "در انتظار", count: statusCounts.open },
              { value: 2, label: "در حال انجام", count: statusCounts.inProgress },
              { value: 3, label: "تکمیل‌شده", count: statusCounts.completed },
              { value: 4, label: "لغوشده", count: dateReceptions.filter((item) => item.status === 4).length },
            ] as const).map((filter) => (
              <button
                key={filter.value ?? "all"}
                type="button"
                aria-pressed={statusFilter === filter.value}
                onClick={() => {
                  setStatusFilter(filter.value);
                  resetPage();
                }}
                className={`flex shrink-0 items-center gap-2 border-l border-slate-200 px-3 py-2.5 text-[11px] transition last:border-l-0 sm:px-4 ${
                  statusFilter === filter.value
                    ? "bg-fuchsia-700 font-semibold text-white"
                    : "text-slate-600 hover:bg-white"
                }`}
              >
                {filter.label}
                <span className={`rounded-full px-1.5 py-0.5 text-[10px] ${
                  statusFilter === filter.value ? "bg-white/20 text-white" : "bg-slate-200/80 text-slate-600"
                }`}>
                  {new Intl.NumberFormat("fa-IR").format(filter.count)}
                </span>
              </button>
            ))}
          </div>

          <div className="mt-4 grid grid-cols-2 gap-3 xl:grid-cols-4">
            <SummaryCard
              label="در صف انتظار"
              value={statusCounts.open}
              icon={<UsersRound className="size-5" />}
              tone="amber"
            />
            <SummaryCard
              label="در حال انجام"
              value={statusCounts.inProgress}
              icon={<Clock3 className="size-5" />}
              tone="fuchsia"
            />
            <SummaryCard
              label="تکمیل‌شده"
              value={statusCounts.completed}
              icon={<CheckCircle2 className="size-5" />}
              tone="emerald"
            />
            <SummaryCard
              label="مبلغ پرداخت‌شده"
              value={amountLabel(paidTotal)}
              icon={<Banknote className="size-5" />}
              tone="blue"
              isAmount
            />
          </div>

          {(actionError || notice) && (
            <div
              role={actionError ? "alert" : "status"}
              className={`mt-4 flex items-center justify-between rounded-xl px-4 py-2.5 text-xs ${
                actionError
                  ? "border border-rose-200 bg-rose-50 text-rose-700"
                  : "border border-emerald-200 bg-emerald-50 text-emerald-700"
              }`}
            >
              <span>{actionError ?? notice}</span>
              <button
                type="button"
                onClick={() => {
                  setActionError(null);
                  setNotice(null);
                }}
                aria-label="بستن پیام"
                className="px-2 text-base"
              >
                ×
              </button>
            </div>
          )}

          <div className="mt-5 overflow-hidden rounded-xl border border-slate-200">
            {error ? (
              <div className="flex min-h-56 flex-col items-center justify-center gap-3 px-5 text-center">
                <p className="text-sm leading-6 text-rose-700" role="alert">{error}</p>
                <Button variant="outline" onClick={() => setRefreshKey((key) => key + 1)} className="rounded-xl">
                  تلاش دوباره
                </Button>
              </div>
            ) : loading ? (
              <div className="flex min-h-56 items-center justify-center gap-3 text-sm text-slate-500">
                <LoaderCircle className="size-5 animate-spin text-fuchsia-700" />
                در حال دریافت پذیرش‌ها...
              </div>
            ) : (
              <>
                <div className="hidden overflow-x-auto lg:block">
                  <table className="w-full min-w-[1050px] text-right">
                    <thead className="bg-slate-50 text-[11px] font-semibold text-slate-500">
                      <tr>
                        <th className="px-4 py-3">شماره صف</th>
                        <th className="px-4 py-3">مشتری</th>
                        <th className="px-4 py-3">تعرفه</th>
                        <th className="px-4 py-3">پذیرشگر</th>
                        <th className="px-4 py-3">ساعت ورود</th>
                        <th className="px-4 py-3">مبلغ کل</th>
                        <th className="px-4 py-3">پرداخت‌شده</th>
                        <th className="px-4 py-3">وضعیت</th>
                        <th className="px-4 py-3">عملیات</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {visibleItems.length === 0 ? (
                        <tr>
                          <td colSpan={9} className="py-14 text-center text-sm text-slate-500">
                            {data?.length === 0
                              ? "پذیرشی برای نمایش وجود ندارد."
                              : "برای این تاریخ و فیلترها پذیرشی پیدا نشد."}
                          </td>
                        </tr>
                      ) : visibleItems.map((reception, index) => (
                        <tr key={reception.id} className="text-xs text-slate-700 transition-colors hover:bg-fuchsia-50/30">
                          <td className="px-4 py-3.5">
                            <span className={`inline-flex min-w-10 justify-center rounded-lg px-2 py-1.5 font-bold ${
                              reception.status === 3 ? "bg-emerald-50 text-emerald-700" :
                                reception.status === 2 ? "bg-fuchsia-50 text-fuchsia-700" :
                                  "bg-amber-50 text-amber-700"
                            }`}>
                              {new Intl.NumberFormat("fa-IR").format((page - 1) * PAGE_SIZE + index + 1)}
                            </span>
                          </td>
                          <td className="px-4 py-3.5">
                            <p className="font-semibold text-slate-800">{reception.customerName}</p>
                            <p className="mt-1 text-[10px] text-slate-400" dir="ltr">{reception.receptionNumber}</p>
                          </td>
                          <td className="px-4 py-3.5">
                            {reception.serviceTariffId
                              ? `تعرفه ${new Intl.NumberFormat("fa-IR").format(reception.serviceTariffId)}`
                              : "—"}
                          </td>
                          <td className="px-4 py-3.5">
                            {new Intl.NumberFormat("fa-IR").format(reception.receptionistId)}
                          </td>
                          <td className="px-4 py-3.5 text-slate-500">{formatReceptionDate(reception.receptionDate)}</td>
                          <td className="px-4 py-3.5 font-medium">{amountLabel(reception.totalAmount)}</td>
                          <td className="px-4 py-3.5 text-emerald-700">{amountLabel(reception.paidAmount)}</td>
                          <td className="px-4 py-3.5"><StatusBadge status={reception.status} /></td>
                          <td className="px-4 py-3.5">
                            <ReceptionActions
                              reception={reception}
                              busy={busyId === reception.id}
                              onDetails={openDetails}
                              onChangeStatus={changeReceptionStatus}
                            />
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                <div className="divide-y divide-slate-100 lg:hidden">
                  {visibleItems.length === 0 ? (
                    <div className="px-4 py-12 text-center text-sm text-slate-500">
                      برای این تاریخ و فیلترها پذیرشی پیدا نشد.
                    </div>
                  ) : visibleItems.map((reception) => (
                    <article key={reception.id} className="space-y-3 p-4">
                      <div className="flex items-start justify-between gap-3">
                        <div>
                          <p className="font-semibold text-slate-800">{reception.customerName}</p>
                          <p dir="ltr" className="mt-1 text-right text-[10px] text-slate-400">{reception.receptionNumber}</p>
                        </div>
                        <StatusBadge status={reception.status} />
                      </div>
                      <div className="grid grid-cols-2 gap-2 text-xs">
                        <MobileValue label="زمان ورود" value={formatReceptionDate(reception.receptionDate)} />
                        <MobileValue label="پذیرشگر" value={new Intl.NumberFormat("fa-IR").format(reception.receptionistId)} />
                        <MobileValue label="مبلغ کل" value={amountLabel(reception.totalAmount)} />
                        <MobileValue label="پرداخت‌شده" value={amountLabel(reception.paidAmount)} />
                      </div>
                      <ReceptionActions
                        reception={reception}
                        busy={busyId === reception.id}
                        onDetails={openDetails}
                        onChangeStatus={changeReceptionStatus}
                      />
                    </article>
                  ))}
                </div>

                {filtered.length > 0 && (
                  <div className="flex flex-col-reverse items-center justify-between gap-3 border-t border-slate-100 px-4 py-3 sm:flex-row">
                    <span className="text-[11px] text-slate-500">
                      نمایش {new Intl.NumberFormat("fa-IR").format((page - 1) * PAGE_SIZE + 1)} تا{" "}
                      {new Intl.NumberFormat("fa-IR").format(Math.min(page * PAGE_SIZE, filtered.length))} از{" "}
                      {new Intl.NumberFormat("fa-IR").format(filtered.length)} پذیرش
                    </span>
                    {totalPages > 1 && (
                      <Pagination className="mx-0 w-auto">
                        <PaginationContent className="gap-1">
                          <PaginationItem>
                            <PaginationPrevious
                              href="#"
                              aria-label="صفحه قبل"
                              onClick={(event) => {
                                event.preventDefault();
                                setPage((current) => Math.max(1, current - 1));
                              }}
                              className={`h-8 rounded-lg border border-slate-200 px-2 ${page === 1 ? "pointer-events-none opacity-40" : ""}`}
                            />
                          </PaginationItem>
                          {Array.from({ length: totalPages }, (_, index) => index + 1)
                            .filter((pageNumber) =>
                              pageNumber === 1 || pageNumber === totalPages || Math.abs(pageNumber - page) <= 1,
                            )
                            .map((pageNumber, index, pages) => (
                              <PaginationItem key={pageNumber}>
                                {index > 0 && pages[index - 1] !== pageNumber - 1
                                  ? <span className="px-1 text-slate-400">…</span>
                                  : null}
                                <PaginationLink
                                  href="#"
                                  isActive={pageNumber === page}
                                  onClick={(event) => {
                                    event.preventDefault();
                                    setPage(pageNumber);
                                  }}
                                  className={`size-8 rounded-lg text-xs ${pageNumber === page ? "border-fuchsia-700 bg-fuchsia-700 text-white hover:bg-fuchsia-800 hover:text-white" : "border border-slate-200"}`}
                                >
                                  {new Intl.NumberFormat("fa-IR").format(pageNumber)}
                                </PaginationLink>
                              </PaginationItem>
                            ))}
                          <PaginationItem>
                            <PaginationNext
                              href="#"
                              aria-label="صفحه بعد"
                              onClick={(event) => {
                                event.preventDefault();
                                setPage((current) => Math.min(totalPages, current + 1));
                              }}
                              className={`h-8 rounded-lg border border-slate-200 px-2 ${page === totalPages ? "pointer-events-none opacity-40" : ""}`}
                            />
                          </PaginationItem>
                        </PaginationContent>
                      </Pagination>
                    )}
                  </div>
                )}
              </>
            )}
          </div>
        </section>
      </main>

      <ReceptionDetailsDialog
        key={`${detailsOpen}-${detailsId ?? "none"}`}
        receptionId={detailsId}
        open={detailsOpen}
        onOpenChange={setDetailsOpen}
      />
    </div>
  );
}

function SummaryCard({
  label,
  value,
  icon,
  tone,
  isAmount = false,
}: {
  label: string;
  value: number | string;
  icon: ReactNode;
  tone: "amber" | "fuchsia" | "emerald" | "blue";
  isAmount?: boolean;
}) {
  const tones = {
    amber: "border-amber-200 bg-gradient-to-br from-amber-50 to-white text-amber-600",
    fuchsia: "border-fuchsia-200 bg-gradient-to-br from-fuchsia-50 to-white text-fuchsia-700",
    emerald: "border-emerald-200 bg-gradient-to-br from-emerald-50 to-white text-emerald-700",
    blue: "border-sky-200 bg-gradient-to-br from-sky-50 to-white text-sky-700",
  };
  return (
    <div className={`flex min-h-[78px] items-center justify-between gap-2 rounded-xl border px-3 py-3 sm:px-4 ${tones[tone]}`}>
      <div className="min-w-0">
        <p className="truncate text-[10px] font-medium opacity-80 sm:text-xs">{label}</p>
        <p className={`mt-1 truncate font-bold ${isAmount ? "text-sm sm:text-base" : "text-xl"}`}>
          {typeof value === "number" ? new Intl.NumberFormat("fa-IR").format(value) : value}
        </p>
      </div>
      <span className="shrink-0 opacity-80">{icon}</span>
    </div>
  );
}

function MobileValue({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg bg-slate-50 px-2.5 py-2">
      <p className="text-[10px] text-slate-400">{label}</p>
      <p className="mt-1 break-words font-medium text-slate-700">{value}</p>
    </div>
  );
}

function ReceptionActions({
  reception,
  busy,
  onDetails,
  onChangeStatus,
}: {
  reception: ReceptionListItem;
  busy: boolean;
  onDetails: (reception: ReceptionListItem) => void;
  onChangeStatus: (reception: ReceptionListItem, status: ReceptionStatus) => void;
}) {
  return (
    <div className="flex flex-wrap items-center gap-2">
      <Button
        type="button"
        variant="outline"
        size="sm"
        onClick={() => onDetails(reception)}
        className="h-8 rounded-lg border-fuchsia-200 px-2.5 text-[11px] text-fuchsia-800 hover:bg-fuchsia-50"
        aria-label={`مشاهده جزئیات پذیرش ${reception.receptionNumber}`}
      >
        <Eye className="ml-1 size-3.5" />
        جزئیات
      </Button>
      {reception.status === 1 && (
        <Button
          type="button"
          size="sm"
          disabled={busy}
          onClick={() => onChangeStatus(reception, 2)}
          className="h-8 rounded-lg bg-fuchsia-700 px-2.5 text-[11px] text-white hover:bg-fuchsia-800"
          aria-label={`شروع پذیرش ${reception.receptionNumber}`}
        >
          {busy ? <LoaderCircle className="ml-1 size-3.5 animate-spin" /> : <Clock3 className="ml-1 size-3.5" />}
          شروع
        </Button>
      )}
      {reception.status === 2 && (
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={busy}
          onClick={() => onChangeStatus(reception, 3)}
          className="h-8 rounded-lg border-emerald-200 px-2.5 text-[11px] text-emerald-700 hover:bg-emerald-50"
          aria-label={`تکمیل پذیرش ${reception.receptionNumber}`}
        >
          {busy ? <LoaderCircle className="ml-1 size-3.5 animate-spin" /> : <CheckCircle2 className="ml-1 size-3.5" />}
          تکمیل
        </Button>
      )}
      {(reception.status === 1 || reception.status === 2) && (
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={busy}
          onClick={() => onChangeStatus(reception, 4)}
          className="h-8 rounded-lg border-rose-200 px-2.5 text-[11px] text-rose-700 hover:bg-rose-50"
          aria-label={`لغو پذیرش ${reception.receptionNumber}`}
        >
          <Ban className="ml-1 size-3.5" />
          لغو
        </Button>
      )}
    </div>
  );
}
