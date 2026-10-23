"use client";

import { useOrderStore } from "@/app/stores/order.store";
import { CompanyCustomerDto } from "@/app/DTOs/customers/customer.dto";
import { ProductSelectDto } from "@/app/DTOs/Products/product.dto";
import { ProductSelector } from "./order-product-selector";
import { OrderItems } from "./order-items";
import { OrderSummary } from "./order-summary";
import { createOrderAction } from "@/actions/companies/orders/company.order.create.actions";
import { Router } from "next/router";
import { useRouter } from "next/navigation";

type AddOrderFormProps = {
  companyId: number;
  customers: CompanyCustomerDto[];
  products: ProductSelectDto[];
  onSuccess: () => void;
};

export function AddOrderForm({
  companyId,
  customers,
  products,
  onSuccess,
}: AddOrderFormProps) {
  const customerId = useOrderStore((state) => state.customerId);
  const setCustomerId = useOrderStore((state) => state.setCustomerId);
  const items = useOrderStore((state) => state.items);
  const clearOrder = useOrderStore((state) => state.clearOrder);

  const router = useRouter();
  async function handleSubmit() {
    if (!customerId) {
      return;
    }

    if (items.length === 0) {
      return;
    }

    const result = await createOrderAction({
      companyId,
      customerId,
      status: "Pending",
      items: items.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
    });

    if (!result.success) {
      console.error(result.error);
      return;
    }

    clearOrder();

    onSuccess();

    router.refresh();
  }
  return (
    <div className="space-y-6">
      {/* Customer */}

      <div className="space-y-2">
        <label className="text-sm font-medium">مشتری</label>

        <select
          value={customerId ?? ""}
          onChange={(e) => setCustomerId(Number(e.target.value))}
          className="w-full rounded-md border bg-background px-3 py-2 text-sm"
        >
          <option value="">انتخاب مشتری...</option>

          {customers.map((customer) => (
            <option key={customer.id} value={customer.id}>
              {customer.name}
            </option>
          ))}
        </select>
      </div>

      <ProductSelector products={products} />
      <OrderItems />
      <OrderSummary />
      <div className="flex justify-end">
        <button
          type="button"
          onClick={handleSubmit}
          className="rounded-md bg-primary px-4 py-2 text-sm text-primary-foreground"
        >
          ثبت سفارش
        </button>
      </div>
    </div>
  );
}
