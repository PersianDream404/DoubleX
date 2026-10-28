"use client";

import Image from "next/image";
import { useEffect, useState, type ReactNode } from "react";
import DatePicker, { type DateObject } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";
import { toGregorian, toJalaali } from "jalaali-js";
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
  Search,
  Trash2,
  UserRound,
  X,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { ApiError } from "@/lib/api/api-client";
import { handleApiError } from "@/lib/api/handle-api-error";
import { useUserContext } from "@/contexts/user-context";

import {
  PersonnelFileService,
  PersonnelUploadCategory,
  getPersonnelFileUrl,
} from "./personnel-file.service";
import { PersonnelCreateSchema } from "./personnel-create.schema";
import { PersonnelService } from "./personnel.service";
import type {
  CreatePersonnelRequest,
  PersonnelDetailsDto,
  PersonnelDocumentRequest,
  PersonnelDocumentType,
} from "./personnel.types";
import { ServiceCatalogService } from "@/features/services/list/service.service";
import type {
  ServiceDto,
  ServicePersonnelDto,
} from "@/features/services/list/service.types";

type CreateTab = "personal" | "job" | "financial" | "health" | "guarantor";
type ImageField =
  | "profile"
  | "training"
  | "healthCourse"
  | "healthTest"
  | "guarantor";
type SelectedImage = { file: File; previewUrl: string };
type ExistingImage = { url: string; fileName: string };
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
  fixedSalaryAmount: string;
  commissionRate: string;
  effectiveFrom: string;
  effectiveTo: string;
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
type ExistingImageFiles = Record<ImageField, ExistingImage[]>;

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
  fixedSalaryAmount: "",
  commissionRate: "",
  effectiveFrom: "",
  effectiveTo: "",
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

const EMPTY_EXISTING_FILES: ExistingImageFiles = {
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
  fixedSalaryAmount: "financial",
  commissionRate: "financial",
  effectiveFrom: "financial",
  effectiveTo: "financial",
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

function dateToJalali(value: string) {
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  const jalali = toJalaali(year, month, day);
  return `${jalali.jy}/${String(jalali.jm).padStart(2, "0")}/${String(
    jalali.jd,
  ).padStart(2, "0")}`;
}

function enumValue(value: string, entries: Record<string, string>) {
  return entries[value] ?? "";
}

function getExistingImages(personnel: PersonnelDetailsDto): ExistingImageFiles {
  const images: ExistingImageFiles = {
    profile: personnel.personalInfo.pictureUrl
      ? [{ url: personnel.personalInfo.pictureUrl, fileName: "تصویر پرسنل" }]
      : [],
    training: personnel.jobInfo.jobPictures.map((url, index) => ({
      url,
      fileName: `تصویر دوره آموزشی ${index + 1}`,
    })),
    healthCourse: personnel.healthInfo.pictureCourse.map((url, index) => ({
      url,
      fileName: `تصویر دوره سلامت ${index + 1}`,
    })),
    healthTest: personnel.healthInfo.pictureTest.map((url, index) => ({
      url,
      fileName: `تصویر آزمایش ${index + 1}`,
    })),
    guarantor: personnel.guarantorInfo.pictureGuaranties.map((url, index) => ({
      url,
      fileName: `تصویر ضمانت ${index + 1}`,
    })),
  };

  return images;
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
  existingFiles,
  onChange,
  onRemoveExisting,
  multiple = true,
  onValidationError,
}: {
  label: string;
  name: string;
  files: SelectedImage[];
  existingFiles: ExistingImage[];
  onChange: (files: SelectedImage[]) => void;
  onRemoveExisting: (index: number) => void;
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
      existingFiles.forEach((_, index) => onRemoveExisting(index));
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
        {existingFiles.map((file, index) => (
          <div key={`${file.url}-${index}`} className="relative aspect-[1.1] overflow-hidden rounded-lg bg-fuchsia-50">
            <Image
              src={file.url}
              alt={file.fileName}
              fill
              unoptimized
              className="object-cover"
              sizes="180px"
            />
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              aria-label={`حذف تصویر ${file.fileName}`}
              onClick={() => onRemoveExisting(index)}
              className="absolute bottom-2 start-2 size-8 rounded-full bg-white text-rose-600 hover:bg-rose-50"
            >
              <Trash2 className="size-4" />
            </Button>
          </div>
        ))}
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
  clearPicture = false,
): CreatePersonnelRequest {
  return {
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
      clearPicture,
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
      fixedSalaryAmount: values.fixedSalaryAmount
        ? Number(values.fixedSalaryAmount)
        : null,
      commissionRate: values.commissionRate
        ? Number(values.commissionRate)
        : null,
      effectiveFrom: values.effectiveFrom
        ? dateToApi(values.effectiveFrom)
        : null,
      effectiveTo: values.effectiveTo ? dateToApi(values.effectiveTo) : null,
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
  personnelId = null,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCreated: () => void;
  personnelId?: number | null;
}) {
  const { salonId, loading: userLoading, error: userError } = useUserContext();
  const salonError =
    userError ??
    (!userLoading && !salonId
      ? "شناسه سالن در توکن ورود شما موجود نیست."
      : null);
  const [activeTab, setActiveTab] = useState<CreateTab>("personal");
  const [values, setValues] = useState<FormValues>(EMPTY_FORM);
  const [files, setFiles] = useState<ImageFiles>(EMPTY_FILES);
  const [existingFiles, setExistingFiles] =
    useState<ExistingImageFiles>(EMPTY_EXISTING_FILES);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [fileError, setFileError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [personnelLoading, setPersonnelLoading] = useState(false);
  const [personnelLoaded, setPersonnelLoaded] = useState(false);
  const [savedPersonnelId, setSavedPersonnelId] = useState<number | null>(null);
  const [serviceOptions, setServiceOptions] = useState<ServiceDto[]>([]);
  const [servicePersonnelLinks, setServicePersonnelLinks] = useState<
    ServicePersonnelDto[]
  >([]);
  const [selectedServiceIds, setSelectedServiceIds] = useState<number[]>([]);
  const [serviceSearch, setServiceSearch] = useState("");
  const [servicePersonnelLoading, setServicePersonnelLoading] = useState(false);
  const [servicePersonnelReady, setServicePersonnelReady] = useState(false);
  const [servicePersonnelError, setServicePersonnelError] = useState<
    string | null
  >(null);
  const [uploadMessage, setUploadMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!open || personnelId !== null || !salonId) return;
    let active = true;
    queueMicrotask(() => {
      if (!active) return;
      setValues((current) =>
        current.salonId === salonId ? current : { ...current, salonId },
      );
    });
    return () => {
      active = false;
    };
  }, [open, personnelId, salonId]);

  useEffect(() => {
    if (!open || personnelId === null || userLoading) {
      return;
    }

    const controller = new AbortController();
    const service = new PersonnelService();
    Promise.resolve()
      .then(() => {
        if (controller.signal.aborted) {
          return null;
        }
        setPersonnelLoading(true);
        setPersonnelLoaded(false);
        setFormError(null);
        return service.getById(personnelId, { signal: controller.signal });
      })
      .then((response) => {
        if (!response) {
          return;
        }
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات پرسنل دریافت نشد.");
        }

        const personnel = response.data;
        setValues({
          salonId: salonId ?? 0,
          fullName: personnel.personalInfo.fullName,
          nationalCode: personnel.personalInfo.nationalCode,
          birthDate: dateToJalali(personnel.personalInfo.birthDate),
          maritalStatus:
            enumValue(personnel.personalInfo.maritalStatus, {
              Single: "0",
              Married: "1",
            }) || "0",
          childrenCount: String(personnel.personalInfo.childrenCount),
          mobilePhone: personnel.personalInfo.mobilePhone,
          landlinePhone: personnel.personalInfo.landlinePhone ?? "",
          addressText: personnel.personalInfo.addressText,
          isCloseToSalon: personnel.personalInfo.isCloseToSalon,
          jobTitle: personnel.jobInfo.jobTitle,
          personnelCode: personnel.jobInfo.personnelCode,
          cooperationStartDate: dateToJalali(
            personnel.jobInfo.cooperationStartDate,
          ),
          workingShiftType: enumValue(personnel.jobInfo.workingShiftType, {
            Commission: "1",
            FixedAndCommission: "2",
            Negotiable: "3",
            Rental: "4",
            FullTime: "5",
            PartTimeMorning: "6",
            PartTimeEvening: "7",
          }),
          workHistory: personnel.jobInfo.workHistory ?? "",
          salaryType: enumValue(personnel.financialInfo.salaryType, {
            Fixed: "0",
            Commission: "1",
            ChairRental: "2",
            RoomRental: "3",
          }),
          cardNumber: personnel.financialInfo.cardNumber,
          accountNumber: personnel.financialInfo.accountNumber,
          fixedSalaryAmount: personnel.financialInfo.fixedSalaryAmount
            ? String(personnel.financialInfo.fixedSalaryAmount)
            : "",
          commissionRate: personnel.financialInfo.commissionRate
            ? String(personnel.financialInfo.commissionRate)
            : "",
          effectiveFrom: personnel.financialInfo.effectiveFrom
            ? dateToJalali(personnel.financialInfo.effectiveFrom)
            : "",
          effectiveTo: personnel.financialInfo.effectiveTo
            ? dateToJalali(personnel.financialInfo.effectiveTo)
            : "",
          healthStatus: String(personnel.healthInfo.healthStatus),
          diseases: personnel.healthInfo.diseases ?? "",
          allergyStatus: String(personnel.healthInfo.allergyStatus),
          allergies: personnel.healthInfo.allergies ?? "",
          guarantorName: personnel.guarantorInfo.guarantorName,
          relationship: personnel.guarantorInfo.relationship,
          guarantorPhone: personnel.guarantorInfo.guarantorPhone,
          guarantorLandlinePhone:
            personnel.guarantorInfo.guarantorLandlinePhone ?? "",
          status: String(personnel.status),
        });
        setExistingFiles(getExistingImages(personnel));
        setPersonnelLoaded(true);
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setFormError(handleApiError(error));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setPersonnelLoading(false);
        }
      });

    return () => controller.abort();
  }, [open, personnelId, salonId, userLoading]);

  useEffect(() => {
    if (
      !open ||
      values.salonId <= 0 ||
      (personnelId !== null && !personnelLoaded)
    ) {
      return;
    }

    const controller = new AbortController();
    const service = new ServiceCatalogService();

    Promise.resolve()
      .then(() => {
        if (controller.signal.aborted) {
          return null;
        }
        setServicePersonnelLoading(true);
        setServicePersonnelReady(false);
        setServicePersonnelError(null);
        setServiceOptions([]);
        setServicePersonnelLinks([]);
        setSelectedServiceIds([]);
        setServiceSearch("");
        return service.getAll({
          signal: controller.signal,
        });
      })
      .then(async (servicesResponse) => {
        if (!servicesResponse) {
          return null;
        }
        if (!servicesResponse.data) {
          throw new Error(
            servicesResponse.message ?? "فهرست خدمات سالن دریافت نشد.",
          );
        }

        const services = servicesResponse.data;
        const assignmentResponse =
          personnelId === null
            ? null
            : await service.getPersonnelServiceAssignments(
                personnelId,
                { signal: controller.signal },
              );
        if (assignmentResponse && !assignmentResponse.data) {
          throw new Error(
            assignmentResponse.message ??
              "خدمت‌های اختصاص‌یافته به پرسنل دریافت نشد.",
          );
        }

        if (controller.signal.aborted) {
          return null;
        }

        const links = assignmentResponse?.data ?? [];
        return { services, links };
      })
      .then((result) => {
        if (result && !controller.signal.aborted) {
          setServiceOptions(result.services);
          setServicePersonnelLinks(result.links);
          setSelectedServiceIds(result.links.map((item) => item.serviceId));
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setServicePersonnelError(handleApiError(error));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setServicePersonnelLoading(false);
          setServicePersonnelReady(true);
        }
      });

    return () => controller.abort();
  }, [open, values.salonId, personnelId, personnelLoaded]);

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

  function removeExistingFile(field: ImageField, index: number) {
    setExistingFiles((current) => ({
      ...current,
      [field]: current[field].filter((_, fileIndex) => fileIndex !== index),
    }));
  }

  function closeDialog() {
    clearFiles(files);
    setValues(EMPTY_FORM);
    setFiles(EMPTY_FILES);
    setExistingFiles(EMPTY_EXISTING_FILES);
    setFieldErrors({});
    setFormError(null);
    setFileError(null);
    setUploadMessage(null);
    setPersonnelLoading(false);
    setPersonnelLoaded(false);
    setSavedPersonnelId(null);
    setServiceOptions([]);
    setServicePersonnelLinks([]);
    setSelectedServiceIds([]);
    setServicePersonnelLoading(false);
    setServicePersonnelReady(false);
    setServicePersonnelError(null);
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

  async function synchronizeServicePersonnel(personnelRecordId: number) {
    const service = new ServiceCatalogService();
    const links = [...servicePersonnelLinks];
    const selectedIds = new Set(selectedServiceIds);

    for (const serviceId of selectedIds) {
      if (links.some((item) => item.serviceId === serviceId)) {
        continue;
      }

      const response = await service.addPersonnelToService({
        serviceId,
        personnelId: personnelRecordId,
      });
      if (response.statusCode < 200 || response.statusCode >= 300) {
        throw new Error(response.message ?? "اختصاص پرسنل به خدمت انجام نشد.");
      }

      const assignments = await service.getServicePersonnels(
        serviceId,
      );
      const link = assignments.data?.find(
        (item) => item.personnelId === personnelRecordId,
      );
      if (!link) {
        throw new Error(
          "خدمت ثبت شد اما ارتباط پرسنل برای تأیید دوباره دریافت نشد.",
        );
      }

      links.push(link);
      setServicePersonnelLinks([...links]);
    }

    for (const link of links.filter((item) => !selectedIds.has(item.serviceId))) {
      const response = await service.removePersonnelFromService(
        link.id,
        link.serviceId,
      );
      if (response.statusCode < 200 || response.statusCode >= 300) {
        throw new Error(response.message ?? "حذف خدمت از پرسنل انجام نشد.");
      }

      const index = links.findIndex((item) => item.id === link.id);
      if (index >= 0) {
        links.splice(index, 1);
      }
      setServicePersonnelLinks([...links]);
    }
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

    const currentPersonnelId = personnelId ?? savedPersonnelId;
    if (
      (currentPersonnelId === null && userLoading) ||
      (currentPersonnelId === null && !salonId)
    ) {
      setFormError(
        salonError ?? "شناسه سالن در توکن ورود شما موجود نیست.",
      );
      return;
    }
    if (!servicePersonnelReady || servicePersonnelLoading || servicePersonnelError) {
      setActiveTab("job");
      setFormError(
        servicePersonnelError ??
          "برای ذخیره، ابتدا فهرست خدمت‌های قابل‌اختصاص دریافت شود.",
      );
      return;
    }

    setSubmitting(true);
    setFormError(null);
    const uploadFiles = filesForUpload();
    const documents: PersonnelDocumentRequest[] = [];
    const existingImageDocuments: PersonnelDocumentRequest[] = [
      ...existingFiles.profile.map((file) => ({
        documentType: 0 as const,
        fileName: file.fileName,
        url: file.url,
      })),
      ...existingFiles.training.map((file) => ({
        documentType: 1 as const,
        fileName: file.fileName,
        url: file.url,
      })),
      ...existingFiles.guarantor.map((file) => ({
        documentType: 3 as const,
        fileName: file.fileName,
        url: file.url,
      })),
      ...existingFiles.healthCourse.map((file) => ({
        documentType: 4 as const,
        fileName: file.fileName,
        url: file.url,
      })),
      ...existingFiles.healthTest.map((file) => ({
        documentType: 5 as const,
        fileName: file.fileName,
        url: file.url,
      })),
    ];
    documents.push(...existingImageDocuments);
    const fileService = new PersonnelFileService();
    const uploadedFileIds: number[] = [];
    let createRequestStarted = false;
    let personnelSaveCompleted = false;

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

      setUploadMessage(
        currentPersonnelId !== null
          ? "در حال ذخیره تغییرات پرسنل..."
          : "در حال ثبت اطلاعات پرسنل...",
      );
      const request = makeRequest(
        values,
        documents,
        currentPersonnelId !== null &&
          !documents.some((document) => document.documentType === 0),
      );
      createRequestStarted = true;
      const service = new PersonnelService();
      const saveResponse = currentPersonnelId !== null
        ? await service.update(currentPersonnelId, request)
        : await service.create(request);
      if (saveResponse.statusCode < 200 || saveResponse.statusCode >= 300) {
        throw new Error(
          saveResponse.message ??
            (currentPersonnelId !== null
              ? "ذخیره تغییرات پرسنل انجام نشد."
              : "ثبت اطلاعات پرسنل انجام نشد."),
        );
      }

      const persistedPersonnelId =
        currentPersonnelId ??
        (typeof saveResponse.data === "object" && saveResponse.data
          ? saveResponse.data.id
          : null);
      if (persistedPersonnelId === null || persistedPersonnelId <= 0) {
        throw new Error("شناسه پرسنل ذخیره‌شده از سرور دریافت نشد.");
      }
      personnelSaveCompleted = true;
      setSavedPersonnelId(persistedPersonnelId);
      clearFiles(files);
      setFiles(EMPTY_FILES);
      const toExistingImages = (documentType: PersonnelDocumentType) =>
        documents
          .filter((document) => document.documentType === documentType)
          .map((document) => ({
            url: document.url,
            fileName: document.fileName,
          }));
      setExistingFiles({
        profile: toExistingImages(0),
        training: toExistingImages(1),
        healthCourse: toExistingImages(4),
        healthTest: toExistingImages(5),
        guarantor: toExistingImages(3),
      });
      setUploadMessage("در حال ذخیره خدمت‌های پرسنل...");
      await synchronizeServicePersonnel(persistedPersonnelId);

      onCreated();
      closeDialog();
    } catch (error: unknown) {
      const failureMessages = [handleApiError(error)];
      if (personnelSaveCompleted) {
        failureMessages.unshift(
          "اطلاعات پرسنل ذخیره شده است؛ خدمت‌های انتخاب‌شده را بررسی و دوباره ذخیره کنید.",
        );
      }
      const safeToDeleteFiles =
        !personnelSaveCompleted &&
        (!createRequestStarted ||
          (error instanceof ApiError &&
            error.statusCode >= 400 &&
            error.statusCode < 500));
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
              existingFiles={existingFiles.profile}
              multiple={false}
              onChange={(nextFiles) => updateFiles("profile", nextFiles)}
              onRemoveExisting={(index) => removeExistingFile("profile", index)}
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
              existingFiles={existingFiles.training}
              onChange={(nextFiles) => updateFiles("training", nextFiles)}
              onRemoveExisting={(index) => removeExistingFile("training", index)}
              onValidationError={setImageError}
            />
            <div className="md:col-span-3">
              <InfoCard
                title="خدمت‌های قابل ارائه"
                icon={BriefcaseBusiness}
                action={
                  <span className="text-xs text-slate-500">
                    {selectedServiceIds.length} خدمت انتخاب شده
                  </span>
                }
              >
                {servicePersonnelLoading ? (
                  <p className="text-sm text-slate-500" role="status">
                    در حال دریافت خدمت‌های سالن...
                  </p>
                ) : servicePersonnelError ? (
                  <p className="text-sm text-rose-700" role="alert">
                    {servicePersonnelError}
                  </p>
                ) : serviceOptions.length === 0 ? (
                  <p className="text-sm text-slate-500">
                    خدمتی برای این سالن ثبت نشده است.
                  </p>
                ) : (
                  <details className="group relative">
                    <summary className="flex min-h-10 cursor-pointer list-none items-center justify-between gap-3 rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 outline-none transition-colors hover:border-fuchsia-300 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 [&::-webkit-details-marker]:hidden">
                      <span>
                        {selectedServiceIds.length > 0
                          ? `${selectedServiceIds.length} خدمت انتخاب شده`
                          : "انتخاب خدمت"}
                      </span>
                      <span
                        aria-hidden="true"
                        className="text-slate-400 transition-transform group-open:rotate-180"
                      >
                        ▾
                      </span>
                    </summary>
                    <div className="absolute inset-x-0 top-full z-20 mt-1 rounded-lg border border-slate-200 bg-white p-2 shadow-xl">
                      <label className="relative block">
                        <span className="sr-only">جست‌وجوی خدمت</span>
                        <Search
                          aria-hidden="true"
                          className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
                        />
                        <Input
                          type="search"
                          value={serviceSearch}
                          onChange={(event) => setServiceSearch(event.target.value)}
                          placeholder="جست‌وجوی خدمت..."
                          className="h-9 rounded-lg border-slate-200 bg-white ps-9 text-sm"
                        />
                      </label>
                      <div
                        role="group"
                        aria-label="خدمت‌های قابل ارائه"
                        className="mt-2 grid max-h-44 gap-1 overflow-y-auto"
                      >
                        {serviceOptions
                          .filter((service) =>
                            service.name
                              .toLocaleLowerCase()
                              .includes(serviceSearch.trim().toLocaleLowerCase()),
                          )
                          .map((service) => {
                            const isSelected = selectedServiceIds.includes(service.id);
                            return (
                              <label
                                key={service.id}
                                className={`flex min-h-9 cursor-pointer items-center gap-2 rounded-md px-2 py-1.5 text-xs transition-colors ${
                                  isSelected
                                    ? "bg-fuchsia-50 text-fuchsia-900"
                                    : "text-slate-700 hover:bg-slate-50"
                                } ${
                                  !service.isActive && !isSelected
                                    ? "cursor-not-allowed opacity-50"
                                    : ""
                                }`}
                              >
                                <input
                                  type="checkbox"
                                  checked={isSelected}
                                  disabled={
                                    submitting ||
                                    (!service.isActive && !isSelected) ||
                                    (values.status !== "true" && !isSelected)
                                  }
                                  onChange={(event) =>
                                    setSelectedServiceIds((current) =>
                                      event.target.checked
                                        ? [...current, service.id]
                                        : current.filter((id) => id !== service.id),
                                    )
                                  }
                                  className="size-4 accent-fuchsia-700"
                                />
                                <span className="min-w-0 flex-1 truncate">
                                  {service.name}
                                </span>
                                {!service.isActive && (
                                  <span className="text-[10px] text-slate-500">
                                    غیرفعال
                                  </span>
                                )}
                              </label>
                            );
                          })}
                        {!serviceOptions.some((service) =>
                          service.name
                            .toLocaleLowerCase()
                            .includes(serviceSearch.trim().toLocaleLowerCase()),
                        ) && (
                          <p className="px-2 py-3 text-center text-xs text-slate-500">
                            خدمتی با این نام پیدا نشد.
                          </p>
                        )}
                      </div>
                    </div>
                  </details>
                )}
                {values.status !== "true" && (
                  <p className="mt-2 text-[11px] text-amber-700">
                    برای افزودن خدمت، ابتدا وضعیت پرسنل را فعال کنید.
                  </p>
                )}
              </InfoCard>
            </div>
          </div>
        );

      case "financial":
        return (
          <div className="grid items-start gap-3 md:grid-cols-[minmax(0,1.1fr)_minmax(0,1.5fr)]">
            <InfoCard title="حقوق و دستمزد" icon={BadgeDollarSign}>
              <div className="grid gap-3">
                <FormField
                  label="نوع قرارداد"
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
                    <option value="2">ترکیبی</option>
                    <option value="3">اجاره اتاق</option>
                  </FormSelect>
                </FormField>

                {(values.salaryType === "0" || values.salaryType === "2") && (
                  <FormField label="حقوق ثابت" name="fixedSalaryAmount">
                    <FormInput
                      name="fixedSalaryAmount"
                      dir="ltr"
                      inputMode="decimal"
                      value={values.fixedSalaryAmount}
                      onChange={(event) =>
                        update("fixedSalaryAmount", event.target.value)
                      }
                    />
                  </FormField>
                )}

                {(values.salaryType === "1" || values.salaryType === "2") && (
                  <FormField label="درصد پورسانت" name="commissionRate">
                    <FormInput
                      name="commissionRate"
                      dir="ltr"
                      inputMode="decimal"
                      value={values.commissionRate}
                      onChange={(event) =>
                        update("commissionRate", event.target.value)
                      }
                    />
                  </FormField>
                )}

                <div className="grid gap-3 sm:grid-cols-2">
                  <FormField label="تاریخ شروع قرارداد" name="effectiveFrom">
                    <JalaliDatePicker
                      name="effectiveFrom"
                      value={values.effectiveFrom}
                      onChange={(value) => update("effectiveFrom", value)}
                    />
                  </FormField>
                  <FormField label="تاریخ پایان قرارداد" name="effectiveTo">
                    <JalaliDatePicker
                      name="effectiveTo"
                      value={values.effectiveTo}
                      onChange={(value) => update("effectiveTo", value)}
                    />
                  </FormField>
                </div>
              </div>
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
              existingFiles={existingFiles.healthTest}
              onChange={(nextFiles) => updateFiles("healthTest", nextFiles)}
              onRemoveExisting={(index) =>
                removeExistingFile("healthTest", index)
              }
              onValidationError={setImageError}
            />
            <ImagePicker
              label="تصاویر دوره‌ها"
              name="healthCourse"
              files={files.healthCourse}
              existingFiles={existingFiles.healthCourse}
              onChange={(nextFiles) => updateFiles("healthCourse", nextFiles)}
              onRemoveExisting={(index) =>
                removeExistingFile("healthCourse", index)
              }
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
              existingFiles={existingFiles.guarantor}
              onChange={(nextFiles) => updateFiles("guarantor", nextFiles)}
              onRemoveExisting={(index) => removeExistingFile("guarantor", index)}
              onValidationError={setImageError}
            />
          </div>
        );
    }
  }

  const avatar = files.profile[0];
  const existingAvatar = existingFiles.profile[0];
  const isEditing = personnelId !== null || savedPersonnelId !== null;

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
              {isEditing ? "ویرایش پرسنل" : "ثبت پرسنل"}
            </DialogTitle>
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              aria-label="بستن فرم پرسنل"
              disabled={submitting || personnelLoading}
              onClick={closeDialog}
              className="text-slate-500"
            >
              <X className="size-5" />
            </Button>
          </header>

          <section className="grid shrink-0 grid-cols-1 items-center gap-3 border-b border-slate-100 px-5 py-2 sm:grid-cols-[minmax(0,1fr)_auto_minmax(0,1.15fr)]">
            <div className="flex min-w-0 items-center gap-3">
              <div className="relative flex size-14 shrink-0 items-center justify-center overflow-hidden rounded-full bg-sky-100 text-sky-700">
                {avatar || existingAvatar ? (
                  <Image
                    src={avatar?.previewUrl ?? existingAvatar.url}
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
              {salonError && (
                <span className="text-destructive" role="alert">
                  {salonError}
                </span>
              )}
            </div>

            <div className="flex min-w-0 items-center gap-2 sm:justify-self-end">
              <MapPin
                aria-hidden="true"
                className="size-4 shrink-0 text-fuchsia-700"
              />
              <span className="max-w-48 truncate text-xs text-slate-600">
                {userLoading
                  ? "در حال دریافت سالن..."
                  : salonId
                    ? salonId.toLocaleString("fa-IR")
                    : "سالن انتخاب نشده"}
              </span>
            </div>
          </section>

          <fieldset
            disabled={
              submitting ||
              personnelLoading ||
              (personnelId !== null && !personnelLoaded)
            }
            className="contents"
          >
            <nav
              aria-label="بخش‌های فرم پرسنل"
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
              {personnelLoading && (
                <p className="mb-3 text-sm text-slate-500" role="status">
                  در حال دریافت اطلاعات پرسنل...
                </p>
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
              disabled={
                submitting ||
                userLoading ||
                !salonId ||
                personnelLoading ||
                servicePersonnelLoading ||
                !servicePersonnelReady ||
                servicePersonnelError !== null ||
                (personnelId !== null && !personnelLoaded)
              }
              className="h-8 rounded-lg bg-fuchsia-700 px-3 text-xs text-white hover:bg-fuchsia-800 disabled:opacity-70"
            >
              {submitting
                ? isEditing
                  ? "در حال ذخیره..."
                  : "در حال ثبت..."
                : isEditing
                  ? "ذخیره تغییرات"
                  : "ثبت پرسنل"}
            </Button>
            <Button
              type="button"
              variant="outline"
              disabled={submitting || personnelLoading}
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
