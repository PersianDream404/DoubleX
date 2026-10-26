"use client";

import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceCatalogService } from "../list/service.service";
import type { ServiceCategoryDto } from "../list/service.types";

type CategoryState = {
  key: string;
  data: ServiceCategoryDto | null;
  error: string | null;
};

export function ServiceCategoryDetailsDialog({
  open,
  onOpenChange,
  categoryId,
  onEdit,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  categoryId: number | null;
  onEdit: (id: number) => void;
}) {
  const key = `${open}:${categoryId ?? ""}`;
  const [state, setState] = useState<CategoryState>({
    key: "",
    data: null,
    error: null,
  });

  useEffect(() => {
    if (!open || categoryId === null) return;
    const controller = new AbortController();
    new ServiceCatalogService()
      .getCategoryById(categoryId, { signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات دسته‌بندی دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setState({ key, data: response.data, error: null });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setState({ key, data: null, error: handleApiError(reason) });
        }
      });
    return () => controller.abort();
  }, [open, categoryId, key]);

  const current = state.key === key ? state : null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir="rtl" className="gap-4 sm:max-w-xl">
        <DialogTitle className="text-right text-lg text-fuchsia-950">جزئیات دسته‌بندی</DialogTitle>
        {!current ? (
          <p role="status" className="py-8 text-center text-sm">در حال دریافت اطلاعات دسته‌بندی...</p>
        ) : current.error ? (
          <p role="alert" className="py-6 text-center text-sm text-destructive">{current.error}</p>
        ) : current.data ? (
          <>
            <section className="grid gap-4 rounded-lg border border-slate-100 p-4">
              <div>
                <p className="text-xs text-slate-500">نام دسته‌بندی</p>
                <p className="mt-1 text-sm font-semibold">{current.data.name}</p>
              </div>
              <div>
                <p className="text-xs text-slate-500">وضعیت</p>
                <p className="mt-1 text-sm">{current.data.isActive ? "فعال" : "غیرفعال"}</p>
              </div>
              <div>
                <p className="text-xs text-slate-500">توضیحات</p>
                <p className="mt-1 whitespace-pre-wrap text-sm">{current.data.description || "توضیحی ثبت نشده است."}</p>
              </div>
            </section>
            <div className="flex justify-end">
              <Button type="button" onClick={() => onEdit(current.data!.id)}>ویرایش دسته‌بندی</Button>
            </div>
          </>
        ) : null}
      </DialogContent>
    </Dialog>
  );
}
