import { z } from "zod";

export const UnitFormSchema = z.object({
  name: z.string().trim().min(1, "نام واحد الزامی است").max(100),
  symbol: z.string().trim().min(1, "نماد واحد الزامی است").max(20),
  unitType: z.union([z.literal(1), z.literal(2), z.literal(3)]),
});

export const UnitListQuerySchema = z.object({
  q: z.string().trim(),
  pageNumber: z.number().int().positive(),
});

export type UnitFormValues = z.infer<typeof UnitFormSchema>;
export type UnitListQuery = z.infer<typeof UnitListQuerySchema>;
