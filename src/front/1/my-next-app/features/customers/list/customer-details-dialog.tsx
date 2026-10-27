"use client";

import { useEffect, useRef, useState } from "react";
import {
  Banknote,
  Building2,
  HeartPlus,
  LoaderCircle,
  MapPin,
  Pencil,
  Phone,
  Save,
  Smartphone,
  Trash2,
  UserRound,
  X,
} from "lucide-react";
import { toGregorian, toJalaali } from "jalaali-js";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Dialog, DialogClose, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";
import { PersonnelService } from "@/features/personnel/list/personnel.service";
import { ReceptionService } from "@/features/receptions/list/reception.service";
import type { ReceptionDetails, ReceptionServiceItem } from "@/features/receptions/list/reception.types";

import { CustomerFormSchema, type CustomerFormValues } from "./customer.schema";
import { CustomerService } from "./customer.service";
import type { CustomerDetailsDto } from "./customer.types";

type CustomerTab = "personal" | "services" | "payments" | "discounts";

const TABS: {
  id: CustomerTab;
  label: string;
  icon: typeof UserRound;
}[] = [
  { id: "personal", label: "اطلاعات شخصی", icon: UserRound },
  { id: "services", label: "آخرین خدمات", icon: Building2 },
  { id: "payments", label: "تاریخچه پرداختی", icon: Banknote },
  { id: "discounts", label: "تاریخچه تخفیفات", icon: HeartPlus },
];

function emptyValue(value: string | null | undefined) {
  return value?.trim() || "—";
}

function formatBirthDate(value: string) {
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    dateStyle: "short",
    timeZone: "UTC",
  }).format(new Date(Date.UTC(year, month - 1, day)));
}

function formatDateTime(value: string) {
  return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
    dateStyle: "short",
    timeZone: "Asia/Tehran",
  }).format(new Date(value));
}

function money(value: number) {
  return `${new Intl.NumberFormat("fa-IR").format(value)} تومان`;
}

function maskNationalCode(value: string) {
  return value.length === 10 ? `${value.slice(0, 3)}*******` : value;
}

function normalizeDigits(value: string) {
  return value
    .replace(/[۰-۹]/g, (digit) => String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)))
    .replace(/[٠-٩]/g, (digit) => String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)));
}

function dateToApi(value: string) {
  const [year, month, day] = value.split("/").map(Number);
  const gregorian = toGregorian(year, month, day);
  return `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(gregorian.gd).padStart(2, "0")}`;
}

function dateToJalali(value: string) {
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  const jalali = toJalaali(year, month, day);
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`;
}

function DetailCard({
  title,
  icon: Icon,
  children,
  action,
  className = "",
}: {
  title: string;
  icon: typeof UserRound;
  children: React.ReactNode;
  action?: React.ReactNode;
  className?: string;
}) {
  return (
    <section className={`min-w-0 rounded-xl border border-slate-100 bg-white p-4 ${className}`}>
      <div className="mb-4 flex items-center justify-between gap-2">
        <div className="flex items-center gap-2 text-sm font-semibold text-slate-700">
          <Icon className="size-4 text-slate-500" />
          {title}
        </div>
        {action}
      </div>
      {children}
    </section>
  );
}

function DetailValue({
  label,
  value,
  direction = "auto",
}: {
  label: string;
  value: string;
  direction?: "ltr" | "rtl" | "auto";
}) {
  return (
    <div className="min-w-0">
      <dt className="text-xs text-slate-500">{label}</dt>
      <dd className="mt-1 whitespace-pre-wrap break-words text-sm text-slate-700" dir={direction}>
        {value}
      </dd>
    </div>
  );
}

function historyItems(receptions: ReceptionDetails[]) {
  return receptions.flatMap((reception) =>
    reception.services
      .filter((service) => service.status !== 4)
      .map((service) => ({ reception, service })),
  );
}

export function CustomerDetailsDialog({
  open,
  customerId,
  startEditing = false,
  onOpenChange,
  onSaved,
  onDelete,
}: {
  open: boolean;
  customerId: number | null;
  startEditing?: boolean;
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
  onDelete: (customer: CustomerDetailsDto) => void;
}) {
  const [customer, setCustomer] = useState<CustomerDetailsDto | null>(null);
  const [receptions, setReceptions] = useState<ReceptionDetails[]>([]);
  const [personnelNames, setPersonnelNames] = useState<Record<number, string>>({});
  const [loading, setLoading] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [historyError, setHistoryError] = useState<string | null>(null);
  const [personnelError, setPersonnelError] = useState<string | null>(null);
  const [tab, setTab] = useState<CustomerTab>("personal");
  const [editing, setEditing] = useState(false);
  const [values, setValues] = useState<CustomerFormValues | null>(null);
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);
  const tabRefs = useRef<Array<HTMLButtonElement | null>>([]);

  useEffect(() => {
    if (!open || customerId === null) return;
    const controller = new AbortController();
    Promise.resolve()
      .then(() => {
        setLoading(true);
        setError(null);
        setCustomer(null);
        setReceptions([]);
        setPersonnelNames({});
        setPersonnelError(null);
        setTab("personal");
        setEditing(startEditing);
        setSaveError(null);
        return new CustomerService().getById(customerId, { signal: controller.signal });
      })
      .then(async (response) => {
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات مشتری دریافت نشد.");
        }
        if (controller.signal.aborted) return;
        const detail = response.data;
        setCustomer(detail);
        setValues({
          fullName: detail.fullName,
          birthDate: dateToJalali(detail.birthDate),
          nationalCode: detail.nationalCode,
          address: detail.address,
          allergyDescription: detail.allergyDescription ?? "",
          jobTitle: detail.jobTitle ?? "",
          landlinePhone: detail.landlinePhone ?? "",
          mobilePhone: detail.mobilePhone,
        });
        setLoading(false);
        setHistoryLoading(true);

        try {
          const receptionService = new ReceptionService();
          const listResponse = await receptionService.getAll(
            { CustomerId: customerId, PageNumber: 1, PageSize: 10 },
            { signal: controller.signal },
          );
          if (!listResponse.data) {
            throw new Error(listResponse.message ?? "سوابق پذیرش مشتری دریافت نشد.");
          }
          const details = await Promise.all(
            listResponse.data.list.map(async (reception) => {
              const detailsResponse = await receptionService.getById(reception.id, {
                signal: controller.signal,
              });
              if (!detailsResponse.data) {
                throw new Error(detailsResponse.message ?? "جزئیات یکی از پذیرش‌ها دریافت نشد.");
              }
              return detailsResponse.data;
            }),
          );
          if (controller.signal.aborted) return;
          setReceptions(details);

          const personnelIds = [
            ...new Set(
              details.flatMap((reception) =>
                reception.services
                  .map((service) => service.personnelId)
                  .filter((id): id is number => id !== null),
              ),
            ),
          ];
          try {
            const personnelService = new PersonnelService();
            const names = await Promise.all(
              personnelIds.map(async (id) => {
                const personnelResponse = await personnelService.getById(id, {
                  signal: controller.signal,
                });
                if (!personnelResponse.data) {
                  throw new Error(personnelResponse.message ?? "اطلاعات پرسنل دریافت نشد.");
                }
                return [id, personnelResponse.data.personalInfo.fullName] as const;
              }),
            );
            if (!controller.signal.aborted) setPersonnelNames(Object.fromEntries(names));
          } catch (requestError: unknown) {
            if (!controller.signal.aborted) setPersonnelError(handleApiError(requestError));
          }
        } catch (requestError: unknown) {
          if (!controller.signal.aborted) setHistoryError(handleApiError(requestError));
        } finally {
          if (!controller.signal.aborted) setHistoryLoading(false);
        }
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setError(handleApiError(requestError));
          setLoading(false);
        }
      });

    return () => controller.abort();
  }, [customerId, open, startEditing]);

  function changeTab(nextTab: CustomerTab) {
    setTab(nextTab);
  }

  function handleTabKeyDown(event: React.KeyboardEvent<HTMLButtonElement>, currentIndex: number) {
    const direction = event.key === "ArrowRight" ? -1 : event.key === "ArrowLeft" ? 1 : 0;
    if (direction === 0) return;
    event.preventDefault();
    const nextIndex = (currentIndex + direction + TABS.length) % TABS.length;
    changeTab(TABS[nextIndex].id);
    tabRefs.current[nextIndex]?.focus();
  }

  async function saveCustomer(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!customer || !values || saving) return;
    setSaveError(null);
    const parsed = CustomerFormSchema.safeParse({
      ...values,
      birthDate: normalizeDigits(values.birthDate),
      nationalCode: normalizeDigits(values.nationalCode),
    });
    if (!parsed.success) {
      setSaveError(parsed.error.issues[0]?.message ?? "اطلاعات فرم را بررسی کنید.");
      return;
    }

    setSaving(true);
    try {
      const request = {
        id: customer.id,
        fullName: parsed.data.fullName,
        birthDate: dateToApi(parsed.data.birthDate),
        nationalCode: parsed.data.nationalCode,
        address: parsed.data.address,
        allergyDescription: parsed.data.allergyDescription.trim() || null,
        jobTitle: parsed.data.jobTitle.trim() || null,
        landlinePhone: parsed.data.landlinePhone.trim() || null,
        mobilePhone: parsed.data.mobilePhone.trim(),
      };
      await new CustomerService().update(request);
      setEditing(false);
      onSaved();
      setCustomer({
        ...customer,
        fullName: request.fullName,
        birthDate: request.birthDate,
        nationalCode: request.nationalCode,
        address: request.address,
        allergyDescription: request.allergyDescription,
        jobTitle: request.jobTitle,
        landlinePhone: request.landlinePhone,
        mobilePhone: request.mobilePhone,
      });
      setValues(parsed.data);
    } catch (requestError: unknown) {
      setSaveError(handleApiError(requestError));
    } finally {
      setSaving(false);
    }
  }

  function requestEdit() {
    if (!customer) return;
    setTab("personal");
    setSaveError(null);
    setEditing(true);
  }

  function changeValue(key: keyof CustomerFormValues, value: string) {
    setValues((current) => current ? { ...current, [key]: value } : current);
  }

  const allServices = historyItems(receptions);
  const payments = receptions.flatMap((reception) =>
    reception.payments
      .filter((payment) => payment.status === 1)
      .map((payment) => ({ reception, payment })),
  );
  const discounts = receptions.flatMap((reception) =>
    reception.services
      .filter((service) => service.status !== 4 && service.discountAmount > 0)
      .map((service) => ({ reception, service })),
  );

  return (
    <Dialog open={open} onOpenChange={(nextOpen) => !saving && onOpenChange(nextOpen)}>
      <DialogContent
        dir="rtl"
        showCloseButton={false}
        className="flex h-[min(820px,92vh)] w-[calc(100vw-1rem)] max-w-[1400px] flex-col gap-0 overflow-hidden rounded-2xl border border-fuchsia-200 bg-white p-0 text-slate-900 shadow-xl sm:w-[calc(100vw-2rem)] sm:max-w-[1400px]"
      >
        <header className="relative flex h-12 shrink-0 items-center justify-between border-b border-slate-100 px-6">
          <DialogTitle className="text-sm font-bold text-fuchsia-950">
            {editing ? "ویرایش جزئیات مشتری" : "جزئیات مشتری"}
          </DialogTitle>
          <DialogClose
            render={<Button variant="ghost" size="icon" aria-label="بستن جزئیات مشتری" className="absolute left-3 top-1/2 size-8 -translate-y-1/2 text-slate-500 hover:bg-slate-100" />}
          >
            <X className="size-4" />
          </DialogClose>
        </header>

        <section className="flex min-h-[68px] shrink-0 items-center justify-between gap-4 px-6 py-2">
          {loading ? (
            <div className="flex w-full items-center justify-center gap-2 text-sm text-slate-500">
              <LoaderCircle className="size-4 animate-spin" />در حال دریافت اطلاعات مشتری...
            </div>
          ) : error ? (
            <p role="alert" className="text-sm text-destructive">{error}</p>
          ) : customer ? (
            <>
              <div className="flex min-w-0 items-center gap-3">
                <div className="flex size-14 shrink-0 items-center justify-center rounded-full bg-sky-100 text-sky-700">
                  <UserRound className="size-8" />
                </div>
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold text-slate-800">{customer.fullName}</p>
                  <p className="mt-1 text-xs text-fuchsia-700">{emptyValue(customer.jobTitle)}</p>
                </div>
                <Badge className={customer.status ? "bg-emerald-50 text-emerald-700 hover:bg-emerald-50" : "bg-slate-100 text-slate-600 hover:bg-slate-100"}>
                  {customer.status ? "فعال" : "غیرفعال"}
                </Badge>
              </div>
              <div className="flex shrink-0 items-center gap-3 border-e border-slate-200 pe-5 text-xs">
                <Smartphone className="size-4 text-fuchsia-700" />
                <div>
                  <p className="text-fuchsia-700">شماره همراه</p>
                  <p className="mt-1 font-medium text-slate-700" dir="ltr">{customer.mobilePhone}</p>
                </div>
              </div>
            </>
          ) : null}
        </section>

        <div
          role="tablist"
          aria-label="بخش‌های جزئیات مشتری"
          className="grid shrink-0 grid-cols-2 border-y border-slate-200 bg-fuchsia-50/70 sm:grid-cols-4"
        >
          {TABS.map((item, index) => {
            const Icon = item.icon;
            const active = tab === item.id;
            return (
              <button
                key={item.id}
                ref={(element) => { tabRefs.current[index] = element; }}
                id={`customer-tab-${item.id}`}
                type="button"
                role="tab"
                aria-selected={active}
                aria-controls="customer-tab-panel"
                tabIndex={active ? 0 : -1}
                onClick={() => changeTab(item.id)}
                onKeyDown={(event) => handleTabKeyDown(event, index)}
                className={`flex h-10 items-center justify-center gap-1.5 border-b-2 px-2 text-xs font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-fuchsia-500 sm:text-sm ${
                  active
                    ? "border-fuchsia-600 bg-fuchsia-100 text-fuchsia-700"
                    : "border-transparent text-slate-500 hover:bg-fuchsia-100/70 hover:text-fuchsia-700"
                }`}
              >
                <Icon className="size-4 shrink-0" />
                {item.label}
              </button>
            );
          })}
        </div>

        <div
          id="customer-tab-panel"
          role="tabpanel"
          aria-labelledby={`customer-tab-${tab}`}
          className="min-h-0 flex-1 overflow-y-auto p-4 sm:p-6"
        >
          {loading ? null : error ? null : customer ? (
            <>
              {tab === "personal" && values && (
                <form id="customer-detail-form" onSubmit={(event) => void saveCustomer(event)}>
                  <div className="grid gap-4 sm:grid-cols-2">
                    <DetailCard title="اطلاعات فردی" icon={UserRound}>
                      {editing ? (
                        <div className="grid grid-cols-2 gap-3">
                          <EditField label="نام و نام خانوادگی" value={values.fullName} onChange={(value) => changeValue("fullName", value)} />
                          <EditField label="کد ملی" value={values.nationalCode} direction="ltr" onChange={(value) => changeValue("nationalCode", value)} />
                          <div className="space-y-1.5">
                            <Label htmlFor="detail-customer-birth-date" className="text-xs">تاریخ تولد</Label>
                            <Input id="detail-customer-birth-date" type="text" dir="ltr" placeholder="۱۳۷۰/۰۱/۰۱" value={values.birthDate} onChange={(event) => changeValue("birthDate", event.target.value)} />
                          </div>
                          <EditField label="عنوان شغلی" value={values.jobTitle} onChange={(value) => changeValue("jobTitle", value)} />
                        </div>
                      ) : (
                        <dl className="grid grid-cols-2 gap-x-4 gap-y-5">
                          <DetailValue label="نام و نام خانوادگی" value={customer.fullName} />
                          <DetailValue label="عنوان شغلی" value={emptyValue(customer.jobTitle)} />
                          <DetailValue label="کد ملی" value={maskNationalCode(customer.nationalCode)} direction="ltr" />
                          <DetailValue label="تاریخ تولد" value={formatBirthDate(customer.birthDate)} direction="ltr" />
                        </dl>
                      )}
                    </DetailCard>

                    <DetailCard title="شماره تماس" icon={Phone}>
                      {editing ? (
                        <div className="grid gap-3">
                          <EditField label="تلفن همراه" value={values.mobilePhone} direction="ltr" onChange={(value) => changeValue("mobilePhone", value)} />
                          <EditField label="تلفن ثابت" value={values.landlinePhone} direction="ltr" onChange={(value) => changeValue("landlinePhone", value)} />
                        </div>
                      ) : (
                        <dl className="grid gap-5">
                          <DetailValue label="تلفن همراه" value={customer.mobilePhone} direction="ltr" />
                          <DetailValue label="تلفن ثابت" value={emptyValue(customer.landlinePhone)} direction="ltr" />
                        </dl>
                      )}
                    </DetailCard>

                    <DetailCard
                      title="اطلاعات سلامتی"
                      icon={MapPin}
                      action={customer.allergyDescription ? <Badge className="bg-fuchsia-100 text-fuchsia-700 hover:bg-fuchsia-100">حساسیت دارد</Badge> : undefined}
                    >
                      {editing ? (
                        <EditField label="شرح حساسیت" value={values.allergyDescription} onChange={(value) => changeValue("allergyDescription", value)} />
                      ) : (
                        <dl>
                          <DetailValue label="شرح حساسیت" value={emptyValue(customer.allergyDescription)} />
                        </dl>
                      )}
                    </DetailCard>

                    <DetailCard title="آدرس" icon={MapPin}>
                      {editing ? (
                        <div className="space-y-1.5">
                          <Label htmlFor="detail-customer-address" className="text-xs">آدرس</Label>
                          <textarea
                            id="detail-customer-address"
                            value={values.address}
                            rows={3}
                            maxLength={1000}
                            onChange={(event) => changeValue("address", event.target.value)}
                            className="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                          />
                        </div>
                      ) : (
                        <dl><DetailValue label="آدرس" value={customer.address} /></dl>
                      )}
                    </DetailCard>
                  </div>
                  {saveError && <p role="alert" className="mt-3 text-sm text-destructive">{saveError}</p>}
                </form>
              )}

              {tab !== "personal" && (
                <HistoryTable
                  tab={tab}
                  loading={historyLoading}
                  error={historyError}
                  receptions={receptions}
                  services={allServices}
                  personnelNames={personnelNames}
                  personnelError={personnelError}
                  payments={payments}
                  discounts={discounts}
                />
              )}
            </>
          ) : null}
        </div>

        <footer className="flex shrink-0 flex-row-reverse items-center justify-between gap-3 border-t border-slate-100 px-6 py-3">
          <div className="flex gap-2">
            {editing ? (
              <>
                <Button type="button" variant="outline" disabled={saving} onClick={() => { setEditing(false); setSaveError(null); }}>
                  انصراف
                </Button>
                <Button type="submit" form="customer-detail-form" disabled={saving || loading || tab !== "personal"} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                  {saving ? <LoaderCircle className="size-4 animate-spin" /> : <Save className="size-4" />}
                  {saving ? "در حال ذخیره..." : "ذخیره اطلاعات"}
                </Button>
              </>
            ) : (
              <>
                <Button
                  type="button"
                  variant="outline"
                  disabled={!customer || loading}
                  onClick={() => customer && onDelete(customer)}
                  className="border-rose-300 text-rose-600 hover:bg-rose-50 hover:text-rose-700"
                >
                  <Trash2 className="size-4" />
                  حذف مشتری
                </Button>
                <Button type="button" disabled={!customer || loading} onClick={requestEdit} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                  <Pencil className="size-4" />
                  ویرایش اطلاعات
                </Button>
              </>
            )}
          </div>
          <div className="hidden text-xs text-slate-500 sm:block">
            {tab !== "personal" && "سوابق از پذیرش‌های ثبت‌شدهٔ مشتری"}
          </div>
        </footer>
      </DialogContent>
    </Dialog>
  );
}

function EditField({
  label,
  value,
  onChange,
  direction,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  direction?: "ltr";
}) {
  const id = `edit-${label.replace(/\s+/g, "-")}`;
  return (
    <div className="space-y-1.5">
      <Label htmlFor={id} className="text-xs">{label}</Label>
      <Input id={id} value={value} dir={direction} onChange={(event) => onChange(event.target.value)} />
    </div>
  );
}

function HistoryTable({
  tab,
  loading,
  error,
  receptions,
  services,
  personnelNames,
  personnelError,
  payments,
  discounts,
}: {
  tab: Exclude<CustomerTab, "personal">;
  loading: boolean;
  error: string | null;
  receptions: ReceptionDetails[];
  services: { reception: ReceptionDetails; service: ReceptionServiceItem }[];
  personnelNames: Record<number, string>;
  personnelError: string | null;
  payments: { reception: ReceptionDetails; payment: ReceptionDetails["payments"][number] }[];
  discounts: { reception: ReceptionDetails; service: ReceptionServiceItem }[];
}) {
  if (loading) {
    return (
      <div className="flex min-h-40 items-center justify-center gap-2 text-sm text-slate-500">
        <LoaderCircle className="size-4 animate-spin" />در حال دریافت سوابق...
      </div>
    );
  }
  if (error) {
    return <p role="alert" className="rounded-lg border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">{error}</p>;
  }

  const hasItems =
    tab === "services" ? services.length > 0 :
      tab === "payments" ? payments.length > 0 :
        discounts.length > 0;

  return (
    <section className="rounded-xl border border-slate-100 p-4">
      <h2 className="mb-4 text-sm font-semibold text-slate-700">
        {tab === "services" ? "آخرین خدمات دریافت‌شده" :
          tab === "payments" ? "تاریخچه پرداخت‌های مشتری" :
            "تاریخچه تخفیفات مشتری"}
      </h2>
      {hasItems ? (
        <div className="overflow-x-auto">
          <table className="w-full min-w-[660px] border-collapse text-right text-xs sm:text-sm">
            {tab === "services" && (
              <>
                <thead className="bg-slate-100 text-slate-700">
                  <tr>
                    <th className="rounded-r-lg px-3 py-3 font-semibold">خدمت دریافت‌شده</th>
                    <th className="px-3 py-3 font-semibold">پرسنل مربوطه</th>
                    <th className="px-3 py-3 font-semibold">شماره پذیرش</th>
                    <th className="rounded-l-lg px-3 py-3 font-semibold">تاریخ آخرین مراجعه</th>
                  </tr>
                </thead>
                <tbody>
                  {services.map(({ reception, service }) => (
                    <tr key={`${reception.id}-${service.id}`} className="border-b border-slate-100 last:border-0">
                      <td className="px-3 py-3">{service.serviceName}</td>
                      <td className="px-3 py-3">{service.personnelId ? personnelNames[service.personnelId] ?? "—" : "—"}</td>
                      <td className="px-3 py-3" dir="ltr">{reception.receptionNumber}</td>
                      <td className="px-3 py-3" dir="ltr">{formatDateTime(reception.receptionDate)}</td>
                    </tr>
                  ))}
                </tbody>
              </>
            )}
            {tab === "payments" && (
              <>
                <thead className="bg-slate-100 text-slate-700">
                  <tr>
                    <th className="rounded-r-lg px-3 py-3 font-semibold">تاریخ پرداخت</th>
                    <th className="px-3 py-3 font-semibold">شماره پذیرش</th>
                    <th className="px-3 py-3 font-semibold">مبلغ کل</th>
                    <th className="px-3 py-3 font-semibold">مبلغ تخفیف</th>
                    <th className="px-3 py-3 font-semibold">مبلغ پرداختی</th>
                    <th className="rounded-l-lg px-3 py-3 font-semibold">نوع پرداخت</th>
                  </tr>
                </thead>
                <tbody>
                  {payments.map(({ reception, payment }) => (
                    <tr key={`${reception.id}-${payment.id}`} className="border-b border-slate-100 last:border-0">
                      <td className="px-3 py-3" dir="ltr">{formatDateTime(payment.paidAt)}</td>
                      <td className="px-3 py-3" dir="ltr">{reception.receptionNumber}</td>
                      <td className="px-3 py-3">{money(reception.totalAmount)}</td>
                      <td className="px-3 py-3">{reception.totalDiscountAmount ? money(reception.totalDiscountAmount) : "—"}</td>
                      <td className="px-3 py-3 font-medium">{money(payment.amount)}</td>
                      <td className="px-3 py-3">{payment.paymentMethod}</td>
                    </tr>
                  ))}
                </tbody>
              </>
            )}
            {tab === "discounts" && (
              <>
                <thead className="bg-slate-100 text-slate-700">
                  <tr>
                    <th className="rounded-r-lg px-3 py-3 font-semibold">عنوان تخفیف</th>
                    <th className="px-3 py-3 font-semibold">مبلغ/درصد تخفیف</th>
                    <th className="rounded-l-lg px-3 py-3 font-semibold">تاریخ تخفیف</th>
                  </tr>
                </thead>
                <tbody>
                  {discounts.map(({ reception, service }) => {
                    const grossAmount = service.quantity * service.unitPrice;
                    const percent = grossAmount > 0
                      ? Math.round((service.discountAmount / grossAmount) * 100)
                      : 0;
                    return (
                      <tr key={`${reception.id}-${service.id}`} className="border-b border-slate-100 last:border-0">
                        <td className="px-3 py-3">{service.serviceName}</td>
                        <td className="px-3 py-3">
                          {money(service.discountAmount)}
                          {percent > 0 && <span className="ms-2 text-slate-500">({new Intl.NumberFormat("fa-IR").format(percent)}٪)</span>}
                        </td>
                        <td className="px-3 py-3" dir="ltr">{formatDateTime(reception.receptionDate)}</td>
                      </tr>
                    );
                  })}
                </tbody>
              </>
            )}
          </table>
        </div>
      ) : (
        <p className="py-10 text-center text-sm text-slate-500">
          {receptions.length === 0
            ? "برای این مشتری سابقهٔ پذیرش ثبت نشده است."
            : tab === "discounts"
              ? "برای خدمات ثبت‌شده تخفیفی ثبت نشده است."
              : "برای این مشتری سابقه‌ای در این بخش ثبت نشده است."}
        </p>
      )}
      {receptions.length > 0 && (
        <p className="mt-3 text-xs text-slate-400">اطلاعات مربوط به ۱۰ پذیرش اخیر مشتری نمایش داده شده است.</p>
      )}
      {tab === "services" && personnelError && (
        <p role="status" className="mt-2 text-xs text-amber-700">
          نام برخی پرسنل دریافت نشد: {personnelError}
        </p>
      )}
    </section>
  );
}
