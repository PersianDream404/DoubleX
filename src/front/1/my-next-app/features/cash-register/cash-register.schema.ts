import { z } from "zod";

export const CashRegisterListQuerySchema = z.object({
  pageNumber: z.number().int().positive(),
  pageSize: z.number().int().min(1).max(100),
  q: z.string().max(150).optional(),
  isActive: z.boolean().optional(),
});

export const CashRegisterFormSchema = z.object({
  name: z.string().trim().min(1, "نام صندوق الزامی است.").max(150),
  code: z
    .string()
    .trim()
    .min(1, "کد صندوق الزامی است.")
    .max(50)
    .regex(/^[A-Za-z0-9_-]+$/, "کد صندوق فقط می‌تواند شامل حروف انگلیسی، عدد، خط تیره و زیرخط باشد."),
  description: z.string().max(1000),
});

export const OpeningBalanceSchema = z.object({
  openingBalance: z
    .string()
    .trim()
    .regex(/^\d+(\.\d{1,2})?$/, "موجودی اولیه را با حداکثر دو رقم اعشار وارد کنید.")
    .transform(Number)
    .pipe(z.number().finite().min(0).max(9999999999999999.99)),
});

export const ClosingBalanceSchema = z.object({
  actualClosingBalance: z
    .string()
    .trim()
    .regex(/^\d+(\.\d{1,2})?$/, "موجودی شمارش‌شده را با حداکثر دو رقم اعشار وارد کنید.")
    .transform(Number)
    .pipe(z.number().finite().min(0).max(9999999999999999.99)),
});

export const CashMovementSchema = z.object({
  amount: z
    .string()
    .trim()
    .regex(/^\d+(\.\d{1,2})?$/, "مبلغ را با حداکثر دو رقم اعشار وارد کنید.")
    .transform(Number)
    .pipe(z.number().finite().positive("مبلغ باید بیشتر از صفر باشد.").max(9999999999999999.99)),
  description: z.string().trim().min(1, "شرح گردش الزامی است.").max(1000),
});

export type CashRegisterFormValues = z.infer<typeof CashRegisterFormSchema>;
