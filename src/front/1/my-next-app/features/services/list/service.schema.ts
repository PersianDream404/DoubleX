import { z } from "zod";

export const ServiceListQuerySchema = z.object({
  q: z.string().trim().max(250),
});

export const ServiceFormSchema = z.object({
  categoryId: z.number().int().positive("انتخاب دسته‌بندی الزامی است."),
  code: z.string().trim().min(1, "کد خدمت الزامی است.").max(50),
  name: z.string().trim().min(1, "نام خدمت الزامی است.").max(200),
  description: z.string().max(1000, "توضیحات بیش از حد طولانی است."),
  durationMinutes: z.number().int().positive("مدت زمان باید بیشتر از صفر باشد."),
});

export const ServiceMaterialFormSchema = z.object({
  materialId: z.number().int().positive("انتخاب ماده مصرفی الزامی است."),
  quantityPerService: z
    .number()
    .positive("مقدار مصرف باید بیشتر از صفر باشد.")
    .max(999999999999999.999)
    .refine((value) => Math.round(value * 1000) === value * 1000, {
      message: "مقدار مصرف حداکثر سه رقم اعشار دارد.",
    }),
  description: z.string().max(1000, "توضیحات بیش از حد طولانی است."),
});
