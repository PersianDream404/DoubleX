"use client";

import { useMemo, useState, type FormEvent, type ReactNode } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import {
  Banknote,
  CalendarDays,
  ChartNoAxesCombined,
  ClipboardList,
  Eye,
  FlaskConical,
  HandCoins,
  LoaderCircle,
  RefreshCw,
  Scissors,
  UsersRound,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ReceptionDetailsDialog } from "../list/reception-details-dialog";
import {
  currentJalaliDate,
  jalaliDateLabel,
  toApiDateTime,
  toApiExclusiveEndDate,
} from "./reception-report-date";
import { receptionReportFilterSchema } from "./reception-report.schema";
import type { ReceptionReportTab } from "./reception-report.service";
import type {
  ReceptionReportDebt,
  ReceptionReportDiscount,
  ReceptionReportFilters,
  ReceptionReportMaterial,
  ReceptionReportPersonnel,
  ReceptionReportReception,
  ReceptionReportServiceItem,
} from "./reception-report.types";
import { useReceptionReports } from "./use-reception-reports";

type ReportFormState = {
  fromDate: string;
  toDate: string;
  customerSearch: string;
  customerId: string;
  receptionNumber: string;
  personnelId: string;
  serviceId: string;
  materialId: string;
  receptionStatus: string;
  financialStatus: string;
  serviceStatus: string;
  hasAppointment: string;
  minimumRemainingAmount: string;
  maximumRemainingAmount: string;
};

const TAB_ITEMS: { id: ReceptionReportTab; label: string; icon: typeof ChartNoAxesCombined }[] = [
  { id: "summary", label: "خلاصه", icon: ChartNoAxesCombined },
  { id: "receptions", label: "پذیرش‌ها", icon: ClipboardList },
  { id: "personnel", label: "پرسنل", icon: UsersRound },
  { id: "services", label: "خدمات", icon: Scissors },
  { id: "debts", label: "بدهی‌ها", icon: HandCoins },
  { id: "discounts", label: "تخفیف‌ها", icon: Banknote },
  { id: "materials", label: "مواد مصرفی", icon: FlaskConical },
];

const RECEPTION_STATUS: Record<number, string> = {
  1: "در انتظار",
  2: "در حال انجام",
  3: "تکمیل‌شده",
  4: "لغوشده",
};

const FINANCIAL_STATUS: Record<number, string> = {
  1: "تسویه‌نشده",
  2: "تسویه‌جزئی",
  3: "تسویه‌شده",
};

const SERVICE_STATUS: Record<number, string> = {
  1: "در انتظار",
  2: "در حال انجام",
  3: "تکمیل‌شده",
  4: "لغوشده",
};

function initialFormValues(): ReportFormState {
  const today = currentJalaliDate();
  return {
    fromDate: `${today.slice(0, 4)}/${today.slice(5, 7)}/01`,
    toDate: today,
    customerSearch: "",
    customerId: "",
    receptionNumber: "",
    personnelId: "",
    serviceId: "",
    materialId: "",
    receptionStatus: "",
    financialStatus: "",
    serviceStatus: "",
    hasAppointment: "",
    minimumRemainingAmount: "",
    maximumRemainingAmount: "",
  };
}

function formatAmount(value: number | null | undefined) {
  if (value === null || value === undefined) return "—";
  return `${new Intl.NumberFormat("fa-IR", { maximumFractionDigits: 2 }).format(value)} تومان`;
}

function formatNumber(value: number | null | undefined) {
  return value === null || value === undefined
    ? "—"
    : new Intl.NumberFormat("fa-IR", { maximumFractionDigits: 3 }).format(value);
}

function formatDate(value: string) {
  const date = new Date(value);
  if (!Number.isFinite(date.getTime())) return "—";
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    timeZone: "Asia/Tehran",
    dateStyle: "short",
  }).format(date);
}

function amountTick(value: number) {
  return new Intl.NumberFormat("fa-IR", { notation: "compact" }).format(value);
}

function statusBadge(status: number, labels: Record<number, string>) {
  const label = labels[status] ?? "نامشخص";
  return (
    <Badge
      variant="secondary"
      className={
        status === 3 || status === 2
          ? "bg-emerald-100 text-emerald-800 hover:bg-emerald-100"
          : status === 4
            ? "bg-slate-100 text-slate-600 hover:bg-slate-100"
            : "bg-amber-100 text-amber-800 hover:bg-amber-100"
      }
    >
      {label}
    </Badge>
  );
}

function ReportDatePicker({
  id,
  label,
  value,
  onChange,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <div className="grid gap-1.5">
      <Label htmlFor={id}>{label}</Label>
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
              value={value.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)])}
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

function FilterInput({
  id,
  label,
  value,
  onChange,
  type = "text",
  placeholder,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: "text" | "number";
  placeholder?: string;
}) {
  return (
    <div className="grid gap-1.5">
      <Label htmlFor={id}>{label}</Label>
      <Input
        id={id}
        type={type}
        min={type === "number" ? 0 : undefined}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        placeholder={placeholder}
        className="h-9 border-slate-200 bg-white text-sm"
      />
    </div>
  );
}

function FilterSelect({
  id,
  label,
  value,
  onChange,
  options,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  options: { value: string; label: string }[];
}) {
  return (
    <div className="grid gap-1.5">
      <Label htmlFor={id}>{label}</Label>
      <select
        id={id}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="h-9 w-full rounded-3xl border border-slate-200 bg-white px-3 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
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
  icon: typeof ChartNoAxesCombined;
  tone: string;
}) {
  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
      <div className="flex items-center justify-between gap-3">
        <p className="text-xs text-slate-500">{title}</p>
        <span className={`flex size-9 items-center justify-center rounded-xl ${tone}`}>
          <Icon className="size-4" />
        </span>
      </div>
      <p className="mt-3 text-lg font-bold text-slate-900">{value}</p>
    </article>
  );
}

function TotalsBar({
  items,
}: {
  items: { label: string; value: string }[];
}) {
  return (
    <div className="grid gap-3 border-b border-slate-100 bg-slate-50/70 p-3 sm:grid-cols-2 lg:grid-cols-4">
      {items.map((item) => (
        <div key={item.label}>
          <p className="text-[11px] text-slate-500">{item.label}</p>
          <p className="mt-1 text-xs font-bold text-slate-800">{item.value}</p>
        </div>
      ))}
    </div>
  );
}

function TableMessage({
  colSpan,
  children,
  alert = false,
}: {
  colSpan: number;
  children: ReactNode;
  alert?: boolean;
}) {
  return (
    <TableRow>
      <TableCell
        colSpan={colSpan}
        role={alert ? "alert" : undefined}
        className={`py-10 text-center text-sm ${alert ? "text-rose-700" : "text-slate-500"}`}
      >
        {children}
      </TableCell>
    </TableRow>
  );
}

function ActionButton({ onClick }: { onClick: () => void }) {
  return (
    <Button
      type="button"
      variant="ghost"
      size="icon"
      aria-label="مشاهده جزئیات پذیرش"
      onClick={onClick}
      className="size-8 text-fuchsia-700 hover:bg-fuchsia-50 hover:text-fuchsia-800"
    >
      <Eye className="size-4" />
    </Button>
  );
}

export function ReceptionReportsPage() {
  const [form, setForm] = useState<ReportFormState>(initialFormValues);
  const [filters, setFilters] = useState<ReceptionReportFilters>(() => {
    const defaults = initialFormValues();
    return {
      fromDate: toApiDateTime(defaults.fromDate),
      toDateExclusive: toApiExclusiveEndDate(defaults.toDate),
    };
  });
  const [activeTab, setActiveTab] = useState<ReceptionReportTab>("summary");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formError, setFormError] = useState<string | null>(null);
  const [selectedReceptionId, setSelectedReceptionId] = useState<number | null>(null);
  const [detailsOpen, setDetailsOpen] = useState(false);
  const {
    summary,
    summaryLoading,
    summaryError,
    report,
    reportLoading,
    reportError,
  } = useReceptionReports(activeTab, filters, page, refreshKey);

  const activeReport = report?.kind === activeTab ? report : null;
  const reportPage = activeReport?.result.page ?? null;
  const totalPages = Math.max(1, reportPage?.pagination.totalPages ?? 1);
  const currentPage = reportPage?.pagination.pageNumber ?? page;
  const pageNumbers = useMemo(() => {
    const start = Math.max(1, Math.min(currentPage - 1, totalPages - 2));
    return Array.from({ length: Math.min(totalPages, 3) }, (_, index) => start + index);
  }, [currentPage, totalPages]);

  function updateForm<K extends keyof ReportFormState>(
    key: K,
    value: ReportFormState[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function parsedFilters(values: ReportFormState): ReceptionReportFilters | null {
    const result = receptionReportFilterSchema.safeParse(values);
    if (!result.success) {
      setFormError(result.error.issues[0]?.message ?? "فیلترهای گزارش معتبر نیستند.");
      return null;
    }

    const parsed = result.data;
    return {
      fromDate: toApiDateTime(parsed.fromDate),
      toDateExclusive: toApiExclusiveEndDate(parsed.toDate),
      ...(parsed.customerSearch ? { customerSearch: parsed.customerSearch } : {}),
      ...(parsed.customerId !== undefined ? { customerId: parsed.customerId } : {}),
      ...(parsed.receptionNumber !== undefined
        ? { receptionNumber: parsed.receptionNumber }
        : {}),
      ...(parsed.personnelId !== undefined ? { personnelId: parsed.personnelId } : {}),
      ...(parsed.serviceId !== undefined ? { serviceId: parsed.serviceId } : {}),
      ...(parsed.materialId !== undefined ? { materialId: parsed.materialId } : {}),
      ...(parsed.receptionStatus !== undefined
        ? { receptionStatus: parsed.receptionStatus }
        : {}),
      ...(parsed.financialStatus !== undefined
        ? { financialStatus: parsed.financialStatus }
        : {}),
      ...(parsed.serviceStatus !== undefined
        ? { serviceStatus: parsed.serviceStatus }
        : {}),
      ...(parsed.hasAppointment
        ? { hasAppointment: parsed.hasAppointment === "true" }
        : {}),
      ...(parsed.minimumRemainingAmount !== undefined
        ? { minimumRemainingAmount: parsed.minimumRemainingAmount }
        : {}),
      ...(parsed.maximumRemainingAmount !== undefined
        ? { maximumRemainingAmount: parsed.maximumRemainingAmount }
        : {}),
    };
  }

  function applyFilters(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const nextFilters = parsedFilters(form);
    if (!nextFilters) return;
    setFormError(null);
    setFilters(nextFilters);
    setPage(1);
  }

  function resetFilters() {
    const defaults = initialFormValues();
    const nextFilters = parsedFilters(defaults);
    if (!nextFilters) return;
    setForm(defaults);
    setFormError(null);
    setFilters(nextFilters);
    setPage(1);
  }

  function openReceptionDetails(id: number) {
    setSelectedReceptionId(id);
    setDetailsOpen(true);
  }

  function renderTableRows() {
    const colSpan = Math.max(1, renderTableHeaders().length);
    if (reportLoading) {
      return <TableMessage colSpan={colSpan}>در حال دریافت گزارش...</TableMessage>;
    }
    if (reportError) {
      return <TableMessage colSpan={colSpan} alert>{reportError}</TableMessage>;
    }
    if (!reportPage || reportPage.list.length === 0) {
      return <TableMessage colSpan={colSpan}>برای فیلترهای انتخاب‌شده گزارشی یافت نشد.</TableMessage>;
    }

    switch (activeTab) {
      case "receptions":
        return (reportPage.list as ReceptionReportReception[]).map((item) => (
          <TableRow key={item.id}>
            <TableCell>{formatNumber(item.receptionNumber)}</TableCell>
            <TableCell>{item.customerName}</TableCell>
            <TableCell dir="ltr" className="text-start">{item.customerMobilePhone ?? "—"}</TableCell>
            <TableCell>{formatDate(item.receptionDate)}</TableCell>
            <TableCell>{formatNumber(item.serviceCount)}</TableCell>
            <TableCell>{formatAmount(item.serviceFinalAmount)}</TableCell>
            <TableCell>{formatAmount(item.discountAmount)}</TableCell>
            <TableCell>{formatAmount(item.paidAmount)}</TableCell>
            <TableCell>{statusBadge(item.status, RECEPTION_STATUS)}</TableCell>
            <TableCell>
              <ActionButton onClick={() => openReceptionDetails(item.id)} />
            </TableCell>
          </TableRow>
        ));
      case "personnel":
        return (reportPage.list as ReceptionReportPersonnel[]).map((item) => (
          <TableRow key={item.personnelId}>
            <TableCell>{item.personnelName ?? `شناسه ${formatNumber(item.personnelId)}`}</TableCell>
            <TableCell>{formatNumber(item.completedServiceCount)}</TableCell>
            <TableCell>{formatNumber(item.receptionCount)}</TableCell>
            <TableCell>{formatAmount(item.serviceSubtotal)}</TableCell>
            <TableCell>{formatAmount(item.discountAmount)}</TableCell>
            <TableCell>{formatAmount(item.serviceFinalAmount)}</TableCell>
          </TableRow>
        ));
      case "services":
        return (reportPage.list as ReceptionReportServiceItem[]).map((item) => (
          <TableRow key={item.serviceId}>
            <TableCell>{item.serviceName}</TableCell>
            <TableCell>{formatNumber(item.completedServiceCount)}</TableCell>
            <TableCell>{formatNumber(item.receptionCount)}</TableCell>
            <TableCell>{formatAmount(item.serviceSubtotal)}</TableCell>
            <TableCell>{formatAmount(item.discountAmount)}</TableCell>
            <TableCell>{formatAmount(item.serviceFinalAmount)}</TableCell>
            <TableCell>{formatAmount(item.averageFinalAmount)}</TableCell>
          </TableRow>
        ));
      case "debts":
        return (reportPage.list as ReceptionReportDebt[]).map((item) => (
          <TableRow key={item.receptionId}>
            <TableCell>{formatNumber(item.receptionNumber)}</TableCell>
            <TableCell>{item.customerName}</TableCell>
            <TableCell dir="ltr" className="text-start">{item.customerMobilePhone ?? "—"}</TableCell>
            <TableCell>{formatDate(item.receptionDate)}</TableCell>
            <TableCell>{formatAmount(item.totalAmount)}</TableCell>
            <TableCell>{formatAmount(item.paidAmount)}</TableCell>
            <TableCell className="font-semibold text-rose-700">{formatAmount(item.remainingAmount)}</TableCell>
            <TableCell>{statusBadge(item.status, RECEPTION_STATUS)}</TableCell>
            <TableCell>
              <ActionButton onClick={() => openReceptionDetails(item.receptionId)} />
            </TableCell>
          </TableRow>
        ));
      case "discounts":
        return (reportPage.list as ReceptionReportDiscount[]).map((item) => (
          <TableRow key={`${item.receptionId}-${item.serviceId}`}>
            <TableCell>{formatNumber(item.receptionNumber)}</TableCell>
            <TableCell>{item.customerName}</TableCell>
            <TableCell>{formatDate(item.receptionDate)}</TableCell>
            <TableCell>{item.serviceName}</TableCell>
            <TableCell>{formatAmount(item.serviceSubtotal)}</TableCell>
            <TableCell>{formatAmount(item.discountAmount)}</TableCell>
            <TableCell>{formatAmount(item.serviceFinalAmount)}</TableCell>
            <TableCell>
              <ActionButton onClick={() => openReceptionDetails(item.receptionId)} />
            </TableCell>
          </TableRow>
        ));
      case "materials":
        return (reportPage.list as ReceptionReportMaterial[]).map((item, index) => (
          <TableRow key={`${item.materialId}-${item.serviceId}-${index}`}>
            <TableCell>{item.materialName ?? `شناسه ${formatNumber(item.materialId)}`}</TableCell>
            <TableCell>{item.serviceName}</TableCell>
            <TableCell>{item.baseUnitName ?? item.baseUnitSymbol ?? "—"}</TableCell>
            <TableCell>{formatNumber(item.usageCount)}</TableCell>
            <TableCell>
              {formatNumber(item.quantity)}
              {item.baseUnitSymbol ? ` ${item.baseUnitSymbol}` : ""}
            </TableCell>
            <TableCell>{formatAmount(item.totalCost)}</TableCell>
          </TableRow>
        ));
      default:
        return null;
    }
  }

  function renderTableHeaders() {
    switch (activeTab) {
      case "receptions":
        return ["شماره", "مشتری", "موبایل", "تاریخ", "تعداد خدمات", "مبلغ خدمات", "تخفیف", "پرداخت‌شده", "وضعیت", "جزئیات"];
      case "personnel":
        return ["پرسنل", "خدمات تکمیل‌شده", "تعداد پذیرش", "جمع پایه", "تخفیف", "مبلغ نهایی"];
      case "services":
        return ["خدمت", "تعداد تکمیل", "تعداد پذیرش", "جمع پایه", "تخفیف", "مبلغ نهایی", "میانگین"];
      case "debts":
        return ["شماره", "مشتری", "موبایل", "تاریخ", "مبلغ کل", "پرداخت‌شده", "مانده", "وضعیت", "جزئیات"];
      case "discounts":
        return ["شماره", "مشتری", "تاریخ", "خدمت", "مبلغ پایه", "تخفیف", "مبلغ نهایی", "جزئیات"];
      case "materials":
        return ["ماده مصرفی", "خدمت", "واحد", "دفعات مصرف", "مقدار", "بهای ثبت‌شده"];
      default:
        return [];
    }
  }

  function totalsItems() {
    if (!activeReport) return [];
    if (activeReport.kind === "debts") {
      return [
        { label: "تعداد پذیرش بدهکار", value: formatNumber(activeReport.result.totals.receptionCount) },
        { label: "مبلغ کل", value: formatAmount(activeReport.result.totals.totalAmount) },
        { label: "پرداخت‌شده", value: formatAmount(activeReport.result.totals.paidAmount) },
        { label: "مانده بدهی", value: formatAmount(activeReport.result.totals.remainingAmount) },
      ];
    }
    if (activeReport.kind === "discounts") {
      return [
        { label: "تعداد خدمات دارای تخفیف", value: formatNumber(activeReport.result.totals.serviceCount) },
        { label: "مبلغ پایه", value: formatAmount(activeReport.result.totals.serviceSubtotal) },
        { label: "تخفیف", value: formatAmount(activeReport.result.totals.discountAmount) },
        { label: "مبلغ نهایی", value: formatAmount(activeReport.result.totals.serviceFinalAmount) },
      ];
    }
    if (activeReport.kind === "materials") {
      return [
        { label: "دفعات مصرف", value: formatNumber(activeReport.result.totals.usageCount) },
        { label: "مقدار مصرف", value: formatNumber(activeReport.result.totals.quantity) },
        { label: "بهای قابل محاسبه", value: formatAmount(activeReport.result.totals.totalCost) },
      ];
    }
    if (
      activeReport.kind !== "receptions" &&
      activeReport.kind !== "personnel" &&
      activeReport.kind !== "services"
    ) return [];
    const items = [
      { label: "تعداد پذیرش", value: formatNumber(activeReport.result.totals.receptionCount) },
      { label: "جمع پایه", value: formatAmount(activeReport.result.totals.serviceSubtotal) },
      { label: "تخفیف", value: formatAmount(activeReport.result.totals.discountAmount) },
      { label: "مبلغ نهایی خدمات", value: formatAmount(activeReport.result.totals.serviceFinalAmount) },
    ];
    if (activeReport.kind === "receptions") {
      items.push(
        { label: "پرداخت‌شده", value: formatAmount(activeReport.result.totals.paidAmount) },
        { label: "مانده", value: formatAmount(activeReport.result.totals.remainingAmount) },
      );
    }
    return items;
  }

  const activeTabTitle = TAB_ITEMS.find((item) => item.id === activeTab)?.label ?? "";

  return (
    <div className="min-h-screen bg-slate-50">
      <section className="min-h-[190px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)] pb-12">
        <div className="mx-auto w-[94%] max-w-[1440px] pt-3">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/receptions" className="text-white/70 hover:text-white">
                  پذیرش و نوبت
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator className="text-white/50" />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">گزارش پذیرش</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-5 flex flex-wrap items-center justify-between gap-3">
            <div>
              <h1 className="text-xl font-bold text-white sm:text-2xl">گزارش‌های پذیرش</h1>
              <p className="mt-1 text-xs text-white/65">
                گزارش عملکرد، خدمات، دریافتی‌ها، بدهی و مواد مصرف‌شده بر اساس اطلاعات ثبت‌شده
              </p>
            </div>
            <Button
              type="button"
              variant="outline"
              disabled={summaryLoading || reportLoading}
              onClick={() => setRefreshKey((current) => current + 1)}
              className="h-9 border-white/30 bg-white/10 text-white hover:bg-white/20 hover:text-white"
            >
              {summaryLoading || reportLoading
                ? <LoaderCircle className="ml-2 size-4 animate-spin" />
                : <RefreshCw className="ml-2 size-4" />}
              به‌روزرسانی
            </Button>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-10 w-[94%] max-w-[1440px] space-y-4 pb-10">
        <form
          onSubmit={applyFilters}
          className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5"
        >
          <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
            <div>
              <h2 className="text-sm font-bold text-slate-800">فیلتر گزارش</h2>
              <p className="mt-1 text-xs text-slate-500">
                بازه تاریخ به‌صورت روز شروع شامل و روز بعد از پایان به‌صورت انحصاری محاسبه می‌شود.
              </p>
            </div>
            <Button
              type="button"
              variant="ghost"
              onClick={resetFilters}
              className="h-8 text-xs text-fuchsia-800 hover:bg-fuchsia-50"
            >
              پاک‌کردن فیلترها
            </Button>
          </div>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-5">
            <ReportDatePicker
              id="report-from-date"
              label="از تاریخ"
              value={form.fromDate}
              onChange={(value) => updateForm("fromDate", value)}
            />
            <ReportDatePicker
              id="report-to-date"
              label="تا تاریخ"
              value={form.toDate}
              onChange={(value) => updateForm("toDate", value)}
            />
            <FilterInput
              id="report-customer-search"
              label="جستجوی مشتری"
              value={form.customerSearch}
              onChange={(value) => updateForm("customerSearch", value)}
              placeholder="نام یا شماره همراه"
            />
            <FilterInput
              id="report-customer-id"
              label="شناسه مشتری"
              value={form.customerId}
              onChange={(value) => updateForm("customerId", value)}
              type="number"
            />
            <FilterInput
              id="report-reception-number"
              label="شماره پذیرش"
              value={form.receptionNumber}
              onChange={(value) => updateForm("receptionNumber", value)}
              type="number"
            />
            <FilterInput
              id="report-personnel-id"
              label="شناسه پرسنل"
              value={form.personnelId}
              onChange={(value) => updateForm("personnelId", value)}
              type="number"
            />
            <FilterInput
              id="report-service-id"
              label="شناسه خدمت"
              value={form.serviceId}
              onChange={(value) => updateForm("serviceId", value)}
              type="number"
            />
            <FilterInput
              id="report-material-id"
              label="شناسه ماده مصرفی"
              value={form.materialId}
              onChange={(value) => updateForm("materialId", value)}
              type="number"
            />
            <FilterSelect
              id="report-reception-status"
              label="وضعیت پذیرش"
              value={form.receptionStatus}
              onChange={(value) => updateForm("receptionStatus", value)}
              options={[
                { value: "", label: "همه وضعیت‌ها" },
                ...Object.entries(RECEPTION_STATUS).map(([value, label]) => ({ value, label })),
              ]}
            />
            <FilterSelect
              id="report-financial-status"
              label="وضعیت مالی"
              value={form.financialStatus}
              onChange={(value) => updateForm("financialStatus", value)}
              options={[
                { value: "", label: "همه وضعیت‌ها" },
                ...Object.entries(FINANCIAL_STATUS).map(([value, label]) => ({ value, label })),
              ]}
            />
            <FilterSelect
              id="report-service-status"
              label="وضعیت خدمت"
              value={form.serviceStatus}
              onChange={(value) => updateForm("serviceStatus", value)}
              options={[
                { value: "", label: "همه وضعیت‌ها" },
                ...Object.entries(SERVICE_STATUS).map(([value, label]) => ({ value, label })),
              ]}
            />
            <FilterSelect
              id="report-appointment"
              label="نوع پذیرش"
              value={form.hasAppointment}
              onChange={(value) => updateForm("hasAppointment", value)}
              options={[
                { value: "", label: "همه" },
                { value: "true", label: "دارای نوبت" },
                { value: "false", label: "بدون نوبت" },
              ]}
            />
            <FilterInput
              id="report-minimum-debt"
              label="حداقل مانده بدهی (تومان)"
              value={form.minimumRemainingAmount}
              onChange={(value) => updateForm("minimumRemainingAmount", value)}
              type="number"
            />
            <FilterInput
              id="report-maximum-debt"
              label="حداکثر مانده بدهی (تومان)"
              value={form.maximumRemainingAmount}
              onChange={(value) => updateForm("maximumRemainingAmount", value)}
              type="number"
            />
          </div>
          {formError && (
            <p className="mt-3 text-xs text-rose-700" role="alert">{formError}</p>
          )}
          <div className="mt-4 flex justify-end">
            <Button type="submit" className="h-9 bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              اعمال فیلترها
            </Button>
          </div>
        </form>

        <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div
            role="tablist"
            aria-label="بخش‌های گزارش پذیرش"
            className="flex gap-1 overflow-x-auto border-b border-slate-100 px-2 pt-2 [scrollbar-width:thin]"
          >
            {TAB_ITEMS.map(({ id, label, icon: Icon }) => (
              <button
                key={id}
                type="button"
                role="tab"
                aria-selected={activeTab === id}
                onClick={() => {
                  setActiveTab(id);
                  setPage(1);
                }}
                className={`flex shrink-0 items-center gap-2 border-b-2 px-3 py-3 text-xs font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-fuchsia-500 ${
                  activeTab === id
                    ? "border-fuchsia-700 text-fuchsia-800"
                    : "border-transparent text-slate-500 hover:text-slate-800"
                }`}
              >
                <Icon className="size-4" />
                {label}
              </button>
            ))}
          </div>

          {activeTab === "summary" ? (
            <div className="p-4 sm:p-5">
              {summaryError ? (
                <div role="alert" className="rounded-xl border border-rose-200 bg-rose-50 p-4 text-sm text-rose-700">
                  {summaryError}
                </div>
              ) : summaryLoading && !summary ? (
                <div className="flex min-h-48 items-center justify-center gap-2 text-sm text-slate-500">
                  <LoaderCircle className="size-5 animate-spin" />
                  در حال دریافت خلاصه گزارش...
                </div>
              ) : summary ? (
                <>
                  <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
                    <SummaryCard title="کل پذیرش‌ها" value={formatNumber(summary.receptionCount)} icon={ClipboardList} tone="bg-fuchsia-50 text-fuchsia-700" />
                    <SummaryCard title="پذیرش تکمیل‌شده" value={formatNumber(summary.completedReceptionCount)} icon={UsersRound} tone="bg-emerald-50 text-emerald-700" />
                    <SummaryCard title="مبلغ نهایی خدمات" value={formatAmount(summary.serviceFinalAmount)} icon={Scissors} tone="bg-violet-50 text-violet-700" />
                    <SummaryCard title="تخفیف خدمات" value={formatAmount(summary.discountAmount)} icon={Banknote} tone="bg-amber-50 text-amber-700" />
                    <SummaryCard title="مبلغ پرداخت‌شده" value={formatAmount(summary.paidAmount)} icon={HandCoins} tone="bg-sky-50 text-sky-700" />
                    <SummaryCard title="مانده قابل پرداخت" value={formatAmount(summary.remainingAmount)} icon={Banknote} tone="bg-rose-50 text-rose-700" />
                  </div>

                  <div className="mt-4 grid gap-4 xl:grid-cols-[minmax(0,1.7fr)_minmax(320px,1fr)]">
                    <article className="rounded-2xl border border-slate-200 p-4">
                      <h3 className="text-sm font-bold text-slate-800">روند مبلغ خدمات و دریافتی</h3>
                      <p className="mt-1 text-xs text-slate-500">مبالغ از سوابق ثبت‌شده پذیرش محاسبه شده‌اند.</p>
                      <div className="mt-4 h-[280px]">
                        {summary.trend.length === 0 ? (
                          <div className="flex h-full items-center justify-center text-sm text-slate-500">
                            داده‌ای در این بازه ثبت نشده است.
                          </div>
                        ) : (
                          <ResponsiveContainer width="100%" height="100%">
                            <LineChart data={summary.trend} margin={{ top: 8, right: 12, left: 8, bottom: 4 }}>
                              <CartesianGrid strokeDasharray="3 3" vertical={false} />
                              <XAxis
                                dataKey="date"
                                tickFormatter={jalaliDateLabel}
                                tick={{ fontSize: 10 }}
                                axisLine={false}
                                tickLine={false}
                              />
                              <YAxis
                                tickFormatter={amountTick}
                                tick={{ fontSize: 10 }}
                                axisLine={false}
                                tickLine={false}
                                width={52}
                              />
                              <Tooltip
                                labelFormatter={(value) => jalaliDateLabel(String(value))}
                                formatter={(value, name) => [
                                  formatAmount(Number(value)),
                                  name === "serviceFinalAmount" ? "مبلغ خدمات" : "دریافتی",
                                ]}
                              />
                              <Line
                                type="monotone"
                                dataKey="serviceFinalAmount"
                                name="serviceFinalAmount"
                                stroke="#a21caf"
                                strokeWidth={2}
                                dot={false}
                              />
                              <Line
                                type="monotone"
                                dataKey="paidAmount"
                                name="paidAmount"
                                stroke="#059669"
                                strokeWidth={2}
                                dot={false}
                              />
                            </LineChart>
                          </ResponsiveContainer>
                        )}
                      </div>
                    </article>
                    <article className="rounded-2xl border border-slate-200 p-4">
                      <h3 className="text-sm font-bold text-slate-800">ترکیب روش پرداخت</h3>
                      <p className="mt-1 text-xs text-slate-500">پرداخت‌های موفق ثبت‌شده در این بازه</p>
                      <div className="mt-4 h-[280px]">
                        {summary.paymentMethods.length === 0 ? (
                          <div className="flex h-full items-center justify-center text-sm text-slate-500">
                            پرداخت موفقی در این بازه ثبت نشده است.
                          </div>
                        ) : (
                          <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={summary.paymentMethods} margin={{ top: 8, right: 8, left: 8, bottom: 4 }}>
                              <CartesianGrid strokeDasharray="3 3" vertical={false} />
                              <XAxis dataKey="paymentMethod" tick={{ fontSize: 10 }} axisLine={false} tickLine={false} />
                              <YAxis tickFormatter={amountTick} tick={{ fontSize: 10 }} axisLine={false} tickLine={false} width={52} />
                              <Tooltip formatter={(value) => [formatAmount(Number(value)), "مبلغ"]} />
                              <Bar dataKey="amount" fill="#a21caf" radius={[6, 6, 0, 0]} />
                            </BarChart>
                          </ResponsiveContainer>
                        )}
                      </div>
                    </article>
                  </div>

                  <div className="mt-4 grid gap-3 rounded-2xl border border-slate-200 bg-slate-50/70 p-4 sm:grid-cols-2 lg:grid-cols-4">
                    <div><p className="text-xs text-slate-500">پذیرش در حال انجام</p><p className="mt-1 text-sm font-bold">{formatNumber(summary.inProgressReceptionCount)}</p></div>
                    <div><p className="text-xs text-slate-500">پذیرش لغوشده</p><p className="mt-1 text-sm font-bold">{formatNumber(summary.cancelledReceptionCount)}</p></div>
                    <div><p className="text-xs text-slate-500">مبلغ پایه خدمات</p><p className="mt-1 text-sm font-bold">{formatAmount(summary.serviceSubtotal)}</p></div>
                    <div>
                      <p className="text-xs text-slate-500">مبلغ مواد مصرفی ثبت‌شده</p>
                      <p className="mt-1 text-sm font-bold">{formatAmount(summary.materialChargeAmount)}</p>
                      <p className="mt-1 text-[10px] text-slate-500">این رقم جدا از جمع خدمات نمایش داده شده است.</p>
                    </div>
                  </div>
                  {summaryLoading && (
                    <p className="mt-3 text-end text-xs text-slate-400" role="status">در حال به‌روزرسانی خلاصه...</p>
                  )}
                </>
              ) : (
                <div className="py-12 text-center text-sm text-slate-500">خلاصه‌ای برای نمایش وجود ندارد.</div>
              )}
            </div>
          ) : (
            <>
              {activeReport && !reportError && (
                <TotalsBar items={totalsItems()} />
              )}
              <div className="border-b border-slate-100 px-4 py-3">
                <h2 className="text-sm font-bold text-slate-800">{activeTabTitle}</h2>
                <p className="mt-1 text-xs text-slate-500">
                  جمع‌های گزارش بر اساس تمام نتایج منطبق با فیلتر محاسبه شده‌اند؛ صفحه‌بندی فقط نمایش ردیف‌ها را محدود می‌کند.
                </p>
              </div>
              <div className="overflow-x-auto">
                <Table>
                  <TableHeader>
                    <TableRow className="bg-slate-50 hover:bg-slate-50">
                      {renderTableHeaders().map((header) => (
                        <TableHead key={header} className="h-10 px-3 text-start text-xs font-semibold text-slate-600">
                          {header}
                        </TableHead>
                      ))}
                    </TableRow>
                  </TableHeader>
                  <TableBody>{renderTableRows()}</TableBody>
                </Table>
              </div>
              {reportPage && reportPage.pagination.totalCount > 0 && (
                <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 px-4 py-3">
                  <p className="text-xs text-slate-500">
                    {formatNumber(reportPage.pagination.totalCount)} ردیف
                  </p>
                  <Pagination dir="ltr" className="mx-0 w-auto">
                    <PaginationContent className="gap-1">
                      <PaginationItem>
                        <PaginationPrevious
                          href="#"
                          text=""
                          aria-label="صفحه قبلی"
                          aria-disabled={currentPage <= 1}
                          className="size-8 rounded-full border border-fuchsia-300 p-0 text-fuchsia-800"
                          onClick={(event) => {
                            event.preventDefault();
                            setPage((value) => Math.max(1, value - 1));
                          }}
                        />
                      </PaginationItem>
                      {pageNumbers.map((pageNumber) => (
                        <PaginationItem key={pageNumber}>
                          <button
                            type="button"
                            aria-current={pageNumber === currentPage ? "page" : undefined}
                            onClick={() => setPage(pageNumber)}
                            className={`size-8 rounded-full text-xs ${
                              pageNumber === currentPage
                                ? "bg-fuchsia-700 text-white"
                                : "border border-slate-200 bg-white text-slate-700 hover:bg-fuchsia-50"
                            }`}
                          >
                            {formatNumber(pageNumber)}
                          </button>
                        </PaginationItem>
                      ))}
                      <PaginationItem>
                        <PaginationNext
                          href="#"
                          text=""
                          aria-label="صفحه بعدی"
                          aria-disabled={currentPage >= totalPages}
                          className="size-8 rounded-full border border-fuchsia-300 p-0 text-fuchsia-800"
                          onClick={(event) => {
                            event.preventDefault();
                            setPage((value) => Math.min(totalPages, value + 1));
                          }}
                        />
                      </PaginationItem>
                    </PaginationContent>
                  </Pagination>
                </div>
              )}
            </>
          )}
        </section>

        <p className="px-1 text-[11px] leading-5 text-slate-500">
          گزارش پرداخت، فقط پرداخت‌های موفق ثبت‌شده را در محاسبه دریافتی لحاظ می‌کند. در وضعیت فعلی، نوع تخفیف و وضعیت‌های برگشت/لغو پرداخت در داده‌های پذیرش ذخیره نمی‌شوند.
        </p>
      </main>

      <ReceptionDetailsDialog
        receptionId={selectedReceptionId}
        open={detailsOpen}
        onOpenChange={setDetailsOpen}
      />
    </div>
  );
}
