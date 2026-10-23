"use client";

import { toggleCompanyStatusAction } from "@/actions/companies/company.toggle-status.actions";
import { useTransition } from "react";

type Props = {
  companyId: number;
  status: boolean;
};

export function ToggleCompanyStatusButton({ companyId, status }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleToggle() {
    startTransition(async () => {
      await toggleCompanyStatusAction(companyId);
    });
  }

  return (
    <button
      type="button"
      onClick={handleToggle}
      disabled={isPending}
      className="disabled:opacity-50"
    >
      {isPending ? "..." : status ? "فعال" : "غیرفعال"}
    </button>
  );
}
