"use client";

import { CalendarClock, CircleX } from "lucide-react";

import { Button } from "@/components/ui/button";
import type { PersonnelDto } from "@/features/personnel/list/personnel.types";

import {
  isSameJalaliMonth,
  jalaliDateLabel,
  jalaliMonthDay,
  jalaliMonthGrid,
  tehranClockParts,
  tehranJalaliDate,
  tehranTimeLabel,
} from "./appointment-calendar-date";
import type { AppointmentListItemDto, AppointmentStatus } from "./appointment.types";

const DAY_START_HOUR = 9;
const DAY_END_HOUR = 21;
const PIXELS_PER_MINUTE = 2;
const SLOT_MINUTES = 10;

const STATUS_CLASSES: Record<AppointmentStatus, string> = {
  1: "border-fuchsia-300 bg-pink-100 text-slate-800 hover:bg-pink-200",
  2: "border-fuchsia-400 bg-pink-100 text-slate-800 hover:bg-pink-200",
  3: "border-sky-300 bg-sky-50 text-slate-700 hover:bg-sky-100",
  4: "border-slate-300 bg-slate-100 text-slate-500 hover:bg-slate-200",
  5: "border-amber-300 bg-amber-50 text-slate-700 hover:bg-amber-100",
};

function formatAmount(value: number) {
  return `${new Intl.NumberFormat("fa-IR").format(value)} تومان`;
}

function durationLabel(appointment: AppointmentListItemDto) {
  const duration = Math.max(
    0,
    Math.round((new Date(appointment.endsAt).getTime() - new Date(appointment.startsAt).getTime()) / 60_000),
  );
  return `${new Intl.NumberFormat("fa-IR").format(duration)} دقیقه`;
}

export type SchedulePersonnel = Pick<PersonnelDto, "id" | "fullName" | "jobTitle">;

type Props = {
  appointments: AppointmentListItemDto[];
  personnel: SchedulePersonnel[];
  selectedDate: string;
  mode: "day" | "month";
  busyId: number | null;
  onSelectDate: (date: string) => void;
  onEdit: (appointment: AppointmentListItemDto) => void;
  onCancel: (appointment: AppointmentListItemDto) => void;
};

export function AppointmentCalendar({
  appointments,
  personnel,
  selectedDate,
  mode,
  busyId,
  onSelectDate,
  onEdit,
  onCancel,
}: Props) {
  if (mode === "month") {
    return (
      <MonthCalendar
        appointments={appointments}
        selectedDate={selectedDate}
        onSelectDate={onSelectDate}
        onEdit={onEdit}
      />
    );
  }

  const schedulePersonnel = [...personnel];
  for (const appointment of appointments) {
    if (!schedulePersonnel.some((item) => item.id === appointment.personnelId)) {
      schedulePersonnel.push({
        id: appointment.personnelId,
        fullName: appointment.personnelName,
        jobTitle: "پرسنل نوبت",
      });
    }
  }

  const appointmentsByPersonnel = new Map<number, AppointmentListItemDto[]>();
  for (const appointment of appointments) {
    const key = appointment.personnelId;
    appointmentsByPersonnel.set(key, [...(appointmentsByPersonnel.get(key) ?? []), appointment]);
  }

  const scheduleStart = Math.min(
    DAY_START_HOUR * 60,
    ...appointments.map((appointment) => {
      const time = tehranClockParts(appointment.startsAt);
      return Math.floor((time.hour * 60 + time.minute) / 60) * 60;
    }),
  );
  const scheduleEnd = Math.max(
    DAY_END_HOUR * 60,
    ...appointments.map((appointment) => {
      const time = tehranClockParts(appointment.endsAt);
      return Math.ceil((time.hour * 60 + time.minute) / 60) * 60;
    }),
  );
  const totalMinutes = scheduleEnd - scheduleStart;
  const rowCount = Math.ceil(totalMinutes / SLOT_MINUTES);
  const gridTemplateColumns = `70px repeat(${Math.max(schedulePersonnel.length, 1)}, minmax(180px, 1fr))`;

  return (
    <div className="max-h-[calc(100dvh-180px)] overflow-auto rounded-b-md border border-slate-200 bg-white shadow-sm">
      {schedulePersonnel.length === 0 ? (
        <div className="flex min-h-56 flex-col items-center justify-center gap-2 px-4 text-center">
          <CalendarClock className="size-8 text-slate-300" />
          <p className="font-medium text-slate-700">پرسنل فعالی برای نمایش تقویم وجود ندارد.</p>
        </div>
      ) : (
        <div className="min-w-full" style={{ minWidth: `${70 + schedulePersonnel.length * 180}px` }}>
          <div
            className="sticky top-0 z-20 grid border-b border-slate-300 bg-white"
            style={{ gridTemplateColumns }}
          >
            <div className="flex h-[52px] items-center justify-center border-l border-slate-200 bg-slate-50 text-[11px] font-medium text-slate-500">
              ساعت
            </div>
            {schedulePersonnel.map((person) => (
              <div
                key={person.id}
                className="flex h-[52px] min-w-0 flex-col items-center justify-center border-l border-slate-300 px-2 text-center last:border-l-0"
              >
                <span className="max-w-full truncate text-xs font-semibold text-slate-800">{person.fullName}</span>
                <span className="mt-1 max-w-full truncate text-[10px] text-slate-500">{person.jobTitle}</span>
              </div>
            ))}
          </div>

          <div className="grid" style={{ gridTemplateColumns }}>
            <div className="relative border-l border-slate-300 bg-white" style={{ height: totalMinutes * PIXELS_PER_MINUTE }}>
              {Array.from({ length: rowCount + 1 }, (_, index) => {
                const minute = scheduleStart + index * SLOT_MINUTES;
                if (minute % 30 !== 0) return null;
                return (
                  <div
                    key={minute}
                    className="absolute inset-x-0 flex -translate-y-1/2 justify-center bg-white px-1 text-[10px] font-medium text-slate-700"
                    style={{ top: (minute - scheduleStart) * PIXELS_PER_MINUTE }}
                  >
                    {tehranTimeLabel(Math.floor(minute / 60), minute % 60)}
                  </div>
                );
              })}
            </div>

            {schedulePersonnel.map((person) => {
              const items = (appointmentsByPersonnel.get(person.id) ?? [])
                .filter((appointment) => tehranJalaliDate(appointment.startsAt) === selectedDate)
                .sort((left, right) => left.startsAt.localeCompare(right.startsAt));
              return (
                <div
                  key={person.id}
                  className="relative border-l border-slate-300 last:border-l-0"
                  style={{
                    height: totalMinutes * PIXELS_PER_MINUTE,
                    backgroundColor: "#fafafa",
                    backgroundImage: "repeating-linear-gradient(to bottom, transparent 0, transparent 19px, #e5e7eb 19px, #e5e7eb 20px)",
                  }}
                >
                  {items.map((appointment) => {
                    const start = tehranClockParts(appointment.startsAt);
                    const end = tehranClockParts(appointment.endsAt);
                    const startMinute = start.hour * 60 + start.minute;
                    let endMinute = end.hour * 60 + end.minute;
                    if (endMinute <= startMinute) endMinute += 24 * 60;
                    const top = (startMinute - scheduleStart) * PIXELS_PER_MINUTE;
                    const height = Math.max((endMinute - startMinute) * PIXELS_PER_MINUTE, 38);
                    return (
                      <article
                        key={appointment.id}
                        className={`group absolute inset-x-0.5 z-10 overflow-hidden border-r-2 px-2 py-1.5 text-right transition-colors ${STATUS_CLASSES[appointment.status]}`}
                        style={{
                          top: Math.max(0, top),
                          height: Math.max(0, Math.min(height, totalMinutes * PIXELS_PER_MINUTE - Math.max(0, top))),
                        }}
                      >
                        <button
                          type="button"
                          onClick={() => onEdit(appointment)}
                          className="block h-full w-full overflow-hidden text-right outline-none focus-visible:ring-2 focus-visible:ring-fuchsia-500"
                          aria-label={`ویرایش نوبت ${appointment.customerName}، ${appointment.serviceName}، ${tehranTimeLabel(start.hour, start.minute)}`}
                        >
                          <span className="block truncate text-[11px] font-semibold leading-5">
                            {tehranTimeLabel(start.hour, start.minute)} تا {tehranTimeLabel(end.hour, end.minute)}
                          </span>
                          <span className="block truncate text-[11px] font-semibold leading-5">{appointment.serviceName}</span>
                          <span className="block truncate text-[10px] leading-5">{appointment.customerName}</span>
                          {height >= 75 && (
                            <span className="block truncate text-[10px] leading-5 text-slate-600">
                              {durationLabel(appointment)} · {formatAmount(appointment.finalAmount)}
                            </span>
                          )}
                        </button>
                        {(appointment.status === 1 || appointment.status === 2) && (
                          <Button
                            type="button"
                            variant="ghost"
                            size="icon-sm"
                            disabled={busyId === appointment.id}
                            aria-label={`لغو نوبت ${appointment.customerName}`}
                            title="لغو نوبت"
                            onClick={() => onCancel(appointment)}
                            className="absolute left-1 top-1 size-6 rounded-full bg-white/70 text-rose-700 opacity-100 hover:bg-white md:opacity-0 md:group-hover:opacity-100 md:group-focus-within:opacity-100"
                          >
                            {busyId === appointment.id
                              ? <span className="size-3 animate-spin rounded-full border-2 border-current border-t-transparent" />
                              : <CircleX className="size-3.5" />}
                          </Button>
                        )}
                      </article>
                    );
                  })}
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}

function MonthCalendar({
  appointments,
  selectedDate,
  onSelectDate,
  onEdit,
}: {
  appointments: AppointmentListItemDto[];
  selectedDate: string;
  onSelectDate: (date: string) => void;
  onEdit: (appointment: AppointmentListItemDto) => void;
}) {
  const dates = jalaliMonthGrid(selectedDate);
  const weekDays = ["شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه"];
  return (
    <div className="overflow-hidden rounded-b-md border border-slate-200 bg-white shadow-sm">
      <div className="grid grid-cols-7 border-b border-slate-300 bg-slate-50">
        {weekDays.map((day) => (
          <div key={day} className="flex h-11 items-center justify-center border-l border-slate-200 text-xs font-medium text-slate-600 last:border-l-0">
            {day}
          </div>
        ))}
      </div>
      <div className="grid grid-cols-7">
        {dates.map((date, index) => {
          const inMonth = isSameJalaliMonth(date, selectedDate);
          const dayAppointments = appointments
            .filter((appointment) => tehranJalaliDate(appointment.startsAt) === date)
            .sort((left, right) => left.startsAt.localeCompare(right.startsAt));
          const isSelected = date === selectedDate;
          return (
            <div
              key={`${date}-${index}`}
              className={`min-h-28 border-b border-l border-slate-200 p-1.5 sm:min-h-32 sm:p-2 ${!inMonth ? "bg-slate-50/80" : "bg-white"}`}
            >
              <button
                type="button"
                onClick={() => onSelectDate(date)}
                className={`mb-1 flex size-7 items-center justify-center rounded-full text-xs transition ${
                  isSelected
                    ? "bg-fuchsia-700 font-semibold text-white"
                    : inMonth
                      ? "text-slate-700 hover:bg-fuchsia-50 hover:text-fuchsia-700"
                      : "text-slate-400 hover:bg-white"
                }`}
                aria-label={`انتخاب ${jalaliDateLabel(date, { dateStyle: "full" })}`}
                aria-pressed={isSelected}
              >
                {new Intl.NumberFormat("fa-IR").format(jalaliMonthDay(date))}
              </button>
              <div className="space-y-1">
                {dayAppointments.slice(0, 3).map((appointment) => {
                  const time = tehranClockParts(appointment.startsAt);
                  return (
                    <button
                      key={appointment.id}
                      type="button"
                      onClick={() => onEdit(appointment)}
                      className={`block w-full truncate rounded px-1.5 py-1 text-right text-[10px] ${
                        appointment.status === 4 ? "bg-slate-100 text-slate-500" : "bg-pink-100 text-slate-700 hover:bg-pink-200"
                      }`}
                      aria-label={`ویرایش نوبت ${appointment.customerName}، ${appointment.serviceName}`}
                    >
                      {tehranTimeLabel(time.hour, time.minute)} · {appointment.customerName}
                    </button>
                  );
                })}
                {dayAppointments.length > 3 && (
                  <button
                    type="button"
                    onClick={() => onSelectDate(date)}
                    className="w-full text-right text-[10px] text-fuchsia-700 hover:underline"
                  >
                    {new Intl.NumberFormat("fa-IR").format(dayAppointments.length - 3)} نوبت دیگر
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
