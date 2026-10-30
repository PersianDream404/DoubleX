import { z } from "zod";

export const StockMovementQuantitySchema = z.object({
  quantity: z
    .number()
    .positive("مقدار باید بیشتر از صفر باشد")
    .max(999999999999999, "مقدار واردشده بیش از حد مجاز است"),
  purchaseUnitPrice: z
    .number()
    .nonnegative("قیمت نمی‌تواند منفی باشد")
    .max(9999999999999999, "قیمت واردشده بیش از حد مجاز است")
    .refine((value) => Math.round(value * 100) / 100 === value, "قیمت حداکثر دو رقم اعشار دارد"),
  description: z.string().trim().max(1000, "توضیحات حداکثر ۱۰۰۰ نویسه است"),
});

export type StockMovementQuantityValues = z.infer<
  typeof StockMovementQuantitySchema
>;
