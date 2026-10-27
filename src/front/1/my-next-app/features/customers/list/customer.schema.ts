import { isValidJalaaliDate, toGregorian } from "jalaali-js";
import { z } from "zod";

export const CustomerListQuerySchema = z.object({
  pageNumber: z.number().int().positive(),
  pageSize: z.number().int().min(1).max(100),
  q: z.string().max(250).optional(),
});

export const CustomerFormSchema = z.object({
  fullName: z.string().trim().min(1, "نام و نام خانوادگی الزامی است.").max(250),
  birthDate: z
    .string()
    .trim()
    .regex(/^\d{4}\/\d{2}\/\d{2}$/, "تاریخ تولد را انتخاب کنید.")
    .refine((value) => {
      const [year, month, day] = value.split("/").map(Number);
      return isValidJalaaliDate(year, month, day);
    }, "تاریخ تولد معتبر نیست.")
    .refine((value) => {
      const [year, month, day] = value.split("/").map(Number);
      const gregorian = toGregorian(year, month, day);
      const date = `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(gregorian.gd).padStart(2, "0")}`;
      const today = new Date();
      const todayDate = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, "0")}-${String(today.getDate()).padStart(2, "0")}`;
      return date < todayDate;
    }, "تاریخ تولد باید قبل از امروز باشد."),
  nationalCode: z
    .string()
    .trim()
    .regex(/^\d{10}$/, "کد ملی باید دقیقاً ۱۰ رقم باشد."),
  address: z.string().trim().min(1, "نشانی الزامی است.").max(1000),
  allergyDescription: z.string().max(1000),
  jobTitle: z.string().max(150),
  landlinePhone: z.string().max(20),
  mobilePhone: z.string().trim().min(1, "شماره همراه الزامی است.").max(20),
});

export type CustomerFormValues = z.infer<typeof CustomerFormSchema>;
