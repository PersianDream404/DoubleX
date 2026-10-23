import { isValidJalaaliDate, toGregorian } from "jalaali-js";

function normalizeDigits(value: string): string {
  return value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));
}

export function jalaliToGregorianDate(
  value?: string,
  endExclusive = false,
): Date | undefined {
  if (!value?.trim()) return undefined;

  const normalized = normalizeDigits(value.trim());
  const match = normalized.match(/^(\d{4})\/(\d{1,2})\/(\d{1,2})$/);

  if (!match) return undefined;

  const [, year, month, day] = match;
  const jy = Number(year);
  const jm = Number(month);
  const jd = Number(day);

  if (!isValidJalaaliDate(jy, jm, jd)) return undefined;

  const { gy, gm, gd } = toGregorian(jy, jm, jd);

  const date = new Date(Date.UTC(gy, gm - 1, gd));

  // برای تاریخ پایان، روز بعد را مرز انحصاری قرار می‌دهیم.
  if (endExclusive) {
    date.setUTCDate(date.getUTCDate() + 1);
  }

  return date;
}
