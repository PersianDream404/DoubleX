import { z } from "zod";

export const MaterialFormSchema = z.object({
  name: z.string().trim().min(1, "نام کالا الزامی است").max(200),
  code: z.string().trim().min(1, "کد کالا الزامی است").max(50),
  brand: z.string().trim().max(100).nullable(),
  baseUnitId: z.number().int().positive("واحد پایه را انتخاب کنید"),
  purchaseUnitId: z.number().int().positive("واحد خرید را انتخاب کنید"),
  purchaseConversionFactor: z
    .number()
    .positive("ضریب تبدیل باید بیشتر از صفر باشد")
    .max(999999999999.999999)
    .refine((value) => Math.round(value * 1_000_000) / 1_000_000 === value, {
      message: "ضریب تبدیل حداکثر ۶ رقم اعشار دارد",
    }),
  minimumStock: z
    .number()
    .nonnegative("حداقل موجودی نمی‌تواند منفی باشد")
    .max(999999999999999.999)
    .refine((value) => Math.round(value * 1_000) / 1_000 === value, {
      message: "حداقل موجودی حداکثر ۳ رقم اعشار دارد",
    }),
});

export const MaterialListQuerySchema = z.object({
  q: z.string().trim(),
  pageNumber: z.number().int().positive(),
});

export type MaterialFormValues = z.infer<typeof MaterialFormSchema>;
