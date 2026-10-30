import { isValidJalaaliDate } from "jalaali-js";
import { z } from "zod";

import { normalizeDigits } from "./reception-report-date";

const optionalPositiveInteger = z.preprocess(
  (value) => (value === "" || value === undefined ? undefined : Number(value)),
  z.number().int().positive().optional(),
);

const optionalNonNegativeAmount = z.preprocess(
  (value) => (value === "" || value === undefined ? undefined : Number(value)),
  z.number().finite().nonnegative().optional(),
);

const optionalEnumNumber = (maximum: number) =>
  z.preprocess(
    (value) => (value === "" || value === undefined ? undefined : Number(value)),
    z.number().int().min(1).max(maximum).optional(),
  );

const jalaliDate = z.string().refine((value) => {
  const [year, month, day] = normalizeDigits(value).split("/").map(Number);
  return isValidJalaaliDate(year, month, day);
}, "تاریخ شمسی معتبر نیست.");

export const receptionReportFilterSchema = z
  .object({
    fromDate: jalaliDate,
    toDate: jalaliDate,
    customerSearch: z.string().trim().max(250, "عبارت جستجو نباید بیشتر از ۲۵۰ نویسه باشد."),
    customerId: optionalPositiveInteger,
    receptionNumber: optionalPositiveInteger,
    personnelId: optionalPositiveInteger,
    serviceId: optionalPositiveInteger,
    materialId: optionalPositiveInteger,
    receptionStatus: optionalEnumNumber(4),
    financialStatus: optionalEnumNumber(3),
    serviceStatus: optionalEnumNumber(4),
    hasAppointment: z.enum(["", "true", "false"]),
    minimumRemainingAmount: optionalNonNegativeAmount,
    maximumRemainingAmount: optionalNonNegativeAmount,
  })
  .superRefine((value, context) => {
    const from = normalizeDigits(value.fromDate);
    const to = normalizeDigits(value.toDate);
    if (from > to) {
      context.addIssue({
        code: "custom",
        path: ["toDate"],
        message: "تاریخ پایان باید بعد از تاریخ شروع باشد.",
      });
    }

    if (
      value.minimumRemainingAmount !== undefined &&
      value.maximumRemainingAmount !== undefined &&
      value.minimumRemainingAmount > value.maximumRemainingAmount
    ) {
      context.addIssue({
        code: "custom",
        path: ["maximumRemainingAmount"],
        message: "حداکثر بدهی باید برابر یا بیشتر از حداقل بدهی باشد.",
      });
    }
  });

export type ReceptionReportFormValues = z.infer<
  typeof receptionReportFilterSchema
>;
