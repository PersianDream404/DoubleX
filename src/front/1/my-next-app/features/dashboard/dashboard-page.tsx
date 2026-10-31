"use client";

import { useState, type ReactNode } from "react";
import {
  AlertCircle,
  Banknote,
  BriefcaseBusiness,
  CalendarClock,
  CalendarDays,
  CheckCircle2,
  CircleDollarSign,
  Clock3,
  LoaderCircle,
  RefreshCw,
  TrendingUp,
  UsersRound,
  Wallet,
} from "lucide-react";
import {
  Cell,
  CartesianGrid,
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

const PERIODS: { value: DashboardPeriod; label: string }[] = [
  { value: 1, label: "امروز" },
  { value: 2, label: "دیروز" },
  { value: 3, label: "۷ روز گذشته" },
  { value: 4, label: "ماه جاری" },
];

const CHART_COLORS = ["#c026d3", "#0ea5a4", "#f59e0b", "#6366f1", "#22c55e"];
const numberFormatter = new Intl.NumberFormat("fa-IR");
const compactNumberFormatter = new Intl.NumberFormat("fa-IR", {
  notation: "compact",
  maximumFractionDigits: 1,
});

function formatNumber(value: number) {
  return numberFormatter.format(value);
}

function formatMoney(value: number) {
  return `${formatNumber(value)} تومان`;
}

function formatCompactMoney(value: number) {
  return `${compactNumberFormatter.format(value)} تومان`;
}

function formatDate(value: string) {
  const [year, month, day] = value.split("-").map(Number);
  if (!year || !month || !day) return value;
  return new Intl.DateTimeFormat("fa-IR", {
    month: "short",
    day: "numeric",
  }).format(new Date(year, month - 1, day));
}

function formatTime(value: string) {
  return new Intl.DateTimeFormat("fa-IR", {
    hour: "2-digit",
    minute: "2-digit",
    timeZone: "Asia/Tehran",
  }).format(new Date(value));
}

function appointmentStatusLabel(status: string) {
  const labels: Record<string, string> = {
    Pending: "در انتظار",
    Confirmed: "تأییدشده",
    Completed: "انجام‌شده",
    Cancelled: "لغوشده",
    NoShow: "عدم مراجعه",
  };
  return labels[status] ?? status;
}

function Panel({
  title,
  description,
  icon,
  action,
  children,
  className = "",
}: {
  title: string;
  description?: string;
  icon: ReactNode;
  action?: ReactNode;
  children: ReactNode;
  className?: string;
}) {
  return (
    <section
      className={`min-w-0 overflow-hidden rounded-2xl border border-slate-200/80 bg-white shadow-sm ${className}`}
    >
      <header className="flex items-start justify-between gap-3 border-b border-slate-100 px-4 py-3.5 sm:px-5">
        <div className="flex min-w-0 items-center gap-2.5">
          <span className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-fuchsia-50 text-fuchsia-600">
            {icon}
          </span>
          <div className="min-w-0">
            <h2 className="truncate text-sm font-bold text-slate-800">{title}</h2>
            {description && (
              <p className="mt-1 text-xs text-slate-400">{description}</p>
            )}
          </div>
        </div>
        {action}
      </header>
      <div className="p-4 sm:p-5">{children}</div>
    </section>
  );
}

function MetricCard({
  title,
  value,
  note,
  icon,
  color,
  tint,
}: {
  title: string;
  value: string;
  note: string;
  icon: React.ReactNode;
  color: string;
  tint: string;
}) {
  return (
    <article
      className={`flex min-h-28 items-center justify-between gap-3 rounded-2xl border border-slate-200/70 bg-gradient-to-br ${tint} p-4 shadow-sm sm:p-5`}
    >
      <div className="min-w-0">
        <p className="text-xs font-medium text-slate-500 sm:text-sm">{title}</p>
        <p className={`mt-2 truncate text-xl font-extrabold sm:text-2xl ${color}`}>
          {value}
        </p>
        <p className="mt-1 text-[11px] text-slate-400 sm:text-xs">{note}</p>
      </div>
      <span
        className={`flex size-10 shrink-0 items-center justify-center rounded-xl bg-white/80 ${color}`}
      >
        {icon}
      </span>
    </article>
  );
}

function SummaryStat({
  label,
  value,
  icon,
  color,
}: {
  label: string;
  value: number | string;
  icon: ReactNode;
  color: string;
}) {
  return (
    <div className="flex min-w-0 items-center gap-3 border-slate-100 px-3 py-2.5 [&:not(:last-child)]:border-l">
      <span className={`flex size-8 shrink-0 items-center justify-center rounded-lg ${color}`}>
        {icon}
      </span>
      <div className="min-w-0">
        <p className="truncate text-[11px] text-slate-400">{label}</p>
        <p className="mt-0.5 text-sm font-bold text-slate-700">
          {typeof value === "number" ? formatNumber(value) : value}
        </p>
      </div>
    </div>
  );
}

function DashboardLoading() {
  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4" aria-label="در حال بارگذاری داشبورد">
      {Array.from({ length: 8 }, (_, index) => (
        <div
          key={index}
          className="h-32 animate-pulse rounded-2xl border border-slate-100 bg-white"
        />
      ))}
    </div>
  );
}

function EmptyState({ message }: { message: string }) {
  return (
    <div className="flex min-h-32 flex-col items-center justify-center gap-2 text-center text-sm text-slate-400">
      <CalendarDays aria-hidden="true" className="size-6 text-slate-300" />
      <p>{message}</p>
    </div>
  );
}

function ErrorState({ error, onRetry }: { error: string; onRetry: () => void }) {
  return (
    <div
      role="alert"
      className="flex flex-col items-center justify-center gap-3 rounded-2xl border border-rose-200 bg-rose-50 px-5 py-10 text-center"
    >
      <AlertCircle aria-hidden="true" className="size-8 text-rose-500" />
      <div>
        <p className="font-bold text-rose-800">دریافت اطلاعات داشبورد ناموفق بود</p>
        <p className="mt-1 text-sm text-rose-700">{error}</p>
      </div>
      <Button variant="outline" onClick={onRetry} className="border-rose-200 bg-white">
        تلاش دوباره
      </Button>
    </div>
  );
}

function AppointmentList({
  appointments,
}: {
  appointments: DashboardAppointment[];
}) {
  if (appointments.length === 0) {
    return <EmptyState message="برای امروز نوبتی ثبت نشده است." />;
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[560px] text-right text-xs">
        <thead>
          <tr className="border-b border-slate-100 text-slate-400">
            <th className="pb-3 font-medium">ساعت</th>
            <th className="pb-3 font-medium">مشتری</th>
            <th className="pb-3 font-medium">خدمت</th>
            <th className="pb-3 font-medium">پرسنل</th>
            <th className="pb-3 font-medium">وضعیت</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-50">
          {appointments.map((appointment) => (
            <tr key={appointment.id} className="text-slate-600">
              <td className="whitespace-nowrap py-3 font-semibold text-slate-700" dir="ltr">
                {formatTime(appointment.startsAt)}
              </td>
              <td className="max-w-32 truncate py-3 font-medium text-slate-800">
                {appointment.customerName}
              </td>
              <td className="max-w-36 truncate py-3">{appointment.serviceName}</td>
              <td className="max-w-32 truncate py-3">{appointment.personnelName}</td>
              <td className="py-3">
                <span className="inline-flex whitespace-nowrap rounded-full bg-slate-100 px-2.5 py-1 text-[10px] text-slate-600">
                  {appointmentStatusLabel(appointment.status)}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function AlertsList({ alerts }: { alerts: DashboardAlert[] }) {
  if (alerts.length === 0) {
    return (
      <div className="flex min-h-32 flex-col items-center justify-center gap-2 text-center">
        <CheckCircle2 aria-hidden="true" className="size-7 text-emerald-500" />
        <p className="text-sm font-medium text-slate-500">موردی برای پیگیری ندارید.</p>
      </div>
    );
  }

  return (
    <ul className="space-y-3">
      {alerts.map((alert) => {
        const tone =
          alert.type === 2
            ? "border-rose-200 bg-rose-50 text-rose-600"
            : alert.type === 3
              ? "border-amber-200 bg-amber-50 text-amber-600"
              : "border-sky-200 bg-sky-50 text-sky-600";
        const AlertIcon = alert.type === 2 ? CircleDollarSign : AlertCircle;

        return (
          <li
            key={`${alert.type}-${alert.title}`}
            className={`rounded-xl border p-3 ${tone}`}
          >
            <div className="flex items-start gap-2.5">
              <AlertIcon aria-hidden="true" className="mt-0.5 size-4 shrink-0" />
              <div className="min-w-0 flex-1">
                <p className="text-xs font-semibold leading-5">{alert.title}</p>
                <div className="mt-1 flex flex-wrap items-center gap-x-3 gap-y-1 text-[11px] opacity-80">
                  <span>{formatNumber(alert.count)} مورد</span>
                  {alert.amount !== null && <span>{formatMoney(alert.amount)}</span>}
                </div>
              </div>
            </div>
          </li>
        );
      })}
    </ul>
  );
}

function DashboardContent({
  summary,
  charts,
  appointments,
  alerts,
}: {
  summary: DashboardSummary;
  charts: DashboardCharts;
  appointments: DashboardTodayAppointments;
  alerts: DashboardAlert[];
}) {
  return (
    <>
      <section aria-label="خلاصه عملکرد" className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
        <MetricCard
          title="پذیرش‌ها"
          value={formatNumber(summary.receptionCount)}
          note={`${formatNumber(summary.completedReceptionCount)} پذیرش تکمیل‌شده`}
          icon={<CalendarClock className="size-5" />}
          color="text-fuchsia-700"
          tint="from-fuchsia-50/90 to-white"
        />
        <MetricCard
          title="مشتریان"
          value={formatNumber(summary.uniqueCustomerCount)}
          note="مشتری یکتای این بازه"
          icon={<UsersRound className="size-5" />}
          color="text-sky-700"
          tint="from-sky-50/90 to-white"
        />
        <MetricCard
          title="درآمد خدمات"
          value={formatMoney(summary.serviceFinalAmount)}
          note={`${formatMoney(summary.paidAmount)} دریافت‌شده`}
          icon={<Banknote className="size-5" />}
          color="text-emerald-700"
          tint="from-emerald-50/90 to-white"
        />
        <MetricCard
          title="مانده دریافتنی"
          value={formatMoney(summary.outstandingAmount)}
          note={`${formatNumber(summary.appointmentReceptionCount)} پذیرش با نوبت`}
          icon={<Wallet className="size-5" />}
          color="text-amber-700"
          tint="from-amber-50/90 to-white"
        />
      </section>

      <section
        aria-label="جزئیات عملکرد"
        className="grid grid-cols-2 divide-x divide-x-reverse divide-slate-100 rounded-2xl border border-slate-200/80 bg-white shadow-sm sm:grid-cols-4"
      >
        <SummaryStat
          label="در حال انجام"
          value={summary.inProgressReceptionCount}
          icon={<Clock3 className="size-4" />}
          color="bg-sky-50 text-sky-600"
        />
        <SummaryStat
          label="لغوشده"
          value={summary.cancelledReceptionCount}
          icon={<AlertCircle className="size-4" />}
          color="bg-rose-50 text-rose-600"
        />
        <SummaryStat
          label="مراجعه حضوری"
          value={summary.walkInReceptionCount}
          icon={<BriefcaseBusiness className="size-4" />}
          color="bg-violet-50 text-violet-600"
        />
        <SummaryStat
          label="تخفیف خدمات"
          value={formatMoney(summary.discountAmount)}
          icon={<TrendingUp className="size-4" />}
          color="bg-amber-50 text-amber-600"
        />
      </section>

      <div className="grid items-start gap-4 xl:grid-cols-12">
        <Panel
          title="نوبت‌های امروز"
          description={`${formatNumber(appointments.totalCount)} نوبت`}
          icon={<CalendarDays className="size-4" />}
          className="xl:col-span-8"
        >
          <AppointmentList appointments={appointments.appointments} />
        </Panel>
        <Panel
          title="موارد نیازمند پیگیری"
          description={`${formatNumber(alerts.reduce((sum, alert) => sum + alert.count, 0))} مورد`}
          icon={<AlertCircle className="size-4" />}
          className="xl:col-span-4"
        >
          <AlertsList alerts={alerts} />
        </Panel>
      </div>

      <div className="grid items-stretch gap-4 xl:grid-cols-12">
        <Panel
          title="روند درآمد و دریافت"
          description="مقایسه مبلغ خدمات با دریافتی"
          icon={<TrendingUp className="size-4" />}
          className="xl:col-span-7"
        >
          {charts.financialTrend.length === 0 ? (
            <EmptyState message="داده‌ای برای نمایش روند مالی وجود ندارد." />
          ) : (
            <>
              <div className="mb-3 flex flex-wrap gap-4 text-[11px] text-slate-500">
                <span className="inline-flex items-center gap-2">
                  <span className="size-2 rounded-full bg-fuchsia-500" />
                  مبلغ خدمات
                </span>
                <span className="inline-flex items-center gap-2">
                  <span className="size-2 rounded-full bg-teal-500" />
                  دریافتی
                </span>
              </div>
              <div className="h-60 w-full" dir="ltr">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={charts.financialTrend}
                    margin={{ top: 8, right: 12, left: 8, bottom: 0 }}
                  >
                    <CartesianGrid stroke="#eef2f7" strokeDasharray="3 3" vertical={false} />
                    <XAxis
                      dataKey="date"
                      tickFormatter={formatDate}
                      tick={{ fill: "#94a3b8", fontSize: 10 }}
                      axisLine={false}
                      tickLine={false}
                    />
                    <YAxis
                      tickFormatter={formatCompactMoney}
                      tick={{ fill: "#94a3b8", fontSize: 10 }}
                      axisLine={false}
                      tickLine={false}
                      width={64}
                    />
                    <Tooltip
                      labelFormatter={(label) => formatDate(String(label))}
                      formatter={(value, name) => [
                        formatMoney(Number(value ?? 0)),
                        name === "serviceFinalAmount" ? "مبلغ خدمات" : "دریافتی",
                      ]}
                      contentStyle={{
                        border: "1px solid #e2e8f0",
                        borderRadius: 12,
                        fontSize: 12,
                      }}
                    />
                    <Line
                      type="monotone"
                      dataKey="serviceFinalAmount"
                      name="serviceFinalAmount"
                      stroke="#c026d3"
                      strokeWidth={2.5}
                      dot={{ r: 3, fill: "#c026d3" }}
                      activeDot={{ r: 5 }}
                    />
                    <Line
                      type="monotone"
                      dataKey="paidAmount"
                      name="paidAmount"
                      stroke="#0ea5a4"
                      strokeWidth={2.5}
                      dot={{ r: 3, fill: "#0ea5a4" }}
                      activeDot={{ r: 5 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </>
          )}
        </Panel>

        <Panel
          title="خدمات پرطرفدار"
          description="بر اساس مبلغ خدمات تکمیل‌شده"
          icon={<TrendingUp className="size-4" />}
          className="xl:col-span-5"
        >
          {charts.popularServices.length === 0 ? (
            <EmptyState message="خدمتی در این بازه ثبت نشده است." />
          ) : (
            <div className="grid items-center gap-3 sm:grid-cols-[minmax(130px,0.9fr)_1.1fr]">
              <div className="h-48 min-w-0" dir="ltr">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={charts.popularServices}
                      dataKey="serviceAmount"
                      nameKey="serviceName"
                      innerRadius="58%"
                      outerRadius="84%"
                      paddingAngle={3}
                      stroke="none"
                    >
                      {charts.popularServices.map((service, index) => (
                        <Cell
                          key={service.serviceId}
                          fill={CHART_COLORS[index % CHART_COLORS.length]}
                        />
                      ))}
                    </Pie>
                    <Tooltip
                      formatter={(value) => formatMoney(Number(value ?? 0))}
                      contentStyle={{
                        border: "1px solid #e2e8f0",
                        borderRadius: 12,
                        fontSize: 12,
                      }}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
              <ul className="space-y-3">
                {charts.popularServices.map((service, index) => (
                  <li
                    key={service.serviceId}
                    className="flex min-w-0 items-start gap-2.5 text-xs"
                  >
                    <span
                      className="mt-1 size-2 shrink-0 rounded-full"
                      style={{
                        backgroundColor: CHART_COLORS[index % CHART_COLORS.length],
                      }}
                    />
                    <span className="min-w-0 flex-1">
                      <span className="block truncate font-medium text-slate-700">
                        {service.serviceName}
                      </span>
                      <span className="mt-1 block text-slate-400">
                        {formatNumber(service.completedServiceCount)} خدمت
                      </span>
                    </span>
                    <span className="whitespace-nowrap font-semibold text-slate-600">
                      {formatCompactMoney(service.serviceAmount)}
                    </span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </Panel>
      </div>

      <div className="grid items-stretch gap-4 xl:grid-cols-12">
        <Panel
          title="عملکرد پرسنل"
          description="خدمات تکمیل‌شده در بازه انتخابی"
          icon={<UsersRound className="size-4" />}
          className="xl:col-span-7"
        >
          {charts.personnelPerformance.length === 0 ? (
            <EmptyState message="اطلاعات عملکرد پرسنل در دسترس نیست." />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full min-w-[420px] text-right text-xs">
                <thead>
                  <tr className="border-b border-slate-100 text-slate-400">
                    <th className="pb-3 font-medium">پرسنل</th>
                    <th className="pb-3 font-medium">خدمات تکمیل‌شده</th>
                    <th className="pb-3 font-medium">مبلغ خدمات</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {charts.personnelPerformance.map((person) => (
                    <tr key={person.personnelId} className="text-slate-600">
                      <td className="py-3 font-medium text-slate-800">
                        {person.personnelName ?? "بدون نام"}
                      </td>
                      <td className="py-3">{formatNumber(person.completedServiceCount)}</td>
                      <td className="py-3 font-semibold">{formatMoney(person.serviceAmount)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </Panel>
        <Panel
          title="وضعیت مالی"
          description="خلاصهٔ مبالغ این بازه"
          icon={<CircleDollarSign className="size-4" />}
          className="xl:col-span-5"
        >
          <div className="space-y-4">
            <FinanceRow label="مبلغ خدمات" amount={summary.serviceFinalAmount} />
            <FinanceRow label="دریافت‌شده" amount={summary.paidAmount} tone="text-emerald-700" />
            <FinanceRow label="مانده دریافتنی" amount={summary.outstandingAmount} tone="text-amber-700" />
            <FinanceRow label="هزینه ثبت‌شده" amount={charts.recordedExpenses} tone="text-slate-600" />
            <FinanceRow label="هزینه لغوشده" amount={charts.cancelledExpenses} tone="text-slate-400" />
          </div>
        </Panel>
      </div>
    </>
  );
}

function FinanceRow({
  label,
  amount,
  tone = "text-slate-700",
}: {
  label: string;
  amount: number;
  tone?: string;
}) {
  return (
    <div className="flex items-center justify-between gap-3 border-b border-slate-100 pb-3 last:border-0 last:pb-0">
      <span className="text-xs text-slate-500">{label}</span>
      <span className={`text-xs font-bold ${tone}`}>{formatMoney(amount)}</span>
    </div>
  );
}

export function DashboardPage() {
  const [period, setPeriod] = useState<DashboardPeriod>(1);
  const { data, error, loading, refresh } = useDashboard(period);

  return (
    <main className="min-h-[calc(100vh-4rem)] bg-slate-50/80">
      <div className="mx-auto max-w-[1600px] space-y-4 px-4 py-5 sm:px-6 sm:py-7 lg:px-8">
        <header className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <p className="text-xs font-medium text-fuchsia-600">مدیریت سالن</p>
            <h1 className="mt-1 text-xl font-extrabold text-slate-900 sm:text-2xl">
              داشبورد مدیریت سالن
            </h1>
            <p className="mt-1 text-xs text-slate-500 sm:text-sm">
              نمای کلی عملکرد و فعالیت‌های سالن در بازه انتخابی
            </p>
          </div>
          <div className="flex items-center gap-2 self-start sm:self-auto">
            <Select
              value={String(period)}
              onValueChange={(value) => {
                const nextPeriod = PERIODS.find(
                  (item) => String(item.value) === value,
                );
                if (nextPeriod) setPeriod(nextPeriod.value);
              }}
            >
              <SelectTrigger aria-label="بازه زمانی داشبورد" className="min-w-36 border-slate-200 bg-white">
                <CalendarDays aria-hidden="true" className="size-4 text-fuchsia-600" />
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
              variant="outline"
              size="icon"
              aria-label="به‌روزرسانی داشبورد"
              onClick={refresh}
              disabled={loading}
              className="border-slate-200 bg-white text-slate-600"
            >
              <RefreshCw className={`size-4 ${loading ? "animate-spin" : ""}`} />
            </Button>
          </div>
        </header>

        {loading ? (
          <DashboardLoading />
        ) : error ? (
          <ErrorState error={error} onRetry={refresh} />
        ) : data ? (
          <DashboardContent
            summary={data.summary}
            charts={data.charts}
            appointments={data.appointments}
            alerts={data.alerts}
          />
        ) : (
          <div className="flex min-h-48 items-center justify-center text-sm text-slate-500">
            <LoaderCircle aria-hidden="true" className="ms-2 size-4 animate-spin" />
            در حال آماده‌سازی داشبورد…
          </div>
        )}
      </div>
    </main>
  );
}
