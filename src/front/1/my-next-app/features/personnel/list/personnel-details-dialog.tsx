"use client";

import Image from "next/image";
import { useEffect, useState } from "react";
import {
  BadgeDollarSign,
  BriefcaseBusiness,
  ContactRound,
  HeartPulse,
  ImageIcon,
  MapPin,
  Phone,
  UserRound,
  X,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogTitle,
} from "@/components/ui/dialog";
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
  const date = new Date(`${value}T00:00:00`);

  return Number.isNaN(date.getTime())
    ? value
    : date.toLocaleDateString("fa-IR");
}

function maskedNumber(value: string) {
  const digits = value.replace(/\D/g, "");
  const visibleDigits = new Set(
    [...digits].slice(-4).map((_, index) => digits.length - 4 + index),
  );
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

function InfoCard({
  title,
  icon: Icon,
  children,
}: {
  title: string;
  icon?: typeof UserRound;
  children: React.ReactNode;
}) {
  return (
    <section className="min-w-0 rounded-lg border border-slate-100 bg-white p-4">
      <h3 className="mb-4 flex items-center gap-2 text-sm font-semibold text-slate-800">
        {Icon && <Icon aria-hidden="true" className="size-4 text-fuchsia-700" />}
        {title}
      </h3>
      {children}
    </section>
  );
}

function InfoField({
  label,
  value,
}: {
  label: string;
  value: React.ReactNode;
}) {
  return (
    <div className="min-w-0 space-y-1">
      <dt className="text-xs text-slate-500">{label}</dt>
      <dd className="break-words text-sm leading-6 text-slate-800">
        {value ?? "—"}
      </dd>
    </div>
  );
}

function PictureCard({
  title,
  urls,
}: {
  title: string;
  urls: string[];
}) {
  return (
    <InfoCard title={title} icon={ImageIcon}>
      {urls.length > 0 ? (
        <div className="grid grid-cols-2 gap-2">
          {urls.map((url, index) => (
            <a
              key={`${url}-${index}`}
              href={url}
              target="_blank"
              rel="noreferrer"
              aria-label={`مشاهده ${title} ${index + 1}`}
              className="relative aspect-square overflow-hidden rounded-lg bg-fuchsia-50 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
            >
              <Image
                src={url}
                alt={`${title} ${index + 1}`}
                fill
                unoptimized
                className="object-cover"
                sizes="180px"
              />
            </a>
          ))}
        </div>
      ) : (
        <div
          role="img"
          aria-label="تصویری ثبت نشده است"
          className="flex aspect-square items-center justify-center rounded-lg bg-fuchsia-50 text-fuchsia-400"
        >
          <ImageIcon aria-hidden="true" className="size-8" />
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
        <div className="grid gap-4 lg:grid-cols-[minmax(0,1.7fr)_minmax(170px,.9fr)_minmax(130px,.7fr)]">
          <InfoCard title="اطلاعات فردی" icon={UserRound}>
            <dl className="grid gap-x-6 gap-y-5 sm:grid-cols-2">
              <InfoField label="نام و نام خانوادگی" value={personalInfo.fullName} />
              <InfoField label="کد ملی" value={personalInfo.nationalCode} />
              <InfoField
                label="تاریخ تولد"
                value={formatDate(personalInfo.birthDate)}
              />
              <InfoField
                label="وضعیت تأهل"
                value={displayEnum(personalInfo.maritalStatus)}
              />
              <InfoField label="تعداد فرزندان" value={personalInfo.childrenCount} />
              <InfoField
                label="نزدیک محل کار"
                value={personalInfo.isCloseToSalon ? "بله" : "خیر"}
              />
            </dl>
            <div className="mt-5 border-t border-slate-100 pt-4">
              <InfoField label="آدرس" value={personalInfo.addressText} />
            </div>
          </InfoCard>

          <InfoCard title="شماره تماس" icon={Phone}>
            <dl className="space-y-5">
              <InfoField label="تلفن همراه" value={personalInfo.mobilePhone} />
              <InfoField label="تلفن ثابت" value={personalInfo.landlinePhone} />
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
        <div className="grid gap-4 lg:grid-cols-[minmax(0,1.5fr)_minmax(220px,1fr)_minmax(180px,.8fr)]">
          <InfoCard title="اطلاعات شغلی" icon={BriefcaseBusiness}>
            <dl className="grid gap-x-6 gap-y-5 sm:grid-cols-2">
              <InfoField label="عنوان شغلی" value={jobInfo.jobTitle} />
              <InfoField label="کد پرسنلی" value={jobInfo.personnelCode} />
              <InfoField
                label="تاریخ شروع همکاری"
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
              <InfoField label="سوابق" value={jobInfo.workHistory} />
              <InfoField
                label="مهارت‌ها"
                value={jobInfo.skills?.length ? jobInfo.skills.join("، ") : "—"}
              />
            </dl>
          </InfoCard>
          <PictureCard title="تصاویر دوره آموزشی" urls={[]} />
        </div>
      );

    case "financial":
      return (
        <div className="grid gap-4 lg:grid-cols-2">
          <InfoCard title="حقوق و دستمزد" icon={BadgeDollarSign}>
            <dl className="grid gap-5 sm:grid-cols-2">
              <InfoField
                label="نوع حقوق و دستمزد"
                value={displayEnum(financialInfo.salaryType)}
              />
            </dl>
          </InfoCard>
          <InfoCard title="اطلاعات حساب بانکی">
            <dl className="grid gap-5 sm:grid-cols-2">
              <InfoField
                label="شماره کارت"
                value={maskedNumber(financialInfo.cardNumber)}
              />
              <InfoField
                label="شماره حساب"
                value={maskedNumber(financialInfo.accountNumber)}
              />
            </dl>
          </InfoCard>
        </div>
      );

    case "health":
      return (
        <div className="grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(160px,.8fr)_minmax(160px,.8fr)]">
          <InfoCard title="اطلاعات سلامتی" icon={HeartPulse}>
            <div className="mb-5 flex flex-wrap gap-2">
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
            </div>
            <dl className="grid gap-5 sm:grid-cols-2">
              <InfoField label="وضعیت بیماری" value={healthInfo.diseases} />
              <InfoField label="وضعیت حساسیت" value={healthInfo.allergies} />
            </dl>
          </InfoCard>
          <PictureCard title="تصاویر آزمایش‌ها" urls={healthInfo.pictureTest} />
          <PictureCard title="تصاویر دوره‌ها" urls={healthInfo.pictureCourse} />
        </div>
      );

    case "guarantor":
      return (
        <div className="grid gap-4 lg:grid-cols-[minmax(0,1.4fr)_minmax(220px,1fr)]">
          <InfoCard title="اطلاعات معرف" icon={ContactRound}>
            <dl className="grid gap-x-6 gap-y-5 sm:grid-cols-2">
              <InfoField label="نام معرف یا ضامن" value={guarantorInfo.guarantorName} />
              <InfoField label="نسبت با پرسنل" value={guarantorInfo.relationship} />
              <InfoField label="تلفن همراه" value={guarantorInfo.guarantorPhone} />
              <InfoField
                label="تلفن ثابت"
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
  const details =
    state.personnelId === personnelId ? state.data : null;
  const error =
    state.personnelId === personnelId ? state.error : null;

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        onOpenChange(nextOpen);
      }}
    >
      <DialogContent
        showCloseButton={false}
        className="flex h-[min(540px,90dvh)] w-[min(980px,calc(100vw-1rem))] max-w-none flex-col gap-0 overflow-hidden rounded-2xl border border-fuchsia-200 bg-white p-0 text-slate-800 shadow-xl sm:max-w-none"
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
          <div className="flex flex-1 items-center justify-center text-sm text-slate-500" role="status">
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
            <section className="flex shrink-0 flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-3">
              <div className="flex items-center gap-3">
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
                <div>
                  <p className="font-semibold">{details.personalInfo.fullName}</p>
                  <p className="mt-1 text-xs text-fuchsia-700">
                    {details.jobInfo.jobTitle}
                  </p>
                </div>
              </div>
              <div className="flex flex-wrap gap-x-5 gap-y-2 text-xs text-slate-600">
                <span>
                  کد پرسنلی:{" "}
                  <strong className="font-medium text-slate-800">
                    {details.jobInfo.personnelCode}
                  </strong>
                </span>
                <span className="flex items-center gap-1">
                  <MapPin aria-hidden="true" className="size-3.5 text-fuchsia-700" />
                  {displayEnum(details.jobInfo.workingShiftType)}
                </span>
                {details.personalInfo.isCloseToSalon && (
                  <Badge className="bg-fuchsia-50 text-fuchsia-700 hover:bg-fuchsia-50">
                    نزدیک محل کار
                  </Badge>
                )}
              </div>
            </section>

            <nav
              aria-label="دسته‌بندی جزئیات پرسنل"
              role="tablist"
              className="flex shrink-0 overflow-x-auto border-b border-slate-200 px-3"
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
                    if (event.key !== "ArrowLeft" && event.key !== "ArrowRight") {
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
                  className={`flex min-w-max items-center justify-center gap-2 border-b-2 px-4 py-3 text-xs transition-colors focus-visible:outline-2 focus-visible:outline-fuchsia-600 ${
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
              className="flex-1 overflow-y-auto bg-white px-5 py-4"
            >
              <PersonnelDetailsContent
                personnel={details}
                activeTab={activeTab}
              />
            </div>
            <footer className="flex shrink-0 justify-start gap-2 border-t border-slate-100 px-5 py-3">
              <Button
                type="button"
                disabled
                className="bg-fuchsia-700 text-white"
                title="ویرایش اطلاعات هنوز در این بخش فعال نشده است"
              >
                ویرایش اطلاعات
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled
                className="border-rose-500 text-rose-600"
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
