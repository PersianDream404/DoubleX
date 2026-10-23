import { z } from "zod";

export const PersonnelListQuerySchema = z.object({
  q: z.string().trim(),
  pageNumber: z.number().int().positive(),
});

export type PersonnelListQuery = z.infer<typeof PersonnelListQuerySchema>;
