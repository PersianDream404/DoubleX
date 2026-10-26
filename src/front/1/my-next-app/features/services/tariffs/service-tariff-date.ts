import { isValidJalaaliDate, toGregorian, toJalaali } from "jalaali-js";

function toEnglishDigits(value: string): string {
  return value.replace(/[۰-۹٠-٩]/g, (digit) => {
    const persianDigit = "۰۱۲۳۴۵۶۷۸۹".indexOf(digit);
    if (persianDigit >= 0) return String(persianDigit);
    return String("٠١٢٣٤٥٦٧٨٩".indexOf(digit));
  });
}

export function jalaliDateToApi(value: string): string | null {
  const normalizedValue = toEnglishDigits(value.trim());
  const match = /^(\d{4})\/(\d{2})\/(\d{2})$/.exec(normalizedValue);
  if (!match) return null;

  const year = Number(match[1]);
  const month = Number(match[2]);
  const day = Number(match[3]);
  if (!isValidJalaaliDate(year, month, day)) return null;

  const gregorian = toGregorian(year, month, day);
  return `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(gregorian.gd).padStart(2, "0")}T00:00:00+03:30`;
}

export function apiDateToJalali(value: string | null): string {
  if (!value) return "";
  const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
  if (!match) return "";

  const year = Number(match[1]);
  const month = Number(match[2]);
  const day = Number(match[3]);
  const date = new Date(Date.UTC(year, month - 1, day));
  if (
    date.getUTCFullYear() !== year ||
    date.getUTCMonth() + 1 !== month ||
    date.getUTCDate() !== day
  ) return "";

  const jalali = toJalaali(year, month, day);
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`;
}

export function formatApiDate(value: string | null): string {
  return apiDateToJalali(value).replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]) || "—";
}
