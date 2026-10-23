"use client";

import { useState } from "react";
import { MoreHorizontal, Pencil } from "lucide-react";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { CompanyOrderDto } from "@/app/DTOs/orders/order.dto";
import { DeleteOrderButton } from "./delete-company-order-button";
import { EditOrderModal } from "./Update/order-update-customer-modal";

type OrderActionsProps = {
  companyId: number;
  orderId: number;
  order: CompanyOrderDto;
};

export function OrderActions({ companyId, orderId, order }: OrderActionsProps) {
  const [open, setOpen] = useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger className="inline-flex size-9 items-center justify-center rounded-md hover:bg-muted">
          <MoreHorizontal className="size-4" />
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setOpen(true)}>
            <Pencil className="ml-2 size-4" />
            ویرایش
          </DropdownMenuItem>

          <DeleteOrderButton companyId={companyId} orderId={orderId} />
        </DropdownMenuContent>
      </DropdownMenu>

      <EditOrderModal
        companyId={companyId}
        orderId={orderId}
        order={{
          id: order.id,
          status: order.status ?? "Pending",
          name: order.name ?? "نام تستی",
        }}
        open={open}
        onOpenChange={setOpen}
      />
    </>
  );
}
