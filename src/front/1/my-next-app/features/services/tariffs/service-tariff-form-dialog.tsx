"use client";

import { useEffect, useState } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceTariffFormSchema } from "./service-tariff.schema";
import { ServiceTariffService } from "./service-tariff.service";
import { apiDateToJalali, jalaliDateToApi } from "./service-tariff-date";
type FormValues = {
  name: string;
  validFrom: string;
  validTo: string;
};

function TariffDatePicker({
  id,
  label,
  value,
  onChange,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <label className="block space-y-1.5 text-xs text-slate-600">
      <span>{label}</span>
      <DatePicker
        calendar={persian}
        locale={persian_fa}
        digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
        format="YYYY/MM/DD"
        value={value || null}
        editable={false}
        calendarPosition="bottom-start"
        portal
        zIndex={160}
        onChange={(date: DateObject | null) => onChange(date?.format("YYYY/MM/DD") ?? "")}
        render={(_displayValue, openCalendar) => (
          <Input
            id={id}
            type="text"
            value={value}
            placeholder="انتخاب تاریخ"
            readOnly
            onClick={openCalendar}
            onKeyDown={(event) => {
              if (event.key === "Enter" || event.key === " ") openCalendar();
            }}
            className="h-10 cursor-pointer bg-white"
          />
        )}
      />
    </label>
  );
}

export function ServiceTariffFormDialog({
  open,
  onOpenChange,
  onSaved,
  tariffId,
  salonId,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  tariffId: number | null;
  salonId: number;
}) {
  return (
    <ServiceTariffFormDialogContent
      key={`${open}:${tariffId ?? "new"}:${salonId}`}
      open={open}
      onOpenChange={onOpenChange}
      onSaved={onSaved}
      tariffId={tariffId}
      salonId={salonId}
    />
  );
}

function ServiceTariffFormDialogContent({
  open,
  onOpenChange,
  onSaved,
  tariffId,
  salonId,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  tariffId: number | null;
  salonId: number;
}) {
  const [values, setValues] = useState<FormValues>({ name: "", validFrom: "", validTo: "" });
  const [loading, setLoading] = useState(tariffId !== null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (!open || tariffId === null) return;
    const controller = new AbortController();
    new ServiceTariffService()
      .getById(tariffId, { signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات تعرفه دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setValues({
            name: response.data.name,
            validFrom: apiDateToJalali(response.data.validFrom),
            validTo: apiDateToJalali(response.data.validTo),
          });
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
  }, [open, tariffId]);

  function update<K extends keyof FormValues>(key: K, value: FormValues[K]) {
    setValues((current) => ({ ...current, [key]: value }));
    setFieldErrors((current) => ({ ...current, [key]: "" }));
    setError(null);
  }

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) return;
    setError(null);
    const parsed = ServiceTariffFormSchema.safeParse({
      salonId,
      name: values.name,
      validFrom: jalaliDateToApi(values.validFrom) ?? "",
      validTo: values.validTo ? jalaliDateToApi(values.validTo) : null,
    });
    if (!parsed.success) {
      setFieldErrors(Object.fromEntries(
        parsed.error.issues.map((issue) => [String(issue.path[0]), issue.message]),
      ));
      return;
    }

    setSubmitting(true);
    try {
      const response = tariffId === null
        ? await new ServiceTariffService().create(parsed.data)
        : await new ServiceTariffService().update(tariffId, parsed.data);
      if (response.statusCode >= 400) {
        throw new Error(response.message ?? "ذخیره تعرفه انجام نشد.");
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
          {tariffId === null ? "ثبت تعرفه جدید" : "ویرایش تعرفه"}
        </DialogTitle>
        {loading ? (
          <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات تعرفه...</p>
        ) : (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <label className="block space-y-1.5 text-xs text-slate-600">
              <span>نام تعرفه</span>
              <Input
                value={values.name}
                maxLength={200}
                onChange={(event) => update("name", event.target.value)}
                aria-invalid={Boolean(fieldErrors.name)}
                className="h-10"
              />
              {fieldErrors.name && <span className="text-destructive">{fieldErrors.name}</span>}
            </label>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <TariffDatePicker
                  id="tariff-valid-from"
                  label="شروع اعتبار"
                  value={values.validFrom}
                  onChange={(value) => update("validFrom", value)}
                />
                {fieldErrors.validFrom && <span className="text-xs text-destructive">{fieldErrors.validFrom}</span>}
              </div>
              <div>
                <TariffDatePicker
                  id="tariff-valid-to"
                  label="پایان اعتبار (اختیاری)"
                  value={values.validTo}
                  onChange={(value) => update("validTo", value)}
                />
                {fieldErrors.validTo && <span className="text-xs text-destructive">{fieldErrors.validTo}</span>}
              </div>
            </div>
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
