"use client";

import { useState } from "react";
import { Plus } from "lucide-react";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

import { CompanyCustomerDto } from "@/app/DTOs/customers/customer.dto";
import { ProductSelectDto } from "@/app/DTOs/Products/product.dto";
import { AddOrderForm } from "./order-add-Customer";



type AddOrderModalProps = {
  companyId: number;
  customers: CompanyCustomerDto[];
  products: ProductSelectDto[];
};

export function AddOrderModal({
  companyId,
  customers,
  products,
}: AddOrderModalProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger className="inline-flex items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90">
        <Plus className="ml-2 size-4" />
        افزودن سفارش
      </DialogTrigger>

      <DialogContent className="max-w-3xl">
        <DialogHeader>
          <DialogTitle>افزودن سفارش</DialogTitle>
        </DialogHeader>

        <AddOrderForm
          companyId={companyId}
          customers={customers}
          products={products}
          onSuccess={() => setOpen(false)}
        />
      </DialogContent>
    </Dialog>
  );
}