"use client";

import { useActionState, useEffect } from "react";
import { useRouter } from "next/navigation";

import { CompanyPaymentDto } from "@/app/DTOs/payments/payment.dto";
import {
  PaymentUpdateActionState,
  updatePaymentAction,
} from "@/actions/companies/payments/company.payment.update.actions";
import { CompanyOrderDto } from "@/app/DTOs/orders/order.dto";

type EditPaymentFormProps = {
  payment: CompanyPaymentDto;
  companyId: number;
  orders: CompanyOrderDto[];
  onSuccess: () => void;
};

export function EditPaymentForm({
  payment,
  companyId,
  orders,
  onSuccess,
}: EditPaymentFormProps) {
  const router = useRouter();

  const initialState: PaymentUpdateActionState = {
    success: false,

    values: {
      id: String(payment.id),
      orderId: String(payment.orderId),
      amount: String(payment.amount),
      status: payment.status,
      method: payment.method,
      companyId: String(companyId),
    },
  };

  const [state, formAction, pending] = useActionState(
    updatePaymentAction,
    initialState,
  );

  useEffect(() => {
    if (state.success) {
      onSuccess();
      router.refresh();
    }
  }, [state.success, onSuccess, router]);

  return (
    <form action={formAction} className="space-y-5">
      <input type="hidden" name="id" value={state.values?.id ?? payment.id} />

      <input
        type="hidden"
        name="companyId"
        value={state.values?.companyId ?? companyId}
      />

      {/* Order */}
      <div className="space-y-2">
        <label
          htmlFor={`orderId-${payment.id}`}
          className="text-sm font-medium"
        >
          سفارش
        </label>

        <select
          id={`orderId-${payment.id}`}
          name="orderId"
          defaultValue={state.values?.orderId ?? ""}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="">انتخاب سفارش...</option>

          {orders.map((order) => (
            <option key={order.id} value={order.id}>
              سفارش #{order.id} - {order.customerName} -{" "}
              {order.totalPrice?.toLocaleString("fa-IR")} تومان
            </option>
          ))}
        </select>

        {state.errors?.orderId && (
          <p className="text-sm text-destructive">{state.errors.orderId[0]}</p>
        )}
      </div>

      {/* Amount */}
      <div className="space-y-2">
        <label htmlFor={`amount-${payment.id}`} className="text-sm font-medium">
          مبلغ پرداخت
        </label>

        <input
          id={`amount-${payment.id}`}
          name="amount"
          type="number"
          min={1}
          defaultValue={state.values?.amount ?? ""}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        />

        {state.errors?.amount && (
          <p className="text-sm text-destructive">{state.errors.amount[0]}</p>
        )}
      </div>

      {/* Method */}
      <div className="space-y-2">
        <label htmlFor={`method-${payment.id}`} className="text-sm font-medium">
          روش پرداخت
        </label>

        <select
          id={`method-${payment.id}`}
          name="method"
          defaultValue={state.values?.method ?? "Cash"}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="Cash">نقدی</option>
          <option value="Card">کارت</option>
          <option value="Transfer">انتقال بانکی</option>
        </select>

        {state.errors?.method && (
          <p className="text-sm text-destructive">{state.errors.method[0]}</p>
        )}
      </div>

      {/* Status */}
      <div className="space-y-2">
        <label htmlFor={`status-${payment.id}`} className="text-sm font-medium">
          وضعیت
        </label>

        <select
          id={`status-${payment.id}`}
          name="status"
          defaultValue={state.values?.status ?? "Pending"}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="Pending">در انتظار</option>
          <option value="Completed">تکمیل شده</option>
          <option value="Failed">ناموفق</option>
        </select>

        {state.errors?.status && (
          <p className="text-sm text-destructive">{state.errors.status[0]}</p>
        )}
      </div>

      <div className="flex justify-end">
        <button
          type="submit"
          disabled={pending}
          className="rounded-md bg-primary px-4 py-2 text-sm text-primary-foreground disabled:opacity-50"
        >
          {pending ? "در حال ذخیره..." : "ذخیره تغییرات"}
        </button>
      </div>
    </form>
  );
}
