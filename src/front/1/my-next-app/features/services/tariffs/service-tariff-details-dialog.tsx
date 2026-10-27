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
import { ServiceCatalogService } from "@/features/services/list/service.service";
import type { ServiceDto } from "@/features/services/list/service.types";

import { TariffServiceFormSchema } from "./service-tariff.schema";
import { ServiceTariffService } from "./service-tariff.service";
import { formatApiDate } from "./service-tariff-date";
import type { ServiceTariffDto, ServiceTariffItemDto } from "./service-tariff.types";

type DetailsState = {
  key: string;
  tariff: ServiceTariffDto | null;
  services: ServiceDto[];
  error: string | null;
};

export function ServiceTariffDetailsDialog({
  open,
  onOpenChange,
  tariffId,
  salonId,
  onChanged,
  onEdit,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  tariffId: number | null;
  salonId: number;
  onChanged: () => void;
  onEdit: (id: number) => void;
}) {
  const [state, setState] = useState<DetailsState>({
    key: "",
    tariff: null,
    services: [],
    error: null,
  });
  const [serviceId, setServiceId] = useState("");
  const [amount, setAmount] = useState("");
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [editingServiceId, setEditingServiceId] = useState<number | null>(null);
  const [editingAmount, setEditingAmount] = useState("");
  const [editError, setEditError] = useState<string | null>(null);
  const [removeTarget, setRemoveTarget] = useState<ServiceTariffItemDto | null>(null);
  const [removing, setRemoving] = useState(false);
  const [removeError, setRemoveError] = useState<string | null>(null);
  const requestKey = `${open}:${salonId}:${tariffId ?? ""}`;

  useEffect(() => {
    if (!open || tariffId === null) return;
    const controller = new AbortController();
    Promise.all([
      new ServiceTariffService().getById(tariffId, { signal: controller.signal }),
      new ServiceCatalogService().getAll({ signal: controller.signal }),
    ])
      .then(([tariffResponse, servicesResponse]) => {
        if (!tariffResponse.data) {
          throw new Error(tariffResponse.message ?? "اطلاعات تعرفه دریافت نشد.");
        }
        if (!servicesResponse.data) {
          throw new Error(servicesResponse.message ?? "فهرست خدمات دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setState({
            key: requestKey,
            tariff: tariffResponse.data,
            services: servicesResponse.data,
            error: null,
          });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            key: requestKey,
            tariff: null,
            services: [],
            error: handleApiError(reason),
          });
        }
      });
    return () => controller.abort();
  }, [open, tariffId, salonId, requestKey]);

  const current = state.key === requestKey ? state : null;
  const availableServices = (current?.services ?? []).filter(
    (service) => service.isActive && !current?.tariff?.services.some((item) => item.serviceId === service.id),
  );

  async function refreshTariff() {
    if (tariffId === null) return;
    const response = await new ServiceTariffService().getById(tariffId);
    if (!response.data) {
      throw new Error(response.message ?? "اطلاعات تعرفه به‌روزرسانی نشد.");
    }
    setState((value) => ({ ...value, tariff: response.data ?? null, key: requestKey }));
  }

  async function addService(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (tariffId === null || saving) return;
    setFormError(null);
    const parsed = TariffServiceFormSchema.safeParse({
      serviceId: Number(serviceId),
      amount: Number(amount),
    });
    if (!parsed.success) {
      setFormError(parsed.error.issues[0]?.message ?? "اطلاعات خدمت معتبر نیست.");
      return;
    }

    setSaving(true);
    try {
      await new ServiceTariffService().addServices(tariffId, {
        services: [{ serviceId: parsed.data.serviceId, amount: parsed.data.amount }],
      });
      await refreshTariff();
      setServiceId("");
      setAmount("");
      onChanged();
    } catch (reason: unknown) {
      setFormError(handleApiError(reason));
    } finally {
      setSaving(false);
    }
  }

  async function saveAmount(service: ServiceTariffItemDto) {
    if (tariffId === null || saving) return;
    setEditError(null);
    const parsed = TariffServiceFormSchema.safeParse({
      serviceId: service.serviceId,
      amount: Number(editingAmount),
    });
    if (!parsed.success) {
      setEditError(parsed.error.issues[0]?.message ?? "مبلغ معتبر نیست.");
      return;
    }
    setSaving(true);
    try {
      await new ServiceTariffService().updateService(tariffId, service.serviceId, {
        amount: parsed.data.amount,
      });
      await refreshTariff();
      setEditingServiceId(null);
      onChanged();
    } catch (reason: unknown) {
      setEditError(handleApiError(reason));
    } finally {
      setSaving(false);
    }
  }

  async function removeService() {
    if (!removeTarget || tariffId === null || removing) return;
    setRemoving(true);
    setRemoveError(null);
    try {
      await new ServiceTariffService().removeService(tariffId, removeTarget.serviceId);
      await refreshTariff();
      setRemoveTarget(null);
      onChanged();
    } catch (reason: unknown) {
      setRemoveError(handleApiError(reason));
    } finally {
      setRemoving(false);
    }
  }

  return (
    <>
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent dir="rtl" className="max-h-[90vh] gap-4 overflow-y-auto sm:max-w-3xl">
          <DialogTitle className="text-right text-lg text-fuchsia-950">جزئیات تعرفه</DialogTitle>
          {!current ? (
            <p role="status" className="py-8 text-center text-sm">در حال دریافت اطلاعات تعرفه...</p>
          ) : current.error ? (
            <p role="alert" className="py-6 text-center text-sm text-destructive">{current.error}</p>
          ) : current.tariff ? (
            <div className="space-y-5">
              <section className="grid gap-3 rounded-lg border border-slate-100 p-4 sm:grid-cols-3">
                <div><p className="text-xs text-slate-500">نام تعرفه</p><p className="mt-1 text-sm font-semibold">{current.tariff.name}</p></div>
                <div><p className="text-xs text-slate-500">شروع اعتبار</p><p className="mt-1 text-sm">{formatApiDate(current.tariff.validFrom)}</p></div>
                <div><p className="text-xs text-slate-500">پایان اعتبار</p><p className="mt-1 text-sm">{formatApiDate(current.tariff.validTo)}</p></div>
              </section>
              <div className="flex justify-end">
                <Button type="button" variant="outline" onClick={() => onEdit(current.tariff!.id)}>ویرایش تعرفه</Button>
              </div>

              <section className="space-y-3">
                <h3 className="text-sm font-semibold text-slate-800">خدمات و قیمت در این تعرفه</h3>
                {current.tariff.services.length === 0 ? (
                  <p className="rounded-lg bg-slate-50 p-4 text-center text-sm text-slate-500">خدمتی به این تعرفه اضافه نشده است.</p>
                ) : (
                  <div className="space-y-2">
                    {current.tariff.services.map((service) => (
                      <div key={service.serviceId} className="grid gap-2 rounded-lg border border-slate-100 p-3 sm:grid-cols-[1fr_180px_auto] sm:items-center">
                        <p className="text-sm font-medium">{service.serviceName}</p>
                        {editingServiceId === service.serviceId ? (
                          <Input
                            aria-label={`قیمت ${service.serviceName}`}
                            type="number"
                            min="0"
                            step="0.01"
                            value={editingAmount}
                            onChange={(event) => setEditingAmount(event.target.value)}
                            className="h-9"
                          />
                        ) : (
                          <p className="text-sm">{service.amount.toLocaleString("fa-IR")} تومان</p>
                        )}
                        <div className="flex gap-1">
                          {editingServiceId === service.serviceId ? (
                            <>
                              <Button type="button" size="sm" disabled={saving} onClick={() => void saveAmount(service)}>ذخیره قیمت</Button>
                              <Button type="button" size="sm" variant="outline" disabled={saving} onClick={() => setEditingServiceId(null)}>لغو</Button>
                            </>
                          ) : (
                            <>
                              <Button
                                type="button"
                                size="sm"
                                variant="outline"
                                onClick={() => {
                                  setEditingServiceId(service.serviceId);
                                  setEditingAmount(String(service.amount));
                                  setEditError(null);
                                }}
                              >
                                ویرایش قیمت
                              </Button>
                              <Button
                                type="button"
                                size="icon"
                                variant="ghost"
                                aria-label={`حذف خدمت ${service.serviceName} از تعرفه`}
                                className="text-destructive"
                                onClick={() => {
                                  setRemoveTarget(service);
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

              <form onSubmit={(event) => void addService(event)} className="space-y-3 rounded-lg border border-fuchsia-100 bg-fuchsia-50/40 p-4">
                <h3 className="text-sm font-semibold text-slate-800">افزودن خدمت به تعرفه</h3>
                <div className="grid gap-3 sm:grid-cols-[1fr_200px_auto] sm:items-end">
                  <label className="space-y-1 text-xs text-slate-600">
                    <span>خدمت</span>
                    <select
                      value={serviceId}
                      onChange={(event) => setServiceId(event.target.value)}
                      className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm"
                    >
                      <option value="">انتخاب خدمت</option>
                      {availableServices.map((service) => (
                        <option key={service.id} value={service.id}>{service.name} ({service.code})</option>
                      ))}
                    </select>
                  </label>
                  <label className="space-y-1 text-xs text-slate-600">
                    <span>قیمت (تومان)</span>
                    <Input type="number" min="0" step="0.01" value={amount} onChange={(event) => setAmount(event.target.value)} className="h-10 bg-white" />
                  </label>
                  <Button type="submit" disabled={saving || !serviceId || amount === ""} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                    <Plus className="size-4" />
                    {saving ? "در حال ذخیره..." : "افزودن خدمت"}
                  </Button>
                </div>
                {formError && <p role="alert" className="text-sm text-destructive">{formError}</p>}
                {availableServices.length === 0 && <p className="text-xs text-slate-500">خدمت فعالِ دیگری برای افزودن وجود ندارد.</p>}
              </form>
            </div>
          ) : null}
        </DialogContent>
      </Dialog>

      <Dialog open={removeTarget !== null} onOpenChange={(nextOpen) => !nextOpen && !removing && setRemoveTarget(null)}>
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">حذف خدمت از تعرفه</DialogTitle>
            <DialogDescription className="text-right leading-6">
              خدمت «{removeTarget?.serviceName}» از این تعرفه حذف شود؟
            </DialogDescription>
          </div>
          {removeError && <p role="alert" className="text-sm text-destructive">{removeError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" variant="destructive" disabled={removing} onClick={() => void removeService()}>
              {removing ? "در حال حذف..." : "حذف خدمت"}
            </Button>
            <Button type="button" variant="outline" disabled={removing} onClick={() => setRemoveTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
