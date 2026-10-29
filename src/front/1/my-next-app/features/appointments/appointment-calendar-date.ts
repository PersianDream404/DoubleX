import {
  isValidJalaaliDate,
  jalaaliMonthLength,
  toGregorian,
  toJalaali,
} from "jalaali-js";

import { jalaliDateToApi } from "@/features/services/tariffs/service-tariff-date";

export type JalaliDateParts = { year: number; month: number; day: number };

export function parseJalaliDate(value: string): JalaliDateParts {
  const normalized = value.replace(/[۰-۹٠-٩]/g, (digit) => {
    const persianIndex = "۰۱۲۳۴۵۶۷۸۹".indexOf(digit);
    return String(persianIndex >= 0 ? persianIndex : "٠١٢٣٤٥٦٧٨٩".indexOf(digit));
  });
  const match = /^(\d{4})\/(\d{1,2})\/(\d{1,2})$/.exec(normalized);
  if (!match) throw new Error("تاریخ جلالی باید به شکل سال/ماه/روز باشد.");

  const [, yearText, monthText, dayText] = match;
  const parts = {
    year: Number(yearText),
    month: Number(monthText),
    day: Number(dayText),
  };
  if (!isValidJalaaliDate(parts.year, parts.month, parts.day)) {
    throw new Error("تاریخ جلالی انتخاب‌شده معتبر نیست.");
  }
  return parts;
}

export function formatJalaliDate({ year, month, day }: JalaliDateParts) {
  return `${year}/${String(month).padStart(2, "0")}/${String(day).padStart(2, "0")}`;
}

export function addJalaliDays(value: string, days: number) {
  const { year, month, day } = parseJalaliDate(value);
  const gregorian = toGregorian(year, month, day);
  const date = new Date(Date.UTC(gregorian.gy, gregorian.gm - 1, gregorian.gd + days));
  const jalali = toJalaali(date.getUTCFullYear(), date.getUTCMonth() + 1, date.getUTCDate());
  return formatJalaliDate({ year: jalali.jy, month: jalali.jm, day: jalali.jd });
}

export function addJalaliMonths(value: string, months: number) {
  const { year, month, day } = parseJalaliDate(value);
  const absoluteMonth = year * 12 + month - 1 + months;
  const nextYear = Math.floor(absoluteMonth / 12);
  const nextMonth = (absoluteMonth % 12) + 1;
  return formatJalaliDate({
    year: nextYear,
    month: nextMonth,
    day: Math.min(day, jalaaliMonthLength(nextYear, nextMonth)),
  });
}

export function tehranTodayJalali() {
  const parts = new Intl.DateTimeFormat("en-US-u-ca-gregory", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(new Date());
  const value = (type: Intl.DateTimeFormatPartTypes) =>
    Number(parts.find((part) => part.type === type)?.value);
  const jalali = toJalaali(value("year"), value("month"), value("day"));
  return formatJalaliDate({ year: jalali.jy, month: jalali.jm, day: jalali.jd });
}

export function jalaliDateRange(date: string, view: "day" | "month") {
  let start = date;
  let end = addJalaliDays(date, 1);
  if (view === "month") {
    const monthGrid = jalaliMonthGrid(date);
    start = monthGrid[0];
    end = addJalaliDays(monthGrid[monthGrid.length - 1], 1);
  }
  const from = jalaliDateToApi(start);
  const to = jalaliDateToApi(end);
  if (!from || !to) throw new Error("تاریخ تقویم معتبر نیست.");
  return { from, to };
}

export function jalaliMonthGrid(date: string) {
  const { year, month } = parseJalaliDate(date);
  const firstDay = formatJalaliDate({ year, month, day: 1 });
  const gregorian = toGregorian(year, month, 1);
  const saturdayOffset = (new Date(Date.UTC(gregorian.gy, gregorian.gm - 1, gregorian.gd)).getUTCDay() + 1) % 7;
  return Array.from({ length: 42 }, (_, index) =>
    addJalaliDays(firstDay, index - saturdayOffset),
  );
}

export function tehranClockParts(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return { hour: 0, minute: 0 };
  const parts = new Intl.DateTimeFormat("en-GB", {
    timeZone: "Asia/Tehran",
    hour: "2-digit",
    minute: "2-digit",
    hourCycle: "h23",
  }).formatToParts(date);
  return {
    hour: Number(parts.find((part) => part.type === "hour")?.value),
    minute: Number(parts.find((part) => part.type === "minute")?.value),
  };
}

export function tehranJalaliDate(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  const parts = new Intl.DateTimeFormat("en-US-u-ca-gregory", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(date);
  const part = (type: Intl.DateTimeFormatPartTypes) =>
    Number(parts.find((item) => item.type === type)?.value);
  const jalali = toJalaali(part("year"), part("month"), part("day"));
  return formatJalaliDate({ year: jalali.jy, month: jalali.jm, day: jalali.jd });
}

export function jalaliDateLabel(value: string, options: Intl.DateTimeFormatOptions = {}) {
  const { year, month, day } = parseJalaliDate(value);
  const gregorian = toGregorian(year, month, day);
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    timeZone: "UTC",
    ...options,
  }).format(new Date(Date.UTC(gregorian.gy, gregorian.gm - 1, gregorian.gd, 12)));
}

export function jalaliMonthLabel(value: string) {
  const { year, month, day } = parseJalaliDate(value);
  const gregorian = toGregorian(year, month, day);
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    timeZone: "UTC",
    month: "long",
    year: "numeric",
  }).format(new Date(Date.UTC(gregorian.gy, gregorian.gm - 1, 15, 12)));
}

export function tehranTimeLabel(hour: number, minute: number) {
  const date = new Date(Date.UTC(2020, 0, 1, hour, minute));
  return new Intl.DateTimeFormat("fa-IR", {
    timeZone: "UTC",
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  }).format(date);
}

export function jalaliMonthDay(value: string) {
  return parseJalaliDate(value).day;
}

export function isSameJalaliMonth(left: string, right: string) {
  const a = parseJalaliDate(left);
  const b = parseJalaliDate(right);
  return a.year === b.year && a.month === b.month;
}
