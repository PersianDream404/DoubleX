"use client";

import { useTransition } from "react";

import { toggleProductStatusAction } from "@/actions/products/product.toggle-status.actions";

type Props = {
  productId: number;
  status: boolean;
};

export function ToggleProductStatusButton({ productId, status }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleToggle() {
    startTransition(async () => {
      await toggleProductStatusAction(productId);
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
