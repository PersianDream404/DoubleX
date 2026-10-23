import { z } from "zod";

export const CreateSalonSchema = z.object({
  name: z.string().trim().min(1, "نام سالن الزامی است"),
});

export const UpdateSalonSchema = CreateSalonSchema.extend({
  id: z.number().int().positive(),
});

export type CreateSalonForm = z.infer<typeof CreateSalonSchema>;
export type UpdateSalonForm = z.infer<typeof UpdateSalonSchema>;
