"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { CalendarDays, Clock3, LoaderCircle, UserRound, X } from "lucide-react";
import { toJalaali } from "jalaali-js";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  ServerSearchSelect,
  type ServerSearchPage,
} from "@/components/ui/server-search-select";
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
  presentation?: "dialog" | "page";
  onOpenChange: (open: boolean) => void;
  onSaved: (message: string, appointmentDate: string) => void;
};

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

export function AppointmentFormDialog({
  open,
  appointmentId,
  presentation = "dialog",
  onOpenChange,
  onSaved,
}: Props) {
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [customer, setCustomer] = useState<CustomerDto | null>(null);
  const [details, setDetails] = useState<AppointmentDetailsDto | null>(null);
  const [servicePersonnel, setServicePersonnel] = useState<ServicePersonnelDto[]>([]);
  const [servicePersonnelLoading, setServicePersonnelLoading] = useState(appointmentId !== null);
  const [serviceId, setServiceId] = useState("");
  const [serviceName, setServiceName] = useState("");
  const [tariffId, setTariffId] = useState("");
  const [selectedTariff, setSelectedTariff] = useState<ServiceTariffDto | null>(null);
  const [personnelId, setPersonnelId] = useState("");
  const [personnelName, setPersonnelName] = useState("");
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
  const appointmentService = useMemo(() => new AppointmentService(), []);
  const customerRequestRef = useRef<AbortController | null>(null);

  const selectedTariffIsValid = useMemo(
    () => validTariffsForDate(selectedTariff ? [selectedTariff] : [], serviceId, date, time)
      .some((item) => String(item.id) === tariffId),
    [date, selectedTariff, serviceId, tariffId, time],
  );

  const loadCustomerOptions = useCallback(
    async (query: string, pageNumber: number, signal: AbortSignal): Promise<ServerSearchPage<CustomerDto>> => {
      const response = await appointmentService.searchCustomers(query, pageNumber, signal);
      if (!response.data) throw new Error(response.message ?? "فهرست مشتریان دریافت نشد.");
      return {
        options: response.data.list
          .filter((item) => item.status)
          .map((item) => ({
            value: String(item.id),
            label: item.fullName,
            description: formatPhone(item.mobilePhone),
            data: item,
          })),
        hasMore: response.data.pagination.pageNumber < response.data.pagination.totalPages,
      };
    },
    [appointmentService],
  );

  const loadServiceOptions = useCallback(
    async (query: string, _pageNumber: number, signal: AbortSignal): Promise<ServerSearchPage<ServiceDto>> => {
      const services = await appointmentService.searchActiveServices(query, signal);
      return {
        options: services.map((item) => ({
          value: String(item.id),
          label: item.name,
          description: `${new Intl.NumberFormat("fa-IR").format(item.durationMinutes)} دقیقه`,
          data: item,
        })),
        hasMore: false,
      };
    },
    [appointmentService],
  );

  const loadPersonnelOptions = useCallback(
    async (query: string, pageNumber: number, signal: AbortSignal): Promise<ServerSearchPage<PersonnelDto>> => {
      if (!serviceId) return { options: [], hasMore: false };
      const assignments = new Set(servicePersonnel.map((item) => item.personnelId));
      const response = await appointmentService.searchPersonnel(query, pageNumber, signal);
      if (!response.data) throw new Error(response.message ?? "فهرست پرسنل دریافت نشد.");
      return {
        options: response.data.list
          .filter((person) => assignments.has(person.id))
          .map((person) => ({
            value: String(person.id),
            label: person.fullName,
            description: person.jobTitle,
            data: person,
          })),
        hasMore: response.data.pagination.pageNumber < response.data.pagination.totalPages,
      };
    },
    [appointmentService, serviceId, servicePersonnel],
  );

  const loadTariffOptions = useCallback(
    async (query: string, _pageNumber: number, signal: AbortSignal): Promise<ServerSearchPage<ServiceTariffDto>> => {
      if (!serviceId || !date) return { options: [], hasMore: false };
      const tariffs = await appointmentService.searchTariffs(query, signal);
      const validTariffs = validTariffsForDate(tariffs, serviceId, date, time);
      return {
        options: validTariffs.map((item) => ({
          value: String(item.id),
          label: item.name,
          description: amountLabel(
            item.services.find((service) => service.serviceId === Number(serviceId))?.amount ?? 0,
          ),
          data: item,
        })),
        hasMore: false,
      };
    },
    [appointmentService, date, serviceId, time],
  );

  useEffect(() => {
    if (!open) return;
    const controller = new AbortController();
    const appointmentRequest: Promise<AppointmentDetailsDto | null> = appointmentId
      ? appointmentService.getById(appointmentId, controller.signal).then((response) => {
            if (!response.data) throw new Error(response.message ?? "جزئیات نوبت دریافت نشد.");
            return response.data;
          })
      : Promise.resolve(null);

    void appointmentRequest
      .then((appointment) => {
        if (controller.signal.aborted) return;
        if (appointment) {
          setDetails(appointment);
          setServiceId(String(appointment.serviceId));
          setServiceName(appointment.serviceName);
          setPersonnelId(String(appointment.personnelId));
          setPersonnelName(appointment.personnelName);
          const current = currentTehranDateTime(appointment.startsAt);
          setDate(current.date);
          setTime(current.time);
          setNotes(appointment.notes ?? "");
        } else {
          setDate(todayJalali());
          setTime("");
          setServiceId("");
          setServiceName("");
          setCustomer(null);
          setPersonnelId("");
          setPersonnelName("");
          setTariffId("");
          setSelectedTariff(null);
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
  }, [appointmentId, appointmentService, open]);

  useEffect(() => {
    if (!open || !serviceId) return;
    const controller = new AbortController();
    void appointmentService
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
  }, [appointmentId, appointmentService, open, serviceId]);

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
      void appointmentService
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
  }, [appointmentId, appointmentService, date, open, personnelId, serviceId, tariffId, time]);

  async function selectCustomer(result: CustomerDto | null) {
    customerRequestRef.current?.abort();
    setFormError(null);
    if (!result) {
      setCustomer(null);
      return;
    }
    const controller = new AbortController();
    customerRequestRef.current = controller;
    setCustomer(result);
    try {
      const response = await appointmentService.getCustomerById(result.id, controller.signal);
      if (!response.data) throw new Error(response.message ?? "اطلاعات مشتری دریافت نشد.");
      if (!response.data.status) throw new Error("این مشتری غیرفعال است و امکان رزرو برای او وجود ندارد.");
      if (!controller.signal.aborted) setCustomer(response.data);
    } catch (cause: unknown) {
      if (!controller.signal.aborted) {
        setCustomer(null);
        setFormError(handleApiError(cause));
      }
    }
  }

  function chooseService(value: string, name = "") {
    setServiceId(value);
    setServiceName(name);
    setPersonnelId("");
    setPersonnelName("");
    setServicePersonnel([]);
    setServicePersonnelLoading(Boolean(value));
    setTariffId("");
    setSelectedTariff(null);
    setAvailability(null);
    setFormError(null);
  }

  function chooseTariff(value: string, selected: ServiceTariffDto | null) {
    setTariffId(value);
    setSelectedTariff(selected);
    setAvailability(null);
    setFormError(null);
  }

  function choosePersonnel(value: string, name = "") {
    setPersonnelId(value);
    setPersonnelName(name);
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
    if (!appointmentId && !selectedTariffIsValid) {
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
      const serviceApi = appointmentService;
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

  const tariffAmount = selectedTariff?.services.find((item) => item.serviceId === Number(serviceId))?.amount;
  const isEditing = appointmentId !== null;
  const isPagePresentation = presentation === "page";
  const summaryBaseAmount = availability?.baseAmount ?? tariffAmount ?? null;
  const normalizedDiscount = Number(normalizeDigits(discount || "0"));
  const summaryFinalAmount =
    summaryBaseAmount === null ? null : Math.max(0, summaryBaseAmount - normalizedDiscount);
  const sectionClassName = isPagePresentation
    ? "rounded-xl border border-slate-200 bg-white/80 p-3 sm:p-3"
    : "rounded-2xl border border-slate-200 p-4 sm:p-5";
  const sectionTitleClassName = isPagePresentation
    ? "mb-3 flex items-center gap-2 text-sm font-bold text-fuchsia-800"
    : "mb-4 font-semibold text-slate-800";

  return (
    <Dialog open={open} modal={!isPagePresentation} onOpenChange={onOpenChange}>
      <DialogContent
        dir="rtl"
        showOverlay={!isPagePresentation}
        unbounded={isPagePresentation}
        showCloseButton={false}
        className={isPagePresentation
          ? "fixed !inset-x-0 !bottom-0 !left-0 !top-[60px] !h-[calc(100dvh-60px)] !translate-x-0 !translate-y-0 grid-cols-1 grid-rows-[32px_auto_minmax(0,1fr)] gap-2 overflow-y-auto rounded-none border-0 bg-[#f7f9fd] p-3 shadow-none ring-0 sm:p-4 lg:grid-cols-[minmax(0,1fr)_300px] lg:grid-rows-[32px_auto_minmax(0,1fr)] lg:gap-3 lg:overflow-hidden lg:p-5"
          : "max-h-[calc(100dvh-2rem)] overflow-y-auto rounded-3xl p-4 sm:p-6 lg:p-8"}
        aria-describedby="appointment-form-description"
      >
        {isPagePresentation && (
          <div className="-mx-3 -mt-3 flex items-center bg-[linear-gradient(90deg,#40104b_0%,#28102f_50%,#40104b_100%)] px-4 text-[10px] font-medium text-white/90 sm:-mx-4 sm:-mt-4 lg:-mx-5 lg:-mt-5">
            <span>رزرو نوبت جدید</span>
            <span className="mx-2 text-fuchsia-300/70">/</span>
            <span>ثبت نوبت</span>
          </div>
        )}
        <div className={`flex items-start justify-between gap-4 border-b border-slate-100 pb-4 ${isPagePresentation ? "col-span-full rounded-xl border border-fuchsia-100 bg-gradient-to-l from-white to-fuchsia-50/70 px-4 py-3" : "pb-5"}`}>
          <div className="space-y-2">
            <DialogTitle className={`flex items-center gap-2 font-bold ${isPagePresentation ? "text-lg text-fuchsia-800" : "text-xl text-slate-900"}`}>
              {isPagePresentation && <CalendarDays aria-hidden="true" className="size-5 text-fuchsia-600" />}
              {isEditing ? "ویرایش نوبت" : "رزرو نوبت جدید"}
            </DialogTitle>
            <DialogDescription id="appointment-form-description" className="text-xs text-slate-500">
              {isEditing
                ? "زمان، پرسنل و توضیحات نوبت را به‌روزرسانی کنید."
                : "اطلاعات مشتری، خدمت و زمان پیشنهادی را وارد کنید."}
            </DialogDescription>
          </div>
          <div className="flex items-center gap-2">
            {isPagePresentation && (
              <p className="hidden max-w-[360px] items-center gap-2 rounded-lg border border-fuchsia-100 bg-white/80 px-3 py-2 text-[10px] leading-5 text-slate-500 sm:flex">
                اطلاعات مشتری پس از انتخاب نمایش داده می‌شود و شماره نوبت پس از ثبت از API دریافت خواهد شد.
              </p>
            )}
            {isEditing && details && (
              <span className="rounded-full bg-slate-100 px-3 py-1 text-xs text-slate-600">
                نوبت {new Intl.NumberFormat("fa-IR").format(details.id)}
              </span>
            )}
            {!isPagePresentation && (
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
            )}
          </div>
        </div>

        {loadingOptions ? (
          <div className={`flex min-h-56 items-center justify-center gap-3 text-sm text-slate-500 ${isPagePresentation ? "col-span-full lg:row-start-3" : ""}`}>
            <LoaderCircle className="size-5 animate-spin" />
            در حال دریافت اطلاعات...
          </div>
        ) : (
          <form
            onSubmit={submit}
            className={`space-y-3 ${isPagePresentation ? "min-h-0 overflow-y-auto pe-1 lg:col-start-1 lg:row-start-3" : "space-y-5"}`}
          >
            {formError && (
              <div role="alert" className={`border border-rose-200 bg-rose-50 px-4 py-3 text-sm leading-6 text-rose-700 ${isPagePresentation ? "rounded-xl" : "rounded-2xl"}`}>
                {formError}
              </div>
            )}

            <section className={sectionClassName}>
              <h3 className={sectionTitleClassName}>
                {isPagePresentation && <span className="grid size-5 place-items-center rounded-full bg-fuchsia-700 text-[10px] text-white">۱</span>}
                <UserRound className="size-4 text-fuchsia-600" />
                اطلاعات فردی مشتری
              </h3>
              {isEditing && details ? (
                <div className="grid gap-3 sm:grid-cols-3">
                  <InfoValue label="نام مشتری" value={details.customerName} />
                  <InfoValue label="شماره مشتری" value={new Intl.NumberFormat("fa-IR").format(details.customerId)} />
                  <InfoValue label="وضعیت VIP" value="در API مشتری موجود نیست" muted />
                </div>
              ) : (
                <div className={`grid gap-3 ${isPagePresentation ? "sm:grid-cols-[minmax(0,1fr)_minmax(0,1fr)] lg:grid-cols-2" : "sm:grid-cols-2"}`}>
                  <FormField label="جستجوی مشتری" htmlFor="appointment-customer" error={fieldErrors.customerId}>
                    <ServerSearchSelect<CustomerDto>
                      id="appointment-customer"
                      ariaLabel="جستجوی مشتری بر اساس نام یا موبایل"
                      value={customer ? String(customer.id) : ""}
                      valueLabel={customer?.fullName}
                      loadOptions={loadCustomerOptions}
                      placeholder="نام یا شماره موبایل مشتری"
                      emptyMessage="مشتری فعالی پیدا نشد."
                      onChange={(_value, option) => void selectCustomer(option?.data ?? null)}
                    />
                  </FormField>
                  <div className={`rounded-xl p-3 text-xs leading-6 text-slate-600 ${isPagePresentation ? "border border-fuchsia-100 bg-fuchsia-50/40" : "bg-slate-50"}`}>
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

            <section className={sectionClassName}>
              <h3 className={sectionTitleClassName}>
                {isPagePresentation && <span className="grid size-5 place-items-center rounded-full bg-fuchsia-700 text-[10px] text-white">۲</span>}
                خدمت درخواستی
              </h3>
              {isEditing && details ? (
                <div className="grid gap-3 sm:grid-cols-2">
                  <InfoValue label="خدمت" value={details.serviceName} />
                  <InfoValue label="مدت خدمت" value={`${new Intl.NumberFormat("fa-IR").format(details.durationMinutes)} دقیقه`} />
                </div>
              ) : (
                <div className={`grid gap-3 ${isPagePresentation ? "sm:grid-cols-2 lg:grid-cols-4" : "sm:grid-cols-2"}`}>
                  <FormField label="نوع خدمت" htmlFor="appointment-service" error={fieldErrors.serviceId}>
                    <ServerSearchSelect<ServiceDto>
                      id="appointment-service"
                      ariaLabel="انتخاب خدمت فعال"
                      value={serviceId}
                      loadOptions={loadServiceOptions}
                      emptyMessage="خدمت فعالی برای این سالن پیدا نشد."
                      onChange={(value, option) => chooseService(value, option?.data?.name ?? "")}
                    />
                  </FormField>
                  <FormField label="پرسنل مرتبط" htmlFor="appointment-personnel" error={fieldErrors.personnelId}>
                    <ServerSearchSelect<PersonnelDto>
                      id="appointment-personnel"
                      ariaLabel="انتخاب پرسنل مرتبط"
                      value={personnelId}
                      valueLabel={personnelId === String(details?.personnelId) ? details?.personnelName : undefined}
                      loadOptions={loadPersonnelOptions}
                      emptyMessage="پرسنل فعالی برای این خدمت پیدا نشد."
                      onChange={(value, option) => choosePersonnel(value, option?.data?.fullName ?? "")}
                      disabled={!serviceId || servicePersonnelLoading}
                    />
                    {serviceId && !servicePersonnelLoading && servicePersonnel.length === 0 && (
                      <p className="text-xs text-amber-700">برای این خدمت پرسنل فعال و واجد شرایط ثبت نشده است.</p>
                    )}
                  </FormField>
                  <FormField label="تعرفهٔ معتبر در تاریخ انتخابی" htmlFor="appointment-tariff" error={fieldErrors.serviceTariffId}>
                    <ServerSearchSelect<ServiceTariffDto>
                      id="appointment-tariff"
                      ariaLabel="جستجو و انتخاب تعرفه معتبر"
                      value={tariffId}
                      valueLabel={selectedTariff?.name}
                      loadOptions={loadTariffOptions}
                      emptyMessage="برای این خدمت و تاریخ، تعرفهٔ معتبری پیدا نشد."
                      onChange={(value, option) => chooseTariff(value, option?.data ?? null)}
                      disabled={!serviceId || !date}
                    />
                    {serviceId && date && !selectedTariffIsValid && (
                      <p className="text-xs text-amber-700">تعرفه را با جستجو انتخاب کنید.</p>
                    )}
                  </FormField>
                  <div className={isPagePresentation ? "lg:col-span-4" : ""}>
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
                </div>
              )}
              {isEditing && (
                <div className="mt-4">
                  <FormField label="پرسنل مرتبط" htmlFor="appointment-personnel-edit" error={fieldErrors.personnelId}>
                    <ServerSearchSelect<PersonnelDto>
                      id="appointment-personnel-edit"
                      ariaLabel="انتخاب پرسنل مرتبط برای ویرایش"
                      value={personnelId}
                      valueLabel={personnelId === String(details?.personnelId) ? details?.personnelName : undefined}
                      loadOptions={loadPersonnelOptions}
                      emptyMessage="پرسنل فعالی برای این خدمت پیدا نشد."
                      onChange={(value, option) => choosePersonnel(value, option?.data?.fullName ?? "")}
                      disabled={servicePersonnelLoading}
                    />
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

            <div className={isPagePresentation ? "flex flex-col gap-3" : "grid gap-5 lg:grid-cols-2"}>
              <section className={`${sectionClassName} ${isPagePresentation ? "order-2" : ""}`}>
                <h3 className={sectionTitleClassName}>
                  {isPagePresentation && <span className="grid size-5 place-items-center rounded-full bg-fuchsia-700 text-[10px] text-white">۴</span>}
                  اطلاعات مالی
                </h3>
                {isEditing && details ? (
                  <div className="grid grid-cols-2 gap-3">
                    <InfoValue label="مبلغ پایه" value={amountLabel(details.baseAmount)} />
                    <InfoValue label="تخفیف ثبت‌شده" value={amountLabel(details.discountAmount)} />
                    <InfoValue label="مبلغ نهایی سرور" value={amountLabel(details.finalAmount)} prominent />
                  </div>
                ) : (
                  <div className="grid gap-3 sm:grid-cols-2">
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
                    <div className={`rounded-xl p-3 text-sm leading-6 ${isPagePresentation ? "border border-blue-100 bg-blue-50/60" : "bg-violet-50"}`}>
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

              <section className={`${sectionClassName} ${isPagePresentation ? "order-1" : ""}`}>
                <h3 className={sectionTitleClassName}>
                  {isPagePresentation && <span className="grid size-5 place-items-center rounded-full bg-fuchsia-700 text-[10px] text-white">۳</span>}
                  <Clock3 className="size-4 text-fuchsia-600" />
                  زمان نوبت
                </h3>
                <div className="grid gap-3 sm:grid-cols-2">
                  <FormField label="تاریخ نوبت" error={fieldErrors.date}>
                    <AppointmentDatePicker value={date} onChange={(nextDate) => {
                      setDate(nextDate);
                      if (!validTariffsForDate(selectedTariff ? [selectedTariff] : [], serviceId, nextDate, time)
                        .some((item) => String(item.id) === tariffId)) {
                        setTariffId("");
                        setSelectedTariff(null);
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
                          if (!validTariffsForDate(selectedTariff ? [selectedTariff] : [], serviceId, date, nextTime)
                            .some((item) => String(item.id) === tariffId)) {
                            setTariffId("");
                            setSelectedTariff(null);
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
              <section className={sectionClassName}>
                <h3 className={sectionTitleClassName}>
                  {isPagePresentation && <span className="grid size-5 place-items-center rounded-full bg-fuchsia-700 text-[10px] text-white">۵</span>}
                  اطلاع‌رسانی به مشتری
                </h3>
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

            <div className={`flex flex-col-reverse gap-3 border-t border-slate-100 pt-3 sm:flex-row sm:justify-end ${isPagePresentation ? "sticky bottom-0 z-10 rounded-b-xl bg-white/95 px-2 pb-2 backdrop-blur-sm" : "pt-4"}`}>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
                disabled={submitting}
                className="h-10 rounded-xl px-6"
              >
                انصراف
              </Button>
              <Button type="submit" disabled={submitting || loadingOptions} className="h-10 rounded-xl bg-fuchsia-700 px-7 text-white hover:bg-fuchsia-800">
                {submitting && <LoaderCircle className="ml-2 size-4 animate-spin" />}
                {submitting ? "در حال ثبت..." : isEditing ? "ذخیرهٔ تغییرات" : "ثبت نوبت"}
              </Button>
            </div>
          </form>
        )}
        {isPagePresentation && !loadingOptions && (
          <aside className="min-w-0 rounded-xl border border-slate-200 bg-white p-3 shadow-sm lg:col-start-2 lg:row-start-3 lg:overflow-y-auto">
            <h2 className="mb-3 text-sm font-bold text-slate-800">خلاصه نوبت</h2>
            <div className="mb-3 flex items-center gap-3 rounded-xl border border-fuchsia-100 bg-fuchsia-50/40 p-3">
              <span className="grid size-10 shrink-0 place-items-center rounded-full bg-fuchsia-100 text-fuchsia-700">
                <UserRound aria-hidden="true" className="size-5" />
              </span>
              <div className="min-w-0">
                <p className="text-[10px] text-slate-500">مشتری انتخاب‌شده</p>
                <p className="truncate text-xs font-bold text-slate-800">{customer?.fullName ?? "—"}</p>
                {customer && <p dir="ltr" className="mt-1 text-start text-[10px] text-slate-500">{formatPhone(customer.mobilePhone)}</p>}
              </div>
            </div>
            <dl className="divide-y divide-slate-100">
              <SummaryItem label="خدمت" value={serviceName || (serviceId ? `خدمت ${new Intl.NumberFormat("fa-IR").format(Number(serviceId))}` : "—")} />
              <SummaryItem label="پرسنل" value={personnelName || "—"} />
              <SummaryItem label="تاریخ نوبت" value={date.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]) || "—"} />
              <SummaryItem label="ساعت" value={time || "—"} />
              <SummaryItem label="مبلغ نهایی" value={summaryFinalAmount === null ? "— تومان" : amountLabel(summaryFinalAmount)} emphasized />
            </dl>
            <p className="mt-3 rounded-xl border border-fuchsia-100 bg-fuchsia-50/70 p-3 text-[10px] leading-5 text-fuchsia-800">
              <span className="mb-1 flex items-center gap-1 font-semibold">
                <CalendarDays aria-hidden="true" className="size-3.5" />
                مبلغ پس از انتخاب خدمت و تاریخ
              </span>
              مبلغ تخفیف از مبلغ پایه کسر می‌شود. مبلغ نهایی قطعی را سرور پس از ثبت محاسبه می‌کند.
            </p>
          </aside>
        )}
      </DialogContent>
    </Dialog>
  );
}

function SummaryItem({
  label,
  value,
  emphasized = false,
}: {
  label: string;
  value: string;
  emphasized?: boolean;
}) {
  return (
    <div className="flex items-center justify-between gap-3 py-3">
      <dt className="shrink-0 text-[11px] text-slate-500">{label}</dt>
      <dd className={`truncate text-start text-[11px] ${emphasized ? "font-bold text-fuchsia-800" : "font-medium text-slate-700"}`}>
        {value}
      </dd>
    </div>
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
