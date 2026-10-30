"use client";

import { useEffect, useState } from "react";
import {
  BadgeCheck,
  CalendarDays,
  CheckCircle2,
  CreditCard,
  FlaskConical,
  Hash,
  LoaderCircle,
  NotepadText,
  Play,
  ReceiptText,
  Tag,
  UserRound,
  Wallet,
  X,
} from "lucide-react";

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

function unitCostLabel(amount: number) {
  return `${new Intl.NumberFormat("fa-IR", { maximumFractionDigits: 8 }).format(amount)} تومان`;
}

function receptionNumberLabel(number: number) {
  return new Intl.NumberFormat("fa-IR").format(number);
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
  const [requestState, setRequestState] = useState<
    | { receptionId: number; status: "loading" }
    | { receptionId: number; status: "success"; details: ReceptionDetails }
    | { receptionId: number; status: "error"; error: string }
    | null
  >(() =>
    open && receptionId !== null
      ? { receptionId, status: "loading" }
      : null,
  );
  const [serviceActionId, setServiceActionId] = useState<number | null>(null);
  const [serviceActionError, setServiceActionError] = useState<string | null>(null);
  const [serviceActionMessage, setServiceActionMessage] = useState<string | null>(null);
  const currentRequest =
    requestState?.receptionId === receptionId ? requestState : null;
  const details =
    currentRequest?.status === "success" ? currentRequest.details : null;
  const error =
    currentRequest?.status === "error" ? currentRequest.error : null;
  const loading =
    open &&
    receptionId !== null &&
    (currentRequest === null || currentRequest.status === "loading");

  useEffect(() => {
    if (!open || receptionId === null) return;

    const controller = new AbortController();
    void new ReceptionService()
      .getById(receptionId, { signal: controller.signal })
      .then((response) => {
        if (!response.data) throw new Error(response.message ?? "جزئیات پذیرش دریافت نشد.");
        if (!controller.signal.aborted) {
          setRequestState({
            receptionId,
            status: "success",
            details: response.data,
          });
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) {
          setRequestState({
            receptionId,
            status: "error",
            error: handleApiError(cause),
          });
        }
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

      setRequestState((current) =>
        current?.receptionId === receptionId && current.status === "success"
          ? {
              ...current,
              details: {
                ...current.details,
                services: current.details.services.map((item) =>
                  item.id === serviceId ? { ...item, status } : item,
                ),
              },
            }
          : current,
      );
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
          setRequestState(null);
          setServiceActionError(null);
          setServiceActionMessage(null);
        }
      }}
    >
      <DialogContent
        dir="rtl"
        showCloseButton={false}
        className="!flex !max-h-[calc(100dvh-1.5rem)] !w-[calc(100vw-1.5rem)] !max-w-[1400px] !flex-col !gap-0 overflow-hidden rounded-2xl !p-0 sm:!max-h-[calc(100dvh-3rem)] sm:!w-[calc(100vw-3rem)]"
        aria-describedby="reception-details-description"
      >
        <div className="flex items-center justify-between border-b border-fuchsia-100 bg-gradient-to-l from-fuchsia-50 via-white to-white px-4 py-4 sm:px-6">
          <div className="flex min-w-0 items-center gap-3">
            <span className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-fuchsia-100 text-fuchsia-700">
              <ReceiptText className="size-5" />
            </span>
            <div className="min-w-0">
              <DialogTitle className="truncate text-base font-bold text-slate-900 sm:text-lg">
                {details ? `جزئیات پذیرش ${receptionNumberLabel(details.receptionNumber)}` : "جزئیات پذیرش"}
              </DialogTitle>
              <DialogDescription id="reception-details-description" className="mt-1 text-xs text-slate-500">
                اطلاعات مشتری، خدمات و پرداخت‌های ثبت‌شده
              </DialogDescription>
            </div>
          </div>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            aria-label="بستن جزئیات پذیرش"
            onClick={() => onOpenChange(false)}
            className="size-9 shrink-0 rounded-xl text-slate-500 hover:bg-fuchsia-100 hover:text-fuchsia-700"
          >
            <X className="size-4" />
          </Button>
        </div>
        {loading ? (
          <div className="flex min-h-64 items-center justify-center gap-2 text-sm text-slate-500">
            <LoaderCircle className="size-5 animate-spin" />
            در حال دریافت جزئیات...
          </div>
        ) : error ? (
          <div className="m-5 rounded-xl border border-rose-200 bg-rose-50 p-4 text-sm leading-6 text-rose-700" role="alert">
            {error}
          </div>
        ) : details ? (
          <div className="flex min-h-0 flex-1 flex-col overflow-y-auto lg:grid lg:grid-cols-[320px_minmax(0,1fr)] lg:overflow-hidden">
            <aside className="order-2 shrink-0 border-t border-slate-100 bg-slate-50/70 p-4 sm:p-5 lg:order-none lg:min-h-0 lg:overflow-y-auto lg:border-l lg:border-t-0">
              <section className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm shadow-slate-200/40">
                <SectionHeading icon={ReceiptText} title="مشخصات پذیرش" />
                <dl className="mt-3 divide-y divide-slate-100">
                  <SidebarValue
                    label="شماره پذیرش"
                    value={receptionNumberLabel(details.receptionNumber)}
                  />
                  <SidebarValue label="تاریخ پذیرش" value={dateTimeLabel(details.receptionDate)} />
                  <SidebarValue
                    label="پذیرشگر"
                    value={new Intl.NumberFormat("fa-IR").format(details.receptionistId)}
                  />
                  <div className="flex items-center justify-between gap-3 py-3">
                    <dt className="text-xs text-slate-500">وضعیت</dt>
                    <StatusBadge status={details.status} />
                  </div>
                </dl>
              </section>

              <section className="mt-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm shadow-slate-200/40">
                <SectionHeading icon={UserRound} title="اطلاعات مشتری" />
                <div className="mt-3 rounded-xl bg-fuchsia-50/70 p-3">
                  <p className="text-sm font-semibold text-slate-800">{details.customerName}</p>
                  <p className="mt-1 text-xs text-slate-500">
                    شناسه مشتری: {new Intl.NumberFormat("fa-IR").format(details.customerId)}
                  </p>
                  {details.customerMobilePhone && (
                    <p className="mt-1 text-xs text-slate-600" dir="ltr">
                      {details.customerMobilePhone}
                    </p>
                  )}
                  {details.customerStatus !== null && (
                    <p className="mt-1 text-xs text-slate-500">
                      وضعیت مشتری: {details.customerStatus ? "فعال" : "غیرفعال"}
                    </p>
                  )}
                </div>
              </section>

              <section className="mt-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm shadow-slate-200/40">
                <SectionHeading icon={Tag} title="تعرفه پذیرش" />
                <dl className="mt-3 divide-y divide-slate-100">
                  <SidebarValue
                    label="شناسه تعرفه"
                    value={
                      details.serviceTariffId === null
                        ? "ثبت نشده"
                        : new Intl.NumberFormat("fa-IR").format(details.serviceTariffId)
                    }
                  />
                  <SidebarValue label="مبلغ پایه" value={amountLabel(details.subTotal)} />
                  <SidebarValue label="تخفیف" value={amountLabel(details.totalDiscountAmount)} />
                </dl>
              </section>

              <section className="mt-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm shadow-slate-200/40">
                <SectionHeading icon={NotepadText} title="یادداشت" />
                <p className="mt-3 min-h-16 whitespace-pre-wrap break-words rounded-xl bg-slate-50 p-3 text-xs leading-6 text-slate-600">
                  {details.notes || "یادداشتی ثبت نشده است."}
                </p>
              </section>
            </aside>

            <main className="order-1 shrink-0 space-y-4 bg-white p-4 sm:p-5 lg:order-none lg:min-h-0 lg:overflow-y-auto">
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

              <section className="overflow-hidden rounded-2xl border border-slate-200">
                <div className="flex items-center gap-2 border-b border-fuchsia-100 bg-gradient-to-l from-fuchsia-50/80 to-white px-4 py-3">
                  <CalendarDays className="size-4 text-fuchsia-700" />
                  <h3 className="text-sm font-bold text-fuchsia-800">اطلاعات پذیرش</h3>
                </div>
                <div className="grid gap-px bg-slate-100 sm:grid-cols-3">
                  <SummaryTile
                    icon={Hash}
                    label="شماره پذیرش"
                    value={receptionNumberLabel(details.receptionNumber)}
                  />
                  <SummaryTile
                    icon={CalendarDays}
                    label="تاریخ و ساعت"
                    value={dateTimeLabel(details.receptionDate)}
                  />
                  <SummaryTile
                    icon={BadgeCheck}
                    label="وضعیت پذیرش"
                    value={RECEPTION_STATUS[details.status] ?? "نامشخص"}
                  />
                </div>
              </section>

              <section className="overflow-hidden rounded-2xl border border-slate-200">
                <SectionBar icon={BadgeCheck} title="خدمات پذیرش" />
                {details.services.length === 0 ? (
                  <EmptySection message="خدمتی برای این پذیرش ثبت نشده است." />
                ) : (
                  <div className="space-y-3 p-3">
                    {details.services.map((service) => (
                      <article key={service.id} className="overflow-hidden rounded-xl border border-slate-200">
                        <div className="grid gap-3 bg-slate-50/80 p-3 sm:grid-cols-2 xl:grid-cols-5">
                          <div className="min-w-0 xl:col-span-2">
                            <p className="text-[11px] text-slate-500">نام خدمت</p>
                            <p className="mt-1 truncate text-sm font-semibold text-slate-800">
                              {service.serviceName}
                            </p>
                          </div>
                          <CompactValue
                            label="تعداد"
                            value={new Intl.NumberFormat("fa-IR").format(service.quantity)}
                          />
                          <CompactValue label="تعرفه واحد" value={amountLabel(service.unitPrice)} />
                          <div>
                            <p className="text-[11px] text-slate-500">وضعیت</p>
                            <div className="mt-1">
                              <StatusBadge status={service.status} service />
                            </div>
                          </div>
                        </div>
                        <div className="grid gap-3 border-t border-slate-100 p-3 sm:grid-cols-3">
                          <CompactValue
                            label="پرسنل مرتبط"
                            value={
                              service.personnelId === null
                                ? "تعیین نشده"
                                : service.personnelName ??
                                  `شناسه ${new Intl.NumberFormat("fa-IR").format(service.personnelId)}`
                            }
                          />
                          <CompactValue label="تخفیف خدمت" value={amountLabel(service.discountAmount)} />
                          <CompactValue label="مبلغ نهایی" value={amountLabel(service.finalAmount)} emphasized />
                        </div>
                        {service.personnelId !== null &&
                          (service.status === 1 || service.status === 2) && (
                            <div className="flex flex-wrap gap-2 border-t border-slate-100 px-3 py-2.5">
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
                        {service.materials.length > 0 && (
                          <div className="border-t border-slate-100">
                            <div className="flex items-center gap-2 bg-fuchsia-50/50 px-3 py-2">
                              <FlaskConical className="size-4 text-fuchsia-700" />
                              <h4 className="text-xs font-semibold text-fuchsia-800">
                                مواد مصرفی این خدمت
                              </h4>
                            </div>
                            <div className="divide-y divide-slate-100">
                              {service.materials.map((material) => (
                                <div
                                  key={material.id}
                                  className="grid gap-2 px-3 py-2.5 text-xs sm:grid-cols-[minmax(0,1fr)_100px_140px]"
                                >
                                  <span className="font-medium text-slate-700">
                                    {material.materialName ??
                                      `ماده با شناسه ${new Intl.NumberFormat("fa-IR").format(material.materialId)}`}
                                  </span>
                                  <span className="text-slate-500">
                                    مقدار: {new Intl.NumberFormat("fa-IR").format(material.quantity)}
                                    {material.baseUnitSymbol ? ` ${material.baseUnitSymbol}` : ""}
                                  </span>
                                  <span className="text-slate-500">
                                    {material.unitCost === null
                                      ? "بهای FIFO ثبت نشده"
                                      : `بهای واحد FIFO: ${unitCostLabel(material.unitCost)}`}
                                  </span>
                                  {material.unitCost !== null && material.totalCost !== null && (
                                    <span className="font-semibold text-slate-700 sm:col-span-3">
                                      {material.movementReversed
                                        ? "مبلغ برگشت‌شده: "
                                        : "مبلغ ماده: "}
                                      {amountLabel(material.totalCost)}
                                    </span>
                                  )}
                                  {material.description && (
                                    <span className="text-slate-500 sm:col-span-3">
                                      توضیحات: {material.description}
                                    </span>
                                  )}
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </article>
                    ))}
                  </div>
                )}
              </section>

              <section className="overflow-hidden rounded-2xl border border-slate-200">
                <SectionBar icon={CreditCard} title="پرداخت‌ها" />
                {details.payments.length === 0 ? (
                  <EmptySection message="پرداختی برای این پذیرش ثبت نشده است." />
                ) : (
                  <div className="divide-y divide-slate-100">
                    {details.payments.map((payment) => (
                      <div key={payment.id} className="grid gap-2 px-4 py-3 sm:grid-cols-[minmax(0,1fr)_minmax(0,1fr)_auto] sm:items-center">
                        <div className="flex items-center gap-2">
                          <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-fuchsia-50 text-fuchsia-700">
                            <Wallet className="size-4" />
                          </span>
                          <div className="min-w-0">
                            <p className="text-xs font-semibold text-slate-800">{payment.paymentMethod}</p>
                            <p className="mt-0.5 text-[11px] text-slate-500">{dateTimeLabel(payment.paidAt)}</p>
                          </div>
                        </div>
                        <p className="truncate text-xs text-slate-500">
                          {payment.transactionReference
                            ? `پیگیری: ${payment.transactionReference}`
                            : payment.description || "بدون توضیحات"}
                        </p>
                        <p className="text-sm font-bold text-slate-800">{amountLabel(payment.amount)}</p>
                      </div>
                    ))}
                  </div>
                )}
              </section>
            </main>
          </div>
        ) : null}
        {details && !loading && !error && (
          <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-200 bg-white px-4 py-3 sm:px-6">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              className="h-9 rounded-xl border-fuchsia-200 px-4 text-xs text-fuchsia-800 hover:bg-fuchsia-50"
            >
            بستن
            </Button>
            <div className="flex flex-wrap items-center gap-x-6 gap-y-2">
              <div>
                <p className="text-[11px] text-slate-500">مبلغ کل</p>
                <p className="mt-0.5 text-sm font-bold text-slate-800">{amountLabel(details.totalAmount)}</p>
              </div>
              <div>
                <p className="text-[11px] text-slate-500">پرداخت‌شده</p>
                <p className="mt-0.5 text-sm font-bold text-emerald-700">{amountLabel(details.paidAmount)}</p>
              </div>
              <div>
                <p className="text-[11px] text-slate-500">مانده</p>
                <p className="mt-0.5 text-sm font-bold text-fuchsia-800">{amountLabel(details.remainingAmount)}</p>
              </div>
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}

function SectionHeading({
  icon: Icon,
  title,
}: {
  icon: typeof ReceiptText;
  title: string;
}) {
  return (
    <h3 className="flex items-center gap-2 text-sm font-bold text-fuchsia-800">
      <Icon className="size-4 text-fuchsia-600" />
      {title}
    </h3>
  );
}

function SectionBar({
  icon: Icon,
  title,
}: {
  icon: typeof ReceiptText;
  title: string;
}) {
  return (
    <div className="flex items-center gap-2 border-b border-fuchsia-100 bg-gradient-to-l from-fuchsia-50/80 to-white px-4 py-3">
      <Icon className="size-4 text-fuchsia-700" />
      <h3 className="text-sm font-bold text-fuchsia-800">{title}</h3>
    </div>
  );
}

function SummaryTile({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof ReceiptText;
  label: string;
  value: string;
}) {
  return (
    <div className="flex min-w-0 items-center gap-3 bg-white p-3 sm:p-4">
      <span className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-fuchsia-50 text-fuchsia-700">
        <Icon className="size-4" />
      </span>
      <div className="min-w-0">
        <p className="text-[11px] text-slate-500">{label}</p>
        <p className="mt-1 truncate text-xs font-semibold text-slate-800">{value}</p>
      </div>
    </div>
  );
}

function SidebarValue({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-start justify-between gap-3 py-3">
      <dt className="shrink-0 text-xs text-slate-500">{label}</dt>
      <dd className="text-left text-xs font-semibold text-slate-700">{value}</dd>
    </div>
  );
}

function CompactValue({
  label,
  value,
  emphasized = false,
}: {
  label: string;
  value: string;
  emphasized?: boolean;
}) {
  return (
    <div className="min-w-0">
      <p className="text-[11px] text-slate-500">{label}</p>
      <p className={`mt-1 break-words text-xs ${emphasized ? "font-bold text-fuchsia-800" : "font-medium text-slate-700"}`}>
        {value}
      </p>
    </div>
  );
}

function EmptySection({ message }: { message: string }) {
  return <p className="px-4 py-5 text-center text-xs text-slate-500">{message}</p>;
}

function StatusBadge({ status, service = false }: { status: number; service?: boolean }) {
  const label = (service ? SERVICE_STATUS : RECEPTION_STATUS)[status] ?? "نامشخص";
  const tone =
    status === 3
      ? "bg-emerald-50 text-emerald-700 ring-emerald-200"
      : status === 2
        ? "bg-fuchsia-50 text-fuchsia-700 ring-fuchsia-200"
        : status === 1
          ? "bg-amber-50 text-amber-700 ring-amber-200"
          : "bg-slate-100 text-slate-600 ring-slate-200";

  return (
    <span className={`inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-1 text-[11px] font-medium ring-1 ring-inset ${tone}`}>
      <span className="size-1.5 rounded-full bg-current" />
      {label}
    </span>
  );
}
