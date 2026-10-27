"use client";

import { useEffect, useState } from "react";
import { Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";
import { MaterialService } from "@/features/inventory/materials/material.service";
import type { MaterialDto } from "@/features/inventory/materials/material.types";

import { ServiceMaterialFormSchema } from "./service.schema";
import { ServiceCatalogService } from "./service.service";
import type { ServiceDto, ServiceMaterialDto } from "./service.types";

type DetailsState = {
  key: string;
  service: ServiceDto | null;
  materials: ServiceMaterialDto[];
  error: string | null;
  loading: boolean;
};

export function ServiceDetailsDialog({
  open,
  onOpenChange,
  serviceId,
  salonId,
  onChanged,
  onEdit,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  serviceId: number | null;
  salonId: number;
  onChanged: () => void;
  onEdit: (id: number) => void;
}) {
  const [state, setState] = useState<DetailsState>({
    key: "",
    service: null,
    materials: [],
    error: null,
    loading: false,
  });
  const [options, setOptions] = useState<MaterialDto[]>([]);
  const [materialId, setMaterialId] = useState("");
  const [quantity, setQuantity] = useState("1");
  const [description, setDescription] = useState("");
  const [materialError, setMaterialError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [removeTarget, setRemoveTarget] = useState<ServiceMaterialDto | null>(null);
  const [removing, setRemoving] = useState(false);
  const [removeError, setRemoveError] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editingQuantity, setEditingQuantity] = useState("");
  const [editingDescription, setEditingDescription] = useState("");
  const [editError, setEditError] = useState<string | null>(null);
  const requestKey = `${open}:${salonId}:${serviceId ?? ""}`;

  useEffect(() => {
    if (!open || serviceId === null) return;
    const controller = new AbortController();

    Promise.all([
      new ServiceCatalogService().getById(serviceId, { signal: controller.signal }),
      new ServiceCatalogService().getMaterials(serviceId, {
        signal: controller.signal,
      }),
    ])
      .then(([serviceResponse, materialResponse]) => {
        if (!serviceResponse.data) {
          throw new Error(serviceResponse.message ?? "اطلاعات خدمت دریافت نشد.");
        }
        if (!materialResponse.data) {
          throw new Error(materialResponse.message ?? "مواد مصرفی دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setState({
            key: requestKey,
            service: serviceResponse.data,
            materials: materialResponse.data,
            error: null,
            loading: false,
          });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            key: requestKey,
            service: null,
            materials: [],
            error: handleApiError(error),
            loading: false,
          });
        }
      });

    return () => controller.abort();
  }, [open, serviceId, salonId, requestKey]);

  useEffect(() => {
    if (!open) return;
    const controller = new AbortController();
    const api = new MaterialService();
    Promise.resolve()
      .then(async () => {
        const first = await api.getAll(
          { PageNumber: 1, PageSize: 100, IsActive: true },
          { signal: controller.signal },
        );
        if (!first.data) {
          throw new Error(first.message ?? "فهرست مواد مصرفی دریافت نشد.");
        }
        const items = [...first.data.list];
        for (let page = 2; page <= first.data.pagination.totalPages; page += 1) {
          const response = await api.getAll(
            { PageNumber: page, PageSize: 100, IsActive: true },
            { signal: controller.signal },
          );
          if (!response.data) {
            throw new Error(response.message ?? "فهرست مواد مصرفی کامل نشد.");
          }
          items.push(...response.data.list);
        }
        if (!controller.signal.aborted) {
          setOptions(items.filter((material) => material.isActive));
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) setMaterialError(handleApiError(error));
      });
    return () => controller.abort();
  }, [open]);

  async function refreshMaterials() {
    if (serviceId === null) return;
    const response = await new ServiceCatalogService().getMaterials(serviceId);
    if (!response.data) {
      throw new Error(response.message ?? "فهرست مواد مصرفی به‌روزرسانی نشد.");
    }
    setState((current) => ({
      ...current,
      materials: response.data ?? [],
    }));
  }

  async function addMaterial(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (serviceId === null || saving) return;
    setMaterialError(null);
    const parsed = ServiceMaterialFormSchema.safeParse({
      materialId: Number(materialId),
      quantityPerService: Number(quantity),
      description,
    });
    if (!parsed.success) {
      setMaterialError(parsed.error.issues[0]?.message ?? "اطلاعات ماده معتبر نیست.");
      return;
    }

    setSaving(true);
    try {
      await new ServiceCatalogService().addMaterial({
        serviceId,
        materialId: parsed.data.materialId,
        quantityPerService: parsed.data.quantityPerService,
        description: parsed.data.description.trim() || null,
      });
      await refreshMaterials();
      setMaterialId("");
      setQuantity("1");
      setDescription("");
      onChanged();
    } catch (error: unknown) {
      setMaterialError(handleApiError(error));
    } finally {
      setSaving(false);
    }
  }

  async function saveMaterial(material: ServiceMaterialDto) {
    if (serviceId === null || saving) return;
    setEditError(null);
    const parsed = ServiceMaterialFormSchema.safeParse({
      materialId: material.materialId,
      quantityPerService: Number(editingQuantity),
      description: editingDescription,
    });
    if (!parsed.success) {
      setEditError(parsed.error.issues[0]?.message ?? "اطلاعات ماده معتبر نیست.");
      return;
    }
    setSaving(true);
    try {
      await new ServiceCatalogService().updateMaterial(material.id, {
        serviceId,
        materialId: material.materialId,
        quantityPerService: parsed.data.quantityPerService,
        description: parsed.data.description.trim() || null,
      });
      await refreshMaterials();
      setEditingId(null);
      onChanged();
    } catch (error: unknown) {
      setEditError(handleApiError(error));
    } finally {
      setSaving(false);
    }
  }

  async function confirmRemove() {
    if (!removeTarget || removing) return;
    setRemoving(true);
    setRemoveError(null);
    try {
      await new ServiceCatalogService().removeMaterial(removeTarget.id);
      await refreshMaterials();
      setRemoveTarget(null);
      onChanged();
    } catch (error: unknown) {
      setRemoveError(handleApiError(error));
    } finally {
      setRemoving(false);
    }
  }

  const requestPending = open && state.key !== requestKey;
  const displayLoading = requestPending || state.loading;
  const displayError = state.key === requestKey ? state.error : null;
  const displayService = state.key === requestKey ? state.service : null;
  const displayMaterials = state.key === requestKey ? state.materials : [];

  return (
    <>
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent dir="rtl" className="max-h-[90vh] gap-4 overflow-y-auto sm:max-w-3xl">
          <DialogTitle className="text-right text-lg text-fuchsia-950">
            جزئیات خدمت
          </DialogTitle>
          {displayLoading ? (
            <p className="py-8 text-center text-sm" role="status">در حال دریافت اطلاعات...</p>
          ) : displayError ? (
            <p role="alert" className="py-6 text-center text-sm text-destructive">{displayError}</p>
          ) : displayService ? (
            <div className="space-y-5">
              <section className="grid gap-3 rounded-lg border border-slate-100 p-4 sm:grid-cols-3">
                <div><p className="text-xs text-slate-500">نام خدمت</p><p className="mt-1 text-sm font-semibold">{displayService.name}</p></div>
                <div><p className="text-xs text-slate-500">کد خدمت</p><p dir="ltr" className="mt-1 text-sm">{displayService.code}</p></div>
                <div><p className="text-xs text-slate-500">مدت زمان</p><p className="mt-1 text-sm">{displayService.durationMinutes.toLocaleString("fa-IR")} دقیقه</p></div>
                {displayService.description && (
                  <div className="sm:col-span-3"><p className="text-xs text-slate-500">توضیحات</p><p className="mt-1 text-sm">{displayService.description}</p></div>
                )}
              </section>
              <div className="flex justify-end">
                <Button type="button" variant="outline" onClick={() => onEdit(displayService.id)}>
                  ویرایش خدمت
                </Button>
              </div>

              <section className="space-y-3">
                <h3 className="text-sm font-semibold text-slate-800">مواد مصرفی خدمت</h3>
                {displayMaterials.length === 0 ? (
                  <p className="rounded-lg bg-slate-50 p-4 text-center text-sm text-slate-500">برای این خدمت ماده مصرفی ثبت نشده است.</p>
                ) : (
                  <div className="space-y-2">
                    {displayMaterials.map((material) => (
                      <div key={material.id} className="grid gap-2 rounded-lg border border-slate-100 p-3 sm:grid-cols-[1fr_130px_1fr_auto] sm:items-center">
                        <div>
                          <p className="text-sm font-medium">{material.materialName}</p>
                          <p className="text-xs text-slate-500">{material.baseUnitName} ({material.baseUnitSymbol})</p>
                        </div>
                        {editingId === material.id ? (
                          <Input
                            aria-label={`مقدار مصرف ${material.materialName}`}
                            type="number"
                            min="0.001"
                            step="0.001"
                            value={editingQuantity}
                            onChange={(event) => setEditingQuantity(event.target.value)}
                            className="h-9"
                          />
                        ) : (
                          <p className="text-sm">{material.quantityPerService.toLocaleString("fa-IR")} {material.baseUnitSymbol}</p>
                        )}
                        {editingId === material.id ? (
                          <Input
                            aria-label={`توضیحات ${material.materialName}`}
                            value={editingDescription}
                            maxLength={1000}
                            onChange={(event) => setEditingDescription(event.target.value)}
                            className="h-9"
                            placeholder="توضیحات (اختیاری)"
                          />
                        ) : (
                          <p className="text-xs text-slate-500">{material.description || "بدون توضیحات"}</p>
                        )}
                        <div className="flex gap-1">
                          {editingId === material.id ? (
                            <>
                              <Button type="button" size="sm" disabled={saving} onClick={() => void saveMaterial(material)}>ذخیره</Button>
                              <Button type="button" size="sm" variant="outline" disabled={saving} onClick={() => setEditingId(null)}>لغو</Button>
                            </>
                          ) : (
                            <>
                              <Button
                                type="button"
                                size="sm"
                                variant="outline"
                                onClick={() => {
                                  setEditingId(material.id);
                                  setEditingQuantity(String(material.quantityPerService));
                                  setEditingDescription(material.description ?? "");
                                  setEditError(null);
                                }}
                              >
                                ویرایش
                              </Button>
                              <Button
                                type="button"
                                size="icon"
                                variant="ghost"
                                aria-label={`حذف ${material.materialName} از خدمت`}
                                className="text-destructive"
                                onClick={() => {
                                  setRemoveTarget(material);
                                  setRemoveError(null);
                                }}
                              >
                                <Trash2 className="size-4" />
                              </Button>
                            </>
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                )}
                {editError && <p role="alert" className="text-sm text-destructive">{editError}</p>}
              </section>

              <form onSubmit={(event) => void addMaterial(event)} className="space-y-3 rounded-lg border border-fuchsia-100 bg-fuchsia-50/40 p-4">
                <h3 className="text-sm font-semibold text-slate-800">افزودن ماده مصرفی</h3>
                <div className="grid gap-3 sm:grid-cols-3">
                  <label className="space-y-1 text-xs text-slate-600">
                    <span>ماده</span>
                    <select
                      value={materialId}
                      onChange={(event) => setMaterialId(event.target.value)}
                      className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm"
                    >
                      <option value="">انتخاب ماده</option>
                      {options
                        .filter((option) => !displayMaterials.some((item) => item.materialId === option.id))
                        .map((option) => (
                          <option key={option.id} value={option.id}>
                            {option.name} — موجودی {option.currentStock} {option.baseUnitSymbol}
                          </option>
                        ))}
                    </select>
                  </label>
                  <label className="space-y-1 text-xs text-slate-600">
                    <span>مقدار مصرف برای هر خدمت</span>
                    <Input type="number" min="0.001" step="0.001" value={quantity} onChange={(event) => setQuantity(event.target.value)} className="h-10 bg-white" />
                  </label>
                  <label className="space-y-1 text-xs text-slate-600">
                    <span>توضیحات</span>
                    <Input value={description} maxLength={1000} onChange={(event) => setDescription(event.target.value)} className="h-10 bg-white" placeholder="اختیاری" />
                  </label>
                </div>
                {materialError && <p role="alert" className="text-sm text-destructive">{materialError}</p>}
                <Button type="submit" disabled={saving || !materialId} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                  <Plus className="size-4" />
                  {saving ? "در حال ذخیره..." : "افزودن ماده"}
                </Button>
              </form>
            </div>
          ) : null}
        </DialogContent>
      </Dialog>

      <Dialog open={removeTarget !== null} onOpenChange={(nextOpen) => !nextOpen && !removing && setRemoveTarget(null)}>
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">حذف ماده از خدمت</DialogTitle>
            <DialogDescription className="text-right leading-6">
              ماده «{removeTarget?.materialName}» از فهرست مواد مصرفی این خدمت حذف شود؟
            </DialogDescription>
          </div>
          {removeError && <p role="alert" className="text-sm text-destructive">{removeError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" variant="destructive" disabled={removing} onClick={() => void confirmRemove()}>
              {removing ? "در حال حذف..." : "حذف"}
            </Button>
            <Button type="button" variant="outline" disabled={removing} onClick={() => setRemoveTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
