"use client";

import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";
import { SalonService } from "@/features/salons/list/salon.service";
import type { GetSelectListSalonResponse } from "@/features/salons/list/salon.types";

import { ServiceFormSchema } from "./service.schema";
import { ServiceCatalogService } from "./service.service";
import type { ServiceCategoryDto, ServiceDto } from "./service.types";

type FormValues = {
  salonId: number;
  categoryId: number;
  code: string;
  name: string;
  description: string;
  durationMinutes: string;
  isActive: boolean;
};

type ServiceFormDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  serviceId: number | null;
  salonId: number;
};

type CategoriesState = {
  salonId: number;
  items: ServiceCategoryDto[];
  error: string | null;
};

const EMPTY_FORM: FormValues = {
  salonId: 0,
  categoryId: 0,
  code: "",
  name: "",
  description: "",
  durationMinutes: "30",
  isActive: true,
};

function FormField({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) {
  return (
    <label className="block space-y-1.5 text-xs text-slate-600">
      <span>{label}</span>
      {children}
    </label>
  );
}

function FormSelect(props: React.ComponentProps<"select">) {
  return (
    <select
      {...props}
      className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-800 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
    />
  );
}

export function ServiceFormDialog(props: ServiceFormDialogProps) {
  return (
    <ServiceFormDialogContent
      key={`${props.open}:${props.serviceId ?? "new"}:${props.salonId}`}
      {...props}
    />
  );
}

function ServiceFormDialogContent({
  open,
  onOpenChange,
  onSaved,
  serviceId,
  salonId,
}: ServiceFormDialogProps) {
  const [values, setValues] = useState<FormValues>({
    ...EMPTY_FORM,
    salonId,
  });
  const [salons, setSalons] = useState<GetSelectListSalonResponse[]>([]);
  const [categoriesState, setCategoriesState] = useState<CategoriesState>({
    salonId: 0,
    items: [],
    error: null,
  });
  const [loading, setLoading] = useState(serviceId !== null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const categories = categoriesState.salonId === values.salonId
    ? categoriesState.items
    : [];
  const categoriesLoading = values.salonId > 0 && categoriesState.salonId !== values.salonId;

  useEffect(() => {
    if (!open) return;

    const controller = new AbortController();

    Promise.resolve()
      .then(async () => {
        const api = new SalonService();
        const first = await api.getSelectList(
          { PageNumber: 1, PageSize: 100 },
          { signal: controller.signal },
        );
        if (!first.data) {
          throw new Error(first.message ?? "فهرست سالن‌ها دریافت نشد.");
        }
        const available = [...first.data.list];
        for (let page = 2; page <= first.data.pagination.totalPages; page += 1) {
          const response = await api.getSelectList(
            { PageNumber: page, PageSize: 100 },
            { signal: controller.signal },
          );
          if (!response.data) {
            throw new Error(response.message ?? "فهرست سالن‌ها کامل دریافت نشد.");
          }
          available.push(...response.data.list);
        }
        if (!controller.signal.aborted) {
          setSalons(available.filter((item) => item.status || item.id === salonId));
        }
        if (serviceId === null || controller.signal.aborted) return;

        const response = await new ServiceCatalogService().getById(serviceId, {
          signal: controller.signal,
        });
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات خدمت دریافت نشد.");
        }
        const service: ServiceDto = response.data;
        if (!controller.signal.aborted) {
          setValues({
            salonId: service.salonId,
            categoryId: service.categoryId,
            code: service.code,
            name: service.name,
            description: service.description ?? "",
            durationMinutes: String(service.durationMinutes),
            isActive: service.isActive,
          });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) setError(handleApiError(reason));
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });

    return () => controller.abort();
  }, [open, serviceId, salonId]);

  useEffect(() => {
    if (!open || values.salonId <= 0) return;
    const controller = new AbortController();
    new ServiceCatalogService()
      .getCategories(values.salonId, { signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "دسته‌بندی خدمات دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setCategoriesState({
            salonId: values.salonId,
            items: response.data.filter((category) => category.isActive),
            error: null,
          });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setCategoriesState({
            salonId: values.salonId,
            items: [],
            error: handleApiError(reason),
          });
        }
      });
    return () => controller.abort();
  }, [open, values.salonId]);

  function update<K extends keyof FormValues>(key: K, value: FormValues[K]) {
    setValues((current) => ({ ...current, [key]: value }));
    setFieldErrors((current) => ({ ...current, [key]: "" }));
  }

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;
    setError(null);
    const parsed = ServiceFormSchema.safeParse({
      ...values,
      durationMinutes: Number(values.durationMinutes),
    });
    if (!parsed.success) {
      setFieldErrors(
        Object.fromEntries(
          parsed.error.issues.map((issue) => [String(issue.path[0]), issue.message]),
        ),
      );
      return;
    }

    setSubmitting(true);
    try {
      const request = {
        ...parsed.data,
        code: parsed.data.code.trim(),
        name: parsed.data.name.trim(),
        description: parsed.data.description.trim() || null,
      };
      const api = new ServiceCatalogService();
      const response = serviceId === null
        ? await api.create(request)
        : await api.update(serviceId, { ...request, isActive: values.isActive });
      if (response.statusCode >= 400) {
        throw new Error(response.message ?? "ذخیره خدمت انجام نشد.");
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
          {serviceId === null ? "ثبت خدمت جدید" : "ویرایش خدمت"}
        </DialogTitle>
        {loading ? (
          <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات...</p>
        ) : (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <div className="grid gap-4 sm:grid-cols-2">
              <FormField label="سالن">
                <FormSelect
                  value={values.salonId || ""}
                  onChange={(event) => {
                    update("salonId", Number(event.target.value));
                    update("categoryId", 0);
                  }}
                  disabled={serviceId !== null}
                >
                  <option value="" disabled>انتخاب سالن</option>
                  {salons.map((salon) => (
                    <option key={salon.id} value={salon.id}>{salon.name}</option>
                  ))}
                </FormSelect>
              </FormField>
              <FormField label="دسته‌بندی">
                <FormSelect
                  value={values.categoryId || ""}
                  onChange={(event) => update("categoryId", Number(event.target.value))}
                  disabled={categoriesLoading || categories.length === 0}
                  aria-invalid={Boolean(fieldErrors.categoryId)}
                >
                  <option value="" disabled>
                    {categoriesLoading ? "در حال دریافت..." : "انتخاب دسته‌بندی"}
                  </option>
                  {categories.map((category) => (
                    <option key={category.id} value={category.id}>{category.name}</option>
                  ))}
                </FormSelect>
                {fieldErrors.categoryId && <span className="text-destructive">{fieldErrors.categoryId}</span>}
                {categoriesState.salonId === values.salonId && categoriesState.error && (
                  <span role="alert" className="text-destructive">{categoriesState.error}</span>
                )}
              </FormField>
              <FormField label="کد خدمت">
                <Input
                  value={values.code}
                  maxLength={50}
                  onChange={(event) => update("code", event.target.value)}
                  aria-invalid={Boolean(fieldErrors.code)}
                  className="h-10"
                />
                {fieldErrors.code && <span className="text-destructive">{fieldErrors.code}</span>}
              </FormField>
              <FormField label="نام خدمت">
                <Input
                  value={values.name}
                  maxLength={200}
                  onChange={(event) => update("name", event.target.value)}
                  aria-invalid={Boolean(fieldErrors.name)}
                  className="h-10"
                />
                {fieldErrors.name && <span className="text-destructive">{fieldErrors.name}</span>}
              </FormField>
              <FormField label="مدت زمان (دقیقه)">
                <Input
                  type="number"
                  min={1}
                  step={1}
                  value={values.durationMinutes}
                  onChange={(event) => update("durationMinutes", event.target.value)}
                  aria-invalid={Boolean(fieldErrors.durationMinutes)}
                  className="h-10"
                />
                {fieldErrors.durationMinutes && <span className="text-destructive">{fieldErrors.durationMinutes}</span>}
              </FormField>
              {serviceId !== null && (
                <FormField label="وضعیت خدمت">
                  <FormSelect
                    value={values.isActive ? "true" : "false"}
                    onChange={(event) => update("isActive", event.target.value === "true")}
                  >
                    <option value="true">فعال</option>
                    <option value="false">غیرفعال</option>
                  </FormSelect>
                </FormField>
              )}
            </div>
            <FormField label="توضیحات">
              <textarea
                value={values.description}
                maxLength={1000}
                onChange={(event) => update("description", event.target.value)}
                className="min-h-24 w-full rounded-lg border border-slate-200 bg-white p-3 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
              />
              {fieldErrors.description && <span className="text-destructive">{fieldErrors.description}</span>}
            </FormField>
            {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
            <div className="flex flex-row-reverse gap-2 border-t pt-3">
              <Button type="submit" disabled={submitting || categoriesLoading || loading}>
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
