"use client";

import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceCategoryFormSchema } from "./service-category.schema";
import { ServiceCatalogService } from "../list/service.service";
import type { ServiceCategoryDto } from "../list/service.types";

export function ServiceCategoryFormDialog({
  open,
  onOpenChange,
  onSaved,
  categoryId,
  salonId,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  categoryId: number | null;
  salonId: number;
}) {
  return (
    <ServiceCategoryFormDialogContent
      key={`${open}:${categoryId ?? "new"}:${salonId}`}
      open={open}
      onOpenChange={onOpenChange}
      onSaved={onSaved}
      categoryId={categoryId}
      salonId={salonId}
    />
  );
}

function ServiceCategoryFormDialogContent({
  open,
  onOpenChange,
  onSaved,
  categoryId,
  salonId,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  categoryId: number | null;
  salonId: number;
}) {
  const [category, setCategory] = useState<ServiceCategoryDto | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [loading, setLoading] = useState(categoryId !== null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

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
          setCategory(response.data);
          setName(response.data.name);
          setDescription(response.data.description ?? "");
          setLoading(false);
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setError(handleApiError(reason));
          setLoading(false);
        }
      });
    return () => controller.abort();
  }, [open, categoryId]);

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;
    setError(null);
    setFieldErrors({});
    const parsed = ServiceCategoryFormSchema.safeParse({ salonId, name, description });
    if (!parsed.success) {
      setFieldErrors(Object.fromEntries(
        parsed.error.issues.map((issue) => [String(issue.path[0]), issue.message]),
      ));
      return;
    }

    setSubmitting(true);
    try {
      const request = {
        ...parsed.data,
        name: parsed.data.name.trim(),
        description: parsed.data.description.trim() || null,
      };
      const response = categoryId === null
        ? await new ServiceCatalogService().createCategory(request)
        : await new ServiceCatalogService().updateCategory(categoryId, {
          ...request,
          isActive: category?.isActive ?? true,
        });
      if (response.statusCode >= 400) {
        throw new Error(response.message ?? "ذخیره دسته‌بندی انجام نشد.");
      }
      onOpenChange(false);
      onSaved();
    } catch (reason: unknown) {
      setError(handleApiError(reason));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={(nextOpen) => !submitting && onOpenChange(nextOpen)}>
      <DialogContent dir="rtl" className="max-h-[90vh] gap-4 overflow-y-auto sm:max-w-xl">
        <DialogTitle className="text-right text-lg text-fuchsia-950">
          {categoryId === null ? "ثبت دسته‌بندی خدمت" : "ویرایش دسته‌بندی"}
        </DialogTitle>
        {loading ? (
          <p role="status" className="py-8 text-center text-sm">در حال دریافت اطلاعات دسته‌بندی...</p>
        ) : (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <label className="block space-y-1.5 text-xs text-slate-600">
              <span>نام دسته‌بندی</span>
              <Input
                value={name}
                maxLength={200}
                onChange={(event) => setName(event.target.value)}
                aria-invalid={Boolean(fieldErrors.name)}
                className="h-10"
              />
              {fieldErrors.name && <span className="text-destructive">{fieldErrors.name}</span>}
            </label>
            <label className="block space-y-1.5 text-xs text-slate-600">
              <span>توضیحات (اختیاری)</span>
              <textarea
                value={description}
                maxLength={1000}
                onChange={(event) => setDescription(event.target.value)}
                aria-invalid={Boolean(fieldErrors.description)}
                className="min-h-24 w-full rounded-lg border border-slate-200 bg-white p-3 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
              />
              {fieldErrors.description && <span className="text-destructive">{fieldErrors.description}</span>}
            </label>
            {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
            <div className="flex flex-row-reverse gap-2 border-t pt-3">
              <Button type="submit" disabled={submitting || loading}>
                {submitting ? "در حال ذخیره..." : "ذخیره"}
              </Button>
              <Button type="button" variant="outline" disabled={submitting} onClick={() => onOpenChange(false)}>
                انصراف
              </Button>
            </div>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
