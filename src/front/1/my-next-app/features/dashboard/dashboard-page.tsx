"use client";

import { useState, type ReactNode } from "react";
import {
  AlertCircle,
  Banknote,
  CalendarDays,
  CalendarPlus,
  CheckCircle2,
  CircleDollarSign,
  Clock3,
  PackageCheck,
  RefreshCw,
  UsersRound,
  Wallet,
} from "lucide-react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useDashboard } from "./use-dashboard";
import type {
  DashboardAlert,
  DashboardAppointment,
  DashboardCharts,
  DashboardPeriod,
  DashboardSummary,
  DashboardTodayAppointments,
} from "./dashboard.types";
import type { MaterialListResult } from "@/features/inventory/materials/material.types";

const PERIODS: { value: DashboardPeriod; label: string }[] = [
  { value: 1, label: "امروز" },
  { value: 2, label: "دیروز" },
  { value: 3, label: "۷ روز گذشته" },
  { value: 4, label: "ماه جاری" },
];

const COLORS = ["#c515c7", "#10a99d", "#ffae42", "#6874e8", "#75bd60"];
const METRIC_TONES = {
  pink: {
    card: "border-fuchsia-100 bg-gradient-to-l from-fuchsia-50/80 to-white",
    value: "text-fuchsia-700",
    icon: "bg-fuchsia-100/70 text-fuchsia-700",
  },
  blue: {
    card: "border-sky-100 bg-gradient-to-l from-sky-50/80 to-white",
    value: "text-sky-700",
    icon: "bg-sky-100/70 text-sky-700",
  },
  orange: {
    card: "border-orange-100 bg-gradient-to-l from-orange-50/80 to-white",
    value: "text-orange-700",
    icon: "bg-orange-100/70 text-orange-700",
  },
  purple: {
    card: "border-purple-100 bg-gradient-to-l from-purple-50/80 to-white",
    value: "text-purple-700",
    icon: "bg-purple-100/70 text-purple-700",
  },
};
const numberFormatter = new Intl.NumberFormat("fa-IR");
const compactFormatter = new Intl.NumberFormat("fa-IR", {
  notation: "compact",
  maximumFractionDigits: 1,
});

function number(value: number) {
  return numberFormatter.format(value);
}

function compactMoney(value: number) {
  return `${compactFormatter.format(value)} تومان`;
}

function dateLabel(value: string) {
  const [year, month, day] = value.split("-").map(Number);
  if (!year || !month || !day) return value;
  return new Intl.DateTimeFormat("fa-IR", {
    month: "numeric",
    day: "numeric",
  }).format(new Date(year, month - 1, day));
}

function timeLabel(value: string) {
  return new Intl.DateTimeFormat("fa-IR", {
    hour: "2-digit",
    minute: "2-digit",
    timeZone: "Asia/Tehran",
  }).format(new Date(value));
}

function titleCaseStatus(value: string) {
  const labels: Record<string, string> = {
    Pending: "در انتظار",
    Confirmed: "تأییدشده",
    Completed: "انجام‌شده",
    Cancelled: "لغوشده",
    NoShow: "عدم مراجعه",
  };
  return labels[value] ?? value;
}

function Panel({
  title,
  icon,
  action,
  children,
  className = "",
}: {
  title: string;
  icon: ReactNode;
  action?: ReactNode;
  children: ReactNode;
  className?: string;
}) {
  return (
    <section
      className={`min-w-0 overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm ${className}`}
    >
      <header className="flex min-h-12 items-center justify-between gap-3 border-b border-slate-100 px-4">
        <div className="flex min-w-0 items-center gap-2.5">
          <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-fuchsia-50 text-fuchsia-700">
            {icon}
          </span>
          <h2 className="truncate text-xs font-bold text-slate-800 sm:text-sm">
            {title}
          </h2>
        </div>
        {action}
      </header>
      <div className="p-3.5 sm:p-4">{children}</div>
    </section>
  );
}

function MetricCard({
  title,
  value,
  caption,
  icon,
  tone,
}: {
  title: string;
  value: string;
  caption: string;
  icon: ReactNode;
  tone: "pink" | "blue" | "orange" | "purple";
}) {
  const classes = METRIC_TONES[tone];

  return (
    <article
      className={`flex min-h-[92px] min-w-0 items-center justify-between gap-3 rounded-xl border px-4 py-3 shadow-sm ${classes.card}`}
    >
      <div className="min-w-0 text-right">
        <p className="truncate text-xs font-semibold text-slate-600">
          {title}
        </p>
        <p className={`mt-1 truncate text-xl font-extrabold leading-6 sm:text-2xl ${classes.value}`}>
          {value}
        </p>
        <p className="mt-1 truncate text-[10px] text-slate-500 sm:text-xs">{caption}</p>
      </div>
      <span className={`flex size-10 shrink-0 items-center justify-center rounded-xl ${classes.icon}`}>
        {icon}
      </span>
    </article>
  );
}

function Empty({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-28 items-center justify-center px-3 text-center text-xs text-slate-500">
      {children}
    </div>
  );
}

function AlertsPanel({ alerts }: { alerts: DashboardAlert[] }) {
  if (alerts.length === 0) {
    return (
      <div className="flex min-h-36 flex-col items-center justify-center gap-2 text-center text-xs text-slate-500">
        <CheckCircle2 className="size-6 text-emerald-500" />
        موردی برای پیگیری ثبت نشده است.
      </div>
    );
  }

  const alertTones = [
    "bg-rose-50 text-rose-700",
    "bg-orange-50 text-orange-700",
    "bg-purple-50 text-purple-700",
  ];

  return (
    <ul className="space-y-2">
      {alerts.map((alert, index) => (
        <li
          key={`${alert.type}-${alert.title}`}
          className="flex min-h-11 items-center gap-2.5 rounded-lg border border-slate-100 px-3 py-2"
        >
          <span
            className={`flex size-6 shrink-0 items-center justify-center rounded-full text-[10px] font-bold ${alertTones[index % alertTones.length]}`}
          >
            {number(alert.count)}
          </span>
          <span className="min-w-0 flex-1 truncate text-xs text-slate-700">
            {alert.title}
          </span>
          {alert.amount !== null && (
            <span className="shrink-0 text-[10px] text-slate-500">
              {compactMoney(alert.amount)}
            </span>
          )}
          <AlertCircle className="size-4 shrink-0 text-orange-500" />
        </li>
      ))}
    </ul>
  );
}

function AppointmentTable({
  appointments,
}: {
  appointments: DashboardAppointment[];
}) {
  if (appointments.length === 0) {
    return <Empty>برای امروز نوبتی ثبت نشده است.</Empty>;
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[380px] text-right">
        <thead>
          <tr className="h-9 text-[10px] text-slate-500 sm:text-xs">
            <th className="font-medium">ساعت</th>
            <th className="font-medium">مشتری</th>
            <th className="font-medium">خدمت</th>
            <th className="font-medium">پرسنل</th>
            <th className="font-medium">وضعیت</th>
          </tr>
        </thead>
        <tbody>
          {appointments.slice(0, 5).map((appointment) => (
            <tr
              key={appointment.id}
              className="h-10 border-t border-slate-100 text-[10px] text-slate-600 sm:text-xs"
            >
              <td className="whitespace-nowrap font-semibold text-[#343960]" dir="ltr">
                {timeLabel(appointment.startsAt)}
              </td>
              <td className="max-w-16 truncate">{appointment.customerName}</td>
              <td className="max-w-20 truncate">{appointment.serviceName}</td>
              <td className="max-w-16 truncate">{appointment.personnelName}</td>
              <td>
                <span className="whitespace-nowrap rounded-md bg-slate-50 px-2 py-1 text-[9px] text-slate-600">
                  {titleCaseStatus(appointment.status)}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {appointments.length > 5 && (
        <p className="pt-2 text-left text-[10px] text-slate-500">
          {number(appointments.length - 5)} نوبت دیگر
        </p>
      )}
    </div>
  );
}

function ReceptionStatus({
  summary,
  appointments,
}: {
  summary: DashboardSummary;
  appointments: DashboardTodayAppointments;
}) {
  const pendingCount = appointments.appointments.filter(
    (appointment) => appointment.status === "Pending",
  ).length;
  const rows = [
    {
      label: "پذیرش تکمیل‌شده",
      value: summary.completedReceptionCount,
      icon: <CheckCircle2 className="size-3" />,
      color: "bg-emerald-50 text-emerald-700",
    },
    {
      label: "در حال انجام",
      value: summary.inProgressReceptionCount,
      icon: <Clock3 className="size-3" />,
      color: "bg-orange-50 text-orange-700",
    },
    {
      label: "نوبت در انتظار",
      value: pendingCount,
      icon: <CalendarPlus className="size-3" />,
      color: "bg-rose-50 text-rose-700",
    },
  ];

  return (
    <ul className="space-y-2">
      {rows.map((row) => (
        <li
          key={row.label}
          className={`flex min-h-10 items-center justify-between gap-2 rounded-lg border border-slate-100 px-3 text-xs ${row.color}`}
        >
          <span className="flex min-w-0 items-center gap-1.5">
            {row.icon}
            <span className="truncate">{row.label}</span>
          </span>
          <span className="shrink-0 font-bold">{number(row.value)} مورد</span>
        </li>
      ))}
    </ul>
  );
}

function LowStockPanel({
  result,
  error,
}: {
  result: MaterialListResult | null;
  error: string | null;
}) {
  if (error) {
    return (
      <div role="alert" className="flex min-h-28 items-center gap-2 text-xs text-rose-700">
        <AlertCircle className="size-4 shrink-0" />
        <p>{error}</p>
      </div>
    );
  }

  if (!result) {
    return <Empty>اطلاعات موجودی مواد در دسترس نیست.</Empty>;
  }

  const { list: materials, pagination } = result;
  const totalCount = pagination.totalCount;

  if (materials.length === 0) {
    return (
      <div className="flex min-h-28 flex-col items-center justify-center gap-2 text-center">
        <CheckCircle2 className="size-6 text-emerald-500" />
        <p className="text-xs font-medium text-slate-600">
          موجودی مواد مصرفی در محدودهٔ مجاز است.
        </p>
      </div>
    );
  }

  const visibleMaterials = materials.slice(0, 4);

  return (
    <div className="space-y-2.5">
      {visibleMaterials.map((material) => {
        const remainingRatio =
          material.minimumStock > 0
            ? Math.min((material.currentStock / material.minimumStock) * 100, 100)
            : 0;
        const isOutOfStock = material.currentStock <= 0;

        return (
          <div
            key={material.id}
            className="rounded-lg border border-slate-100 px-3 py-2.5"
          >
            <div className="flex items-start justify-between gap-3">
              <div className="min-w-0">
                <p className="truncate text-xs font-semibold text-slate-800">
                  {material.name}
                </p>
                <p className="mt-1 text-[10px] text-slate-500">
                  حداقل موجودی: {number(material.minimumStock)} {material.baseUnitSymbol}
                </p>
              </div>
              <span
                className={`shrink-0 rounded-full px-2 py-1 text-[10px] font-medium ${
                  isOutOfStock
                    ? "bg-rose-50 text-rose-700"
                    : "bg-amber-50 text-amber-700"
                }`}
              >
                {isOutOfStock ? "ناموجود" : "رو به اتمام"}
              </span>
            </div>
            <div className="mt-2 flex items-center gap-2">
              <div
                className="h-1.5 flex-1 overflow-hidden rounded-full bg-slate-100"
                role="meter"
                aria-label={`موجودی ${material.name}`}
                aria-valuemin={0}
                aria-valuemax={material.minimumStock}
                aria-valuenow={material.currentStock}
              >
                <div
                  className={`h-full rounded-full ${
                    isOutOfStock ? "bg-rose-500" : "bg-amber-500"
                  }`}
                  style={{ width: `${remainingRatio}%` }}
                />
              </div>
              <span className="shrink-0 text-[10px] font-semibold text-slate-600">
                {number(material.currentStock)} {material.baseUnitSymbol}
              </span>
            </div>
          </div>
        );
      })}
      {totalCount > visibleMaterials.length && (
        <p className="text-center text-[10px] text-slate-500">
          و {number(totalCount - visibleMaterials.length)} مادهٔ دیگر
        </p>
      )}
    </div>
  );
}

function RecentPerformance({
  charts,
}: {
  charts: DashboardCharts;
}) {
  const recent = charts.financialTrend.slice(-4).reverse();
  if (recent.length === 0) {
    return <Empty>اطلاعاتی برای این بازه ثبت نشده است.</Empty>;
  }

  return (
    <ul className="space-y-1">
      {recent.map((item) => (
        <li
          key={item.date}
          className="flex min-h-8 items-center gap-2 border-b border-slate-50 text-[10px] last:border-0"
        >
          <span className="shrink-0 font-semibold text-[#41456d]">
            {dateLabel(item.date)}
          </span>
          <span className="min-w-0 flex-1 truncate text-slate-500">
            {number(item.receptionCount)} پذیرش
          </span>
          <span className="shrink-0 text-slate-500">{compactMoney(item.serviceFinalAmount)}</span>
        </li>
      ))}
    </ul>
  );
}

function MiniStat({
  title,
  icon,
  rows,
}: {
  title: string;
  icon: ReactNode;
  rows: { label: string; value: string; tone?: string }[];
}) {
  return (
    <section className="min-w-0 rounded-xl border border-slate-200 bg-white px-3.5 py-3 shadow-sm">
      <h2 className="flex items-center gap-2 text-xs font-bold text-slate-800">
        <span className="text-fuchsia-600">{icon}</span>
        <span className="truncate">{title}</span>
      </h2>
      <div className="mt-3 grid grid-cols-2 gap-3">
        {rows.map((row) => (
          <div key={row.label} className="min-w-0 border-s-2 border-slate-100 ps-2 last:border-0">
            <p className="truncate text-[10px] text-slate-500">{row.label}</p>
            <p className={`mt-1 truncate text-xs font-bold ${row.tone ?? "text-slate-700"}`}>
              {row.value}
            </p>
          </div>
        ))}
      </div>
    </section>
  );
}

function MoneyTooltip({
  active,
  payload,
  label,
}: {
  active?: boolean;
  payload?: { value?: number; name?: string; color?: string }[];
  label?: string;
}) {
  if (!active || !payload?.length) return null;
  return (
    <div className="rounded-lg border border-slate-200 bg-white px-3 py-2 text-[10px] shadow-md">
      {label && <p className="mb-1 text-slate-500">{dateLabel(label)}</p>}
      {payload.map((item) => (
        <p key={item.name} style={{ color: item.color }} dir="rtl">
          {item.name}: {compactMoney(Number(item.value ?? 0))}
        </p>
      ))}
    </div>
  );
}

function TrendChart({ charts }: { charts: DashboardCharts }) {
  if (charts.financialTrend.length === 0) {
    return <Empty>داده‌ای برای نمایش وجود ندارد.</Empty>;
  }

  return (
    <div className="h-[190px] w-full" dir="ltr">
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={charts.financialTrend} margin={{ top: 4, right: 4, left: -20, bottom: 0 }}>
          <CartesianGrid stroke="#edf1f7" vertical={false} />
          <XAxis
            dataKey="date"
            tickFormatter={dateLabel}
            tick={{ fill: "#8991ad", fontSize: 10 }}
            axisLine={false}
            tickLine={false}
          />
          <YAxis
            tickFormatter={(value: number) => compactFormatter.format(value)}
            tick={{ fill: "#8991ad", fontSize: 10 }}
            axisLine={false}
            tickLine={false}
            width={48}
          />
          <Tooltip content={<MoneyTooltip />} />
          <Line
            type="monotone"
            dataKey="serviceFinalAmount"
            name="مبلغ خدمات"
            stroke="#c515c7"
            strokeWidth={2}
            dot={{ r: 2, fill: "#c515c7" }}
          />
          <Line
            type="monotone"
            dataKey="paidAmount"
            name="دریافتی"
            stroke="#10a99d"
            strokeWidth={2}
            dot={{ r: 2, fill: "#10a99d" }}
          />
          <Line
            type="monotone"
            dataKey="discountAmount"
            name="تخفیف"
            stroke="#ffae42"
            strokeWidth={1.5}
            dot={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}

function RevenueBarChart({ charts }: { charts: DashboardCharts }) {
  if (charts.financialTrend.length === 0) {
    return <Empty>عملکردی برای نمایش وجود ندارد.</Empty>;
  }

  const data = charts.financialTrend.map((item) => ({
    ...item,
    name: dateLabel(item.date),
  }));

  return (
    <div className="h-[190px] w-full" dir="ltr">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data} margin={{ top: 4, right: 0, left: -20, bottom: 0 }}>
          <CartesianGrid stroke="#edf1f7" vertical={false} />
          <XAxis
            dataKey="name"
            tick={{ fill: "#8991ad", fontSize: 10 }}
            axisLine={false}
            tickLine={false}
            interval={0}
          />
          <YAxis
            tickFormatter={(value: number) => compactFormatter.format(value)}
            tick={{ fill: "#8991ad", fontSize: 10 }}
            axisLine={false}
            tickLine={false}
            width={48}
          />
          <Tooltip content={<MoneyTooltip />} />
          <Bar dataKey="serviceFinalAmount" name="مبلغ خدمات" fill="#c515c7" radius={[2, 2, 0, 0]} />
          <Bar dataKey="paidAmount" name="دریافتی" fill="#10a99d" radius={[2, 2, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

function PopularServicesChart({ charts }: { charts: DashboardCharts }) {
  if (charts.popularServices.length === 0) {
    return <Empty>خدمتی در این بازه ثبت نشده است.</Empty>;
  }

  const services = charts.popularServices.slice(0, 6);
  return (
    <div className="grid h-[190px] min-w-0 grid-cols-[minmax(100px,1fr)_1fr] items-center gap-2">
      <div className="h-full min-w-0" dir="ltr">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={services}
              dataKey="serviceAmount"
              nameKey="serviceName"
              innerRadius="50%"
              outerRadius="82%"
              paddingAngle={2}
              stroke="none"
            >
              {services.map((service, index) => (
                <Cell key={service.serviceId} fill={COLORS[index % COLORS.length]} />
              ))}
            </Pie>
            <Tooltip formatter={(value) => compactMoney(Number(value ?? 0))} />
          </PieChart>
        </ResponsiveContainer>
      </div>
      <ul className="min-w-0 space-y-1">
        {services.slice(0, 5).map((service, index) => (
          <li key={service.serviceId} className="flex min-w-0 items-center gap-1.5 text-[10px]">
            <span
              className="size-2 shrink-0 rounded-full"
              style={{ backgroundColor: COLORS[index % COLORS.length] }}
            />
            <span className="min-w-0 flex-1 truncate text-slate-500">{service.serviceName}</span>
            <span className="shrink-0 font-semibold text-slate-600">
              {number(service.completedServiceCount)}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}

function LoadingState() {
  return (
    <div className="grid grid-cols-2 gap-3 md:grid-cols-4" aria-label="در حال بارگذاری داشبورد">
      {Array.from({ length: 8 }, (_, index) => (
        <div
          key={index}
          className="h-[92px] animate-pulse rounded-xl border border-slate-100 bg-white"
        />
      ))}
    </div>
  );
}

export function DashboardPage() {
  const [period, setPeriod] = useState<DashboardPeriod>(1);
  const { data, error, loading, refresh } = useDashboard(period);

  return (
    <div className="min-h-screen bg-slate-50 text-foreground">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[94%] max-w-[1440px] pt-5 sm:w-[88%]">
          <header className="flex min-h-12 items-center justify-between gap-4 text-white">
            <div className="order-1 min-w-0">
              <p className="text-xs font-medium text-fuchsia-200">مدیریت سالن زیبایی</p>
              <h1 className="mt-1 text-xl font-extrabold sm:text-2xl">داشبورد مدیریت سالن</h1>
              <p className="mt-1 text-xs text-white/65">
                نمای کلی عملکرد و فعالیت‌های مجموعه
              </p>
            </div>
            <div className="order-2 flex items-center gap-1.5">
              <Select
                value={String(period)}
                onValueChange={(value) => {
                  const next = PERIODS.find((item) => String(item.value) === value);
                  if (next) setPeriod(next.value);
                }}
              >
                <SelectTrigger
                  aria-label="بازه زمانی داشبورد"
                  className="h-9 min-w-[142px] gap-2 rounded-full border border-white/30 bg-white px-3 text-xs text-slate-700 shadow-none"
                >
                  <CalendarDays className="size-4 text-fuchsia-700" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {PERIODS.map((item) => (
                    <SelectItem key={item.value} value={String(item.value)}>
                      {item.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <Button
                variant="ghost"
                size="icon"
                aria-label="به‌روزرسانی داشبورد"
                onClick={refresh}
                disabled={loading}
                className="size-9 rounded-full text-white hover:bg-white/10 hover:text-white"
              >
                <RefreshCw className={`size-4 ${loading ? "animate-spin" : ""}`} />
              </Button>
            </div>
          </header>
        </div>
      </section>

      <main className="mx-auto -mt-[112px] w-[94%] max-w-[1440px] pb-10 sm:w-[88%]">
        <div className="space-y-4 rounded-2xl border border-slate-200 bg-background p-3 shadow-lg shadow-slate-900/5 sm:p-5">
        {loading ? (
          <LoadingState />
        ) : error ? (
          <div
            role="alert"
            className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-10 text-center"
          >
            <p className="text-sm font-bold text-rose-800">
              دریافت اطلاعات داشبورد ناموفق بود
            </p>
            <p className="mt-1 text-xs text-rose-700">{error}</p>
            <Button
              variant="outline"
              size="sm"
              onClick={refresh}
              className="mt-3 bg-white"
            >
              تلاش دوباره
            </Button>
          </div>
        ) : data ? (
          <div className="space-y-4">
            <section aria-label="آمار امروز" className="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4">
              <MetricCard
                title="پذیرش امروز"
                value={number(data.summary.receptionCount)}
                caption={`${number(data.summary.completedReceptionCount)} تکمیل‌شده`}
                icon={<CalendarPlus className="size-4" />}
                tone="pink"
              />
              <MetricCard
                title="درآمد خدمات"
                value={compactMoney(data.summary.serviceFinalAmount)}
                caption={`${compactMoney(data.summary.paidAmount)} دریافتی`}
                icon={<Banknote className="size-4" />}
                tone="blue"
              />
              <MetricCard
                title="مشتریان امروز"
                value={number(data.summary.uniqueCustomerCount)}
                caption="مشتری یکتا"
                icon={<UsersRound className="size-4" />}
                tone="orange"
              />
              <MetricCard
                title="مانده دریافتنی"
                value={compactMoney(data.summary.outstandingAmount)}
                caption={`${number(data.summary.appointmentReceptionCount)} پذیرش با نوبت`}
                icon={<Wallet className="size-4" />}
                tone="purple"
              />
            </section>

            <div className="grid items-start gap-4 xl:grid-cols-12">
              <div className="space-y-4 xl:col-span-3">
                <Panel
                  title="مواد مصرفی رو به اتمام"
                  icon={<PackageCheck className="size-4" />}
                  action={
                    <span
                      className={`rounded-full px-2.5 py-1 text-[10px] font-medium ${
                        (data.lowStockMaterials?.pagination.totalCount ?? 0) > 0
                          ? "bg-amber-50 text-amber-700"
                          : data.lowStockError
                            ? "bg-rose-50 text-rose-700"
                            : "bg-emerald-50 text-emerald-700"
                      }`}
                    >
                      {data.lowStockError
                        ? "خطا"
                        : `${number(data.lowStockMaterials?.pagination.totalCount ?? 0)} ماده`}
                    </span>
                  }
                >
                  <LowStockPanel
                    result={data.lowStockMaterials}
                    error={data.lowStockError}
                  />
                </Panel>

                <Panel
                  title="هشدارها و موارد نیازمند پیگیری"
                  icon={<AlertCircle className="size-4" />}
                  action={
                    <span className="rounded-full bg-fuchsia-50 px-2.5 py-1 text-[10px] font-medium text-fuchsia-700">
                      {number(data.alerts.reduce((sum, item) => sum + item.count, 0))}
                    </span>
                  }
                >
                  <AlertsPanel alerts={data.alerts} />
                </Panel>
              </div>

              <Panel
                title="نوبت‌های امروز"
                icon={<CalendarDays className="size-4" />}
                action={
                  <span className="rounded-full bg-fuchsia-50 px-2.5 py-1 text-[10px] font-medium text-fuchsia-700">
                    {number(data.appointments.totalCount)} نوبت
                  </span>
                }
                className="xl:col-span-6"
              >
                <AppointmentTable appointments={data.appointments.appointments} />
              </Panel>

              <div className="grid gap-4 sm:grid-cols-2 xl:col-span-3 xl:grid-cols-1">
                <Panel title="وضعیت پذیرش" icon={<PackageCheck className="size-4" />}>
                  <ReceptionStatus
                    summary={data.summary}
                    appointments={data.appointments}
                  />
                </Panel>
                <Panel title="عملکرد روزهای اخیر" icon={<Clock3 className="size-4" />}>
                  <RecentPerformance charts={data.charts} />
                </Panel>
              </div>
            </div>

            <section
              aria-label="خلاصه فعالیت سالن"
              className="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4"
            >
              <MiniStat
                title="نوبت‌های امروز"
                icon={<CalendarPlus className="size-4" />}
                rows={[
                  { label: "کل نوبت‌ها", value: number(data.appointments.totalCount) },
                  {
                    label: "در انتظار",
                    value: number(data.appointments.appointments.filter((item) => item.status === "Pending").length),
                    tone: "text-amber-600",
                  },
                ]}
              />
              <MiniStat
                title="مشتریان"
                icon={<UsersRound className="size-4" />}
                rows={[
                  { label: "مشتری یکتا", value: number(data.summary.uniqueCustomerCount) },
                  { label: "مراجعه حضوری", value: number(data.summary.walkInReceptionCount) },
                ]}
              />
              <MiniStat
                title="درآمد و دریافتی"
                icon={<CircleDollarSign className="size-4" />}
                rows={[
                  { label: "درآمد خدمات", value: compactMoney(data.summary.serviceFinalAmount) },
                  { label: "دریافتی", value: compactMoney(data.summary.paidAmount), tone: "text-emerald-600" },
                ]}
              />
              <MiniStat
                title="پذیرش‌های امروز"
                icon={<CalendarDays className="size-4" />}
                rows={[
                  { label: "تکمیل‌شده", value: number(data.summary.completedReceptionCount), tone: "text-emerald-600" },
                  { label: "در حال انجام", value: number(data.summary.inProgressReceptionCount), tone: "text-amber-600" },
                ]}
              />
            </section>

            <section
              aria-label="نمودارهای داشبورد"
              className="grid items-stretch gap-4 xl:grid-cols-12"
            >
              <Panel
                title="خدمات پرطرفدار"
                icon={<Banknote className="size-4" />}
                className="xl:col-span-3"
              >
                <PopularServicesChart charts={data.charts} />
              </Panel>

              <Panel
                title="مقایسه مبلغ خدمات و دریافتی"
                icon={<CircleDollarSign className="size-4" />}
                className="xl:col-span-4"
              >
                <RevenueBarChart charts={data.charts} />
              </Panel>

              <Panel
                title="روند درآمد و دریافتی"
                icon={<CircleDollarSign className="size-4" />}
                className="xl:col-span-5"
              >
                <div className="mb-2 flex flex-wrap gap-4 text-[10px] text-slate-600">
                  <span className="flex items-center gap-1.5">
                    <span className="size-2 rounded-full bg-fuchsia-600" />
                    مبلغ خدمات
                  </span>
                  <span className="flex items-center gap-1.5">
                    <span className="size-2 rounded-full bg-teal-500" />
                    دریافتی
                  </span>
                  <span className="flex items-center gap-1.5">
                    <span className="size-2 rounded-full bg-orange-400" />
                    تخفیف
                  </span>
                </div>
                <TrendChart charts={data.charts} />
              </Panel>
            </section>
          </div>
        ) : (
          <Empty>
            <RefreshCw className="me-1 size-3 animate-spin" />
            در حال آماده‌سازی داشبورد…
          </Empty>
        )}
        </div>
      </main>
    </div>
  );
}
