"use client";

import { useEffect, useMemo, useState } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import {
  CalendarDays,
  ChevronLeft,
  ChevronRight,
  LoaderCircle,
  Plus,
  RefreshCw,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { AppointmentCalendar } from "./appointment-calendar";
import {
  addJalaliDays,
  addJalaliMonths,
  jalaliDateLabel,
  jalaliDateRange,
  jalaliMonthLabel,
  tehranTodayJalali,
} from "./appointment-calendar-date";
import { AppointmentFormDialog } from "./appointment-form-dialog";
import { AppointmentService } from "./appointment.service";
import { useAppointments } from "./use-appointments";
import { handleApiError } from "@/lib/api/handle-api-error";
import type { PersonnelDto } from "@/features/personnel/list/personnel.types";
import type { ServiceDto } from "@/features/services/list/service.types";
import type { AppointmentListItemDto } from "./appointment.types";

type ViewMode = "day" | "month";

function AppointmentDatePicker({
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
          className="h-9 min-w-[142px] justify-between rounded-md border-slate-200 bg-white px-2.5 text-[11px] font-medium text-slate-700 hover:bg-slate-50"
          aria-label="انتخاب تاریخ تقویم"
        >
          <CalendarDays className="size-4 text-slate-500" />
          {jalaliDateLabel(value, { dateStyle: "full" })}
        </Button>
      )}
    />
  );
}

export function AppointmentsCalendarPage({
  initialDialogOpen = false,
}: {
  initialDialogOpen?: boolean;
}) {
  const [selectedDate, setSelectedDate] = useState(tehranTodayJalali);
  const [mode, setMode] = useState<ViewMode>("day");
  const [serviceFilter, setServiceFilter] = useState("");
  const [refreshKey, setRefreshKey] = useState(0);
  const [services, setServices] = useState<ServiceDto[]>([]);
  const [personnel, setPersonnel] = useState<PersonnelDto[]>([]);
  const [optionsLoading, setOptionsLoading] = useState(true);
  const [optionsError, setOptionsError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(initialDialogOpen);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [busyId, setBusyId] = useState<number | null>(null);

  const range = useMemo(() => jalaliDateRange(selectedDate, mode), [mode, selectedDate]);
  const { appointments, loading, error } = useAppointments(
    refreshKey,
    range.from,
    range.to,
    serviceFilter ? Number(serviceFilter) : null,
  );

  useEffect(() => {
    const controller = new AbortController();
    void Promise.all([
      new AppointmentService().getActiveServices(controller.signal),
      new AppointmentService().getAllPersonnel(controller.signal),
    ])
      .then(([activeServices, activePersonnel]) => {
        if (controller.signal.aborted) return;
        setServices(activeServices);
        setPersonnel(activePersonnel);
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) setOptionsError(handleApiError(cause));
      })
      .finally(() => {
        if (!controller.signal.aborted) setOptionsLoading(false);
      });
    return () => controller.abort();
  }, []);

  function openCreate() {
    setEditingId(null);
    setDialogOpen(true);
    setActionError(null);
  }

  function openEdit(appointment: AppointmentListItemDto) {
    setEditingId(appointment.id);
    setDialogOpen(true);
    setActionError(null);
  }

  async function cancelAppointment(appointment: AppointmentListItemDto) {
    if (!window.confirm(`نوبت ${appointment.customerName} لغو شود؟`)) return;
    setBusyId(appointment.id);
    setActionError(null);
    try {
      const response = await new AppointmentService().changeStatus(appointment.id, 4);
      if (!response.data) throw new Error(response.message ?? "لغو نوبت از سرور تأیید نشد.");
      setNotice("نوبت با موفقیت لغو شد.");
      setRefreshKey((key) => key + 1);
    } catch (cause: unknown) {
      setActionError(handleApiError(cause));
    } finally {
      setBusyId(null);
    }
  }

  function handleSaved(message: string, appointmentDate: string) {
    setNotice(message);
    setActionError(null);
    setSelectedDate(appointmentDate);
    setMode("day");
    setRefreshKey((key) => key + 1);
  }

  function changeDate(direction: -1 | 1) {
    setSelectedDate((current) =>
      mode === "day"
        ? addJalaliDays(current, direction)
        : addJalaliMonths(current, direction),
    );
  }

  function selectCalendarDate(date: string) {
    setSelectedDate(date);
    setMode("day");
  }

  return (
    <main dir="rtl" className="relative min-h-[calc(100vh-4rem)] overflow-hidden bg-white">
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-x-0 top-0 h-[280px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]"
      />
      <div className="relative mx-auto w-full max-w-[1240px] px-3 sm:px-5">
        <div className="flex h-9 items-center border-b border-fuchsia-900/70 px-2 text-[11px] font-medium text-white">
          مدیریت رزرو و نوبت
        </div>

        <div className="flex min-h-[60px] flex-wrap items-center justify-between gap-2 py-2">
          <div className="flex flex-wrap items-center gap-2">
            <div className="flex h-9 overflow-hidden rounded-md border border-slate-200 bg-white">
              <Button
                type="button"
                variant="ghost"
                size="icon-sm"
                className="h-9 w-8 rounded-none border-l border-slate-200"
                onClick={() => changeDate(-1)}
                aria-label={mode === "day" ? "روز قبل" : "ماه قبل"}
              >
                <ChevronRight className="size-4" />
              </Button>
              <Button
                type="button"
                variant="ghost"
                className="h-9 rounded-none px-3 text-[11px] text-slate-700"
                onClick={() => setSelectedDate(tehranTodayJalali())}
              >
                امروز
              </Button>
              <AppointmentDatePicker value={selectedDate} onChange={setSelectedDate} />
              <Button
                type="button"
                variant="ghost"
                size="icon-sm"
                className="h-9 w-8 rounded-none border-r border-slate-200"
                onClick={() => changeDate(1)}
                aria-label={mode === "day" ? "روز بعد" : "ماه بعد"}
              >
                <ChevronLeft className="size-4" />
              </Button>
            </div>

            <div className="flex h-9 overflow-hidden rounded-md border border-slate-200 bg-white">
              <Button
                type="button"
                variant="ghost"
                aria-pressed={mode === "month"}
                className={`h-9 rounded-none px-3 text-[11px] ${mode === "month" ? "bg-fuchsia-700 text-white hover:bg-fuchsia-800 hover:text-white" : "text-slate-600"}`}
                onClick={() => setMode("month")}
              >
                ماهانه
              </Button>
              <Button
                type="button"
                variant="ghost"
                aria-pressed={mode === "day"}
                className={`h-9 rounded-none px-3 text-[11px] ${mode === "day" ? "bg-fuchsia-700 text-white hover:bg-fuchsia-800 hover:text-white" : "text-slate-600"}`}
                onClick={() => setMode("day")}
              >
                روزانه
              </Button>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-2">
            <label className="sr-only" htmlFor="appointment-service-filter">فیلتر نوع خدمت</label>
            <select
              id="appointment-service-filter"
              value={serviceFilter}
              onChange={(event) => setServiceFilter(event.target.value)}
              className="h-9 min-w-[140px] rounded-md border border-slate-200 bg-white px-2.5 text-[11px] text-slate-700 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-2 focus-visible:ring-fuchsia-200"
            >
              <option value="">نوع خدمت</option>
              {services.map((service) => (
                <option key={service.id} value={service.id}>{service.name}</option>
              ))}
            </select>
            <Button
              type="button"
              onClick={openCreate}
              className="h-9 rounded-md bg-fuchsia-700 px-3 text-[11px] text-white hover:bg-fuchsia-800"
            >
              <Plus className="ml-1 size-3.5" />
              رزرو نوبت جدید
            </Button>
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              onClick={() => setRefreshKey((key) => key + 1)}
              disabled={loading}
              className="h-9 w-9 rounded-md text-white hover:bg-white/10 hover:text-white"
              aria-label="تازه‌سازی نوبت‌ها"
              title="تازه‌سازی"
            >
              <RefreshCw className={`size-4 ${loading ? "animate-spin" : ""}`} />
            </Button>
          </div>
        </div>

        <div className="mb-2 flex items-center justify-between gap-3 text-xs text-white/85">
          <span>{mode === "day" ? jalaliDateLabel(selectedDate, { dateStyle: "full" }) : jalaliMonthLabel(selectedDate)}</span>
          <span className="hidden sm:inline">برای مشاهده جزئیات یا ویرایش، روی نوبت کلیک کنید.</span>
        </div>

        {(notice || actionError || optionsError) && (
          <div
            role={actionError || optionsError ? "alert" : "status"}
            className={`mb-2 flex items-center justify-between gap-3 rounded-md px-3 py-2 text-xs ${
              actionError || optionsError
                ? "border border-rose-200 bg-rose-50 text-rose-700"
                : "border border-emerald-200 bg-emerald-50 text-emerald-700"
            }`}
          >
            <span>{actionError ?? optionsError ?? notice}</span>
            <button
              type="button"
              onClick={() => {
                setActionError(null);
                setNotice(null);
              }}
              className="px-1 text-base leading-none"
              aria-label="بستن پیام"
            >
              ×
            </button>
          </div>
        )}

        {error ? (
          <div className="flex min-h-56 flex-col items-center justify-center gap-3 rounded-md border border-rose-200 bg-white px-5 text-center">
            <p className="text-sm leading-6 text-rose-700" role="alert">{error}</p>
            <Button variant="outline" onClick={() => setRefreshKey((key) => key + 1)} className="rounded-md">
              تلاش دوباره
            </Button>
          </div>
        ) : loading || optionsLoading ? (
          <div className="flex min-h-56 items-center justify-center gap-3 rounded-md border border-slate-200 bg-white text-sm text-slate-500">
            <LoaderCircle className="size-5 animate-spin" />
            در حال دریافت تقویم نوبت‌ها...
          </div>
        ) : (
          <>
            <AppointmentCalendar
              appointments={appointments}
              personnel={personnel}
              selectedDate={selectedDate}
              mode={mode}
              busyId={busyId}
              onSelectDate={selectCalendarDate}
              onEdit={openEdit}
              onCancel={cancelAppointment}
            />
          </>
        )}
      </div>

      <AppointmentFormDialog
        key={`${dialogOpen ? "open" : "closed"}-${editingId ?? "new"}`}
        open={dialogOpen}
        appointmentId={editingId}
        onOpenChange={(nextOpen) => {
          setDialogOpen(nextOpen);
          if (!nextOpen) setEditingId(null);
        }}
        onSaved={handleSaved}
      />
    </main>
  );
}
