import { z } from "zod";
import { isValidJalaaliDate } from "jalaali-js";

import { jalaliDateToApi } from "@/features/services/tariffs/service-tariff-date";

const persianDigitMap = "۰۱۲۳۴۵۶۷۸۹";

export function normalizeDigits(value: string) {
  return value.replace(/[۰-۹٠-٩]/g, (digit) => {
    const persianIndex = persianDigitMap.indexOf(digit);
    if (persianIndex >= 0) return String(persianIndex);
    return String("٠١٢٣٤٥٦٧٨٩".indexOf(digit));
  });
}

export const CreateAppointmentFormSchema = z.object({
  customerId: z.number().int().positive("مشتری را انتخاب کنید."),
  serviceId: z.number().int().positive("خدمت را انتخاب کنید."),
  serviceTariffId: z.number().int().positive("برای این خدمت تعرفهٔ معتبر وجود ندارد."),
  personnelId: z.number().int().positive("پرسنل را انتخاب کنید."),
  date: z.string().min(1, "تاریخ نوبت را انتخاب کنید."),
  time: z.string().regex(/^\d{2}:\d{2}$/, "ساعت نوبت را وارد کنید."),
  discountAmount: z.number().nonnegative("مبلغ تخفیف نمی‌تواند منفی باشد."),
  notes: z.string().max(1000, "توضیحات حداکثر ۱۰۰۰ نویسه باشد."),
  notificationChannel: z.union([z.literal(0), z.literal(1), z.literal(2)]),
}).superRefine((form, context) => {
  const date = normalizeDigits(form.date);
  const match = /^(\d{4})\/(\d{1,2})\/(\d{1,2})$/.exec(date);
  if (!match || !isValidJalaaliDate(Number(match[1]), Number(match[2]), Number(match[3]))) {
    context.addIssue({
      code: z.ZodIssueCode.custom,
      path: ["date"],
      message: "تاریخ جلالی معتبر نیست.",
    });
  } else if (!jalaliDateToApi(date)) {
    context.addIssue({
      code: z.ZodIssueCode.custom,
      path: ["date"],
      message: "تاریخ نوبت معتبر نیست.",
    });
  }
});

export const UpdateAppointmentFormSchema = z.object({
  personnelId: z.number().int().positive("پرسنل را انتخاب کنید."),
  date: z.string().min(1, "تاریخ نوبت را انتخاب کنید."),
  time: z.string().regex(/^\d{2}:\d{2}$/, "ساعت نوبت را وارد کنید."),
  notes: z.string().max(1000, "توضیحات حداکثر ۱۰۰۰ نویسه باشد."),
});
