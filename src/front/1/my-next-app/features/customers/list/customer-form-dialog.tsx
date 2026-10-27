"use client";

import { useEffect, useState } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { CalendarDays, LoaderCircle } from "lucide-react";
import { toGregorian, toJalaali } from "jalaali-js";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";

import { CustomerFormSchema, type CustomerFormValues } from "./customer.schema";
import { CustomerService } from "./customer.service";
import type { CustomerDto, CustomerRequest } from "./customer.types";

const EMPTY_FORM: CustomerFormValues = {
  fullName: "",
  birthDate: "",
  nationalCode: "",
  address: "",
  allergyDescription: "",
  jobTitle: "",
  landlinePhone: "",
  mobilePhone: "",
};

function normalizeDigits(value: string) {
  return value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));
}

function toPersianDigits(value: string) {
  return value.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]);
}

function dateToApi(value: string) {
  const [year, month, day] = normalizeDigits(value).split("/").map(Number);
  const gregorian = toGregorian(year, month, day);
  return `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(gregorian.gd).padStart(2, "0")}`;
}

function dateToJalali(value: string) {
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  const jalali = toJalaali(year, month, day);
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`;
}

function JalaliDateField({
  value,
  onChange,
}: {
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
      format="YYYY/MM/DD"
      value={value || null}
      editable={false}
      calendarPosition="bottom-start"
      portal
      zIndex={150}
      onChange={(date: DateObject | null) => onChange(date?.format("YYYY/MM/DD") ?? "")}
      render={(_displayValue, openCalendar) => (
        <div className="relative w-full">
          <Input
            id="customer-birth-date"
            type="text"
            readOnly
            dir="ltr"
            inputMode="none"
            placeholder="۱۴۰۰/۰۱/۰۱"
            value={toPersianDigits(value)}
            aria-label="تاریخ تولد"
            aria-haspopup="dialog"
            className="h-10 cursor-pointer border-slate-200 bg-white pe-10 text-sm"
            onClick={openCalendar}
            onKeyDown={(event) => {
              if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                openCalendar();
              }
            }}
          />
          <CalendarDays className="pointer-events-none absolute end-3 top-1/2 size-4 -translate-y-1/2 text-slate-400" />
        </div>
      )}
    />
  );
}

export function CustomerFormDialog({
  open,
  customer,
  onOpenChange,
  onSaved,
}: {
  open: boolean;
  customer: CustomerDto | null;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
}) {
  const [values, setValues] = useState<CustomerFormValues>(EMPTY_FORM);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    const controller = new AbortController();
    if (!customer) {
      Promise.resolve().then(() => {
        if (!controller.signal.aborted) {
          setError(null);
          setValues(EMPTY_FORM);
          setLoading(false);
        }
      });
      return () => controller.abort();
    }

    Promise.resolve()
      .then(() => {
        setError(null);
        setLoading(true);
        return new CustomerService().getById(customer.id, { signal: controller.signal });
      })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات مشتری دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setValues({
            fullName: response.data.fullName,
            birthDate: dateToJalali(response.data.birthDate),
            nationalCode: response.data.nationalCode,
            address: response.data.address,
            allergyDescription: response.data.allergyDescription ?? "",
            jobTitle: response.data.jobTitle ?? "",
            landlinePhone: response.data.landlinePhone ?? "",
            mobilePhone: response.data.mobilePhone,
          });
        }
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) setError(handleApiError(requestError));
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });

    return () => controller.abort();
  }, [customer, open]);

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting || loading) return;
    setError(null);
    const parsed = CustomerFormSchema.safeParse({
      ...values,
      nationalCode: normalizeDigits(values.nationalCode),
      birthDate: normalizeDigits(values.birthDate),
    });
    if (!parsed.success) {
      setError(parsed.error.issues[0]?.message ?? "اطلاعات فرم را بررسی کنید.");
      return;
    }

    setSubmitting(true);
    try {
      const request: CustomerRequest = {
        fullName: parsed.data.fullName,
        birthDate: dateToApi(parsed.data.birthDate),
        nationalCode: parsed.data.nationalCode,
        address: parsed.data.address,
        allergyDescription: parsed.data.allergyDescription.trim() || null,
        jobTitle: parsed.data.jobTitle.trim() || null,
        landlinePhone: parsed.data.landlinePhone.trim() || null,
        mobilePhone: parsed.data.mobilePhone.trim(),
      };
      if (customer) {
        await new CustomerService().update({ ...request, id: customer.id });
      } else {
        await new CustomerService().create(request);
      }
      onOpenChange(false);
      onSaved();
    } catch (requestError: unknown) {
      setError(handleApiError(requestError));
    } finally {
      setSubmitting(false);
    }
  }

  const fields: {
    key: keyof CustomerFormValues;
    label: string;
    maxLength: number;
    required?: boolean;
    direction?: "ltr";
  }[] = [
    { key: "fullName", label: "نام و نام خانوادگی", maxLength: 250, required: true },
    { key: "nationalCode", label: "کد ملی", maxLength: 10, required: true, direction: "ltr" },
    { key: "mobilePhone", label: "شماره همراه", maxLength: 20, required: true, direction: "ltr" },
    { key: "landlinePhone", label: "تلفن ثابت", maxLength: 20, direction: "ltr" },
    { key: "jobTitle", label: "شغل", maxLength: 150 },
    { key: "allergyDescription", label: "حساسیت‌ها", maxLength: 1000 },
  ];

  return (
    <Dialog open={open} onOpenChange={(nextOpen) => !submitting && onOpenChange(nextOpen)}>
      <DialogContent dir="rtl" className="max-h-[92vh] overflow-y-auto sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>{customer ? "ویرایش اطلاعات مشتری" : "ثبت مشتری جدید"}</DialogTitle>
        </DialogHeader>
        {loading ? (
          <div className="flex items-center justify-center gap-2 py-12 text-sm text-slate-500">
            <LoaderCircle className="size-4 animate-spin" />
            در حال دریافت اطلاعات مشتری...
          </div>
        ) : (
          <form onSubmit={(event) => void submit(event)} className="space-y-4">
            <div className="grid gap-4 sm:grid-cols-2">
              {fields.map((field) => (
                <div key={field.key} className="space-y-1.5">
                  <Label htmlFor={`customer-${field.key}`}>
                    {field.label}{field.required && <span className="text-rose-600"> *</span>}
                  </Label>
                  <Input
                    id={`customer-${field.key}`}
                    value={values[field.key]}
                    maxLength={field.maxLength}
                    dir={field.direction}
                    onChange={(event) => setValues((current) => ({ ...current, [field.key]: event.target.value }))}
                  />
                </div>
              ))}
              <div className="space-y-1.5">
                <Label>تاریخ تولد <span className="text-rose-600">*</span></Label>
                <JalaliDateField value={values.birthDate} onChange={(birthDate) => setValues((current) => ({ ...current, birthDate }))} />
              </div>
              <div className="space-y-1.5 sm:col-span-2">
                <Label htmlFor="customer-address">نشانی <span className="text-rose-600">*</span></Label>
                <textarea
                  id="customer-address"
                  value={values.address}
                  maxLength={1000}
                  rows={3}
                  onChange={(event) => setValues((current) => ({ ...current, address: event.target.value }))}
                  className="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                />
              </div>
            </div>
            {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
            <div className="flex justify-end gap-2 border-t border-slate-100 pt-4">
              <Button type="button" variant="outline" disabled={submitting} onClick={() => onOpenChange(false)}>انصراف</Button>
              <Button type="submit" disabled={submitting} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                {submitting && <LoaderCircle className="size-4 animate-spin" />}
                {submitting ? "در حال ذخیره..." : customer ? "ذخیره تغییرات" : "ثبت مشتری"}
              </Button>
            </div>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
