"use client";

import { Plus } from "lucide-react";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { AddCustomerForm } from "./company-add-Customer";

type AddCustomerModalProps = {
  companyId: number;
};

export function AddCustomerModal({ companyId }: AddCustomerModalProps) {
  return (
    <Dialog>
      <DialogTrigger className="inline-flex items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90">
        <Plus className="ml-2 size-4" />
        افزودن مشتری
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>افزودن مشتری</DialogTitle>
        </DialogHeader>

        <AddCustomerForm companyId={companyId} />
      </DialogContent>
    </Dialog>
  );
}
