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
import { CompanyOrderDto, OrderSelectDto } from "@/app/DTOs/orders/order.dto";
import { AddPaymentForm } from "./payment-add-company-form";



type AddPaymentModalProps = {
  companyId: number;
  orders: CompanyOrderDto[];
};

export function AddPaymentModal({
  companyId,
  orders,
}: AddPaymentModalProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger className="inline-flex items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90">
        <Plus className="ml-2 size-4" />
        افزودن پرداخت
      </DialogTrigger>

      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>افزودن پرداخت</DialogTitle>
        </DialogHeader>

        <AddPaymentForm
          companyId={companyId}
          orders={orders}
          onSuccess={() => setOpen(false)}
        />
      </DialogContent>
    </Dialog>
  );
}