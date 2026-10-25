"use client";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";

interface InventoryConfirmDialogProps {
  open: boolean;
  title: string;
  description: string;
  confirmLabel: string;
  pendingLabel: string;
  error: string | null;
  pending: boolean;
  destructive?: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: () => void;
}

export function InventoryConfirmDialog({
  open,
  title,
  description,
  confirmLabel,
  pendingLabel,
  error,
  pending,
  destructive = false,
  onOpenChange,
  onConfirm,
}: InventoryConfirmDialogProps) {
  return (
    <Dialog open={open} onOpenChange={(nextOpen) => !pending && onOpenChange(nextOpen)}>
      <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
        <div className="space-y-2">
          <DialogTitle className="text-right">{title}</DialogTitle>
          <DialogDescription className="text-right leading-6">
            {description}
          </DialogDescription>
        </div>
        {error && (
          <p className="text-sm text-destructive" role="alert">
            {error}
          </p>
        )}
        <DialogFooter className="flex-row-reverse">
          <Button
            type="button"
            variant={destructive ? "destructive" : "default"}
            disabled={pending}
            onClick={onConfirm}
          >
            {pending ? pendingLabel : confirmLabel}
          </Button>
          <Button
            type="button"
            variant="outline"
            disabled={pending}
            onClick={() => onOpenChange(false)}
          >
            انصراف
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
