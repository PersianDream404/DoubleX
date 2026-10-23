"use client";

import { useTransition } from "react";
import { Trash2 } from "lucide-react";
import { deleteCompanyAction } from "@/actions/companies/company.delete.actions";

type Props = {
  companyId: number;
};

export function DeleteCompanyButton({ companyId }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این شرکت مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      await deleteCompanyAction(companyId);
    });
  }

  return (
    <button
      type="button"
      onClick={handleDelete}
      disabled={isPending}
      className="flex w-full items-center gap-2 px-2 py-1.5 text-sm text-destructive"
    >
      <Trash2 className="size-4" />
      {isPending ? "در حال حذف..." : "حذف"}
    </button>
  );
}
