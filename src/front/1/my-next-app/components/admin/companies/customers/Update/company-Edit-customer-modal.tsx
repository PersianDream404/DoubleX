"use client";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { EditCustomerForm } from "./company-Edit-Customer";
import { UpdateCustomerDto } from "@/app/DTOs/customers/customer.dto";

type EditCustomerModalProps = {
  companyId: number;
  customerId: number;
  customer: UpdateCustomerDto;

  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export function EditCustomerModal({
  companyId,
  customerId,
  customer,
  open,
  onOpenChange,
}: EditCustomerModalProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ویرایش مشتری</DialogTitle>
        </DialogHeader>

        <EditCustomerForm
          companyId={companyId}
          customerId={customerId}
          customer={customer}
        />
      </DialogContent>
    </Dialog>
  );
}
