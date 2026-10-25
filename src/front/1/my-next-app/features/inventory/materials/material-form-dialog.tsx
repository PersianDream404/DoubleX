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

import { MaterialFormSchema } from "./material.schema";
import { MaterialService } from "./material.service";
import type { MaterialDto } from "./material.types";
import { UnitService } from "../units/unit.service";
import type { UnitDto } from "../units/unit.types";

interface MaterialFormDialogProps {
  open: boolean;
  materialId: number | null;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
}

type MaterialFormState = {
  name: string;
  code: string;
  brand: string;
  baseUnitId: string;
  purchaseUnitId: string;
  purchaseConversionFactor: string;
  minimumStock: string;
};

const DEFAULT_FORM: MaterialFormState = {
  name: "",
  code: "",
  brand: "",
  baseUnitId: "",
  purchaseUnitId: "",
  purchaseConversionFactor: "1",
  minimumStock: "0",
};

function toFormState(material: MaterialDto): MaterialFormState {
  return {
    name: material.name,
    code: material.code,
    brand: material.brand ?? "",
    baseUnitId: String(material.baseUnitId),
    purchaseUnitId: String(material.purchaseUnitId),
    purchaseConversionFactor: String(material.purchaseConversionFactor),
    minimumStock: String(material.minimumStock),
  };
}

export function MaterialFormDialog({
  open,
  materialId,
  onOpenChange,
  onSaved,
}: MaterialFormDialogProps) {
  const [form, setForm] = useState(DEFAULT_FORM);
  const [units, setUnits] = useState<UnitDto[]>([]);
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
        setLoading(true);
        setError(null);
        setForm(DEFAULT_FORM);

        const unitsPromise = new UnitService().getAll(
          { PageNumber: 1, PageSize: 501 },
          { signal: controller.signal },
        );
        const materialPromise =
          materialId === null
            ? Promise.resolve(null)
            : new MaterialService().getById(materialId, { signal: controller.signal });
        return Promise.all([unitsPromise, materialPromise]);
      })
      .then(([unitsResponse, materialResponse]) => {
        if (!unitsResponse.data) {
          throw new Error(unitsResponse.message ?? "فهرست واحدها دریافت نشد.");
        }
        setUnits(unitsResponse.data.list);
        if (materialId === null) {
          setIsActive(true);
          return;
        }
        if (!materialResponse?.data) {
          throw new Error(materialResponse?.message ?? "اطلاعات کالا دریافت نشد.");
        }
        setForm(toFormState(materialResponse.data));
        setIsActive(materialResponse.data.isActive);
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
  }, [materialId, open]);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (saving) {
      return;
    }

    const parsed = MaterialFormSchema.safeParse({
      name: form.name,
      code: form.code,
      brand: form.brand.trim() || null,
      baseUnitId: Number(form.baseUnitId),
      purchaseUnitId: Number(form.purchaseUnitId),
      purchaseConversionFactor: Number(form.purchaseConversionFactor),
      minimumStock: Number(form.minimumStock),
    });
    if (!parsed.success) {
      setError(parsed.error.issues[0]?.message ?? "اطلاعات واردشده معتبر نیست.");
      return;
    }

    setSaving(true);
    setError(null);
    try {
      const service = new MaterialService();
      if (materialId === null) {
        await service.create(parsed.data);
      } else {
        await service.update({
          id: materialId,
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
      <DialogContent dir="rtl" className="max-h-[90vh] gap-4 overflow-y-auto sm:max-w-2xl">
        <div className="space-y-2">
          <DialogTitle className="text-right">
            {materialId === null ? "ثبت کالای جدید" : "ویرایش کالا"}
          </DialogTitle>
          <DialogDescription className="text-right">
            همه مقادیر موجودی بر اساس واحد پایه ذخیره می‌شوند.
          </DialogDescription>
        </div>
        {loading ? (
          <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات کالا و واحدها...</p>
        ) : (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="material-name">نام کالا</Label>
                <Input id="material-name" maxLength={200} required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-code">کد کالا</Label>
                <Input id="material-code" maxLength={50} dir="ltr" required value={form.code} onChange={(event) => setForm({ ...form, code: event.target.value })} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-brand">برند (اختیاری)</Label>
                <Input id="material-brand" maxLength={100} value={form.brand} onChange={(event) => setForm({ ...form, brand: event.target.value })} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-base-unit">واحد پایه</Label>
                <select
                  id="material-base-unit"
                  required
                  value={form.baseUnitId}
                  onChange={(event) => setForm({ ...form, baseUnitId: event.target.value })}
                  className="h-9 w-full rounded-3xl border border-input bg-white px-3 text-sm focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                >
                  <option value="">انتخاب واحد پایه</option>
                  {units.map((unit) => (
                    <option key={unit.id} value={unit.id} disabled={!unit.isActive}>
                      {unit.name} ({unit.symbol}){unit.isActive ? "" : " - غیرفعال"}
                    </option>
                  ))}
                </select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-purchase-unit">واحد خرید</Label>
                <select
                  id="material-purchase-unit"
                  required
                  value={form.purchaseUnitId}
                  onChange={(event) => setForm({ ...form, purchaseUnitId: event.target.value })}
                  className="h-9 w-full rounded-3xl border border-input bg-white px-3 text-sm focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                >
                  <option value="">انتخاب واحد خرید</option>
                  {units.map((unit) => (
                    <option key={unit.id} value={unit.id} disabled={!unit.isActive}>
                      {unit.name} ({unit.symbol}){unit.isActive ? "" : " - غیرفعال"}
                    </option>
                  ))}
                </select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-conversion">ضریب تبدیل واحد خرید به پایه</Label>
                <Input id="material-conversion" type="number" min="0.000001" max="999999999999.999999" step="0.000001" required value={form.purchaseConversionFactor} onChange={(event) => setForm({ ...form, purchaseConversionFactor: event.target.value })} />
                <p className="text-xs text-muted-foreground">مثال: هر تیوب معادل ۱۰۰ گرم، ضریب ۱۰۰</p>
              </div>
              <div className="space-y-2">
                <Label htmlFor="material-minimum-stock">حداقل موجودی (واحد پایه)</Label>
                <Input id="material-minimum-stock" type="number" min="0" max="999999999999999.999" step="0.001" required value={form.minimumStock} onChange={(event) => setForm({ ...form, minimumStock: event.target.value })} />
              </div>
              {materialId !== null && (
                <div className="space-y-2">
                  <Label htmlFor="material-active">وضعیت</Label>
                  <select
                    id="material-active"
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
              <Button type="submit" disabled={saving || units.length === 0}>
                {saving ? "در حال ذخیره..." : materialId === null ? "ثبت کالا" : "ذخیره تغییرات"}
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
