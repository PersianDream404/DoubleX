"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { ArrowRight, LoaderCircle, Plus, Trash2 } from "lucide-react";

import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { handleApiError } from "@/lib/api/handle-api-error";
import type { ApiResponse } from "@/lib/api/types/api-response";
import type { PagedResult } from "@/lib/api/types/paged-result";
import { useUserContext } from "@/contexts/user-context";
import { MaterialService } from "@/features/inventory/materials/material.service";
import type { MaterialDto } from "@/features/inventory/materials/material.types";
import { PersonnelService } from "@/features/personnel/list/personnel.service";
import type { PersonnelDto } from "@/features/personnel/list/personnel.types";
import { ServiceCatalogService } from "@/features/services/list/service.service";
import type { ServiceDto } from "@/features/services/list/service.types";
import { ServiceTariffService } from "@/features/services/tariffs/service-tariff.service";
import type { ServiceTariffDto } from "@/features/services/tariffs/service-tariff.types";

import {
  CreateReceptionFormSchema,
  type ReceptionPaymentForm,
} from "./reception.schema";
import { ReceptionService } from "./reception.service";
import type { CustomerSelectItem, ReceptionServiceItem } from "./reception.types";

type ServiceOption = {
  id: string;
  serviceId: number;
  serviceTariffId: number;
  name: string;
  amount: number;
};

type MaterialDraft = {
  key: string;
  materialId: number | null;
  quantity: number;
  description: string;
};

type ServiceDraft = {
  key: string;
  serviceId: number | null;
  serviceTariffId: number | null;
  personnelId: number | null;
  quantity: number;
  discountAmount: number;
  unitPrice: number;
  materials: MaterialDraft[];
};

type PaymentDraft = ReceptionPaymentForm & { key: string };

type ReceptionOptions = {
  personnels: PersonnelDto[];
  services: ServiceDto[];
  tariffs: ServiceTariffDto[];
  materials: MaterialDto[];
};

const EMPTY_SERVICE: ServiceDraft = {
  key: "service-1",
  serviceId: null,
  serviceTariffId: null,
  personnelId: null,
  quantity: 1,
  discountAmount: 0,
  unitPrice: 0,
  materials: [],
};

const PAGE_SIZE = 100;

async function loadAllPages<TItem>(
  loadPage: (
    pageNumber: number,
    pageSize: number,
  ) => Promise<ApiResponse<PagedResult<TItem>>>,
): Promise<TItem[]> {
  const first = await loadPage(1, PAGE_SIZE);
  if (!first.data) {
    throw new Error(first.message ?? "فهرست اطلاعات دریافت نشد.");
  }

  const items = [...first.data.list];
  for (let pageNumber = 2; pageNumber <= first.data.pagination.totalPages; pageNumber += 1) {
    const response = await loadPage(pageNumber, PAGE_SIZE);
    if (!response.data) {
      throw new Error(response.message ?? "بخشی از فهرست اطلاعات دریافت نشد.");
    }
    items.push(...response.data.list);
  }
  return items;
}

function createServiceDraft(key: string): ServiceDraft {
  return { ...EMPTY_SERVICE, key, materials: [] };
}

function createMaterialDraft(key: string): MaterialDraft {
  return { key, materialId: null, quantity: 1, description: "" };
}

function amountLabel(amount: number) {
  return `${new Intl.NumberFormat("fa-IR").format(amount)} تومان`;
}

function Field({
  label,
  children,
  hint,
}: {
  label: string;
  children: React.ReactNode;
  hint?: string;
}) {
  return (
    <div className="min-w-0 space-y-1.5">
      <Label className="text-xs text-slate-600">{label}</Label>
      {children}
      {hint && <p className="text-xs leading-5 text-slate-500">{hint}</p>}
    </div>
  );
}

function FormSelect({
  value,
  onChange,
  children,
  disabled,
  ariaLabel,
}: {
  value: string;
  onChange: (value: string) => void;
  children: React.ReactNode;
  disabled?: boolean;
  ariaLabel: string;
}) {
  return (
    <select
      aria-label={ariaLabel}
      value={value}
      onChange={(event) => onChange(event.target.value)}
      disabled={disabled}
      className="h-10 w-full min-w-0 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-800 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 disabled:bg-slate-50 disabled:text-slate-400"
    >
      {children}
    </select>
  );
}

function InfoCard({
  title,
  action,
  children,
}: {
  title: string;
  action?: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <section className="min-w-0 rounded-xl border border-slate-100 bg-white p-4 shadow-sm">
      <div className="mb-4 flex items-center justify-between gap-3">
        <h2 className="text-sm font-semibold text-slate-800">{title}</h2>
        {action}
      </div>
      {children}
    </section>
  );
}

export function NewReceptionPage() {
  const router = useRouter();
  const { salonId, loading: userLoading, error: userError } = useUserContext();
  const receptionService = useMemo(() => new ReceptionService(), []);
  const nextKey = useRef(2);
  const [options, setOptions] = useState<ReceptionOptions | null>(null);
  const [optionsLoading, setOptionsLoading] = useState(true);
  const [optionsError, setOptionsError] = useState<string | null>(null);
  const [customerQuery, setCustomerQuery] = useState("");
  const [customers, setCustomers] = useState<CustomerSelectItem[]>([]);
  const [customerLoading, setCustomerLoading] = useState(false);
  const [customerError, setCustomerError] = useState<string | null>(null);
  const [customerId, setCustomerId] = useState<number | null>(null);
  const [receptionistId, setReceptionistId] = useState<number | null>(null);
  const [serviceTariffId, setServiceTariffId] = useState<number | null>(null);
  const [notes, setNotes] = useState("");
  const [services, setServices] = useState<ServiceDraft[]>([EMPTY_SERVICE]);
  const [payments, setPayments] = useState<PaymentDraft[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [createdReceptionId, setCreatedReceptionId] = useState<number | null>(null);

  useEffect(() => {
    if (!salonId) return;

    const controller = new AbortController();
    const personnelService = new PersonnelService();
    const materialService = new MaterialService();
    const catalogService = new ServiceCatalogService();
    const tariffService = new ServiceTariffService();

    async function loadOptions() {
      setOptionsError(null);
      try {
        const [personnels, materials, serviceResponse, tariffResponse] = await Promise.all([
          loadAllPages((pageNumber, pageSize) =>
            personnelService.getAll(
              { PageNumber: pageNumber, PageSize: pageSize },
              { signal: controller.signal },
            ),
          ),
          loadAllPages((pageNumber, pageSize) =>
            materialService.getAll(
              { PageNumber: pageNumber, PageSize: pageSize, IsActive: true },
              { signal: controller.signal },
            ),
          ),
          catalogService.getAll({ signal: controller.signal }),
          tariffService.getAll({ signal: controller.signal }),
        ]);
        if (!serviceResponse.data || !tariffResponse.data) {
          throw new Error(
            serviceResponse.message ??
              tariffResponse.message ??
              "خدمات و تعرفه‌های سالن دریافت نشد.",
          );
        }

        if (!controller.signal.aborted) {
          setOptions({
            personnels: personnels.filter((personnel) => personnel.status),
            materials: materials.filter(
              (material) => material.isActive && material.currentStock > 0,
            ),
            services: serviceResponse.data.filter((service) => service.isActive),
            tariffs: tariffResponse.data.filter((tariff) => {
              const now = Date.now();
              return (
                new Date(tariff.validFrom).getTime() <= now &&
                (!tariff.validTo || new Date(tariff.validTo).getTime() >= now)
              );
            }),
          });
          setOptionsLoading(false);
        }
      } catch (error: unknown) {
        if (!controller.signal.aborted) {
          setOptionsError(handleApiError(error));
          setOptionsLoading(false);
        }
      }
    }

    void loadOptions();
    return () => controller.abort();
  }, [salonId]);

  useEffect(() => {
    if (!salonId) return;

    const controller = new AbortController();
    const timeout = window.setTimeout(() => {
      setCustomerLoading(true);
      setCustomerError(null);
      receptionService
        .getCustomers(
          { q: customerQuery.trim(), pageNumber: 1, pageSize: 100 },
          { signal: controller.signal },
        )
        .then((response) => {
          if (!response.data) {
            throw new Error(response.message ?? "فهرست مشتریان دریافت نشد.");
          }
          if (!controller.signal.aborted) {
            setCustomers(response.data.list.filter((customer) => customer.status));
            setCustomerLoading(false);
          }
        })
        .catch((error: unknown) => {
          if (!controller.signal.aborted) {
            setCustomerError(handleApiError(error));
            setCustomerLoading(false);
          }
        });
    }, 250);

    return () => {
      window.clearTimeout(timeout);
      controller.abort();
    };
  }, [salonId, customerQuery, receptionService]);

  const serviceOptions = useMemo<ServiceOption[]>(() => {
    if (!options || !serviceTariffId) return [];
    const servicesById = new Map(options.services.map((service) => [service.id, service]));
    const tariff = options.tariffs.find((item) => item.id === serviceTariffId);
    if (!tariff) return [];
    return tariff.services.flatMap((tariffService) => {
      const service = servicesById.get(tariffService.serviceId);
      if (!service) return [];
      return [{
        id: `${tariff.id}:${service.id}`,
        serviceId: service.id,
        serviceTariffId: tariff.id,
        name: service.name,
        amount: tariffService.amount,
      }];
    });
  }, [options, serviceTariffId]);

  const totalAmount = services.reduce(
    (total, service) =>
      total + Math.max(0, service.quantity * service.unitPrice - service.discountAmount),
    0,
  );
  const totalPayment = payments.reduce((total, payment) => total + payment.amount, 0);
  const remainingAmount = Math.max(0, totalAmount - totalPayment);
  const salonError = userError
    ? `${userError} لطفاً خارج شوید و دوباره وارد شوید.`
    : !userLoading && !salonId
      ? "شناسه سالن در توکن ورود شما موجود نیست؛ لطفاً دوباره وارد شوید."
      : null;
  const currentOptionsError = optionsError ?? salonError;
  const currentOptionsLoading = userLoading || (Boolean(salonId) && optionsLoading);

  const updateService = useCallback(
    (key: string, update: (service: ServiceDraft) => ServiceDraft) => {
      setServices((current) =>
        current.map((service) => (service.key === key ? update(service) : service)),
      );
    },
    [],
  );

  function addService() {
    setServices((current) => [...current, createServiceDraft(`service-${nextKey.current++}`)]);
  }

  function addPayment() {
    setPayments((current) => [
      ...current,
      {
        key: `payment-${nextKey.current++}`,
        amount: remainingAmount,
        paymentMethod: "کارت‌خوان",
        transactionReference: "",
        description: "",
      },
    ]);
  }

  async function submitReception(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting || createdReceptionId !== null) return;
    if (!salonId) {
      setFormError("شناسه سالن در توکن ورود شما موجود نیست.");
      return;
    }
    setFormError(null);

    const parsedForm = CreateReceptionFormSchema.safeParse({
      customerId: customerId ?? 0,
      receptionistId: receptionistId ?? 0,
      serviceTariffId: serviceTariffId ?? 0,
      notes,
      services: services.map((service) => ({
        serviceId: service.serviceId ?? 0,
        serviceTariffId: service.serviceTariffId ?? 0,
        personnelId: service.personnelId,
        quantity: service.quantity,
        discountAmount: service.discountAmount,
        unitPrice: service.unitPrice,
        materials: service.materials.map((material) => ({
          materialId: material.materialId ?? 0,
          quantity: material.quantity,
          description: material.description,
        })),
      })),
      payments: payments.map((payment) => ({
        amount: payment.amount,
        paymentMethod: payment.paymentMethod,
        transactionReference: payment.transactionReference,
        description: payment.description,
      })),
    });

    if (!parsedForm.success) {
      setFormError(parsedForm.error.issues[0]?.message ?? "اطلاعات فرم را بررسی کنید.");
      return;
    }

    const materialUsage = new Map<number, number>();
    for (const service of parsedForm.data.services) {
      for (const material of service.materials) {
        materialUsage.set(
          material.materialId,
          (materialUsage.get(material.materialId) ?? 0) + material.quantity,
        );
      }
    }
    for (const [materialId, quantity] of materialUsage) {
      const availableStock = options?.materials.find((item) => item.id === materialId)?.currentStock;
      if (availableStock === undefined || quantity > availableStock) {
        setFormError("مقدار ماده مصرفی از موجودی فعلی بیشتر است.");
        return;
      }
    }

    setSubmitting(true);
    let activeReceptionId: number | null = null;
    try {
      const personnelIds = new Set([
        parsedForm.data.receptionistId,
        ...parsedForm.data.services
          .map((service) => service.personnelId)
          .filter((id): id is number => id !== null),
      ]);
      const personnelService = new PersonnelService();
      const personnelDetails = await Promise.all(
        [...personnelIds].map((id) =>
          personnelService.getById(id),
        ),
      );
      if (
        personnelDetails.some(
          (response) =>
            !response.data ||
            response.data.salonId !== salonId ||
            !response.data.status,
        )
      ) {
        throw new Error("پرسنل انتخاب‌شده فعال نیست یا به سالن جاری تعلق ندارد.");
      }

      const createResponse = await receptionService.create({
        customerId: parsedForm.data.customerId,
        receptionistId: parsedForm.data.receptionistId,
        serviceTariffId: parsedForm.data.serviceTariffId,
        receptionDate: new Date().toISOString(),
        notes: parsedForm.data.notes.trim() || null,
        appointmentId: null,
      });
      activeReceptionId = createResponse.data?.id ?? null;
      if (!activeReceptionId) {
        throw new Error(createResponse.message ?? "شناسه پذیرش ایجادشده از سرور دریافت نشد.");
      }
      setCreatedReceptionId(activeReceptionId);

      const addedReceptionServiceIds = new Set<number>();
      for (const service of parsedForm.data.services) {
        await receptionService.addService(activeReceptionId, {
          serviceTariffId: service.serviceTariffId,
          serviceId: service.serviceId,
          personnelId: service.personnelId,
          quantity: service.quantity,
          discountAmount: service.discountAmount,
        });

        const detailResponse = await receptionService.getById(activeReceptionId);
        if (!detailResponse.data) {
          throw new Error(detailResponse.message ?? "خدمت به پذیرش اضافه شد اما جزئیات آن دریافت نشد.");
        }
        const addedService = detailResponse.data.services.find(
          (item: ReceptionServiceItem) =>
            !addedReceptionServiceIds.has(item.id) &&
            item.serviceId === service.serviceId &&
            item.serviceTariffId === service.serviceTariffId &&
            item.personnelId === service.personnelId &&
            item.quantity === service.quantity &&
            item.unitPrice === service.unitPrice,
        );
        if (!addedService) {
          throw new Error("شناسه خدمت ثبت‌شده از سرور دریافت نشد؛ ادامه ثبت متوقف شد.");
        }
        addedReceptionServiceIds.add(addedService.id);

        for (const material of service.materials) {
          await receptionService.registerMaterial(activeReceptionId, addedService.id, {
            materialId: material.materialId,
            quantity: material.quantity,
            idempotencyKey: crypto.randomUUID(),
            description: material.description.trim() || null,
          });
        }
      }

      for (const payment of parsedForm.data.payments) {
        await receptionService.registerPayment(activeReceptionId, {
          amount: payment.amount,
          paymentMethod: payment.paymentMethod,
          paidAt: new Date().toISOString(),
          transactionReference: payment.transactionReference.trim() || null,
          idempotencyKey: crypto.randomUUID(),
          description: payment.description.trim() || null,
        });
      }

      router.replace("/receptions");
    } catch (error: unknown) {
      const message = handleApiError(error);
      setFormError(
        activeReceptionId
          ? `پذیرش شماره ${activeReceptionId} ایجاد شد، اما ثبت همه جزئیات کامل نشد. برای جلوگیری از ثبت تکراری، این فرم قفل شده است. خطا: ${message}`
          : message,
      );
    } finally {
      setSubmitting(false);
    }
  }

  function chooseService(key: string, value: string) {
    const option = serviceOptions.find((item) => item.id === value);
    updateService(key, (current) => ({
      ...current,
      serviceId: option?.serviceId ?? null,
      serviceTariffId: option?.serviceTariffId ?? null,
      unitPrice: option?.amount ?? 0,
    }));
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[142px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/receptions">مدیریت پذیرش</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">ثبت پذیرش جدید</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-4 flex items-center gap-3 text-white">
            <Button
              render={<Link href="/receptions" />}
              variant="ghost"
              size="icon"
              aria-label="بازگشت به فهرست پذیرش‌ها"
              className="text-white hover:bg-white/10 hover:text-white"
            >
              <ArrowRight className="size-5" />
            </Button>
            <div>
              <h1 className="text-xl font-semibold">ثبت پذیرش جدید</h1>
              <p className="mt-1 text-xs text-white/70">اطلاعات مشتری، خدمات، مواد مصرفی و پرداخت را وارد کنید.</p>
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-7 w-[92%] pb-10 sm:w-[84%] sm:max-w-[1200px]">
        <form onSubmit={submitReception} className="space-y-4">
          {currentOptionsError && (
            <p className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
              {currentOptionsError}
            </p>
          )}
          {formError && (
            <p className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm leading-6 text-red-700" role="alert">
              {formError}
              {createdReceptionId && (
                <Link href="/receptions" className="ms-2 font-semibold underline">
                  بازگشت به فهرست پذیرش‌ها
                </Link>
              )}
            </p>
          )}

          <InfoCard title="اطلاعات پذیرش">
            <div className="grid gap-4 md:grid-cols-2">
              <Field label="تعرفه پذیرش">
                <FormSelect
                  ariaLabel="انتخاب تعرفه پذیرش"
                  value={serviceTariffId ? String(serviceTariffId) : ""}
                  onChange={(value) => {
                    const selectedId = value ? Number(value) : null;
                    setServiceTariffId(selectedId);
                    setServices((current) =>
                      current.map((service) => ({
                        ...service,
                        serviceId: null,
                        serviceTariffId: selectedId,
                        unitPrice: 0,
                        materials: [],
                      })),
                    );
                  }}
                  disabled={currentOptionsLoading || Boolean(createdReceptionId)}
                >
                  <option value="">انتخاب تعرفه</option>
                  {options?.tariffs.map((tariff) => (
                    <option key={tariff.id} value={tariff.id}>{tariff.name}</option>
                  ))}
                </FormSelect>
              </Field>
              <Field label="جستجوی مشتری">
                <Input
                  value={customerQuery}
                  onChange={(event) => {
                    setCustomerQuery(event.target.value);
                    setCustomerId(null);
                  }}
                  placeholder="نام مشتری را جستجو کنید"
                  className="h-10 rounded-lg border-slate-200 text-sm"
                  disabled={!salonId}
                />
              </Field>
              <Field label="مشتری">
                <FormSelect
                  ariaLabel="انتخاب مشتری"
                  value={customerId ? String(customerId) : ""}
                  onChange={(value) => setCustomerId(value ? Number(value) : null)}
                  disabled={!salonId || customerLoading || customers.length === 0}
                >
                  <option value="">
                    {customerLoading ? "در حال دریافت مشتریان..." : "انتخاب مشتری"}
                  </option>
                  {customers.map((customer) => (
                    <option key={customer.id} value={customer.id}>{customer.fullName}</option>
                  ))}
                </FormSelect>
                {customerError && <p className="text-xs text-destructive" role="alert">{customerError}</p>}
              </Field>
              <Field label="پذیرشگر">
                <FormSelect
                  ariaLabel="انتخاب پذیرشگر"
                  value={receptionistId ? String(receptionistId) : ""}
                  onChange={(value) => setReceptionistId(value ? Number(value) : null)}
                  disabled={currentOptionsLoading || !options?.personnels.length}
                >
                  <option value="">انتخاب پرسنل پذیرشگر</option>
                  {options?.personnels.map((personnel) => (
                    <option key={personnel.id} value={personnel.id}>
                      {personnel.fullName} - {personnel.jobTitle}
                    </option>
                  ))}
                </FormSelect>
              </Field>
              <Field label="یادداشت">
                <Input
                  value={notes}
                  onChange={(event) => setNotes(event.target.value)}
                  maxLength={2000}
                  placeholder="یادداشت پذیرش (اختیاری)"
                  className="h-10 rounded-lg border-slate-200 text-sm"
                />
              </Field>
            </div>
          </InfoCard>

          <InfoCard
            title="خدمات"
            action={
              <Button type="button" variant="outline" size="sm" onClick={addService} disabled={Boolean(createdReceptionId) || !serviceTariffId}>
                <Plus className="size-4" />
                افزودن خدمت
              </Button>
            }
          >
            {currentOptionsLoading ? (
              <p className="text-sm text-slate-500">در حال دریافت خدمات و تعرفه‌ها...</p>
            ) : !serviceTariffId ? (
              <p className="text-sm text-slate-500">برای نمایش خدمات، ابتدا تعرفهٔ پذیرش را انتخاب کنید.</p>
            ) : serviceOptions.length === 0 ? (
              <p className="text-sm text-amber-700">خدمت فعال با تعرفه معتبر برای این سالن پیدا نشد.</p>
            ) : (
              <div className="space-y-4">
                {services.map((service, index) => (
                  <div key={service.key} className="rounded-lg border border-slate-100 bg-slate-50/60 p-3">
                    <div className="mb-3 flex items-center justify-between gap-2">
                      <span className="text-xs font-semibold text-slate-700">خدمت {new Intl.NumberFormat("fa-IR").format(index + 1)}</span>
                      {services.length > 1 && (
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon-sm"
                          aria-label={`حذف خدمت ${index + 1}`}
                          onClick={() => setServices((current) => current.filter((item) => item.key !== service.key))}
                          disabled={Boolean(createdReceptionId)}
                          className="text-rose-600 hover:bg-rose-50 hover:text-rose-700"
                        >
                          <Trash2 className="size-4" />
                        </Button>
                      )}
                    </div>
                    <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
                      <Field label="خدمت و تعرفه">
                        <FormSelect
                          ariaLabel={`انتخاب خدمت ${index + 1}`}
                          value={service.serviceId && service.serviceTariffId ? `${service.serviceTariffId}:${service.serviceId}` : ""}
                          onChange={(value) => chooseService(service.key, value)}
                          disabled={Boolean(createdReceptionId)}
                        >
                          <option value="">انتخاب خدمت</option>
                          {serviceOptions.map((option) => (
                            <option key={option.id} value={option.id}>
                              {option.name} ({amountLabel(option.amount)})
                            </option>
                          ))}
                        </FormSelect>
                      </Field>
                      <Field label="پرسنل ارائه‌دهنده">
                        <FormSelect
                          ariaLabel={`انتخاب پرسنل خدمت ${index + 1}`}
                          value={service.personnelId ? String(service.personnelId) : ""}
                          onChange={(value) => updateService(service.key, (current) => ({
                            ...current,
                            personnelId: value ? Number(value) : null,
                          }))}
                          disabled={Boolean(createdReceptionId)}
                        >
                          <option value="">بدون انتخاب</option>
                          {options?.personnels.map((personnel) => (
                            <option key={personnel.id} value={personnel.id}>{personnel.fullName}</option>
                          ))}
                        </FormSelect>
                      </Field>
                      <Field label="تعداد">
                        <Input
                          type="number"
                          min="0.01"
                          step="0.01"
                          value={service.quantity}
                          onChange={(event) => updateService(service.key, (current) => ({
                            ...current,
                            quantity: Number(event.target.value),
                          }))}
                          className="h-10 rounded-lg border-slate-200"
                          disabled={Boolean(createdReceptionId)}
                        />
                      </Field>
                      <Field label="تخفیف">
                        <Input
                          type="number"
                          min="0"
                          step="0.01"
                          value={service.discountAmount}
                          onChange={(event) => updateService(service.key, (current) => ({
                            ...current,
                            discountAmount: Number(event.target.value),
                          }))}
                          className="h-10 rounded-lg border-slate-200"
                          disabled={Boolean(createdReceptionId)}
                        />
                      </Field>
                    </div>

                    <div className="mt-3 rounded-lg bg-white p-3">
                      <div className="mb-3 flex items-center justify-between gap-2">
                        <h3 className="text-xs font-semibold text-slate-700">مواد مصرفی این خدمت</h3>
                        <Button
                          type="button"
                          variant="outline"
                          size="sm"
                          onClick={() => updateService(service.key, (current) => ({
                            ...current,
                            materials: [...current.materials, createMaterialDraft(`material-${nextKey.current++}`)],
                          }))}
                          disabled={Boolean(createdReceptionId) || !options?.materials.length}
                        >
                          <Plus className="size-3.5" />
                          افزودن ماده
                        </Button>
                      </div>
                      {service.materials.length === 0 ? (
                        <p className="text-xs text-slate-400">برای این خدمت ماده‌ای ثبت نشده است.</p>
                      ) : (
                        <div className="space-y-2">
                          {service.materials.map((material, materialIndex) => (
                            <div key={material.key} className="grid items-end gap-2 sm:grid-cols-[minmax(0,2fr)_minmax(100px,1fr)_minmax(0,2fr)_auto]">
                              <Field label={`ماده ${materialIndex + 1}`}>
                                <FormSelect
                                  ariaLabel={`انتخاب ماده مصرفی ${materialIndex + 1}`}
                                  value={material.materialId ? String(material.materialId) : ""}
                                  onChange={(value) => updateService(service.key, (current) => ({
                                    ...current,
                                    materials: current.materials.map((item) =>
                                      item.key === material.key
                                        ? { ...item, materialId: value ? Number(value) : null }
                                        : item,
                                    ),
                                  }))}
                                  disabled={Boolean(createdReceptionId)}
                                >
                                  <option value="">انتخاب ماده</option>
                                  {options?.materials.map((option) => (
                                    <option key={option.id} value={option.id}>
                                      {option.name} - موجودی {new Intl.NumberFormat("fa-IR").format(option.currentStock)} {option.baseUnitSymbol}
                                    </option>
                                  ))}
                                </FormSelect>
                              </Field>
                              <Field label="مقدار">
                                <Input
                                  type="number"
                                  min="0.001"
                                  step="0.001"
                                  value={material.quantity}
                                  onChange={(event) => updateService(service.key, (current) => ({
                                    ...current,
                                    materials: current.materials.map((item) =>
                                      item.key === material.key
                                        ? { ...item, quantity: Number(event.target.value) }
                                        : item,
                                    ),
                                  }))}
                                  className="h-10 rounded-lg border-slate-200"
                                  disabled={Boolean(createdReceptionId)}
                                />
                              </Field>
                              <Field label="توضیحات">
                                <Input
                                  value={material.description}
                                  onChange={(event) => updateService(service.key, (current) => ({
                                    ...current,
                                    materials: current.materials.map((item) =>
                                      item.key === material.key
                                        ? { ...item, description: event.target.value }
                                        : item,
                                    ),
                                  }))}
                                  maxLength={1000}
                                  className="h-10 rounded-lg border-slate-200"
                                  disabled={Boolean(createdReceptionId)}
                                />
                              </Field>
                              <Button
                                type="button"
                                variant="ghost"
                                size="icon"
                                aria-label={`حذف ماده مصرفی ${materialIndex + 1}`}
                                onClick={() => updateService(service.key, (current) => ({
                                  ...current,
                                  materials: current.materials.filter((item) => item.key !== material.key),
                                }))}
                                disabled={Boolean(createdReceptionId)}
                                className="text-rose-600 hover:bg-rose-50"
                              >
                                <Trash2 className="size-4" />
                              </Button>
                            </div>
                          ))}
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </InfoCard>

          <InfoCard
            title="پرداخت‌ها"
            action={
              <Button type="button" variant="outline" size="sm" onClick={addPayment} disabled={Boolean(createdReceptionId) || totalAmount <= 0}>
                <Plus className="size-4" />
                افزودن پرداخت
              </Button>
            }
          >
            {payments.length === 0 ? (
              <p className="text-sm text-slate-500">در صورت دریافت وجه، پرداخت را ثبت کنید. پرداخت می‌تواند جزئی باشد.</p>
            ) : (
              <div className="space-y-3">
                {payments.map((payment, index) => (
                  <div key={payment.key} className="grid items-end gap-3 rounded-lg border border-slate-100 p-3 sm:grid-cols-2 lg:grid-cols-4">
                    <Field label={`مبلغ پرداخت ${index + 1}`}>
                      <Input
                        type="number"
                        min="0.01"
                        step="0.01"
                        value={payment.amount}
                        onChange={(event) => setPayments((current) => current.map((item) =>
                          item.key === payment.key ? { ...item, amount: Number(event.target.value) } : item,
                        ))}
                        className="h-10 rounded-lg border-slate-200"
                        disabled={Boolean(createdReceptionId)}
                      />
                    </Field>
                    <Field label="روش پرداخت">
                      <FormSelect
                        ariaLabel={`روش پرداخت ${index + 1}`}
                        value={payment.paymentMethod}
                        onChange={(value) => setPayments((current) => current.map((item) =>
                          item.key === payment.key ? { ...item, paymentMethod: value } : item,
                        ))}
                        disabled={Boolean(createdReceptionId)}
                      >
                        <option value="کارت‌خوان">کارت‌خوان</option>
                        <option value="نقدی">نقدی</option>
                        <option value="انتقال بانکی">انتقال بانکی</option>
                      </FormSelect>
                    </Field>
                    <Field label="شماره پیگیری">
                      <Input
                        value={payment.transactionReference}
                        onChange={(event) => setPayments((current) => current.map((item) =>
                          item.key === payment.key ? { ...item, transactionReference: event.target.value } : item,
                        ))}
                        maxLength={150}
                        className="h-10 rounded-lg border-slate-200"
                        disabled={Boolean(createdReceptionId)}
                      />
                    </Field>
                    <div className="flex items-end gap-2">
                      <Field label="توضیحات">
                        <Input
                          value={payment.description}
                          onChange={(event) => setPayments((current) => current.map((item) =>
                            item.key === payment.key ? { ...item, description: event.target.value } : item,
                          ))}
                          maxLength={1000}
                          className="h-10 rounded-lg border-slate-200"
                          disabled={Boolean(createdReceptionId)}
                        />
                      </Field>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        aria-label={`حذف پرداخت ${index + 1}`}
                        onClick={() => setPayments((current) => current.filter((item) => item.key !== payment.key))}
                        disabled={Boolean(createdReceptionId)}
                        className="shrink-0 text-rose-600 hover:bg-rose-50"
                      >
                        <Trash2 className="size-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </InfoCard>

          <div className="flex flex-col-reverse items-stretch justify-between gap-4 rounded-xl border border-fuchsia-100 bg-white p-4 shadow-sm sm:flex-row sm:items-center">
            <div className="text-sm text-slate-600">
              <div>مبلغ کل: <strong className="text-slate-900">{amountLabel(totalAmount)}</strong></div>
              <div className="mt-1">مانده: <strong className="text-fuchsia-800">{amountLabel(remainingAmount)}</strong></div>
            </div>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
              <Button
                type="button"
                variant="outline"
                render={<Link href="/receptions" />}
                disabled={submitting}
                className="border-slate-300"
              >
                انصراف
              </Button>
              <Button
                type="submit"
                disabled={submitting || currentOptionsLoading || !salonId || Boolean(currentOptionsError) || createdReceptionId !== null}
                className="bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              >
                {submitting && <LoaderCircle className="size-4 animate-spin" />}
                {submitting ? "در حال ثبت پذیرش..." : "ثبت پذیرش"}
              </Button>
            </div>
          </div>
        </form>
      </main>
    </div>
  );
}
