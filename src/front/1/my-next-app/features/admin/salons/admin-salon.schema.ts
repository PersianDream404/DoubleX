import { z } from "zod";

export const CreateSalonWithAdminSchema = z.object({
  salonName: z.string().trim().min(1, "نام سالن الزامی است").max(200),
  adminFullName: z.string().trim().min(1, "نام مدیر سالن الزامی است").max(200),
  adminMobile: z.string().trim().min(5, "شماره همراه معتبر نیست"),
  adminPassword: z.string().min(3, "رمز عبور باید حداقل ۳ کاراکتر باشد"),
});

export type CreateSalonWithAdminForm = z.infer<
  typeof CreateSalonWithAdminSchema
>;
