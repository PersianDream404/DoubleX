"use client";

import { useEffect, useState, type FormEvent } from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";

import { UnitFormSchema } from "./unit.schema";
import { UnitService } from "./unit.service";
import type { UnitType } from "./unit.types";

interface UnitFormDialogProps {
  open: boolean;
  unitId: number | null;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
}

const DEFAULT_FORM = { name: "", symbol: "", unitType: "1" };
const UNIT_TYPES: { value: UnitType; label: string }[] = [
  { value: 1, label: "وزن" },
  { value: 2, label: "حجم" },
  { value: 3, label: "تعداد" },
];

export function UnitFormDialog({
  open,
  unitId,
  onOpenChange,
  onSaved,
}: UnitFormDialogProps) {
  const [form, setForm] = useState(DEFAULT_FORM);
  const [isActive, setIsActive] = useState(true);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    const controller = new AbortController();
    Promise.resolve()
      .then(() => {
        setError(null);
        if (unitId === null) {
          setForm(DEFAULT_FORM);
          setIsActive(true);
          return null;
        }

        setLoading(true);
        return new UnitService().getById(unitId, { signal: controller.signal });
      })
      .then((response) => {
        if (response === null) {
          return;
        }
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات واحد دریافت نشد.");
        }
        setForm({
          name: response.data.name,
          symbol: response.data.symbol,
          unitType: String(response.data.unitType),
        });
        setIsActive(response.data.isActive);
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) {
          setError(handleApiError(cause));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setLoading(false);
        }
      });

    return () => controller.abort();
  }, [open, unitId]);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (saving) {
      return;
    }

    const parsed = UnitFormSchema.safeParse({
      ...form,
      unitType: Number(form.unitType),
    });
    if (!parsed.success) {
      setError(parsed.error.issues[0]?.message ?? "اطلاعات واردشده معتبر نیست.");
      return;
    }

    setSaving(true);
    setError(null);
    try {
      const service = new UnitService();
      if (unitId === null) {
        await service.create(parsed.data);
      } else {
        await service.update({
          id: unitId,
          ...parsed.data,
          isActive,
        });
      }
      onOpenChange(false);
      onSaved();
    } catch (cause: unknown) {
      setError(handleApiError(cause));
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={(nextOpen) => !saving && onOpenChange(nextOpen)}>
      <DialogContent dir="rtl" className="gap-4 sm:max-w-lg">
        <div className="space-y-2">
          <DialogTitle className="text-right">
            {unitId === null ? "ثبت واحد جدید" : "ویرایش واحد"}
          </DialogTitle>
          <DialogDescription className="text-right">
            مشخصات واحد اندازه‌گیری را وارد کنید.
          </DialogDescription>
        </div>
        {loading ? (
          <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات واحد...</p>
        ) : (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="unit-name">نام واحد</Label>
                <Input
                  id="unit-name"
                  maxLength={100}
                  value={form.name}
                  onChange={(event) => setForm({ ...form, name: event.target.value })}
                  placeholder="مثلاً گرم"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="unit-symbol">نماد</Label>
                <Input
                  id="unit-symbol"
                  maxLength={20}
                  dir="ltr"
                  value={form.symbol}
                  onChange={(event) => setForm({ ...form, symbol: event.target.value })}
                  placeholder="g"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="unit-type">نوع واحد</Label>
                <select
                  id="unit-type"
                  value={form.unitType}
                  onChange={(event) => setForm({ ...form, unitType: event.target.value })}
                  className="h-9 w-full rounded-3xl border border-input bg-white px-3 text-sm focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                >
                  {UNIT_TYPES.map((type) => (
                    <option key={type.value} value={type.value}>{type.label}</option>
                  ))}
                </select>
              </div>
              {unitId !== null && (
                <div className="space-y-2">
                  <Label htmlFor="unit-active">وضعیت</Label>
                  <select
                    id="unit-active"
                    value={isActive ? "true" : "false"}
                    onChange={(event) => setIsActive(event.target.value === "true")}
                    className="h-9 w-full rounded-3xl border border-input bg-white px-3 text-sm focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                  >
                    <option value="true">فعال</option>
                    <option value="false">غیرفعال</option>
                  </select>
                </div>
              )}
            </div>
            {error && <p className="text-sm text-destructive" role="alert">{error}</p>}
            <DialogFooter className="flex-row-reverse">
              <Button type="submit" disabled={saving}>
                {saving ? "در حال ذخیره..." : unitId === null ? "ثبت واحد" : "ذخیره تغییرات"}
              </Button>
              <Button type="button" variant="outline" disabled={saving} onClick={() => onOpenChange(false)}>
                انصراف
              </Button>
            </DialogFooter>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
