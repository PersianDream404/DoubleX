"use client";

import { useOrderStore } from "@/app/stores/order.store";

export function OrderSummary() {
  const items = useOrderStore((state) => state.items);

  const totalPrice = items.reduce(
    (total, item) => total + item.price * item.quantity,
    0,
  );

  return (
    <div className="rounded-md border bg-muted/30 p-4">
      <div className="flex items-center justify-between">
        <span className="text-sm text-muted-foreground">مبلغ کل سفارش</span>

        <span className="text-lg font-bold">
          {totalPrice.toLocaleString("fa-IR")} تومان
        </span>
      </div>
    </div>
  );
}
