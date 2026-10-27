import { z } from "zod";

export const ReceptionListQuerySchema = z.object({
  pageNumber: z.number().int().positive(),
  pageSize: z.number().int().min(1).max(100),
});

export const ReceptionServiceFormSchema = z.object({
  serviceId: z.number().int().positive(),
  serviceTariffId: z.number().int().positive(),
  personnelId: z.number().int().positive().nullable(),
  quantity: z.number().positive(),
  discountAmount: z.number().nonnegative(),
  unitPrice: z.number().nonnegative(),
  materials: z.array(
    z.object({
      materialId: z.number().int().positive(),
      quantity: z.number().positive(),
      description: z.string().max(1000),
    }),
  ),
});

export const ReceptionPaymentFormSchema = z.object({
  amount: z.number().positive(),
  paymentMethod: z.string().trim().min(1).max(50),
  transactionReference: z.string().max(150),
  description: z.string().max(1000),
});

export const CreateReceptionFormSchema = z
  .object({
    salonId: z.number().int().positive(),
    customerId: z.number().int().positive(),
    receptionistId: z.number().int().positive(),
    serviceTariffId: z.number().int().positive(),
    notes: z.string().max(2000),
    services: z.array(ReceptionServiceFormSchema).min(1),
    payments: z.array(ReceptionPaymentFormSchema),
  })
  .superRefine((form, context) => {
    let totalAmount = 0;
    form.services.forEach((service, index) => {
      const serviceAmount = service.quantity * service.unitPrice;
      if (service.discountAmount > serviceAmount) {
        context.addIssue({
          code: "custom",
          message: "مبلغ تخفیف از مبلغ خدمت بیشتر است.",
          path: ["services", index, "discountAmount"],
        });
      }
      totalAmount += serviceAmount - service.discountAmount;
      if (service.serviceTariffId !== form.serviceTariffId) {
        context.addIssue({
          code: "custom",
          message: "تعرفه همه خدمات باید با تعرفه انتخاب‌شده برای پذیرش یکسان باشد.",
          path: ["services", index, "serviceTariffId"],
        });
      }
    });

    const paymentAmount = form.payments.reduce(
      (total, payment) => total + payment.amount,
      0,
    );
    if (paymentAmount > totalAmount) {
      context.addIssue({
        code: "custom",
        message: "مجموع پرداختی از مبلغ نهایی پذیرش بیشتر است.",
        path: ["payments"],
      });
    }
  });

export type ReceptionServiceForm = z.infer<typeof ReceptionServiceFormSchema>;
export type ReceptionPaymentForm = z.infer<typeof ReceptionPaymentFormSchema>;
