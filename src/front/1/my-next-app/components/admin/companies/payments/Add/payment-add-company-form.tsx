"use client";

import { useActionState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { CompanyOrderDto, OrderSelectDto } from "@/app/DTOs/orders/order.dto";
import {
  createPaymentAction,
  PaymentActionState,
} from "@/actions/companies/payments/company.payment.create.actions";
type AddPaymentFormProps = {
  companyId: number;
  orders: CompanyOrderDto[];
  onSuccess: () => void;
};

const initialState: PaymentActionState = {
  success: false,
};

export function AddPaymentForm({
  companyId,
  orders,
  onSuccess,
}: AddPaymentFormProps) {
  const router = useRouter();

  const [state, formAction, pending] = useActionState(
    createPaymentAction,
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
      {/* Company Id */}
      <input type="hidden" name="companyId" value={companyId} />

      {/* Order */}
      <div className="space-y-2">
        <label
          htmlFor="orderId"
          className="text-sm font-medium"
        >
          سفارش
        </label>

        <select
          id="orderId"
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
          <p className="text-sm text-destructive">
            {state.errors.orderId[0]}
          </p>
        )}
      </div>

      {/* Amount */}
      <div className="space-y-2">
        <label
          htmlFor="amount"
          className="text-sm font-medium"
        >
          مبلغ پرداخت
        </label>

        <input
          id="amount"
          name="amount"
          type="number"
          min={1}
          defaultValue={state.values?.amount ?? ""}
          placeholder="مبلغ را وارد کنید"
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        />

        {state.errors?.amount && (
          <p className="text-sm text-destructive">
            {state.errors.amount[0]}
          </p>
        )}
      </div>

      {/* Method */}
      <div className="space-y-2">
        <label
          htmlFor="method"
          className="text-sm font-medium"
        >
          روش پرداخت
        </label>

        <select
          id="method"
          name="method"
          defaultValue={state.values?.method ?? "Cash"}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="Cash">نقدی</option>
          <option value="Card">کارت</option>
          <option value="Transfer">انتقال بانکی</option>
        </select>

        {state.errors?.method && (
          <p className="text-sm text-destructive">
            {state.errors.method[0]}
          </p>
        )}
      </div>

      {/* Status */}
      <div className="space-y-2">
        <label
          htmlFor="status"
          className="text-sm font-medium"
        >
          وضعیت
        </label>

        <select
          id="status"
          name="status"
          defaultValue={state.values?.status ?? "Pending"}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="Pending">در انتظار</option>
          <option value="Completed">تکمیل شده</option>
          <option value="Failed">ناموفق</option>
        </select>

        {state.errors?.status && (
          <p className="text-sm text-destructive">
            {state.errors.status[0]}
          </p>
        )}
      </div>

      <div className="flex justify-end">
        <button
          type="submit"
          disabled={pending}
          className="rounded-md bg-primary px-4 py-2 text-sm text-primary-foreground disabled:opacity-50"
        >
          {pending ? "در حال ثبت..." : "ثبت پرداخت"}
        </button>
      </div>
    </form>
  );
}