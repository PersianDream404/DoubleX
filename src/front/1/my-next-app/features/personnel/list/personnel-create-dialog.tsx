"use client";

import Image from "next/image";
import { useEffect, useState, type ReactNode } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { toGregorian } from "jalaali-js";
import {
  BadgeDollarSign,
  BriefcaseBusiness,
  CalendarDays,
  Check,
  ContactRound,
  HeartPulse,
  ImagePlus,
  MapPin,
  Phone,
  Trash2,
  UserRound,
  X,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { ApiError } from "@/lib/api/api-client";
import { handleApiError } from "@/lib/api/handle-api-error";

import { SalonService } from "@/features/salons/list/salon.service";
import type { GetSelectListSalonResponse } from "@/features/salons/list/salon.types";

import {
  PersonnelFileService,
  PersonnelUploadCategory,
  getPersonnelFileUrl,
} from "./personnel-file.service";
import { PersonnelCreateSchema } from "./personnel-create.schema";
import { PersonnelService } from "./personnel.service";
import type {
  CreatePersonnelRequest,
  PersonnelDocumentRequest,
  PersonnelDocumentType,
} from "./personnel.types";

type CreateTab = "personal" | "job" | "financial" | "health" | "guarantor";
type ImageField =
  | "profile"
  | "training"
  | "healthCourse"
  | "healthTest"
  | "guarantor";
type SelectedImage = { file: File; previewUrl: string };
type FormValues = {
  salonId: number;
  fullName: string;
  nationalCode: string;
  birthDate: string;
  maritalStatus: string;
  childrenCount: string;
  mobilePhone: string;
  landlinePhone: string;
  addressText: string;
  isCloseToSalon: boolean;
  jobTitle: string;
  personnelCode: string;
  cooperationStartDate: string;
  workingShiftType: string;
  workHistory: string;
  salaryType: string;
  cardNumber: string;
  accountNumber: string;
  healthStatus: string;
  diseases: string;
  allergyStatus: string;
  allergies: string;
  guarantorName: string;
  relationship: string;
  guarantorPhone: string;
  guarantorLandlinePhone: string;
  status: string;
};
type ImageFiles = Record<ImageField, SelectedImage[]>;

const TABS: { id: CreateTab; label: string; icon: typeof UserRound }[] = [
  { id: "personal", label: "اطلاعات شخصی", icon: UserRound },
  { id: "job", label: "اطلاعات شغلی", icon: BriefcaseBusiness },
  { id: "financial", label: "اطلاعات مالی", icon: BadgeDollarSign },
  { id: "health", label: "اطلاعات سلامتی", icon: HeartPulse },
  { id: "guarantor", label: "اطلاعات حقوقی", icon: ContactRound },
];

const EMPTY_FORM: FormValues = {
  salonId: 0,
  fullName: "",
  nationalCode: "",
  birthDate: "",
  maritalStatus: "0",
  childrenCount: "0",
  mobilePhone: "",
  landlinePhone: "",
  addressText: "",
  isCloseToSalon: false,
  jobTitle: "",
  personnelCode: "",
  cooperationStartDate: "",
  workingShiftType: "1",
  workHistory: "",
  salaryType: "0",
  cardNumber: "",
  accountNumber: "",
  healthStatus: "false",
  diseases: "",
  allergyStatus: "false",
  allergies: "",
  guarantorName: "",
  relationship: "",
  guarantorPhone: "",
  guarantorLandlinePhone: "",
  status: "true",
};

const EMPTY_FILES: ImageFiles = {
  profile: [],
  training: [],
  healthCourse: [],
  healthTest: [],
  guarantor: [],
};

const MAX_IMAGE_SIZE = 5 * 1024 * 1024;
const ALLOWED_IMAGE_TYPES = new Set(["image/jpeg", "image/png", "image/webp"]);

const ERROR_TAB: Record<string, CreateTab> = {
  salonId: "personal",
  fullName: "personal",
  nationalCode: "personal",
  birthDate: "personal",
  maritalStatus: "personal",
  childrenCount: "personal",
  mobilePhone: "personal",
  landlinePhone: "personal",
  addressText: "personal",
  jobTitle: "job",
  personnelCode: "job",
  cooperationStartDate: "job",
  workingShiftType: "job",
  workHistory: "job",
  salaryType: "financial",
  cardNumber: "financial",
  accountNumber: "financial",
  healthStatus: "health",
  diseases: "health",
  allergyStatus: "health",
  allergies: "health",
  guarantorName: "guarantor",
  relationship: "guarantor",
  guarantorPhone: "guarantor",
  guarantorLandlinePhone: "guarantor",
};

function normalizeDigits(value: string) {
  return value
    .replace(/[۰-۹]/g, (digit) =>
      String("۰۱۲۳۴۵۶۷۸۹".indexOf(digit)),
    )
    .replace(/[٠-٩]/g, (digit) =>
      String("٠١٢٣٤٥٦٧٨٩".indexOf(digit)),
    );
}

function dateToApi(value: string) {
  const [year, month, day] = normalizeDigits(value).split("/").map(Number);
  const gregorian = toGregorian(year, month, day);
  return `${gregorian.gy}-${String(gregorian.gm).padStart(2, "0")}-${String(
    gregorian.gd,
  ).padStart(2, "0")}`;
}

function toPersianDigits(value: string) {
  return value.replace(/\d/g, (digit) => "۰۱۲۳۴۵۶۷۸۹"[Number(digit)]);
}

function JalaliDatePicker({
  name,
  value,
  error,
  onChange,
}: {
  name: string;
  value: string;
  error?: string;
  onChange: (value: string) => void;
}) {
  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      digits={["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]}
      format="YYYY/MM/DD"
      value={value || null}
      editable={false}
      calendarPosition="bottom-start"
      portal
      zIndex={150}
      onChange={(date: DateObject | null) =>
        onChange(date?.format("YYYY/MM/DD") ?? "")
      }
      render={(_displayValue, openCalendar) => (
        <div className="relative w-full">
          <Input
            id={`personnel-create-${name}`}
            name={name}
            type="text"
            readOnly
            dir="ltr"
            inputMode="none"
            placeholder="۱۴۰۰/۰۱/۰۱"
            value={toPersianDigits(value)}
            aria-invalid={Boolean(error)}
            aria-describedby={
              error ? `personnel-create-${name}-error` : undefined
            }
            aria-haspopup="dialog"
            className="h-10 cursor-pointer rounded-lg border-slate-200 bg-white pe-10 text-sm shadow-none focus-visible:border-fuchsia-500 focus-visible:ring-fuchsia-500/20"
            onClick={openCalendar}
            onKeyDown={(event) => {
              if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                openCalendar();
              }
            }}
          />
          <CalendarDays
            aria-hidden="true"
            className="pointer-events-none absolute end-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
          />
        </div>
      )}
    />
  );
}

function FormField({
  label,
  name,
  error,
  required = false,
  children,
}: {
  label: string;
  name: string;
  error?: string;
  required?: boolean;
  children: ReactNode;
}) {
  const id = `personnel-create-${name}`;

  return (
    <div className="min-w-0 space-y-1.5">
      <label htmlFor={id} className="block text-xs text-slate-600">
        {label}
        {required && <span className="ms-1 text-rose-600">*</span>}
      </label>
      {children}
      {error && (
        <p id={`${id}-error`} className="text-xs text-destructive" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}

function FormInput({
  name,
  error,
  dir,
  ...props
}: React.ComponentProps<typeof Input> & {
  name: string;
  error?: string;
  dir?: "ltr" | "rtl" | "auto";
}) {
  return (
    <Input
      id={`personnel-create-${name}`}
      aria-invalid={Boolean(error)}
      aria-describedby={error ? `personnel-create-${name}-error` : undefined}
      dir={dir}
      className="h-10 rounded-lg border-slate-200 bg-white text-sm shadow-none focus-visible:border-fuchsia-500 focus-visible:ring-fuchsia-500/20"
      {...props}
    />
  );
}

function FormSelect({
  name,
  error,
  children,
  ...props
}: React.ComponentProps<"select"> & {
  name: string;
  error?: string;
  children: ReactNode;
}) {
  return (
    <select
      id={`personnel-create-${name}`}
      aria-invalid={Boolean(error)}
      aria-describedby={error ? `personnel-create-${name}-error` : undefined}
      className="h-10 w-full min-w-0 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-800 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 aria-invalid:border-destructive"
      {...props}
    >
      {children}
    </select>
  );
}

function InfoCard({
  title,
  icon: Icon,
  action,
  children,
}: {
  title: string;
  icon?: typeof UserRound;
  action?: ReactNode;
  children: ReactNode;
}) {
  return (
    <section className="min-w-0 rounded-lg border border-slate-100 bg-white p-3">
      <h3 className="mb-3 flex min-h-5 items-center justify-between gap-2 text-sm font-semibold text-slate-800">
        <span className="flex items-center gap-2">
          {Icon && (
            <Icon aria-hidden="true" className="size-4 text-slate-500" />
          )}
          {title}
        </span>
        {action}
      </h3>
      {children}
    </section>
  );
}

function ImagePicker({
  label,
  name,
  files,
  onChange,
  multiple = true,
  onValidationError,
}: {
  label: string;
  name: string;
  files: SelectedImage[];
  onChange: (files: SelectedImage[]) => void;
  multiple?: boolean;
  onValidationError: (message: string | null) => void;
}) {
  const inputId = `personnel-create-${name}-files`;

  function selectFiles(event: React.ChangeEvent<HTMLInputElement>) {
    const selected = Array.from(event.currentTarget.files ?? []);
    event.currentTarget.value = "";

    const invalid = selected.find(
      (file) =>
        !ALLOWED_IMAGE_TYPES.has(file.type) || file.size > MAX_IMAGE_SIZE,
    );

    if (invalid) {
      onValidationError(
        !ALLOWED_IMAGE_TYPES.has(invalid.type)
          ? "فقط تصویر JPG، PNG یا WEBP قابل انتخاب است."
          : "حجم هر تصویر باید حداکثر ۵ مگابایت باشد.",
      );
      return;
    }

    onValidationError(null);
    const nextFiles = multiple ? selected : selected.slice(0, 1);
    const additions = nextFiles.map((file) => ({
      file,
      previewUrl: URL.createObjectURL(file),
    }));
    if (!multiple) {
      files.forEach((file) => URL.revokeObjectURL(file.previewUrl));
    }
    onChange(multiple ? [...files, ...additions] : additions);
  }

  function removeFile(index: number) {
    const file = files[index];
    if (file) {
      URL.revokeObjectURL(file.previewUrl);
    }
    onChange(files.filter((_, fileIndex) => fileIndex !== index));
  }

  return (
    <InfoCard title={label}>
      <div className="grid grid-cols-1 gap-2">
        {files.map(({ file, previewUrl }, index) => (
          <div
            key={`${file.name}-${file.lastModified}-${index}`}
            className="relative aspect-[1.1] overflow-hidden rounded-lg bg-fuchsia-50"
          >
            <Image
              src={previewUrl}
              alt={`پیش‌نمایش ${file.name}`}
              fill
              unoptimized
              className="object-cover"
              sizes="180px"
            />
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              aria-label={`حذف تصویر ${file.name}`}
              onClick={() => removeFile(index)}
              className="absolute bottom-2 start-2 size-8 rounded-full bg-white text-rose-600 hover:bg-rose-50"
            >
              <Trash2 className="size-4" />
            </Button>
          </div>
        ))}
        {files.length === 0 && (
          <div className="flex aspect-[1.1] items-center justify-center rounded-lg bg-fuchsia-50 text-fuchsia-300">
            <ImagePlus aria-hidden="true" className="size-8" />
          </div>
        )}
      </div>
      <div className="mt-2 flex justify-center">
        <label
          htmlFor={inputId}
          className="inline-flex h-8 cursor-pointer items-center rounded-lg border border-fuchsia-300 px-3 text-xs text-fuchsia-700 transition-colors hover:bg-fuchsia-50 focus-within:outline-2 focus-within:outline-fuchsia-600"
        >
          انتخاب فایل
        </label>
        <input
          id={inputId}
          type="file"
          accept="image/jpeg,image/png,image/webp"
          multiple={multiple}
          className="sr-only"
          onChange={selectFiles}
        />
      </div>
    </InfoCard>
  );
}

function makeRequest(
  values: FormValues,
  documents: PersonnelDocumentRequest[],
): CreatePersonnelRequest {
  return {
    salonId: values.salonId,
    status: values.status === "true",
    personalInfo: {
      fullName: values.fullName.trim(),
      nationalCode: normalizeDigits(values.nationalCode),
      birthDate: dateToApi(values.birthDate),
      maritalStatus: values.maritalStatus === "1" ? 1 : 0,
      childrenCount: Number(values.childrenCount),
      mobilePhone: normalizeDigits(values.mobilePhone),
      landlinePhone: normalizeDigits(values.landlinePhone) || null,
      addressText: values.addressText.trim(),
      isCloseToSalon: values.isCloseToSalon,
      pictureUrl:
        documents.find((document) => document.documentType === 0)?.url ?? null,
    },
    jobInfo: {
      jobTitle: values.jobTitle.trim(),
      personnelCode: values.personnelCode.trim(),
      cooperationStartDate: dateToApi(values.cooperationStartDate),
      workingShiftType: Number(values.workingShiftType),
      jobPictures: documents
        .filter((document) => document.documentType === 1)
        .map((document) => document.url),
      workHistory: values.workHistory.trim() || null,
    },
    financialInfo: {
      salaryType: Number(values.salaryType),
      cardNumber: normalizeDigits(values.cardNumber),
      accountNumber: normalizeDigits(values.accountNumber),
    },
    healthInfo: {
      healthStatus: values.healthStatus,
      allergyStatus: values.allergyStatus,
      diseases: values.diseases.trim() || null,
      allergies: values.allergies.trim() || null,
    },
    guarantorInfo: {
      guarantorName: values.guarantorName.trim(),
      relationship: values.relationship.trim(),
      guarantorPhone: normalizeDigits(values.guarantorPhone),
      guarantorLandlinePhone: normalizeDigits(
        values.guarantorLandlinePhone,
      ),
    },
    documents,
  };
}

export function PersonnelCreateDialog({
  open,
  onOpenChange,
  onCreated,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCreated: () => void;
}) {
  const [activeTab, setActiveTab] = useState<CreateTab>("personal");
  const [values, setValues] = useState<FormValues>(EMPTY_FORM);
  const [files, setFiles] = useState<ImageFiles>(EMPTY_FILES);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [fileError, setFileError] = useState<string | null>(null);
  const [salons, setSalons] = useState<GetSelectListSalonResponse[]>([]);
  const [salonsLoading, setSalonsLoading] = useState(false);
  const [salonsError, setSalonsError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [uploadMessage, setUploadMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    const controller = new AbortController();
    const service = new SalonService();
    Promise.resolve()
      .then(() => {
        setSalonsLoading(true);
        setSalonsError(null);
        return service.getSelectList(
          { PageNumber: 1, PageSize: 100 },
          { signal: controller.signal },
        );
      })
      .then(async (response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست سالن‌ها دریافت نشد.");
        }

        const allSalons = [...response.data.list];
        const totalPages = response.data.pagination.totalPages;
        for (let pageNumber = 2; pageNumber <= totalPages; pageNumber += 1) {
          const pageResponse = await service.getSelectList(
            { PageNumber: pageNumber, PageSize: 100 },
            { signal: controller.signal },
          );
          if (!pageResponse.data) {
            throw new Error(pageResponse.message ?? "فهرست سالن‌ها کامل نشد.");
          }
          allSalons.push(...pageResponse.data.list);
        }

        if (controller.signal.aborted) {
          return;
        }

        const activeSalons = allSalons.filter((salon) => salon.status);
        setSalons(activeSalons);
        if (activeSalons.length === 1) {
          setValues((current) => ({ ...current, salonId: activeSalons[0].id }));
        } else if (activeSalons.length === 0) {
          setSalonsError("سالن فعالی برای ثبت پرسنل یافت نشد.");
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setSalonsError(handleApiError(error));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setSalonsLoading(false);
        }
      });

    return () => controller.abort();
  }, [open]);

  function update<K extends keyof FormValues>(key: K, value: FormValues[K]) {
    setValues((current) => ({ ...current, [key]: value }));
    setFieldErrors((current) => {
      if (!(key in current)) {
        return current;
      }
      const next = { ...current };
      delete next[key];
      return next;
    });
    setFormError(null);
  }

  function updateFiles(field: ImageField, nextFiles: SelectedImage[]) {
    setFiles((current) => ({ ...current, [field]: nextFiles }));
  }

  function clearFiles(currentFiles: ImageFiles) {
    Object.values(currentFiles)
      .flat()
      .forEach((file) => URL.revokeObjectURL(file.previewUrl));
  }

  function closeDialog() {
    clearFiles(files);
    setValues(EMPTY_FORM);
    setFiles(EMPTY_FILES);
    setFieldErrors({});
    setFormError(null);
    setFileError(null);
    setUploadMessage(null);
    setActiveTab("personal");
    onOpenChange(false);
  }

  function filesForUpload() {
    return [
      ...files.profile.map((file) => ({
        selected: file,
        field: "profile" as const,
        category: PersonnelUploadCategory.photo,
        documentType: 0 as PersonnelDocumentType,
      })),
      ...files.training.map((file) => ({
        selected: file,
        field: "training" as const,
        category: PersonnelUploadCategory.training,
        documentType: 1 as PersonnelDocumentType,
      })),
      ...files.healthCourse.map((file) => ({
        selected: file,
        field: "healthCourse" as const,
        category: PersonnelUploadCategory.healthCourse,
        documentType: 4 as PersonnelDocumentType,
      })),
      ...files.healthTest.map((file) => ({
        selected: file,
        field: "healthTest" as const,
        category: PersonnelUploadCategory.healthTest,
        documentType: 5 as PersonnelDocumentType,
      })),
      ...files.guarantor.map((file) => ({
        selected: file,
        field: "guarantor" as const,
        category: PersonnelUploadCategory.guarantor,
        documentType: 3 as PersonnelDocumentType,
      })),
    ];
  }

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting) {
      return;
    }

    const parsed = PersonnelCreateSchema.safeParse({
      ...values,
      salonId: values.salonId,
    });

    if (!parsed.success) {
      const errors: Record<string, string> = {};
      for (const issue of parsed.error.issues) {
        const key = String(issue.path[0] ?? "form");
        errors[key] ??= issue.message;
      }
      setFieldErrors(errors);
      setActiveTab(ERROR_TAB[Object.keys(errors)[0]] ?? "personal");
      setFormError("لطفاً خطاهای فرم را در تب مشخص‌شده اصلاح کنید.");
      return;
    }

    if (salonsLoading || salons.length === 0) {
      setFormError(
        salonsError ?? "برای ثبت پرسنل ابتدا باید یک سالن فعال انتخاب شود.",
      );
      return;
    }

    setSubmitting(true);
    setFormError(null);
    const uploadFiles = filesForUpload();
    const documents: PersonnelDocumentRequest[] = [];
    const fileService = new PersonnelFileService();
    const uploadedFileIds: number[] = [];
    let createRequestStarted = false;

    try {
      for (const [index, item] of uploadFiles.entries()) {
        setUploadMessage(
          `در حال بارگذاری تصویر ${index + 1} از ${uploadFiles.length}...`,
        );
        const response = await fileService.upload(
          item.selected.file,
          item.category,
        );
        if (!response.data) {
          throw new Error(
            response.message ?? `بارگذاری ${item.selected.file.name} ناموفق بود.`,
          );
        }

        uploadedFileIds.push(response.data.id);
        if (!response.data.path) {
          throw new Error(
            response.message ?? `مسیر فایل ${item.selected.file.name} دریافت نشد.`,
          );
        }
        documents.push({
          documentType: item.documentType,
          fileName: item.selected.file.name,
          url: getPersonnelFileUrl(response.data.path),
        });
      }

      setUploadMessage("در حال ثبت اطلاعات پرسنل...");
      const request = makeRequest(values, documents);
      createRequestStarted = true;
      const createResponse = await new PersonnelService().create(request);
      if (createResponse.statusCode < 200 || createResponse.statusCode >= 300) {
        throw new Error(
          createResponse.message ?? "ثبت اطلاعات پرسنل انجام نشد.",
        );
      }

      onCreated();
      closeDialog();
    } catch (error: unknown) {
      const failureMessages = [handleApiError(error)];
      const safeToDeleteFiles =
        !createRequestStarted ||
        (error instanceof ApiError &&
          error.statusCode >= 400 &&
          error.statusCode < 500);
      if (safeToDeleteFiles) {
        for (const fileId of uploadedFileIds) {
          try {
            await fileService.delete(fileId);
          } catch (cleanupError: unknown) {
            failureMessages.push(
              `پاک‌سازی فایل موقت نیز ناموفق بود: ${handleApiError(cleanupError)}`,
            );
          }
        }
      }
      setFormError(failureMessages.join(" "));
    } finally {
      setSubmitting(false);
      setUploadMessage(null);
    }
  }

  function setImageError(message: string | null) {
    setFileError(message);
  }

  function renderTab() {
    switch (activeTab) {
      case "personal":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.65fr)_minmax(0,1.28fr)_minmax(0,1fr)]">
            <div className="grid gap-3">
              <InfoCard
                title="اطلاعات فردی"
                icon={UserRound}
                action={
                  <select
                    aria-label="وضعیت پرسنل"
                    value={values.status}
                    onChange={(event) => update("status", event.target.value)}
                    className="h-7 rounded-md border border-slate-200 bg-white px-2 text-xs outline-none focus-visible:border-fuchsia-500"
                  >
                    <option value="true">فعال</option>
                    <option value="false">غیرفعال</option>
                  </select>
                }
              >
                <div className="grid gap-x-4 gap-y-3 sm:grid-cols-2">
                  <FormField
                    label="نام و نام خانوادگی"
                    name="fullName"
                    required
                    error={fieldErrors.fullName}
                  >
                    <FormInput
                      name="fullName"
                      value={values.fullName}
                      error={fieldErrors.fullName}
                      onChange={(event) => update("fullName", event.target.value)}
                    />
                  </FormField>
                  <FormField
                    label="کد ملی"
                    name="nationalCode"
                    required
                    error={fieldErrors.nationalCode}
                  >
                    <FormInput
                      name="nationalCode"
                      inputMode="numeric"
                      dir="ltr"
                      value={values.nationalCode}
                      error={fieldErrors.nationalCode}
                      onChange={(event) =>
                        update("nationalCode", normalizeDigits(event.target.value))
                      }
                    />
                  </FormField>
                  <FormField
                    label="تاریخ تولد"
                    name="birthDate"
                    required
                    error={fieldErrors.birthDate}
                  >
                    <JalaliDatePicker
                      name="birthDate"
                      value={values.birthDate}
                      error={fieldErrors.birthDate}
                      onChange={(value) => update("birthDate", value)}
                    />
                  </FormField>
                  <div className="grid grid-cols-2 gap-2">
                    <FormField
                      label="وضعیت تأهل"
                      name="maritalStatus"
                      error={fieldErrors.maritalStatus}
                    >
                      <FormSelect
                        name="maritalStatus"
                        value={values.maritalStatus}
                        error={fieldErrors.maritalStatus}
                        onChange={(event) =>
                          update("maritalStatus", event.target.value)
                        }
                      >
                        <option value="0">مجرد</option>
                        <option value="1">متأهل</option>
                      </FormSelect>
                    </FormField>
                    <FormField
                      label="تعداد فرزند"
                      name="childrenCount"
                      error={fieldErrors.childrenCount}
                    >
                      <FormInput
                        name="childrenCount"
                        inputMode="numeric"
                        dir="ltr"
                        value={values.childrenCount}
                        error={fieldErrors.childrenCount}
                        onChange={(event) =>
                          update(
                            "childrenCount",
                            normalizeDigits(event.target.value),
                          )
                        }
                      />
                    </FormField>
                  </div>
                </div>
              </InfoCard>

              <InfoCard title="آدرس" icon={MapPin}>
                <FormField
                  label="آدرس"
                  name="addressText"
                  required
                  error={fieldErrors.addressText}
                >
                  <textarea
                    id="personnel-create-addressText"
                    value={values.addressText}
                    aria-invalid={Boolean(fieldErrors.addressText)}
                    aria-describedby={
                      fieldErrors.addressText
                        ? "personnel-create-addressText-error"
                        : undefined
                    }
                    onChange={(event) =>
                      update("addressText", event.target.value)
                    }
                    rows={2}
                    className="w-full resize-y rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm leading-6 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 aria-invalid:border-destructive"
                  />
                </FormField>
                <label className="mt-3 flex w-fit cursor-pointer items-center gap-2 text-xs text-slate-600">
                  <input
                    type="checkbox"
                    checked={values.isCloseToSalon}
                    onChange={(event) =>
                      update("isCloseToSalon", event.target.checked)
                    }
                    className="size-4 accent-fuchsia-700"
                  />
                  نزدیک محل کار
                </label>
              </InfoCard>
            </div>

            <InfoCard title="شماره تماس پرسنل" icon={Phone}>
              <div className="grid gap-3">
                <FormField
                  label="تلفن همراه"
                  name="mobilePhone"
                  required
                  error={fieldErrors.mobilePhone}
                >
                  <FormInput
                    name="mobilePhone"
                    dir="ltr"
                    inputMode="tel"
                    value={values.mobilePhone}
                    error={fieldErrors.mobilePhone}
                    onChange={(event) =>
                      update("mobilePhone", normalizeDigits(event.target.value))
                    }
                  />
                </FormField>
                <FormField
                  label="تلفن ثابت"
                  name="landlinePhone"
                  error={fieldErrors.landlinePhone}
                >
                  <FormInput
                    name="landlinePhone"
                    dir="ltr"
                    inputMode="tel"
                    value={values.landlinePhone}
                    error={fieldErrors.landlinePhone}
                    onChange={(event) =>
                      update(
                        "landlinePhone",
                        normalizeDigits(event.target.value),
                      )
                    }
                  />
                </FormField>
              </div>
            </InfoCard>

            <ImagePicker
              label="تصویر پرسنل"
              name="profile"
              files={files.profile}
              multiple={false}
              onChange={(nextFiles) => updateFiles("profile", nextFiles)}
              onValidationError={setImageError}
            />
          </div>
        );

      case "job":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.9fr)_minmax(0,1fr)_minmax(0,1fr)]">
            <InfoCard title="اطلاعات شغلی" icon={BriefcaseBusiness}>
              <div className="grid gap-x-4 gap-y-3 sm:grid-cols-2">
                <FormField
                  label="عنوان شغلی"
                  name="jobTitle"
                  required
                  error={fieldErrors.jobTitle}
                >
                  <FormInput
                    name="jobTitle"
                    value={values.jobTitle}
                    error={fieldErrors.jobTitle}
                    onChange={(event) => update("jobTitle", event.target.value)}
                  />
                </FormField>
                <FormField
                  label="کد پرسنلی"
                  name="personnelCode"
                  required
                  error={fieldErrors.personnelCode}
                >
                  <FormInput
                    name="personnelCode"
                    dir="ltr"
                    value={values.personnelCode}
                    error={fieldErrors.personnelCode}
                    onChange={(event) =>
                      update("personnelCode", event.target.value)
                    }
                  />
                </FormField>
                <FormField
                  label="تاریخ شروع همکاری"
                  name="cooperationStartDate"
                  required
                  error={fieldErrors.cooperationStartDate}
                >
                  <JalaliDatePicker
                    name="cooperationStartDate"
                    value={values.cooperationStartDate}
                    error={fieldErrors.cooperationStartDate}
                    onChange={(value) => update("cooperationStartDate", value)}
                  />
                </FormField>
                <FormField
                  label="نوع همکاری / ساعت کاری"
                  name="workingShiftType"
                  required
                  error={fieldErrors.workingShiftType}
                >
                  <FormSelect
                    name="workingShiftType"
                    value={values.workingShiftType}
                    error={fieldErrors.workingShiftType}
                    onChange={(event) =>
                      update("workingShiftType", event.target.value)
                    }
                  >
                    <option value="1">پورسانتی</option>
                    <option value="2">ثابت و پورسانت</option>
                    <option value="3">توافقی</option>
                    <option value="4">اجاره‌ای</option>
                    <option value="5">تمام‌وقت</option>
                    <option value="6">نیمه‌وقت (صبح)</option>
                    <option value="7">نیمه‌وقت (عصر)</option>
                  </FormSelect>
                </FormField>
              </div>
            </InfoCard>
            <InfoCard title="سوابق کاری">
              <FormField label="سوابق" name="workHistory">
                <textarea
                  id="personnel-create-workHistory"
                  value={values.workHistory}
                  onChange={(event) =>
                    update("workHistory", event.target.value)
                  }
                  rows={5}
                  className="w-full resize-y rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm leading-6 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                />
              </FormField>
            </InfoCard>
            <ImagePicker
              label="تصاویر دوره آموزشی"
              name="training"
              files={files.training}
              onChange={(nextFiles) => updateFiles("training", nextFiles)}
              onValidationError={setImageError}
            />
          </div>
        );

      case "financial":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,1fr)_minmax(0,1.5fr)]">
            <InfoCard title="حقوق و دستمزد" icon={BadgeDollarSign}>
              <FormField
                label="نوع پرداخت"
                name="salaryType"
                required
                error={fieldErrors.salaryType}
              >
                <FormSelect
                  name="salaryType"
                  value={values.salaryType}
                  error={fieldErrors.salaryType}
                  onChange={(event) => update("salaryType", event.target.value)}
                >
                  <option value="0">ثابت</option>
                  <option value="1">پورسانتی</option>
                  <option value="2">اجاره صندلی</option>
                  <option value="3">اجاره اتاق</option>
                </FormSelect>
              </FormField>
            </InfoCard>
            <InfoCard title="اطلاعات حساب بانکی">
              <div className="grid gap-3 sm:grid-cols-2">
                <FormField
                  label="شماره کارت"
                  name="cardNumber"
                  required
                  error={fieldErrors.cardNumber}
                >
                  <FormInput
                    name="cardNumber"
                    dir="ltr"
                    inputMode="numeric"
                    maxLength={16}
                    value={values.cardNumber}
                    error={fieldErrors.cardNumber}
                    onChange={(event) =>
                      update("cardNumber", normalizeDigits(event.target.value))
                    }
                  />
                </FormField>
                <FormField
                  label="شماره حساب"
                  name="accountNumber"
                  required
                  error={fieldErrors.accountNumber}
                >
                  <FormInput
                    name="accountNumber"
                    dir="ltr"
                    inputMode="numeric"
                    value={values.accountNumber}
                    error={fieldErrors.accountNumber}
                    onChange={(event) =>
                      update(
                        "accountNumber",
                        normalizeDigits(event.target.value),
                      )
                    }
                  />
                </FormField>
              </div>
            </InfoCard>
          </div>
        );

      case "health":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.9fr)_minmax(0,1fr)_minmax(0,1fr)]">
            <InfoCard title="اطلاعات سلامتی" icon={HeartPulse}>
              <div className="grid gap-3">
                <FormField
                  label="وضعیت بیماری"
                  name="healthStatus"
                  error={fieldErrors.healthStatus}
                >
                  <FormSelect
                    name="healthStatus"
                    value={values.healthStatus}
                    error={fieldErrors.healthStatus}
                    onChange={(event) =>
                      update("healthStatus", event.target.value)
                    }
                  >
                    <option value="false">سالم</option>
                    <option value="true">بیمار است</option>
                  </FormSelect>
                </FormField>
                <FormField label="شرح بیماری" name="diseases">
                  <textarea
                    id="personnel-create-diseases"
                    value={values.diseases}
                    onChange={(event) => update("diseases", event.target.value)}
                    rows={2}
                    className="w-full resize-y rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm leading-6 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                  />
                </FormField>
                <FormField
                  label="وضعیت حساسیت"
                  name="allergyStatus"
                  error={fieldErrors.allergyStatus}
                >
                  <FormSelect
                    name="allergyStatus"
                    value={values.allergyStatus}
                    error={fieldErrors.allergyStatus}
                    onChange={(event) =>
                      update("allergyStatus", event.target.value)
                    }
                  >
                    <option value="false">بدون حساسیت</option>
                    <option value="true">حساسیت دارد</option>
                  </FormSelect>
                </FormField>
                <FormField label="شرح حساسیت" name="allergies">
                  <textarea
                    id="personnel-create-allergies"
                    value={values.allergies}
                    onChange={(event) => update("allergies", event.target.value)}
                    rows={2}
                    className="w-full resize-y rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm leading-6 outline-none focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20"
                  />
                </FormField>
              </div>
            </InfoCard>
            <ImagePicker
              label="تصاویر آزمایش‌ها"
              name="healthTest"
              files={files.healthTest}
              onChange={(nextFiles) => updateFiles("healthTest", nextFiles)}
              onValidationError={setImageError}
            />
            <ImagePicker
              label="تصاویر دوره‌ها"
              name="healthCourse"
              files={files.healthCourse}
              onChange={(nextFiles) => updateFiles("healthCourse", nextFiles)}
              onValidationError={setImageError}
            />
          </div>
        );

      case "guarantor":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)_minmax(0,.55fr)]">
            <InfoCard title="اطلاعات معرف">
              <div className="grid gap-3">
                <FormField
                  label="نام معرف یا ضامن"
                  name="guarantorName"
                  required
                  error={fieldErrors.guarantorName}
                >
                  <FormInput
                    name="guarantorName"
                    value={values.guarantorName}
                    error={fieldErrors.guarantorName}
                    onChange={(event) =>
                      update("guarantorName", event.target.value)
                    }
                  />
                </FormField>
                <FormField
                  label="نسبت با پرسنل"
                  name="relationship"
                  required
                  error={fieldErrors.relationship}
                >
                  <FormInput
                    name="relationship"
                    value={values.relationship}
                    error={fieldErrors.relationship}
                    onChange={(event) =>
                      update("relationship", event.target.value)
                    }
                  />
                </FormField>
              </div>
            </InfoCard>
            <InfoCard title="اطلاعات تماس معرف" icon={Phone}>
              <div className="grid gap-3">
                <FormField
                  label="تلفن همراه"
                  name="guarantorPhone"
                  required
                  error={fieldErrors.guarantorPhone}
                >
                  <FormInput
                    name="guarantorPhone"
                    dir="ltr"
                    inputMode="tel"
                    value={values.guarantorPhone}
                    error={fieldErrors.guarantorPhone}
                    onChange={(event) =>
                      update(
                        "guarantorPhone",
                        normalizeDigits(event.target.value),
                      )
                    }
                  />
                </FormField>
                <FormField
                  label="تلفن ثابت"
                  name="guarantorLandlinePhone"
                  required
                  error={fieldErrors.guarantorLandlinePhone}
                >
                  <FormInput
                    name="guarantorLandlinePhone"
                    dir="ltr"
                    inputMode="tel"
                    value={values.guarantorLandlinePhone}
                    error={fieldErrors.guarantorLandlinePhone}
                    onChange={(event) =>
                      update(
                        "guarantorLandlinePhone",
                        normalizeDigits(event.target.value),
                      )
                    }
                  />
                </FormField>
              </div>
            </InfoCard>
            <ImagePicker
              label="تصاویر ضمانت"
              name="guarantor"
              files={files.guarantor}
              onChange={(nextFiles) => updateFiles("guarantor", nextFiles)}
              onValidationError={setImageError}
            />
          </div>
        );
    }
  }

  const avatar = files.profile[0];
  const selectedSalon = salons.find((salon) => salon.id === values.salonId);

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (nextOpen) {
          onOpenChange(true);
        } else if (!submitting) {
          closeDialog();
        }
      }}
    >
      <DialogContent
        showCloseButton={false}
        dir="rtl"
        className="flex h-[min(520px,94dvh)] w-[min(980px,calc(100vw-1rem))] max-w-none flex-col gap-0 overflow-hidden rounded-[18px] border border-[#d9b4da] bg-white p-0 text-slate-800 shadow-xl sm:max-w-none"
      >
        <form className="flex min-h-0 flex-1 flex-col" onSubmit={submit}>
          <header className="flex h-12 shrink-0 items-center justify-between border-b border-slate-100 px-5">
            <DialogTitle className="text-sm font-semibold text-fuchsia-950">
              ثبت پرسنل
            </DialogTitle>
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              aria-label="بستن فرم ثبت پرسنل"
              disabled={submitting}
              onClick={closeDialog}
              className="text-slate-500"
            >
              <X className="size-5" />
            </Button>
          </header>

          <section className="grid shrink-0 grid-cols-1 items-center gap-3 border-b border-slate-100 px-5 py-2 sm:grid-cols-[minmax(0,1fr)_auto_minmax(0,1.15fr)]">
            <div className="flex min-w-0 items-center gap-3">
              <div className="relative flex size-14 shrink-0 items-center justify-center overflow-hidden rounded-full bg-sky-100 text-sky-700">
                {avatar ? (
                  <Image
                    src={avatar.previewUrl}
                    alt=""
                    fill
                    unoptimized
                    className="object-cover"
                    sizes="56px"
                  />
                ) : (
                  <UserRound aria-hidden="true" className="size-7" />
                )}
              </div>
              <div className="min-w-0">
                <p className="truncate font-semibold">
                  {values.fullName || "پرسنل جدید"}
                </p>
                <p className="mt-1 truncate text-xs text-fuchsia-700">
                  {values.jobTitle || "اطلاعات را در تب‌ها تکمیل کنید"}
                </p>
              </div>
              <span
                className={`shrink-0 rounded-full px-2 py-1 text-[10px] ${
                  values.status === "true"
                    ? "bg-emerald-100 text-emerald-700"
                    : "bg-slate-100 text-slate-600"
                }`}
              >
                {values.status === "true" ? "فعال" : "غیرفعال"}
              </span>
            </div>

            <div className="grid gap-1 border-slate-200 text-xs text-slate-600 sm:border-e sm:pe-5">
              <span className="flex items-center gap-2">
                <BriefcaseBusiness
                  aria-hidden="true"
                  className="size-3.5 text-fuchsia-700"
                />
                <strong className="font-medium text-slate-800">
                  کد پرسنلی
                </strong>
                {values.personnelCode || "—"}
              </span>
              <span className="flex items-center gap-2">
                <ContactRound
                  aria-hidden="true"
                  className="size-3.5 text-fuchsia-700"
                />
                {values.workingShiftType === "1"
                  ? "پورسانتی"
                  : values.workingShiftType === "2"
                    ? "ثابت و پورسانت"
                    : "نوع همکاری"}
              </span>
              {salonsError && (
                <span className="text-destructive" role="alert">
                  {salonsError}
                </span>
              )}
            </div>

            <div className="flex min-w-0 items-center gap-2 sm:justify-self-end">
              <MapPin
                aria-hidden="true"
                className="size-4 shrink-0 text-fuchsia-700"
              />
              {salons.length > 1 ? (
                <select
                  aria-label="سالن محل همکاری"
                  value={values.salonId || ""}
                  disabled={salonsLoading || submitting}
                  onChange={(event) =>
                    update("salonId", Number(event.target.value))
                  }
                  className="h-8 max-w-52 rounded-lg border border-slate-200 bg-white px-2 text-xs outline-none focus-visible:border-fuchsia-500"
                >
                  <option value="" disabled>
                    انتخاب سالن
                  </option>
                  {salons.map((salon) => (
                    <option key={salon.id} value={salon.id}>
                      {salon.name}
                    </option>
                  ))}
                </select>
              ) : (
                <span className="max-w-48 truncate text-xs text-slate-600">
                  {salonsLoading
                    ? "در حال دریافت سالن..."
                    : selectedSalon?.name ?? "سالن انتخاب نشده"}
                </span>
              )}
            </div>
          </section>

          <fieldset disabled={submitting} className="contents">
            <nav
              aria-label="بخش‌های فرم ثبت پرسنل"
              role="tablist"
              className="flex h-[42px] shrink-0 overflow-x-auto border-b border-slate-200 px-3"
            >
              {TABS.map(({ id, label, icon: Icon }) => (
                <button
                  key={id}
                  id={`personnel-create-tab-${id}`}
                  type="button"
                  role="tab"
                  aria-selected={activeTab === id}
                  aria-controls="personnel-create-panel"
                  onClick={() => setActiveTab(id)}
                  onKeyDown={(event) => {
                    if (
                      event.key !== "ArrowLeft" &&
                      event.key !== "ArrowRight"
                    ) {
                      return;
                    }
                    event.preventDefault();
                    const currentIndex = TABS.findIndex(
                      (tab) => tab.id === id,
                    );
                    const direction = event.key === "ArrowRight" ? 1 : -1;
                    const nextIndex =
                      (currentIndex + direction + TABS.length) % TABS.length;
                    const nextTab = TABS[nextIndex];
                    setActiveTab(nextTab.id);
                    document
                      .getElementById(
                        `personnel-create-tab-${nextTab.id}`,
                      )
                      ?.focus();
                  }}
                  className={`flex min-w-[145px] flex-1 items-center justify-center gap-2 border-b-2 px-3 text-xs transition-colors focus-visible:outline-2 focus-visible:outline-fuchsia-600 ${
                    activeTab === id
                      ? "border-fuchsia-600 text-fuchsia-700"
                      : "border-transparent text-slate-500 hover:text-fuchsia-700"
                  }`}
                >
                  <Icon aria-hidden="true" className="size-4" />
                  {label}
                </button>
              ))}
            </nav>

            <div
              id="personnel-create-panel"
              role="tabpanel"
              aria-labelledby={`personnel-create-tab-${activeTab}`}
              className="min-h-0 flex-1 overflow-y-auto bg-white px-5 py-2"
            >
              {formError && (
                <div
                  className="mb-2 rounded-lg bg-rose-50 px-3 py-2 text-xs text-rose-700"
                  role="alert"
                >
                  {formError}
                </div>
              )}
              {fileError && (
                <div
                  className="mb-2 rounded-lg bg-rose-50 px-3 py-2 text-xs text-rose-700"
                  role="alert"
                >
                  {fileError}
                </div>
              )}
              {uploadMessage && (
                <div
                  className="mb-2 flex items-center gap-2 text-xs text-fuchsia-700"
                  role="status"
                >
                  <Check aria-hidden="true" className="size-4" />
                  {uploadMessage}
                </div>
              )}
              {renderTab()}
            </div>
          </fieldset>

          <footer className="flex min-h-14 shrink-0 items-center justify-start gap-2 border-t border-slate-100 px-5">
            <Button
              type="submit"
              disabled={submitting || salonsLoading}
              className="h-8 rounded-lg bg-fuchsia-700 px-3 text-xs text-white hover:bg-fuchsia-800 disabled:opacity-70"
            >
              {submitting ? "در حال ثبت..." : "ثبت پرسنل"}
            </Button>
            <Button
              type="button"
              variant="outline"
              disabled={submitting}
              onClick={closeDialog}
              className="h-8 rounded-lg border-rose-500 px-3 text-xs text-rose-600"
            >
              انصراف
            </Button>
          </footer>
        </form>
      </DialogContent>
    </Dialog>
  );
}
