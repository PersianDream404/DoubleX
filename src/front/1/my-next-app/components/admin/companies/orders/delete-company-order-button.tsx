"use client";

import { useTransition } from "react";
import { Trash2 } from "lucide-react";

import { deleteOrderAction } from "@/actions/companies/orders/company.order.delete.actions";

type Props = {
  orderId: number;
  companyId: number;
};

export function DeleteOrderButton({ orderId, companyId }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این سفارش مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      const result = await deleteOrderAction(orderId, companyId);

      console.log("DELETE ORDER RESULT:", result);
    });
  }

  return (
    <button
      type="button"
      onClick={handleDelete}
      disabled={isPending}
      className="flex w-full items-center gap-2 px-2 py-1.5 text-sm text-destructive hover:bg-muted"
    >
      <Trash2 className="size-4" />

      {isPending ? "در حال حذف..." : "حذف"}
    </button>
  );
}
