"use client";

import { useEffect, useState } from "react";
import { CheckCircle2, LoaderCircle, Play } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog";
import { handleApiError } from "@/lib/api/handle-api-error";

import {
  ReceptionService,
  type ReceptionServiceStatus,
} from "./reception.service";
import type { ReceptionDetails } from "./reception.types";

const RECEPTION_STATUS: Record<number, string> = {
  1: "در انتظار",
  2: "در حال انجام",
  3: "تکمیل‌شده",
  4: "لغوشده",
};

const SERVICE_STATUS: Record<number, string> = {
  1: "در انتظار",
  2: "در حال انجام",
  3: "تکمیل‌شده",
  4: "لغوشده",
};

function amountLabel(amount: number) {
  return `${new Intl.NumberFormat("fa-IR").format(amount)} تومان`;
}

function dateTimeLabel(value: string) {
  const date = new Date(value);
  if (!Number.isFinite(date.getTime())) return "—";
  return new Intl.DateTimeFormat("fa-IR", {
    timeZone: "Asia/Tehran",
    dateStyle: "long",
    timeStyle: "short",
  }).format(date);
}

export function ReceptionDetailsDialog({
  receptionId,
  open,
  onOpenChange,
}: {
  receptionId: number | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const [details, setDetails] = useState<ReceptionDetails | null>(null);
  const [loading, setLoading] = useState(open && receptionId !== null);
  const [error, setError] = useState<string | null>(null);
  const [serviceActionId, setServiceActionId] = useState<number | null>(null);
  const [serviceActionError, setServiceActionError] = useState<string | null>(null);
  const [serviceActionMessage, setServiceActionMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!open || receptionId === null) return;

    const controller = new AbortController();
    void new ReceptionService()
      .getById(receptionId, { signal: controller.signal })
      .then((response) => {
        if (!response.data) throw new Error(response.message ?? "جزئیات پذیرش دریافت نشد.");
        if (!controller.signal.aborted) {
          setDetails(response.data);
          setError(null);
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) setError(handleApiError(cause));
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [open, receptionId]);

  async function changeServiceStatus(
    serviceId: number,
    status: ReceptionServiceStatus,
  ) {
    if (!details || !receptionId || serviceActionId !== null) return;
    setServiceActionId(serviceId);
    setServiceActionError(null);
    setServiceActionMessage(null);

    try {
      const service = new ReceptionService();
      const response = await service.changeServiceStatus(
        receptionId,
        serviceId,
        details.salonId,
        status,
      );
      if (!response.data) {
        throw new Error(response.message ?? "وضعیت خدمت تغییر نکرد.");
      }

      setDetails((current) => current && ({
        ...current,
        services: current.services.map((item) =>
          item.id === serviceId ? { ...item, status } : item,
        ),
      }));
      setServiceActionMessage(
        status === 3
          ? "خدمت تکمیل شد؛ محاسبه و ثبت پورسانت در Finance به‌صورت خودکار در صف انجام قرار گرفت."
          : "خدمت شروع شد.",
      );
    } catch (cause: unknown) {
      setServiceActionError(handleApiError(cause));
    } finally {
      setServiceActionId(null);
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        onOpenChange(nextOpen);
        if (!nextOpen) {
          setDetails(null);
          setError(null);
          setLoading(false);
          setServiceActionError(null);
          setServiceActionMessage(null);
        }
      }}
    >
      <DialogContent
        dir="rtl"
        className="max-h-[calc(100dvh-2rem)] overflow-y-auto rounded-2xl p-4 sm:p-6"
        aria-describedby="reception-details-description"
      >
        <div>
          <DialogTitle className="text-lg font-bold text-slate-900">
            {details ? `جزئیات پذیرش ${details.receptionNumber}` : "جزئیات پذیرش"}
          </DialogTitle>
          <DialogDescription id="reception-details-description" className="mt-2 text-sm text-slate-500">
            اطلاعات مشتری، خدمات و پرداخت‌های ثبت‌شده
          </DialogDescription>
        </div>
        {loading ? (
          <div className="flex min-h-48 items-center justify-center gap-2 text-sm text-slate-500">
            <LoaderCircle className="size-5 animate-spin" />
            در حال دریافت جزئیات...
          </div>
        ) : error ? (
          <div className="rounded-xl border border-rose-200 bg-rose-50 p-4 text-sm leading-6 text-rose-700" role="alert">
            {error}
          </div>
        ) : details ? (
          <div className="space-y-5">
            {(serviceActionError || serviceActionMessage) && (
              <p
                role={serviceActionError ? "alert" : "status"}
                className={`rounded-xl border px-3 py-2.5 text-xs leading-5 ${
                  serviceActionError
                    ? "border-rose-200 bg-rose-50 text-rose-700"
                    : "border-emerald-200 bg-emerald-50 text-emerald-700"
                }`}
              >
                {serviceActionError ?? serviceActionMessage}
              </p>
            )}
            <dl className="grid gap-3 sm:grid-cols-2">
              <DetailValue label="مشتری" value={details.customerName} />
              <DetailValue label="تاریخ پذیرش" value={dateTimeLabel(details.receptionDate)} />
              <DetailValue label="پذیرشگر" value={new Intl.NumberFormat("fa-IR").format(details.receptionistId)} />
              <DetailValue label="وضعیت" value={RECEPTION_STATUS[details.status] ?? "نامشخص"} />
              <DetailValue label="مبلغ کل" value={amountLabel(details.totalAmount)} />
              <DetailValue label="تخفیف کل" value={amountLabel(details.totalDiscountAmount)} />
              <DetailValue label="پرداخت‌شده" value={amountLabel(details.paidAmount)} />
              <DetailValue label="مانده" value={amountLabel(details.remainingAmount)} />
            </dl>

            <section>
              <h3 className="mb-2 text-sm font-semibold text-slate-800">خدمات پذیرش</h3>
              {details.services.length === 0 ? (
                <p className="rounded-xl bg-slate-50 p-3 text-xs text-slate-500">خدمتی برای این پذیرش ثبت نشده است.</p>
              ) : (
                <div className="space-y-2">
                  {details.services.map((service) => (
                    <article key={service.id} className="rounded-xl border border-slate-200 p-3">
                      <div className="flex flex-wrap items-start justify-between gap-2">
                        <div>
                          <p className="text-sm font-medium text-slate-800">{service.serviceName}</p>
                          <p className="mt-1 text-xs text-slate-500">
                            تعداد {new Intl.NumberFormat("fa-IR").format(service.quantity)} · {SERVICE_STATUS[service.status] ?? "نامشخص"}
                          </p>
                        </div>
                        <p className="text-sm font-semibold text-slate-700">{amountLabel(service.finalAmount)}</p>
                      </div>
                      {service.personnelId !== null && (
                        <p className="mt-2 text-xs text-slate-500">
                          شناسه پرسنل: {new Intl.NumberFormat("fa-IR").format(service.personnelId)}
                        </p>
                      )}
                      {service.personnelId !== null &&
                        (service.status === 1 || service.status === 2) && (
                          <div className="mt-3 flex flex-wrap gap-2">
                            {service.status === 1 && (
                              <Button
                                type="button"
                                size="sm"
                                disabled={serviceActionId !== null}
                                onClick={() => void changeServiceStatus(service.id, 2)}
                                className="h-8 rounded-lg bg-fuchsia-700 px-3 text-xs text-white hover:bg-fuchsia-800"
                              >
                                {serviceActionId === service.id
                                  ? <LoaderCircle className="ml-1 size-3.5 animate-spin" />
                                  : <Play className="ml-1 size-3.5" />}
                                شروع خدمت
                              </Button>
                            )}
                            {service.status === 2 && (
                              <Button
                                type="button"
                                size="sm"
                                disabled={serviceActionId !== null}
                                onClick={() => void changeServiceStatus(service.id, 3)}
                                className="h-8 rounded-lg bg-emerald-600 px-3 text-xs text-white hover:bg-emerald-700"
                              >
                                {serviceActionId === service.id
                                  ? <LoaderCircle className="ml-1 size-3.5 animate-spin" />
                                  : <CheckCircle2 className="ml-1 size-3.5" />}
                                تکمیل خدمت
                              </Button>
                            )}
                          </div>
                        )}
                    </article>
                  ))}
                </div>
              )}
            </section>

            <section>
              <h3 className="mb-2 text-sm font-semibold text-slate-800">پرداخت‌ها</h3>
              {details.payments.length === 0 ? (
                <p className="rounded-xl bg-slate-50 p-3 text-xs text-slate-500">پرداختی ثبت نشده است.</p>
              ) : (
                <div className="space-y-2">
                  {details.payments.map((payment) => (
                    <div key={payment.id} className="flex flex-wrap justify-between gap-2 rounded-xl border border-slate-200 p-3 text-xs">
                      <span>{payment.paymentMethod} · {dateTimeLabel(payment.paidAt)}</span>
                      <span className="font-semibold">{amountLabel(payment.amount)}</span>
                    </div>
                  ))}
                </div>
              )}
            </section>

            {details.notes && (
              <DetailValue label="یادداشت" value={details.notes} />
            )}
          </div>
        ) : null}
        <div className="flex justify-end border-t border-slate-100 pt-4">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)} className="rounded-xl">
            بستن
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}

function DetailValue({ label, value }: { label: string; value: string }) {
  return (
    <div className="min-w-0 rounded-xl bg-slate-50 p-3">
      <dt className="text-xs text-slate-500">{label}</dt>
      <dd className="mt-1 break-words text-sm font-medium text-slate-800">{value}</dd>
    </div>
  );
}
