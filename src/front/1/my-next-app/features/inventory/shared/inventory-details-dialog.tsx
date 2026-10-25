"use client";

import { useEffect, useState, type ReactNode } from "react";

import { Dialog, DialogContent, DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { handleApiError } from "@/lib/api/handle-api-error";

interface InventoryDetailsDialogProps<TDetails> {
  open: boolean;
  id: number | null;
  title: string;
  onOpenChange: (open: boolean) => void;
  load: (id: number, signal: AbortSignal) => Promise<TDetails>;
  render: (details: TDetails) => ReactNode;
}

export function InventoryDetailsDialog<TDetails>({
  open,
  id,
  title,
  onOpenChange,
  load,
  render,
}: InventoryDetailsDialogProps<TDetails>) {
  const [state, setState] = useState<{
    requestedId: number | null;
    details: TDetails | null;
    error: string | null;
    loading: boolean;
  }>({ requestedId: null, details: null, error: null, loading: false });

  useEffect(() => {
    if (!open || id === null) {
      return;
    }

    const controller = new AbortController();
    load(id, controller.signal)
      .then((details) => {
        if (!controller.signal.aborted) {
          setState({ requestedId: id, details, error: null, loading: false });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            requestedId: id,
            details: null,
            error: handleApiError(error),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [id, load, open]);

  const isLoading =
    open &&
    id !== null &&
    (state.requestedId !== id ||
      state.loading ||
      (state.details === null && state.error === null));

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir="rtl" className="max-h-[85vh] gap-4 overflow-y-auto sm:max-w-xl">
        <div className="space-y-2">
          <DialogTitle className="text-right">{title}</DialogTitle>
          <DialogDescription className="sr-only">نمایش جزئیات {title}</DialogDescription>
        </div>
        {isLoading ? (
          <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات...</p>
        ) : state.error ? (
          <p className="py-4 text-sm text-destructive" role="alert">{state.error}</p>
        ) : state.details && state.requestedId === id ? (
          render(state.details)
        ) : null}
      </DialogContent>
    </Dialog>
  );
}
