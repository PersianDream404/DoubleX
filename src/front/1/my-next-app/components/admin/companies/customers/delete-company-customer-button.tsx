"use client";

import { useTransition } from "react";
import { Trash2 } from "lucide-react";

import { deleteCustomerAction } from "@/actions/companies/customers/company.customer.delete.actions";

type Props = {
  customerId: number;
  companyId: number;
};

export function DeleteCustomerButton({ customerId, companyId }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این مشتری مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      const result = await deleteCustomerAction(customerId, companyId);

      console.log("DELETE CUSTOMER RESULT:", result);
    });
  }

  return (
    <button
      type="button"
      onClick={handleDelete}
      disabled={isPending}
      className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm text-destructive hover:bg-muted"
    >
      <Trash2 className="size-4" />

      {isPending ? "در حال حذف..." : "حذف"}
    </button>
  );
}
