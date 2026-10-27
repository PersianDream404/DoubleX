import { z } from "zod";

export const ServiceCategoryFormSchema = z.object({
  name: z.string().trim().min(1, "نام دسته‌بندی الزامی است.").max(200, "نام دسته‌بندی حداکثر ۲۰۰ نویسه است."),
  description: z.string().max(1000, "توضیحات حداکثر ۱۰۰۰ نویسه است."),
});
