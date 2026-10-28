import { z } from "zod";
import { isValidJalaaliDate } from "jalaali-js";

const normalizeDigits = (value: string) =>
  value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));

export function isValidJalaliDate(value: string) {
  const normalized = normalizeDigits(value);
  if (!/^\d{4}\/\d{2}\/\d{2}$/.test(normalized)) return false;
  const [year, month, day] = normalized.split("/").map(Number);
  return isValidJalaaliDate(year, month, day);
}

export const FixedSalarySchema = z
  .object({
    personnelId: z.number().int().positive("پرسنل را انتخاب کنید."),
    periodStart: z.string().refine(isValidJalaliDate, "تاریخ شروع معتبر نیست."),
    periodEnd: z.string().refine(isValidJalaliDate, "تاریخ پایان معتبر نیست."),
  })
  .refine(
    ({ periodStart, periodEnd }) =>
      normalizeDigits(periodStart) <= normalizeDigits(periodEnd),
    {
      path: ["periodEnd"],
      message: "تاریخ پایان باید بعد از تاریخ شروع باشد.",
    },
  );

export const ReceptionEarningsSchema = z.object({
  receptionId: z
    .string()
    .trim()
    .regex(/^[1-9]\d*$/, "شناسه پذیرش باید عددی مثبت باشد.")
    .transform(Number),
});

export const PersonnelSettlementSchema = z.object({
  personnelId: z.number().int().positive("پرسنل را انتخاب کنید."),
  earningType: z.union([z.literal(1), z.literal(2)]),
  amount: z
    .string()
    .trim()
    .regex(/^\d+(\.\d{1,2})?$/, "مبلغ را با حداکثر دو رقم اعشار وارد کنید.")
    .transform(Number)
    .refine((amount) => amount > 0, "مبلغ تسویه باید بیشتر از صفر باشد."),
  paymentMethod: z
    .string()
    .trim()
    .min(1, "روش پرداخت را وارد کنید.")
    .max(50, "روش پرداخت حداکثر ۵۰ نویسه است."),
  description: z.string().trim().max(1000, "توضیحات بیش از حد طولانی است."),
});
