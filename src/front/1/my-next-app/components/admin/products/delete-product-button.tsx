"use client";

import { useTransition } from "react";
import { Trash2 } from "lucide-react";

import { deleteProductAction } from "@/actions/products/product.delete.actions";

type Props = {
  productId: number;
};

export function DeleteProductButton({ productId }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این محصول مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      await deleteProductAction(productId);
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
