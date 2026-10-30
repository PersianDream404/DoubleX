import { toGregorian } from "jalaali-js";

export function normalizeDigits(value: string) {
  return value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));
}

export function currentJalaliDate() {
  const parts = new Intl.DateTimeFormat("en-US-u-ca-persian-nu-latn", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(new Date());
  const part = (type: Intl.DateTimeFormatPartTypes) =>
    parts.find((item) => item.type === type)?.value ?? "";
  return `${part("year")}/${part("month")}/${part("day")}`;
}

function toGregorianDate(value: string) {
  const [year, month, day] = normalizeDigits(value).split("/").map(Number);
  const gregorian = toGregorian(year, month, day);
  return {
    year: gregorian.gy,
    month: gregorian.gm,
    day: gregorian.gd,
  };
}

function tehranOffset(date: { year: number; month: number; day: number }) {
  const noonUtc = new Date(Date.UTC(date.year, date.month - 1, date.day, 12));
  const offsetLabel = new Intl.DateTimeFormat("en-US", {
    timeZone: "Asia/Tehran",
    timeZoneName: "longOffset",
  })
    .formatToParts(noonUtc)
    .find((part) => part.type === "timeZoneName")?.value;

  if (!offsetLabel) throw new Error("منطقه زمانی تهران قابل تشخیص نیست.");
  return offsetLabel === "GMT" ? "+00:00" : offsetLabel.replace(/^GMT/, "");
}

function formatGregorianDate(date: {
  year: number;
  month: number;
  day: number;
}) {
  return `${date.year}-${String(date.month).padStart(2, "0")}-${String(date.day).padStart(2, "0")}`;
}

export function toApiDateTime(value: string) {
  const date = toGregorianDate(value);
  return `${formatGregorianDate(date)}T00:00:00${tehranOffset(date)}`;
}

export function toApiExclusiveEndDate(value: string) {
  const date = toGregorianDate(value);
  const nextDay = new Date(Date.UTC(date.year, date.month - 1, date.day + 1));
  const exclusiveEnd = {
    year: nextDay.getUTCFullYear(),
    month: nextDay.getUTCMonth() + 1,
    day: nextDay.getUTCDate(),
  };
  return `${formatGregorianDate(exclusiveEnd)}T00:00:00${tehranOffset(exclusiveEnd)}`;
}

export function jalaliDateLabel(value: string) {
  const [year, month, day] = value.split("-").map(Number);
  const date = new Date(Date.UTC(year, month - 1, day, 12));
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    timeZone: "Asia/Tehran",
    dateStyle: "short",
  }).format(date);
}
