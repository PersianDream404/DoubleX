"use client";

import { useEffect, useMemo, useState } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { CalendarDays, Clock3, LoaderCircle, Search, UserRound, X } from "lucide-react";
import { toJalaali } from "jalaali-js";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";
import { jalaliDateToApi } from "@/features/services/tariffs/service-tariff-date";
import type { CustomerDto } from "@/features/customers/list/customer.types";
import type { PersonnelDto, } from "@/features/personnel/list/personnel.types";
import type { ServiceDto, ServicePersonnelDto } from "@/features/services/list/service.types";
import type { ServiceTariffDto } from "@/features/services/tariffs/service-tariff.types";

import {
  CreateAppointmentFormSchema,
  normalizeDigits,
  UpdateAppointmentFormSchema,
} from "./appointment.schema";
import { AppointmentService } from "./appointment.service";
import type {
  AppointmentAvailabilityDto,
  AppointmentDetailsDto,
  AppointmentNotificationChannel,
} from "./appointment.types";

type Props = {
  open: boolean;
  appointmentId: number | null;
  onOpenChange: (open: boolean) => void;
  onSaved: (message: string, appointmentDate: string) => void;
};

type Options = {
  services: ServiceDto[];
  tariffs: ServiceTariffDto[];
  personnel: PersonnelDto[];
};

const EMPTY_OPTIONS: Options = { services: [], tariffs: [], personnel: [] };
const inputClass = "h-11 rounded-xl border-slate-200 bg-white";

function todayJalali() {
  const date = new Date();
  const jalali = toJalaali(date.getFullYear(), date.getMonth() + 1, date.getDate());
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`;
}

function apiStartsAt(date: string, time: string) {
  const midnight = jalaliDateToApi(normalizeDigits(date));
  return midnight?.replace("T00:00:00", `T${normalizeDigits(time)}:00`) ?? null;
}

function currentTehranDateTime(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return { date: "", time: "" };
  const parts = new Intl.DateTimeFormat("en-US-u-ca-gregory", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(date);
  const part = (type: Intl.DateTimeFormatPartTypes) =>
    parts.find((item) => item.type === type)?.value ?? "";
  const jalali = toJalaali(Number(part("year")), Number(part("month")), Number(part("day")));
  const time = new Intl.DateTimeFormat("en-GB", {
    timeZone: "Asia/Tehran",
    hour: "2-digit",
    minute: "2-digit",
    hourCycle: "h23",
  }).format(date);
  return {
    date: `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(jalali.jd).padStart(2, "0")}`,
    time,
  };
}

function amountLabel(amount: number) {
  return `${new Intl.NumberFormat("fa-IR").format(amount)} تومان`;
}

function formatPhone(value: string) {
  const digits = normalizeDigits(value).replace(/\D/g, "");
  const formatted = /^09\d{9}$/.test(digits)
    ? digits.replace(/^(\d{4})(\d{3})(\d{4})$/, "$1 $2 $3")
    : value;
  return formatted.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]);
}

function validTariffsForDate(
  tariffs: ServiceTariffDto[],
  serviceId: string,
  date: string,
  time: string,
) {
  const dayStart = date ? apiStartsAt(date, time || "00:00") : null;
  const dayEnd = date && !time ? apiStartsAt(date, "23:59") : dayStart;
  if (!serviceId || !dayStart || !dayEnd) return [];
  const startBoundary = new Date(dayStart).getTime();
  const endBoundary = new Date(dayEnd).getTime();
  return tariffs
    .filter((tariff) => {
      const validFrom = new Date(tariff.validFrom).getTime();
      const validTo = tariff.validTo ? new Date(tariff.validTo).getTime() : Number.POSITIVE_INFINITY;
      return tariff.services.some((item) => item.serviceId === Number(serviceId)) &&
        validFrom <= endBoundary && validTo >= startBoundary;
    })
    .sort((left, right) => new Date(right.validFrom).getTime() - new Date(left.validFrom).getTime());
}

function AppointmentDatePicker({
  value,
  onChange,
  disabled,
}: {
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
}) {
  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
      format="YYYY/MM/DD"
      value={value || null}
      minDate={todayJalali()}
      editable={false}
      disabled={disabled}
      calendarPosition="bottom-start"
      portal
      zIndex={160}
      onChange={(date: DateObject | null) => onChange(date?.format("YYYY/MM/DD") ?? "")}
      render={(_displayValue, openCalendar) => (
        <div className="relative">
          <Input
            type="text"
            value={value}
            placeholder="انتخاب تاریخ"
            readOnly
            disabled={disabled}
            onClick={openCalendar}
            onKeyDown={(event) => {
              if (!disabled && (event.key === "Enter" || event.key === " ")) openCalendar();
            }}
            className={`${inputClass} cursor-pointer pr-10`}
            aria-label="تاریخ نوبت"
          />
          <CalendarDays className="pointer-events-none absolute top-3 right-3 size-4 text-slate-400" />
        </div>
      )}
    />
  );
}

function FormField({
  label,
  htmlFor,
  children,
  error,
  hint,
}: {
  label: string;
  htmlFor?: string;
  children: React.ReactNode;
  error?: string;
  hint?: string;
}) {
  return (
    <div className="min-w-0 space-y-2">
      <Label htmlFor={htmlFor} className="text-sm font-medium text-slate-700">{label}</Label>
      {children}
      {error && <p className="text-xs leading-5 text-rose-600" role="alert">{error}</p>}
      {hint && !error && <p className="text-xs leading-5 text-slate-500">{hint}</p>}
    </div>
  );
}

export function AppointmentFormDialog({ open, appointmentId, onOpenChange, onSaved }: Props) {
  const [options, setOptions] = useState<Options>(EMPTY_OPTIONS);
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [customerQuery, setCustomerQuery] = useState("");
  const [customerResults, setCustomerResults] = useState<CustomerDto[]>([]);
  const [searchingCustomers, setSearchingCustomers] = useState(false);
  const [customer, setCustomer] = useState<CustomerDto | null>(null);
  const [details, setDetails] = useState<AppointmentDetailsDto | null>(null);
  const [servicePersonnel, setServicePersonnel] = useState<ServicePersonnelDto[]>([]);
  const [servicePersonnelLoading, setServicePersonnelLoading] = useState(appointmentId !== null);
  const [serviceId, setServiceId] = useState("");
  const [tariffId, setTariffId] = useState("");
  const [personnelId, setPersonnelId] = useState("");
  const [date, setDate] = useState("");
  const [time, setTime] = useState("");
  const [discount, setDiscount] = useState("0");
  const [notes, setNotes] = useState("");
  const [notificationChannel, setNotificationChannel] = useState<AppointmentNotificationChannel>(0);
  const [availability, setAvailability] = useState<AppointmentAvailabilityDto | null>(null);
  const [checkingAvailability, setCheckingAvailability] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const selectedPersonnel = options.personnel.filter((person) =>
    servicePersonnel.some((assignment) => assignment.personnelId === person.id),
  );
  const activeTariffs = useMemo(() => {
    return validTariffsForDate(options.tariffs, serviceId, date, time);
  }, [date, options.tariffs, serviceId, time]);

  useEffect(() => {
    if (!open) return;
    const controller = new AbortController();
    const serviceApi = new AppointmentService();

    const loads: [
      Promise<ServiceDto[]>,
      Promise<ServiceTariffDto[]>,
      Promise<PersonnelDto[]>,
      Promise<AppointmentDetailsDto | null>,
    ] = [
      serviceApi.getActiveServices(controller.signal),
      serviceApi.getTariffs(controller.signal),
      serviceApi.getAllPersonnel(controller.signal),
      appointmentId
        ? serviceApi.getById(appointmentId, controller.signal).then((response) => {
            if (!response.data) throw new Error(response.message ?? "جزئیات نوبت دریافت نشد.");
            return response.data;
          })
        : Promise.resolve(null),
    ];

    void Promise.all(loads)
      .then(([services, tariffs, personnel, appointment]) => {
        if (controller.signal.aborted) return;
        setOptions({ services, tariffs, personnel });
        if (appointment) {
          setDetails(appointment);
          setServiceId(String(appointment.serviceId));
          setPersonnelId(String(appointment.personnelId));
          const current = currentTehranDateTime(appointment.startsAt);
          setDate(current.date);
          setTime(current.time);
          setNotes(appointment.notes ?? "");
        } else {
          setDate(todayJalali());
          setTime("");
          setServiceId("");
          setPersonnelId("");
          setTariffId("");
          setDiscount("0");
          setNotes("");
          setNotificationChannel(0);
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) setFormError(handleApiError(cause));
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoadingOptions(false);
      });

    return () => controller.abort();
  }, [appointmentId, open]);

  useEffect(() => {
    if (!open || !serviceId) return;
    const controller = new AbortController();
    void new AppointmentService()
      .getServicePersonnel(Number(serviceId), controller.signal)
      .then((assignments) => {
        if (!controller.signal.aborted) setServicePersonnel(assignments);
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) setFormError(handleApiError(cause));
      })
      .finally(() => {
        if (!controller.signal.aborted) setServicePersonnelLoading(false);
      });
    return () => controller.abort();
  }, [appointmentId, open, serviceId]);

  useEffect(() => {
    if (!open || customerQuery.trim().length < 2) return;
    const controller = new AbortController();
    const timer = window.setTimeout(() => {
      setSearchingCustomers(true);
      void new AppointmentService()
        .searchCustomers(customerQuery.trim(), controller.signal)
        .then((customers) => {
          if (!controller.signal.aborted) setCustomerResults(customers);
        })
        .catch((cause: unknown) => {
          if (!controller.signal.aborted) setFormError(handleApiError(cause));
        })
        .finally(() => {
          if (!controller.signal.aborted) setSearchingCustomers(false);
        });
    }, 300);
    return () => {
      window.clearTimeout(timer);
      controller.abort();
    };
  }, [customerQuery, open]);

  useEffect(() => {
    if (!open || appointmentId || !serviceId || !tariffId || !personnelId || !date || !time) {
      return;
    }
    const startsAt = apiStartsAt(date, time);
    if (!startsAt) {
      return;
    }
    const controller = new AbortController();
    const timeout = window.setTimeout(() => {
      setCheckingAvailability(true);
      void new AppointmentService()
        .getAvailability({
          serviceId: Number(serviceId),
          serviceTariffId: Number(tariffId),
          personnelId: Number(personnelId),
          startsAt,
        }, controller.signal)
        .then((response) => {
          if (!response.data) throw new Error(response.message ?? "دسترس‌پذیری زمان بررسی نشد.");
          if (!controller.signal.aborted) {
            setAvailability(response.data);
            if (!response.data.isAvailable) {
              setFormError("این زمان قابل رزرو نیست. ساعت دیگری انتخاب کنید.");
            } else {
              setFormError(null);
            }
          }
        })
        .catch((cause: unknown) => {
          if (!controller.signal.aborted) {
            setAvailability(null);
            setFormError(handleApiError(cause));
          }
        })
        .finally(() => {
          if (!controller.signal.aborted) setCheckingAvailability(false);
        });
    }, 250);
    return () => {
      window.clearTimeout(timeout);
      controller.abort();
      setCheckingAvailability(false);
    };
  }, [appointmentId, date, open, personnelId, serviceId, tariffId, time]);

  async function selectCustomer(result: CustomerDto) {
    setFormError(null);
    setCustomerResults([]);
    setCustomerQuery("");
    try {
      const response = await new AppointmentService().getCustomerById(result.id);
      if (!response.data) throw new Error(response.message ?? "اطلاعات مشتری دریافت نشد.");
      if (!response.data.status) throw new Error("این مشتری غیرفعال است و امکان رزرو برای او وجود ندارد.");
      setCustomer(response.data);
    } catch (cause: unknown) {
      setFormError(handleApiError(cause));
    }
  }

  function chooseService(value: string) {
    setServiceId(value);
    setPersonnelId("");
    setServicePersonnel([]);
    setServicePersonnelLoading(Boolean(value));
    setTariffId("");
    setAvailability(null);
    setFormError(null);
  }

  function chooseTariff(value: string) {
    setTariffId(value);
    setAvailability(null);
    setFormError(null);
  }

  function choosePersonnel(value: string) {
    setPersonnelId(value);
    setAvailability(null);
    setFormError(null);
  }

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);
    setFieldErrors({});

    const startsAt = apiStartsAt(date, time);
    const normalizedDiscount = Number(normalizeDigits(discount || "0"));
    const formData = appointmentId
      ? UpdateAppointmentFormSchema.safeParse({
          personnelId: Number(personnelId),
          date,
          time,
          notes,
        })
      : CreateAppointmentFormSchema.safeParse({
          customerId: customer?.id ?? 0,
          serviceId: Number(serviceId),
          serviceTariffId: Number(tariffId),
          personnelId: Number(personnelId),
          date,
          time,
          discountAmount: normalizedDiscount,
          notes,
          notificationChannel,
        });

    if (!formData.success) {
      const errors = Object.fromEntries(
        formData.error.issues.map((issue) => [String(issue.path[0]), issue.message]),
      );
      setFieldErrors(errors);
      return;
    }
    if (!startsAt) {
      setFieldErrors({ date: "تاریخ و ساعت معتبر نیست." });
      return;
    }
    if (!appointmentId && !activeTariffs.some((item) => String(item.id) === tariffId)) {
      setFieldErrors({ serviceTariffId: "برای تاریخ انتخاب‌شده تعرفهٔ معتبر وجود ندارد." });
      return;
    }
    if (!appointmentId && checkingAvailability) {
      setFormError("لطفاً تا پایان بررسی زمان منتظر بمانید.");
      return;
    }
    if (!appointmentId && (!availability || !availability.isAvailable)) {
      setFormError("ابتدا یک زمان قابل رزرو انتخاب کنید.");
      return;
    }
    if (!appointmentId && normalizedDiscount > (availability?.baseAmount ?? 0)) {
      setFieldErrors({ discountAmount: "تخفیف نمی‌تواند از مبلغ پایهٔ خدمت بیشتر باشد." });
      return;
    }

    setSubmitting(true);
    try {
      const serviceApi = new AppointmentService();
      if (appointmentId) {
        const response = await serviceApi.update(appointmentId, {
          startsAt,
          personnelId: Number(personnelId),
          notes: notes.trim() || null,
        });
        if (!response.data) throw new Error(response.message ?? "به‌روزرسانی نوبت تأیید نشد.");
        onSaved("نوبت با موفقیت به‌روزرسانی شد.", date);
      } else {
        if (!customer) throw new Error("مشتری را انتخاب کنید.");
        const response = await serviceApi.create({
          idempotencyKey: crypto.randomUUID(),
          customerId: customer.id,
          serviceId: Number(serviceId),
          serviceTariffId: Number(tariffId),
          personnelId: Number(personnelId),
          startsAt,
          discountAmount: normalizedDiscount,
          notes: notes.trim() || null,
          notificationChannel,
        });
        if (!response.data?.id) throw new Error(response.message ?? "ثبت نوبت از سرور تأیید نشد.");
        onSaved(
          `نوبت با شناسهٔ ${new Intl.NumberFormat("fa-IR").format(response.data.id)} ثبت شد.`,
          date,
        );
      }
      onOpenChange(false);
    } catch (cause: unknown) {
      setFormError(handleApiError(cause));
    } finally {
      setSubmitting(false);
    }
  }

  const selectedTariff = activeTariffs.find((item) => String(item.id) === tariffId) ?? null;
  const tariffAmount = selectedTariff?.services.find((item) => item.serviceId === Number(serviceId))?.amount;
  const isEditing = appointmentId !== null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        dir="rtl"
        showCloseButton={false}
        className="max-h-[calc(100dvh-2rem)] overflow-y-auto rounded-3xl p-4 sm:p-6 lg:p-8"
        aria-describedby="appointment-form-description"
      >
        <div className="flex items-start justify-between gap-4 border-b border-slate-100 pb-5">
          <div className="space-y-2">
            <DialogTitle className="text-xl font-bold text-slate-900">
              {isEditing ? "ویرایش نوبت" : "رزرو نوبت جدید"}
            </DialogTitle>
            <DialogDescription id="appointment-form-description" className="text-sm text-slate-500">
              {isEditing
                ? "زمان، پرسنل و توضیحات نوبت را به‌روزرسانی کنید."
                : "اطلاعات مشتری، خدمت و زمان پیشنهادی را وارد کنید."}
            </DialogDescription>
          </div>
          <div className="flex items-center gap-2">
            {isEditing && details && (
              <span className="rounded-full bg-slate-100 px-3 py-1 text-xs text-slate-600">
                نوبت {new Intl.NumberFormat("fa-IR").format(details.id)}
              </span>
            )}
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              onClick={() => onOpenChange(false)}
              aria-label="بستن فرم"
              className="shrink-0 rounded-xl"
            >
              <X className="size-4" />
            </Button>
          </div>
        </div>

        {loadingOptions ? (
          <div className="flex min-h-56 items-center justify-center gap-3 text-sm text-slate-500">
            <LoaderCircle className="size-5 animate-spin" />
            در حال دریافت اطلاعات...
          </div>
        ) : (
          <form onSubmit={submit} className="space-y-5">
            {formError && (
              <div role="alert" className="rounded-2xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm leading-6 text-rose-700">
                {formError}
              </div>
            )}

            <section className="rounded-2xl border border-slate-200 p-4 sm:p-5">
              <h3 className="mb-4 flex items-center gap-2 font-semibold text-slate-800">
                <UserRound className="size-4 text-violet-600" />
                اطلاعات فردی مشتری
              </h3>
              {isEditing && details ? (
                <div className="grid gap-3 sm:grid-cols-3">
                  <InfoValue label="نام مشتری" value={details.customerName} />
                  <InfoValue label="شماره مشتری" value={new Intl.NumberFormat("fa-IR").format(details.customerId)} />
                  <InfoValue label="وضعیت VIP" value="در API مشتری موجود نیست" muted />
                </div>
              ) : (
                <div className="grid gap-4 sm:grid-cols-2">
                  <FormField label="جستجوی مشتری" htmlFor="appointment-customer" error={fieldErrors.customerId}>
                    <div className="relative">
                      <Search className="absolute top-3.5 right-3 size-4 text-slate-400" />
                      <Input
                        id="appointment-customer"
                        value={customerQuery}
                        onChange={(event) => {
                          const query = event.target.value;
                          setCustomerQuery(query);
                          if (query.trim().length < 2) setCustomerResults([]);
                        }}
                        placeholder="نام یا شماره موبایل مشتری"
                        className={`${inputClass} pr-10`}
                        autoComplete="off"
                        aria-label="جستجوی مشتری بر اساس نام یا موبایل"
                      />
                      {searchingCustomers && (
                        <LoaderCircle className="absolute top-3.5 left-3 size-4 animate-spin text-slate-400" />
                      )}
                      {customerQuery.trim().length >= 2 && !searchingCustomers && customerResults.length === 0 && (
                        <div className="absolute z-20 mt-1 w-full rounded-xl border border-slate-200 bg-white p-3 text-sm text-slate-500 shadow-lg">
                          مشتری فعالی پیدا نشد. ثبت مشتری جدید از این فرم پشتیبانی نمی‌شود.
                        </div>
                      )}
                      {customerResults.length > 0 && (
                        <ul className="absolute z-20 mt-1 max-h-52 w-full overflow-y-auto rounded-xl border border-slate-200 bg-white p-1 shadow-lg">
                          {customerResults.map((item) => (
                            <li key={item.id}>
                              <button
                                type="button"
                                onClick={() => void selectCustomer(item)}
                                className="flex w-full items-center justify-between rounded-lg px-3 py-2 text-right text-sm hover:bg-violet-50 focus-visible:bg-violet-50 focus-visible:outline-none"
                              >
                                <span>{item.fullName}</span>
                                <span dir="ltr" className="text-xs text-slate-500">{formatPhone(item.mobilePhone)}</span>
                              </button>
                            </li>
                          ))}
                        </ul>
                      )}
                    </div>
                  </FormField>
                  <div className="rounded-xl bg-slate-50 p-3 text-xs leading-6 text-slate-600">
                    {customer ? (
                      <div className="flex flex-wrap items-center justify-between gap-2">
                        <span className="font-semibold text-slate-800">{customer.fullName}</span>
                        <span dir="ltr">{formatPhone(customer.mobilePhone)}</span>
                        <span className="w-full border-t border-slate-200 pt-2">وضعیت مشتری: {customer.status ? "فعال" : "غیرفعال"} · وضعیت VIP در API مشتری وجود ندارد.</span>
                      </div>
                    ) : (
                      <p>پس از انتخاب مشتری، نام و شمارهٔ ثبت‌شده از API نمایش داده می‌شود.</p>
                    )}
                  </div>
                </div>
              )}
            </section>

            <section className="rounded-2xl border border-slate-200 p-4 sm:p-5">
              <h3 className="mb-4 font-semibold text-slate-800">خدمت درخواستی</h3>
              {isEditing && details ? (
                <div className="grid gap-3 sm:grid-cols-2">
                  <InfoValue label="خدمت" value={details.serviceName} />
                  <InfoValue label="مدت خدمت" value={`${new Intl.NumberFormat("fa-IR").format(details.durationMinutes)} دقیقه`} />
                </div>
              ) : (
                <div className="grid gap-4 sm:grid-cols-2">
                  <FormField label="نوع خدمت" htmlFor="appointment-service" error={fieldErrors.serviceId}>
                    <select
                      id="appointment-service"
                      value={serviceId}
                      onChange={(event) => chooseService(event.target.value)}
                      className={`${inputClass} w-full px-3 text-sm`}
                      required
                    >
                      <option value="">انتخاب خدمت فعال</option>
                      {options.services.map((item) => (
                        <option key={item.id} value={item.id}>{item.name}</option>
                      ))}
                    </select>
                    {options.services.length === 0 && (
                      <p className="text-xs text-amber-700">خدمت فعال قابل رزروی برای این سالن وجود ندارد.</p>
                    )}
                  </FormField>
                  <FormField label="پرسنل مرتبط" htmlFor="appointment-personnel" error={fieldErrors.personnelId}>
                    <select
                      id="appointment-personnel"
                      value={personnelId}
                      onChange={(event) => choosePersonnel(event.target.value)}
                      disabled={!serviceId || servicePersonnelLoading}
                      className={`${inputClass} w-full px-3 text-sm disabled:bg-slate-100`}
                      required
                    >
                      <option value="">
                        {servicePersonnelLoading ? "در حال دریافت..." : serviceId ? "انتخاب پرسنل" : "ابتدا خدمت را انتخاب کنید"}
                      </option>
                      {selectedPersonnel.map((item) => (
                        <option key={item.id} value={item.id}>{item.fullName}</option>
                      ))}
                    </select>
                    {serviceId && !servicePersonnelLoading && selectedPersonnel.length === 0 && (
                      <p className="text-xs text-amber-700">برای این خدمت پرسنل فعال و واجد شرایط ثبت نشده است.</p>
                    )}
                  </FormField>
                  <FormField label="تعرفهٔ معتبر در تاریخ انتخابی" htmlFor="appointment-tariff" error={fieldErrors.serviceTariffId}>
                    <select
                      id="appointment-tariff"
                      value={tariffId}
                      onChange={(event) => chooseTariff(event.target.value)}
                      disabled={!serviceId || !date}
                      className={`${inputClass} w-full px-3 text-sm disabled:bg-slate-100`}
                    >
                      <option value="">انتخاب تعرفه</option>
                      {activeTariffs.map((item) => (
                        <option key={item.id} value={item.id}>{item.name}</option>
                      ))}
                    </select>
                    {serviceId && date && activeTariffs.length === 0 && (
                      <p className="text-xs text-amber-700">برای این خدمت در تاریخ انتخابی تعرفهٔ فعال وجود ندارد.</p>
                    )}
                  </FormField>
                  <FormField label="توضیحات" htmlFor="appointment-notes" error={fieldErrors.notes}>
                    <textarea
                      id="appointment-notes"
                      value={notes}
                      onChange={(event) => setNotes(event.target.value)}
                      maxLength={1000}
                      rows={3}
                      placeholder="توضیحات اختیاری دربارهٔ نوبت"
                      className="w-full resize-y rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm outline-none transition focus-visible:border-violet-400 focus-visible:ring-3 focus-visible:ring-violet-100"
                    />
                  </FormField>
                </div>
              )}
              {isEditing && (
                <div className="mt-4">
                  <FormField label="پرسنل مرتبط" htmlFor="appointment-personnel-edit" error={fieldErrors.personnelId}>
                    <select
                      id="appointment-personnel-edit"
                      value={personnelId}
                      onChange={(event) => choosePersonnel(event.target.value)}
                      disabled={servicePersonnelLoading}
                      className={`${inputClass} w-full px-3 text-sm disabled:bg-slate-100`}
                      required
                    >
                      <option value="">{servicePersonnelLoading ? "در حال دریافت..." : "انتخاب پرسنل"}</option>
                      {selectedPersonnel.map((item) => (
                        <option key={item.id} value={item.id}>{item.fullName}</option>
                      ))}
                    </select>
                  </FormField>
                  <div className="mt-4">
                    <FormField label="توضیحات" htmlFor="appointment-notes-edit" error={fieldErrors.notes}>
                      <textarea
                        id="appointment-notes-edit"
                        value={notes}
                        onChange={(event) => setNotes(event.target.value)}
                        maxLength={1000}
                        rows={3}
                        className="w-full resize-y rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm outline-none focus-visible:border-violet-400 focus-visible:ring-3 focus-visible:ring-violet-100"
                      />
                    </FormField>
                  </div>
                </div>
              )}
            </section>

            <div className="grid gap-5 lg:grid-cols-2">
              <section className="rounded-2xl border border-slate-200 p-4 sm:p-5">
                <h3 className="mb-4 font-semibold text-slate-800">اطلاعات مالی</h3>
                {isEditing && details ? (
                  <div className="grid grid-cols-2 gap-3">
                    <InfoValue label="مبلغ پایه" value={amountLabel(details.baseAmount)} />
                    <InfoValue label="تخفیف ثبت‌شده" value={amountLabel(details.discountAmount)} />
                    <InfoValue label="مبلغ نهایی سرور" value={amountLabel(details.finalAmount)} prominent />
                  </div>
                ) : (
                  <div className="grid gap-4 sm:grid-cols-2">
                    <FormField label="مبلغ تخفیف (تومان)" htmlFor="appointment-discount" error={fieldErrors.discountAmount}>
                      <Input
                        id="appointment-discount"
                        type="text"
                        inputMode="numeric"
                        value={discount}
                        onChange={(event) => setDiscount(normalizeDigits(event.target.value).replace(/[^\d]/g, ""))}
                        className={inputClass}
                        placeholder="۰"
                      />
                      <p className="text-xs leading-5 text-slate-500">کد تخفیف در API فعلی پشتیبانی نمی‌شود.</p>
                    </FormField>
                    <div className="rounded-xl bg-violet-50 p-3 text-sm leading-6">
                      <div className="flex justify-between gap-2">
                        <span className="text-slate-600">مبلغ پایه از سرور</span>
                        <span className="font-semibold text-slate-900">
                          {availability ? amountLabel(availability.baseAmount) : tariffAmount !== undefined ? amountLabel(tariffAmount) : "پس از بررسی زمان"}
                        </span>
                      </div>
                      <p className="mt-2 text-xs text-violet-700">
                        مبلغ نهایی را سرور پس از ثبت محاسبه می‌کند؛ ثبت نوبت پرداختی ایجاد نمی‌کند.
                      </p>
                    </div>
                  </div>
                )}
              </section>

              <section className="rounded-2xl border border-slate-200 p-4 sm:p-5">
                <h3 className="mb-4 flex items-center gap-2 font-semibold text-slate-800">
                  <Clock3 className="size-4 text-violet-600" />
                  زمان نوبت
                </h3>
                <div className="grid gap-4 sm:grid-cols-2">
                  <FormField label="تاریخ نوبت" error={fieldErrors.date}>
                    <AppointmentDatePicker value={date} onChange={(nextDate) => {
                      setDate(nextDate);
                      if (!validTariffsForDate(options.tariffs, serviceId, nextDate, time)
                        .some((item) => String(item.id) === tariffId)) {
                        setTariffId("");
                      }
                      setAvailability(null);
                      setFormError(null);
                    }} />
                  </FormField>
                  <FormField label="ساعت نوبت" htmlFor="appointment-time" error={fieldErrors.time}>
                    <div className="relative">
                      <Clock3 className="absolute top-3.5 right-3 size-4 text-slate-400" />
                      <Input
                        id="appointment-time"
                        type="time"
                        step={60}
                        value={time}
                        onChange={(event) => {
                          const nextTime = event.target.value;
                          setTime(nextTime);
                          if (!validTariffsForDate(options.tariffs, serviceId, date, nextTime)
                            .some((item) => String(item.id) === tariffId)) {
                            setTariffId("");
                          }
                          setAvailability(null);
                          setFormError(null);
                        }}
                        className={`${inputClass} pr-10`}
                        required
                      />
                    </div>
                  </FormField>
                </div>
                {isEditing ? (
                  <p className="mt-3 text-xs leading-5 text-slate-500">
                    زمان جدید هنگام ذخیره در سرور بررسی می‌شود؛ در صورت تداخل، فرم باز می‌ماند.
                  </p>
                ) : (
                  <div className={`mt-3 rounded-xl px-3 py-2 text-xs leading-5 ${
                    checkingAvailability
                      ? "bg-slate-50 text-slate-600"
                      : availability?.isAvailable
                        ? "bg-emerald-50 text-emerald-700"
                        : availability
                          ? "bg-rose-50 text-rose-700"
                          : "bg-amber-50 text-amber-800"
                  }`}>
                    {checkingAvailability
                      ? "در حال بررسی زمان انتخابی با سرور..."
                      : availability?.isAvailable
                            ? `در بررسی تداخل نوبت‌های ثبت‌شده مانعی یافت نشد · مدت خدمت ${new Intl.NumberFormat("fa-IR").format(availability.durationMinutes)} دقیقه`
                        : availability
                          ? "این زمان در سرور قابل رزرو نیست؛ ساعت دیگری انتخاب کنید."
                          : "API فهرست ساعت‌های آزاد یا برنامهٔ کاری ندارد؛ زمان پیشنهادی با معیارهای قابل بررسی سرور سنجیده می‌شود."}
                  </div>
                )}
              </section>
            </div>

            {!isEditing && (
              <section className="rounded-2xl border border-slate-200 p-4 sm:p-5">
                <h3 className="mb-3 font-semibold text-slate-800">اطلاع‌رسانی به مشتری</h3>
                <div className="flex flex-wrap gap-3">
                  {([
                    [0, "بدون درخواست اطلاع‌رسانی"],
                    [1, "پیامک"],
                    [2, "واتساپ"],
                  ] as const).map(([channel, label]) => (
                    <label key={channel} className="flex cursor-pointer items-center gap-2 rounded-xl border border-slate-200 px-3 py-2 text-sm text-slate-700">
                      <input
                        type="radio"
                        name="appointment-notification"
                        value={channel}
                        checked={notificationChannel === channel}
                        onChange={() => setNotificationChannel(channel)}
                        className="accent-violet-600"
                      />
                      {label}
                    </label>
                  ))}
                </div>
                <p className="mt-2 text-xs text-slate-500">
                  این انتخاب فقط به درخواست API ارسال می‌شود و به معنی ارسال موفق پیام نیست.
                </p>
              </section>
            )}

            <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-4 sm:flex-row sm:justify-end">
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
                disabled={submitting}
                className="h-11 rounded-xl px-6"
              >
                انصراف
              </Button>
              <Button type="submit" disabled={submitting || loadingOptions} className="h-11 rounded-xl px-7">
                {submitting && <LoaderCircle className="ml-2 size-4 animate-spin" />}
                {submitting ? "در حال ثبت..." : isEditing ? "ذخیرهٔ تغییرات" : "ثبت نوبت"}
              </Button>
            </div>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}

function InfoValue({
  label,
  value,
  muted = false,
  prominent = false,
}: {
  label: string;
  value: string;
  muted?: boolean;
  prominent?: boolean;
}) {
  return (
    <div className={`rounded-xl px-3 py-2.5 ${prominent ? "bg-violet-50 sm:col-span-2" : "bg-slate-50"}`}>
      <p className="text-xs text-slate-500">{label}</p>
      <p className={`mt-1 text-sm font-medium ${muted ? "text-slate-400" : "text-slate-800"}`}>{value}</p>
    </div>
  );
}
