"use client";

import DatePicker from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import persian_fa from "react-date-object/locales/persian_fa";

type Props = {
  fromDate?: string;
  toDate?: string;
};

export function OrderReportDateFilter({ fromDate, toDate }: Props) {
  return (
    <div>
      <div className="space-y-2">
        <label className="block text-sm font-medium">از تاریخ</label>

        <DatePicker
          calendar={persian}
          locale={persian_fa}
          calendarPosition="bottom-right"
          format="YYYY/MM/DD"
          value={fromDate || ""}
          inputClass="h-10 w-full rounded-md border bg-background px-3 text-sm"
          name="fromDate"
          placeholder="تاریخ شروع"
        />
      </div>

      <div className="space-y-2">
        <label className="block text-sm font-medium">تا تاریخ</label>

        <DatePicker
          calendar={persian}
          locale={persian_fa}
          calendarPosition="bottom-right"
          format="YYYY/MM/DD"
          value={toDate || ""}
          inputClass="h-10 w-full rounded-md border bg-background px-3 text-sm"
          name="toDate"
          placeholder="تاریخ پایان"
        />
      </div>
    </div>
  );
}
