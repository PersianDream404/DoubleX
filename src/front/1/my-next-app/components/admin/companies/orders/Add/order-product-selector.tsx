"use client";

import { ProductSelectDto } from "@/app/DTOs/Products/product.dto";
import { useOrderStore } from "@/app/stores/order.store";

type ProductSelectorProps = {
  products: ProductSelectDto[];
};

export function ProductSelector({ products }: ProductSelectorProps) {
  const addItem = useOrderStore((state) => state.addItem);

  function handleChange(productId: string) {
    if (!productId) return;

    const product = products.find((item) => item.id === Number(productId));

    if (!product) return;

    addItem({
      productId: product.id,
      productName: product.name,
      price: product.price,
      quantity: 1,
    });
  }

  return (
    <div className="space-y-2">
      <label className="text-sm font-medium">محصول</label>

      <select
        defaultValue=""
        onChange={(event) => {
          handleChange(event.target.value);
          event.target.value = "";
        }}
        className="w-full rounded-md border bg-background px-3 py-2 text-sm"
      >
        <option value="">انتخاب محصول...</option>

        {products.map((product) => (
          <option key={product.id} value={product.id}>
            {product.name} - {product.price.toLocaleString("fa-IR")} تومان
          </option>
        ))}
      </select>
    </div>
  );
}
