"use client";

import Image from "next/image";
import { useEffect, useState } from "react";
import {
  BadgeDollarSign,
  BriefcaseBusiness,
  ContactRound,
  Download,
  HeartPulse,
  MapPin,
  MoveUpRight,
  Phone,
  Printer,
  UserRound,
  X,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { handleApiError } from "@/lib/api/handle-api-error";

import { PersonnelService } from "./personnel.service";
import type { PersonnelDetailsDto } from "./personnel.types";

type DetailsTab = "personal" | "job" | "financial" | "health" | "guarantor";

type DetailsState = {
  personnelId: number | null;
  data: PersonnelDetailsDto | null;
  error: string | null;
};

const TABS: {
  id: DetailsTab;
  label: string;
  icon: typeof UserRound;
}[] = [
  { id: "personal", label: "اطلاعات شخصی", icon: UserRound },
  { id: "job", label: "اطلاعات شغلی", icon: BriefcaseBusiness },
  { id: "financial", label: "اطلاعات مالی", icon: BadgeDollarSign },
  { id: "health", label: "اطلاعات سلامتی", icon: HeartPulse },
  { id: "guarantor", label: "اطلاعات حقوقی", icon: ContactRound },
];

const ENUM_LABELS: Record<string, string> = {
  Single: "مجرد",
  Married: "متأهل",
  Commission: "پورسانتی",
  FixedAndCommission: "ثابت و پورسانت",
  Negotiable: "توافقی",
  Rental: "اجاره‌ای",
  FullTime: "تمام‌وقت",
  PartTimeMorning: "نیمه‌وقت (صبح)",
  PartTimeEvening: "نیمه‌وقت (عصر)",
  Fixed: "ثابت",
  ChairRental: "اجاره صندلی",
  RoomRental: "اجاره اتاق",
};

function displayEnum(value: string) {
  return ENUM_LABELS[value] ?? value;
}

function formatDate(value: string) {
  const [year, month, day] = value.slice(0, 10).split("-");

  return year && month && day ? `${year}/${month}/${day}` : value;
}

function maskedNumber(value: string, visiblePrefix = 4, visibleSuffix = 4) {
  const digits = value.replace(/\D/g, "");
  const visibleDigits = new Set<number>();

  for (
    let index = 0;
    index < Math.min(visiblePrefix, digits.length);
    index += 1
  ) {
    visibleDigits.add(index);
  }

  for (
    let index = Math.max(visiblePrefix, digits.length - visibleSuffix);
    index < digits.length;
    index += 1
  ) {
    visibleDigits.add(index);
  }

  let digitIndex = 0;

  return [...value]
    .map((character) => {
      if (!/\d/.test(character)) {
        return character;
      }

      const masked = visibleDigits.has(digitIndex) ? character : "*";
      digitIndex += 1;
      return masked;
    })
    .join("");
}

function downloadPersonnelDetails(details: PersonnelDetailsDto) {
  const file = new Blob([JSON.stringify(details, null, 2)], {
    type: "application/json",
  });
  const url = URL.createObjectURL(file);
  const link = document.createElement("a");

  link.href = url;
  link.download = `personnel-${details.id}.json`;
  link.click();
  window.setTimeout(() => URL.revokeObjectURL(url), 0);
}

function InfoCard({
  title,
  icon: Icon,
  action,
  centerTitle = false,
  children,
}: {
  title: string;
  icon?: typeof UserRound;
  action?: React.ReactNode;
  centerTitle?: boolean;
  children: React.ReactNode;
}) {
  return (
    <section className="min-w-0 rounded-lg border border-slate-100 bg-white p-3">
      <h3
        className={`mb-3 flex min-h-5 items-center gap-2 text-sm font-semibold text-slate-800 ${
          centerTitle ? "justify-center" : "justify-start"
        }`}
      >
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

function InfoField({
  label,
  value,
  dir = "auto",
}: {
  label: string;
  value: React.ReactNode;
  dir?: "ltr" | "rtl" | "auto";
}) {
  return (
    <div className="min-w-0 space-y-1">
      <dt className="text-xs text-slate-500">{label}</dt>
      <dd dir={dir} className="break-words text-sm leading-6 text-slate-800">
        {value ?? "—"}
      </dd>
    </div>
  );
}

function PictureCard({
  title,
  urls,
  aspectClass = "aspect-square",
}: {
  title: string;
  urls: string[];
  aspectClass?: string;
}) {
  return (
    <InfoCard title={title} centerTitle>
      {urls.length > 0 ? (
        <div className="grid grid-cols-1 gap-2">
          {urls.map((url, index) => (
            <a
              key={`${url}-${index}`}
              href={url}
              target="_blank"
              rel="noreferrer"
              aria-label={`مشاهده ${title} ${index + 1}`}
              className={`group relative ${aspectClass} overflow-hidden rounded-lg bg-fuchsia-50 focus-visible:outline-2 focus-visible:outline-fuchsia-600`}
            >
              <Image
                src={url}
                alt={`${title} ${index + 1}`}
                fill
                unoptimized
                className="object-cover"
                sizes="180px"
              />
              <span className="absolute bottom-2 start-2 flex size-9 items-center justify-center rounded-full bg-white text-fuchsia-600">
                <MoveUpRight aria-hidden="true" className="size-4" />
              </span>
            </a>
          ))}
        </div>
      ) : (
        <div
          role="img"
          aria-label="تصویری ثبت نشده است"
          className={`relative flex ${aspectClass} items-center justify-center rounded-lg bg-fuchsia-50 text-fuchsia-200`}
        >
          <span className="absolute bottom-2 start-2 flex size-9 items-center justify-center rounded-full bg-white text-fuchsia-600">
            <MoveUpRight aria-hidden="true" className="size-4" />
          </span>
        </div>
      )}
    </InfoCard>
  );
}

function PersonnelDetailsContent({
  personnel,
  activeTab,
}: {
  personnel: PersonnelDetailsDto;
  activeTab: DetailsTab;
}) {
  const { personalInfo, jobInfo, financialInfo, healthInfo, guarantorInfo } =
    personnel;

  switch (activeTab) {
    case "personal":
      return (
        <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.65fr)_minmax(0,1.28fr)_minmax(0,1fr)]">
          <div className="grid gap-3">
            <InfoCard
              title="اطلاعات فردی"
              action={
                <Badge className="bg-emerald-100 text-emerald-700 hover:bg-emerald-100">
                  {personnel.status ? "فعال" : "غیرفعال"}
                </Badge>
              }
            >
              <dl className="grid gap-x-6 gap-y-4 sm:grid-cols-2">
                <InfoField label="نام خانوادگی" value={personalInfo.fullName} />
                <InfoField
                  dir="rtl"
                  label="کد ملی"
                  value={maskedNumber(personalInfo.nationalCode, 3, 0)}
                />
                <InfoField
                  label="تاریخ تولد"
                  dir="rtl"
                  value={formatDate(personalInfo.birthDate)}
                />
                <InfoField
                  label="وضعیت تأهل"
                  value={`${displayEnum(personalInfo.maritalStatus)}${personalInfo.childrenCount ? ` - ${personalInfo.childrenCount} فرزند` : ""}`}
                />
              </dl>
            </InfoCard>

            <InfoCard
              title="آدرس"
              icon={MapPin}
              action={
                personalInfo.isCloseToSalon ? (
                  <Badge className="bg-fuchsia-50 text-fuchsia-700 hover:bg-fuchsia-50">
                    نزدیک محل کار
                  </Badge>
                ) : null
              }
            >
              <dl>
                <InfoField label="آدرس" value={personalInfo.addressText} />
              </dl>
            </InfoCard>
          </div>

          <InfoCard title="شماره تماس" icon={Phone}>
            <dl className="space-y-4">
              <InfoField
                dir="rtl"
                label="تلفن همراه"
                value={personalInfo.mobilePhone}
              />
              <InfoField
                dir="rtl"
                label="تلفن ثابت"
                value={personalInfo.landlinePhone}
              />
            </dl>
          </InfoCard>

          <PictureCard
            title="تصویر پرسنل"
            urls={personalInfo.pictureUrl ? [personalInfo.pictureUrl] : []}
          />
        </div>
      );

    case "job":
      return (
        <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.1fr)_minmax(0,1fr)_minmax(0,.82fr)]">
          <InfoCard title="اطلاعات شغلی">
            <dl className="grid gap-x-6 gap-y-4 sm:grid-cols-2">
              <InfoField label="عنوان شغلی" value={jobInfo.jobTitle} />
              <InfoField  dir="rtl" label="کد پرسنلی" value={jobInfo.personnelCode} />
              <InfoField
                label="تاریخ شروع همکاری"
                 dir="rtl"
                value={formatDate(jobInfo.cooperationStartDate)}
              />
              <InfoField
                label="نوع همکاری"
                value={displayEnum(jobInfo.workingShiftType)}
              />
            </dl>
          </InfoCard>
          <InfoCard title="سوابق کاری">
            <dl className="space-y-4">
              <InfoField  dir="rtl" label="سوابق" value={jobInfo.workHistory} />
            </dl>
          </InfoCard>
          <PictureCard
            title="تصاویر دوره آموزشی"
            urls={jobInfo.jobPictures}
            aspectClass="aspect-[1.55]"
          />
        </div>
      );

    case "financial":
      return (
        <div className="grid items-start gap-3 md:grid-cols-[minmax(0,1fr)_minmax(0,1.5fr)]">
          <InfoCard title="حقوق و دستمزد">
            <dl>
              <InfoField
                label="نوع پرداخت"
                value={displayEnum(financialInfo.salaryType)}
              />
            </dl>
          </InfoCard>
          <InfoCard title="اطلاعات حساب بانکی">
            <dl className="grid gap-5 sm:grid-cols-2">
              <InfoField
                label="شماره کارت"  dir="rtl"
                value={maskedNumber(financialInfo.cardNumber)}
              />
              <InfoField
                label="شماره حساب"  dir="rtl"
                value={maskedNumber(financialInfo.accountNumber)}
              />
            </dl>
          </InfoCard>
        </div>
      );

    case "health":
      return (
        <div className="grid items-start gap-3 md:grid-cols-[minmax(0,2.9fr)_minmax(0,1fr)_minmax(0,1fr)]">
          <InfoCard
            title="اطلاعات سلامتی"
            action={
              <span className="flex flex-wrap gap-2">
                <Badge
                  className={
                    healthInfo.healthStatus
                      ? "bg-rose-100 text-rose-700 hover:bg-rose-100"
                      : "bg-emerald-100 text-emerald-700 hover:bg-emerald-100"
                  }
                >
                  {healthInfo.healthStatus ? "بیمار است" : "سالم"}
                </Badge>
                <Badge
                  className={
                    healthInfo.allergyStatus
                      ? "bg-fuchsia-100 text-fuchsia-700 hover:bg-fuchsia-100"
                      : "bg-muted text-muted-foreground hover:bg-muted"
                  }
                >
                  {healthInfo.allergyStatus ? "حساسیت دارد" : "بدون حساسیت"}
                </Badge>
              </span>
            }
          >
            <dl className="space-y-5">
              <InfoField
                label="وضعیت بیماری"
                value={
                  healthInfo.diseases ||
                  (healthInfo.healthStatus ? "بیمار است" : "سالم")
                }
              />
              <InfoField
                label="وضعیت حساسیت"
                value={
                  healthInfo.allergies ||
                  (healthInfo.allergyStatus ? "حساسیت دارد" : "حساسیت ندارد")
                }
              />
            </dl>
          </InfoCard>
          <PictureCard title="تصاویر دوره‌ها" urls={healthInfo.pictureCourse} />
          <PictureCard title="تصاویر آزمایش‌ها" urls={healthInfo.pictureTest} />
        </div>
      );

    case "guarantor":
      return (
        <div className="grid items-start gap-3 md:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)_minmax(0,.55fr)]">
          <InfoCard title="اطلاعات معرف">
            <dl className="space-y-5">
              <InfoField
                label="نام معرف یا ضامن"
                value={guarantorInfo.guarantorName}
              />
              <InfoField
                label="نسبت با پرسنل"
                value={guarantorInfo.relationship}
              />
            </dl>
          </InfoCard>
          <InfoCard title="اطلاعات تماس معرف" icon={Phone}>
            <dl className="space-y-5">
              <InfoField
                label="تلفن همراه"  dir="rtl"
                value={guarantorInfo.guarantorPhone}
              />
              <InfoField
                label="تلفن ثابت"  dir="rtl"
                value={guarantorInfo.guarantorLandlinePhone}
              />
            </dl>
          </InfoCard>
          <PictureCard
            title="تصاویر ضمانت"
            urls={guarantorInfo.pictureGuaranties}
          />
        </div>
      );
  }
}

export function PersonnelDetailsDialog({
  personnelId,
  onOpenChange,
}: {
  personnelId: number | null;
  onOpenChange: (open: boolean) => void;
}) {
  const open = personnelId !== null;
  const [activeTab, setActiveTab] = useState<DetailsTab>("personal");
  const [state, setState] = useState<DetailsState>({
    personnelId: null,
    data: null,
    error: null,
  });

  useEffect(() => {
    if (!open || personnelId === null) {
      return;
    }

    const controller = new AbortController();
    const service = new PersonnelService();

    Promise.resolve()
      .then(() => service.getById(personnelId, { signal: controller.signal }))
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "جزئیات پرسنل دریافت نشد.");
        }

        if (!controller.signal.aborted) {
          setState({
            personnelId,
            data: response.data,
            error: null,
          });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          setState({
            personnelId,
            data: null,
            error: handleApiError(error),
          });
        }
      });

    return () => controller.abort();
  }, [open, personnelId]);

  const loading =
    open && personnelId !== null && state.personnelId !== personnelId;
  const details = state.personnelId === personnelId ? state.data : null;
  const error = state.personnelId === personnelId ? state.error : null;

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        onOpenChange(nextOpen);
      }}
    >
      <DialogContent
        showCloseButton={false}
        className="flex h-[min(520px,94dvh)] w-[min(980px,calc(100vw-1rem))] max-w-none flex-col gap-0 overflow-hidden rounded-[18px] border border-[#d9b4da] bg-white p-0 text-slate-800 shadow-xl sm:max-w-none"
        dir="rtl"
      >
        <header className="flex h-12 shrink-0 items-center justify-between border-b border-slate-100 px-5">
          <DialogTitle className="text-sm font-semibold text-fuchsia-950">
            جزئیات پرسنل
          </DialogTitle>
          <Button
            type="button"
            variant="ghost"
            size="icon-sm"
            aria-label="بستن جزئیات"
            onClick={() => onOpenChange(false)}
            className="text-slate-500"
          >
            <X className="size-5" />
          </Button>
        </header>

        {loading ? (
          <div
            className="flex flex-1 items-center justify-center text-sm text-slate-500"
            role="status"
          >
            در حال دریافت جزئیات پرسنل...
          </div>
        ) : error ? (
          <div
            className="flex flex-1 items-center justify-center px-6 text-center text-sm text-destructive"
            role="alert"
          >
            {error}
          </div>
        ) : details ? (
          <>
            <section className="grid shrink-0 grid-cols-1 items-center gap-3 border-b border-slate-100 px-5 py-2 sm:grid-cols-[minmax(0,1fr)_auto_minmax(0,1.15fr)]">
              <div className="flex min-w-0 items-center gap-3">
                <div className="relative flex size-14 shrink-0 items-center justify-center overflow-hidden rounded-full bg-sky-100 text-sky-700">
                  {details.personalInfo.pictureUrl ? (
                    <Image
                      src={details.personalInfo.pictureUrl}
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
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-semibold">
                      {details.personalInfo.fullName}
                    </p>
                    <Badge
                      className={
                        details.status
                          ? "bg-emerald-100 text-emerald-700 hover:bg-emerald-100"
                          : "bg-slate-100 text-slate-600 hover:bg-slate-100"
                      }
                    >
                      {details.status ? "فعال" : "غیرفعال"}
                    </Badge>
                  </div>
                  <p className="mt-1 text-xs text-fuchsia-700">
                    {details.jobInfo.jobTitle}
                  </p>
                </div>
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
                  {details.jobInfo.personnelCode}
                </span>
                <span className="flex items-center gap-2">
                  <ContactRound
                    aria-hidden="true"
                    className="size-3.5 text-fuchsia-700"
                  />
                  {displayEnum(details.jobInfo.workingShiftType)}
                </span>
                <span className="text-slate-500">
                  شروع همکاری:{" "}
                  {formatDate(details.jobInfo.cooperationStartDate)}
                </span>
              </div>
              <div className="flex items-center gap-1 sm:justify-self-end">
                {details.personalInfo.isCloseToSalon && (
                  <Badge className="me-2 bg-fuchsia-50 text-fuchsia-700 hover:bg-fuchsia-50">
                    نزدیک محل کار
                  </Badge>
                )}
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-sm"
                  aria-label="چاپ جزئیات پرسنل"
                  title="چاپ"
                  onClick={() => window.print()}
                  className="text-slate-500 hover:text-fuchsia-700"
                >
                  <Printer className="size-5" />
                </Button>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-sm"
                  aria-label="دریافت فایل جزئیات پرسنل"
                  title="دریافت جزئیات"
                  onClick={() => downloadPersonnelDetails(details)}
                  className="text-slate-500 hover:text-fuchsia-700"
                >
                  <Download className="size-5" />
                </Button>
              </div>
            </section>

            <nav
              aria-label="دسته‌بندی جزئیات پرسنل"
              role="tablist"
              className="flex h-[38px] shrink-0 overflow-x-auto border-b border-slate-200 px-3"
            >
              {TABS.map(({ id, label, icon: Icon }) => (
                <button
                  key={id}
                  id={`personnel-tab-${id}`}
                  type="button"
                  role="tab"
                  aria-selected={activeTab === id}
                  aria-controls="personnel-details-panel"
                  onClick={() => setActiveTab(id)}
                  onKeyDown={(event) => {
                    if (
                      event.key !== "ArrowLeft" &&
                      event.key !== "ArrowRight"
                    ) {
                      return;
                    }

                    event.preventDefault();
                    const currentIndex = TABS.findIndex((tab) => tab.id === id);
                    const direction = event.key === "ArrowRight" ? 1 : -1;
                    const nextIndex =
                      (currentIndex + direction + TABS.length) % TABS.length;
                    const nextTab = TABS[nextIndex];

                    setActiveTab(nextTab.id);
                    document
                      .getElementById(`personnel-tab-${nextTab.id}`)
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
              id="personnel-details-panel"
              role="tabpanel"
              aria-labelledby={`personnel-tab-${activeTab}`}
              className="flex-1 overflow-y-auto bg-white px-5 py-1"
            >
              <PersonnelDetailsContent
                personnel={details}
                activeTab={activeTab}
              />
            </div>
            <footer className="flex h-14 shrink-0 items-center justify-start gap-2 border-t border-slate-100 px-5">
              <Button
                type="button"
                disabled
                className="h-8 rounded-lg bg-fuchsia-700 px-3 text-xs text-white disabled:cursor-not-allowed disabled:opacity-100"
                title="ویرایش اطلاعات هنوز در این بخش فعال نشده است"
              >
                ویرایش اطلاعات
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled
                className="h-8 rounded-lg border-rose-500 px-3 text-xs text-rose-600 disabled:cursor-not-allowed disabled:opacity-100"
                title="حذف پرسنل هنوز در این بخش فعال نشده است"
              >
                حذف پرسنل
              </Button>
            </footer>
          </>
        ) : (
          <div className="flex flex-1 items-center justify-center text-sm text-slate-500">
            پرسنلی انتخاب نشده است.
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
