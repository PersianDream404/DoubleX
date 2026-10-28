import { z } from "zod";
import { isValidJalaaliDate } from "jalaali-js";

const datePattern = /^\d{4}\/\d{2}\/\d{2}$/;
const phonePattern = /^\d{10,11}$/;
const normalizeDigits = (value: string) =>
  value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));

const isDateValid = (value: string) => {
  const normalized = normalizeDigits(value);
  if (!datePattern.test(normalized)) {
    return false;
  }

  const [year, month, day] = normalized.split("/").map(Number);
  return isValidJalaaliDate(year, month, day);
};

export const PersonnelCreateSchema = z.object({
  salonId: z.number().int().positive(),
  status: z.enum(["true", "false"]),
  fullName: z
    .string()
    .trim()
    .min(1, "نام و نام خانوادگی الزامی است.")
    .max(250, "نام و نام خانوادگی بیش از حد طولانی است."),
  nationalCode: z
    .string()
    .regex(/^\d{10}$/, "کد ملی باید ۱۰ رقم باشد."),
  birthDate: z
    .string()
    .refine(isDateValid, "تاریخ تولد را به شکل معتبر ۱۴۰۰/۰۱/۰۱ وارد کنید."),
  maritalStatus: z.enum(["0", "1"]),
  childrenCount: z
    .string()
    .regex(/^\d+$/, "تعداد فرزند باید عدد صحیح نامنفی باشد.")
    .transform(Number),
  mobilePhone: z.string().regex(phonePattern, "شماره همراه معتبر نیست."),
  landlinePhone: z
    .string()
    .refine((phone) => !phone || phonePattern.test(phone), "تلفن ثابت معتبر نیست."),
  addressText: z
    .string()
    .trim()
    .min(1, "آدرس الزامی است.")
    .max(1000, "آدرس بیش از حد طولانی است."),
  isCloseToSalon: z.boolean(),
  jobTitle: z
    .string()
    .trim()
    .min(1, "عنوان شغلی الزامی است.")
    .max(150, "عنوان شغلی بیش از حد طولانی است."),
  personnelCode: z
    .string()
    .trim()
    .min(1, "کد پرسنلی الزامی است.")
    .max(100, "کد پرسنلی بیش از حد طولانی است."),
  cooperationStartDate: z
    .string()
    .refine(
      isDateValid,
      "تاریخ شروع همکاری را به شکل معتبر ۱۴۰۰/۰۱/۰۱ وارد کنید.",
    ),
  workingShiftType: z.string().regex(/^[1-7]$/),
  workHistory: z.string().max(2000, "سوابق کاری بیش از حد طولانی است."),
  salaryType: z.string().regex(/^[0-3]$/),
  cardNumber: z.string().regex(/^\d{16}$/, "شماره کارت باید ۱۶ رقم باشد."),
  accountNumber: z
    .string()
    .trim()
    .min(1, "شماره حساب الزامی است.")
    .max(50, "شماره حساب بیش از حد طولانی است."),
  fixedSalaryAmount: z
    .string()
    .refine(
      (value) => value === "" || /^\d+(\.\d+)?$/.test(value),
      "مبلغ حقوق ثابت نامعتبر است.",
    )
    .default(""),
  commissionRate: z
    .string()
    .refine(
      (value) => value === "" || /^\d+(\.\d+)?$/.test(value),
      "درصد پورسانت نامعتبر است.",
    )
    .default(""),
  effectiveFrom: z
    .string()
    .refine(
      (value) => value === "" || isDateValid(value),
      "تاریخ شروع قرارداد را به شکل معتبر وارد کنید.",
    )
    .default(""),
  effectiveTo: z
    .string()
    .refine(
      (value) => value === "" || isDateValid(value),
      "تاریخ پایان قرارداد نامعتبر است.",
    )
    .default(""),
  healthStatus: z.enum(["true", "false"]),
  diseases: z.string(),
  allergyStatus: z.enum(["true", "false"]),
  allergies: z.string(),
  guarantorName: z
    .string()
    .trim()
    .min(1, "نام معرف یا ضامن الزامی است.")
    .max(200, "نام معرف بیش از حد طولانی است."),
  relationship: z
    .string()
    .trim()
    .min(1, "نسبت با پرسنل الزامی است.")
    .max(150, "نسبت بیش از حد طولانی است."),
  guarantorPhone: z
    .string()
    .regex(phonePattern, "شماره همراه معرف معتبر نیست."),
  guarantorLandlinePhone: z
    .string()
    .regex(phonePattern, "شماره ثابت معرف معتبر نیست."),
});

export type PersonnelCreateForm = z.input<typeof PersonnelCreateSchema>;
