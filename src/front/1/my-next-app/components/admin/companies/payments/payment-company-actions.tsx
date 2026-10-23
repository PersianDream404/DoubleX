"use client";

import { useState } from "react";
import { MoreHorizontal, Pencil } from "lucide-react";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { CompanyPaymentDto } from "@/app/DTOs/payments/payment.dto";
import { CompanyOrderDto } from "@/app/DTOs/orders/order.dto";

import { EditPaymentModal } from "./Update/payment-update-company-modal";
import { DeletePaymentButton } from "./delete-payment-company-button";


type PaymentActionsProps = {
  payment: CompanyPaymentDto;
  companyId: number;
  orders: CompanyOrderDto[];
};

export function PaymentActions({
  payment,
  companyId,
  orders,
}: PaymentActionsProps) {
  const [open, setOpen] = useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger className="inline-flex size-8 items-center justify-center rounded-md hover:bg-muted">
          <MoreHorizontal className="size-4" />
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          {/* ویرایش */}
          <DropdownMenuItem onClick={() => setOpen(true)}>
            <Pencil className="ml-2 size-4" />
            ویرایش
          </DropdownMenuItem>

          {/* حذف */}
          <DeletePaymentButton
            paymentId={payment.id}
            companyId={companyId}
          />
        </DropdownMenuContent>
      </DropdownMenu>

      <EditPaymentModal
        payment={payment}
        companyId={companyId}
        orders={orders}
        open={open}
        onOpenChange={setOpen}
      />
    </>
  );
}