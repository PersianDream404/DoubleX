"use client";

import { useTransition } from "react";
import { useRouter } from "next/navigation";
import { Trash2 } from "lucide-react";

import { deletePaymentAction } from "@/actions/companies/payments/company.payment.delete.actions";

type Props = {
  paymentId: number;
  companyId: number;
};

export function DeletePaymentButton({ paymentId, companyId }: Props) {
  const [isPending, startTransition] = useTransition();
  const router = useRouter();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این پرداخت مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      const result = await deletePaymentAction(paymentId, companyId);

      if (result.success) {
        router.refresh();
      }
    });
  }

  return (
    <button
      type="button"
      onClick={handleDelete}
      disabled={isPending}
      className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm text-destructive hover:bg-muted disabled:opacity-50"
    >
      <Trash2 className="size-4" />

      {isPending ? "در حال حذف..." : "حذف"}
    </button>
  );
}
