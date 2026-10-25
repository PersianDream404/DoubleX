"use client";

import { useState, type FormEvent } from "react";

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

import type { MaterialDto } from "../materials/material.types";
import { StockMovementQuantitySchema } from "./stock-movement.schema";
import { StockMovementService } from "./stock-movement.service";

export interface StockMovementTarget {
  material: MaterialDto;
  idempotencyKey: string;
}

interface StockMovementDialogProps {
  target: StockMovementTarget | null;
  operation: "receipt" | "consumption";
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
}

const MAX_BASE_QUANTITY = 999999999999999;

export function StockMovementDialog({
  target,
  operation,
  onOpenChange,
  onSaved,
}: StockMovementDialogProps) {
  const [quantity, setQuantity] = useState("");
  const [description, setDescription] = useState("");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const material = target?.material;
  const parsedQuantity = Number(quantity);
  const previewBaseQuantity = material && Number.isFinite(parsedQuantity) && parsedQuantity > 0
    ? operation === "receipt"
      ? parsedQuantity * material.purchaseConversionFactor
      : parsedQuantity
    : null;

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!target || saving) {
      return;
    }

    const parsed = StockMovementQuantitySchema.safeParse({
      quantity: Number(quantity),
      description,
    });
    if (!parsed.success) {
      setError(parsed.error.issues[0]?.message ?? "اطلاعات واردشده معتبر نیست.");
      return;
    }

    if (
      operation === "consumption" &&
      Math.round(parsed.data.quantity * 1000) / 1000 !== parsed.data.quantity
    ) {
      setError("مقدار برداشت حداکثر ۳ رقم اعشار دارد.");
      return;
    }

    const unroundedBaseQuantity = operation === "receipt"
      ? parsed.data.quantity * target.material.purchaseConversionFactor
      : parsed.data.quantity;
    if (
      !Number.isFinite(unroundedBaseQuantity) ||
      unroundedBaseQuantity > MAX_BASE_QUANTITY ||
      Math.abs(unroundedBaseQuantity * 1000 - Math.round(unroundedBaseQuantity * 1000)) >
        0.0000001
    ) {
      setError(
        `مقدار تبدیل‌شده باید حداکثر سه رقم اعشار داشته باشد و در محدوده مجاز واحد پایه (${target.material.baseUnitName}) باشد.`,
      );
      return;
    }

    const baseQuantity = Number(unroundedBaseQuantity.toFixed(3));
    if (baseQuantity <= 0) {
      setError("مقدار تبدیل‌شده باید حداقل ۰٫۰۰۱ واحد پایه باشد.");
      return;
    }

    setSaving(true);
    setError(null);
    try {
      await new StockMovementService().registerMovement({
        materialId: target.material.id,
        type: operation === "receipt" ? 1 : 2,
        quantity: baseQuantity,
        occurredAt: new Date().toISOString(),
        referenceType: null,
        referenceId: null,
        description: parsed.data.description || null,
        idempotencyKey: target.idempotencyKey,
      });
      onOpenChange(false);
      onSaved();
    } catch (cause: unknown) {
      setError(handleApiError(cause));
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog
      open={target !== null}
      onOpenChange={(open) => !saving && onOpenChange(open)}
    >
      <DialogContent dir="rtl" className="gap-4 sm:max-w-lg">
        <div className="space-y-2">
          <DialogTitle className="text-right">
            {operation === "receipt" ? "افزایش موجودی کالا" : "برداشت از موجودی کالا"}
          </DialogTitle>
          <DialogDescription className="text-right leading-6">
            {material
              ? operation === "receipt"
                ? `ثبت ورود برای «${material.name}». مقدار را بر اساس واحد خرید وارد کنید؛ موجودی با واحد پایه ذخیره می‌شود.`
                : `ثبت برداشت برای «${material.name}». مقدار را بر اساس واحد پایه وارد کنید. موجودی منفی مجاز نیست.`
              : ""}
          </DialogDescription>
        </div>
        {material && (
          <form className="space-y-4" onSubmit={(event) => void submit(event)}>
            <div className="rounded-lg border bg-muted/30 p-3 text-sm">
              <p>
                واحد خرید: {material.purchaseUnitName} ({material.purchaseUnitSymbol})
              </p>
              <p className="mt-1">
                واحد پایه: {material.baseUnitName} ({material.baseUnitSymbol})
              </p>
              <p className="mt-1">
                هر {material.purchaseUnitName} ={" "}
                {material.purchaseConversionFactor.toLocaleString("fa-IR")}{" "}
                {material.baseUnitSymbol}
              </p>
              <p className="mt-1 font-medium">
                موجودی فعلی:{" "}
                {material.currentStock.toLocaleString("fa-IR", {
                  maximumFractionDigits: 3,
                })}{" "}
                {material.baseUnitSymbol}
              </p>
            </div>
            <div className="space-y-2">
              <Label htmlFor="stock-receipt-quantity">
                مقدار {operation === "receipt" ? "ورود" : "برداشت"} (
                {operation === "receipt"
                  ? material.purchaseUnitSymbol
                  : material.baseUnitSymbol}
                )
              </Label>
              <Input
                id="stock-receipt-quantity"
                type="number"
                min={operation === "receipt" ? "0.000001" : "0.001"}
                max={MAX_BASE_QUANTITY}
                step={operation === "receipt" ? "any" : "0.001"}
                required
                value={quantity}
                onChange={(event) => setQuantity(event.target.value)}
              />
            </div>
            {previewBaseQuantity !== null && Number.isFinite(previewBaseQuantity) && (
              <p className="text-sm text-muted-foreground" aria-live="polite">
                مقدار {operation === "receipt" ? "افزوده‌شده" : "برداشت‌شده"}:{" "}
                {previewBaseQuantity.toLocaleString("fa-IR", {
                  maximumFractionDigits: 3,
                })}{" "}
                {material.baseUnitSymbol}
              </p>
            )}
            <div className="space-y-2">
              <Label htmlFor="stock-receipt-description">توضیحات (اختیاری)</Label>
              <Input
                id="stock-receipt-description"
                maxLength={1000}
                value={description}
                onChange={(event) => setDescription(event.target.value)}
              />
            </div>
            {error && (
              <p className="text-sm text-destructive" role="alert">
                {error}
              </p>
            )}
            <DialogFooter className="flex-row-reverse">
              <Button type="submit" disabled={saving}>
                {saving
                  ? "در حال ثبت..."
                  : operation === "receipt"
                    ? "ثبت افزایش موجودی"
                    : "ثبت برداشت"}
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled={saving}
                onClick={() => onOpenChange(false)}
              >
                انصراف
              </Button>
            </DialogFooter>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
