"use client";

import { useActionState, useEffect } from "react";
import { useRouter } from "next/navigation";

import {
  updateOrderAction,
  OrderActionState,
} from "@/actions/companies/orders/company.order.update.actions";

type EditOrderFormProps = {
  companyId: number;
  orderId: number;
  order: {
    id: number;
    name: string;
    status: string;
  };
  onSuccess: () => void;
};

const initialState: OrderActionState = {
  success: false,
  values: {
    name: "",
    status: "Pending",
  },
};

export function EditOrderForm({
  companyId,
  orderId,
  order,
  onSuccess,
}: EditOrderFormProps) {
  const router = useRouter();

  const [state, formAction, pending] = useActionState(updateOrderAction, {
    ...initialState,
    values: {
      name: order.name,
      status: order.status,
    },
  });

  useEffect(() => {
    if (!state.success) return;

    onSuccess();
    router.refresh();
  }, [state.success, onSuccess, router]);

  return (
    <form action={formAction} className="space-y-5">
      <input type="hidden" name="orderId" value={orderId} />

      <input type="hidden" name="companyId" value={companyId} />

      <div className="space-y-2">
        <label htmlFor={`name-${orderId}`} className="text-sm font-medium">
          نام سفارش
        </label>

        <input
          id={`name-${orderId}`}
          name="name"
          defaultValue={state.values.name}
          className="w-full rounded-md border px-3 py-2"
        />

        {state.errors?.name && (
          <p className="text-sm text-destructive">{state.errors.name[0]}</p>
        )}
      </div>

      <div className="space-y-2">
        <label htmlFor={`status-${orderId}`} className="text-sm font-medium">
          وضعیت
        </label>

        <select
          id={`status-${orderId}`}
          name="status"
          defaultValue={state.values.status}
          className="w-full rounded-md border px-3 py-2"
        >
          <option value="Pending">در انتظار</option>
          <option value="Completed">تکمیل شده</option>
        </select>

        {state.errors?.status && (
          <p className="text-sm text-destructive">{state.errors.status[0]}</p>
        )}
      </div>

      <button
        type="submit"
        disabled={pending}
        className="rounded-md bg-primary px-4 py-2 text-sm text-primary-foreground disabled:opacity-50"
      >
        {pending ? "در حال ذخیره..." : "ذخیره تغییرات"}
      </button>
    </form>
  );
}
