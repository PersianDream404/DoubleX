"use client";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { UpdateOrderDto } from "@/app/DTOs/orders/order.dto";
import { EditOrderForm } from "./order-update-Customer";

type EditOrderModalProps = {
  companyId: number;
  orderId: number;
  order: UpdateOrderDto;

  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export function EditOrderModal({
  companyId,
  orderId,
  order,
  open,
  onOpenChange,
}: EditOrderModalProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ویرایش فاکتور</DialogTitle>
        </DialogHeader>

        <EditOrderForm
          companyId={companyId}
          orderId={orderId}
          order={order}
          onSuccess={() => onOpenChange(false)}
        />
      </DialogContent>
    </Dialog>
  );
}
