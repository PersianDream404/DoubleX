import { z } from "zod";

export const IdentityUserIdSchema = z
  .string()
  .trim()
  .regex(/^[0-9]+$/, "شناسه حساب باید عددی باشد.")
  .transform(Number)
  .refine(Number.isSafeInteger, "شناسه حساب معتبر نیست.")
  .refine((value) => value > 0, "شناسه حساب باید بزرگ‌تر از صفر باشد.");
