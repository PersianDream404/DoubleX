import { z } from "zod";

export const ServiceTariffFormSchema = z.object({
  name: z.string().trim().min(1, "نام تعرفه الزامی است.").max(200, "نام تعرفه حداکثر ۲۰۰ نویسه است."),
  validFrom: z.string().regex(/^\d{4}-\d{2}-\d{2}T00:00:00\+03:30$/, "تاریخ شروع اعتبار را وارد کنید."),
  validTo: z.string().nullable(),
}).superRefine((values, context) => {
  if (values.validTo && values.validTo <= values.validFrom) {
    context.addIssue({
      code: "custom",
      path: ["validTo"],
      message: "پایان اعتبار باید بعد از شروع اعتبار باشد.",
    });
  }
});

export const TariffServiceFormSchema = z.object({
  serviceId: z.number().int().positive("انتخاب خدمت الزامی است."),
  amount: z.number()
    .min(0, "قیمت نمی‌تواند منفی باشد.")
    .max(9999999999999999.99, "مبلغ بیشتر از حد مجاز است.")
    .refine((amount) => Math.round(amount * 100) === amount * 100, {
      message: "مبلغ حداکثر دو رقم اعشار دارد.",
    }),
});
