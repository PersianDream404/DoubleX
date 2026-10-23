"use client";

import { Trash2 } from "lucide-react";

import { useOrderStore } from "@/app/stores/order.store";

export function OrderItems() {
  const items = useOrderStore((state) => state.items);
  const removeItem = useOrderStore((state) => state.removeItem);
  const updateQuantity = useOrderStore(
    (state) => state.updateQuantity,
  );

  if (items.length === 0) {
    return (
      <div className="rounded-md border border-dashed p-6 text-center text-sm text-muted-foreground">
        هنوز محصولی به سفارش اضافه نشده است.
      </div>
    );
  }

  return (
    <div className="rounded-md border">
      <div className="grid grid-cols-[1fr_150px_100px_50px] gap-4 border-b bg-muted/50 p-3 text-sm font-medium">
        <div>محصول</div>
        <div>قیمت</div>
        <div>تعداد</div>
        <div></div>
      </div>

      {items.map((item) => (
        <div
          key={item.productId}
          className="grid grid-cols-[1fr_150px_100px_50px] items-center gap-4 border-b p-3 last:border-b-0"
        >
          <div className="font-medium">
            {item.productName}
          </div>

          <div>
            {item.price.toLocaleString("fa-IR")} تومان
          </div>

          <input
            type="number"
            min={1}
            value={item.quantity}
            onChange={(event) =>
              updateQuantity(
                item.productId,
                Math.max(1, Number(event.target.value)),
              )
            }
            className="w-20 rounded-md border px-2 py-1 text-center"
          />

          <button
            type="button"
            onClick={() => removeItem(item.productId)}
            className="inline-flex size-8 items-center justify-center rounded-md text-destructive hover:bg-destructive/10"
          >
            <Trash2 className="size-4" />
          </button>
        </div>
      ))}
    </div>
  );
}