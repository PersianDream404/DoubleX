"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { Plus, Search } from "lucide-react";

import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Pagination, PaginationContent, PaginationEllipsis, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";

import { useReceptions } from "./use-receptions";

const PAGE_SIZE = 10;

function formatAmount(amount: number) {
  return `${new Intl.NumberFormat("fa-IR").format(amount)} تومان`;
}

function formatDate(value: string) {
  const date = new Date(value);
  if (!Number.isFinite(date.getTime())) return "—";
  return new Intl.DateTimeFormat("fa-IR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(date);
}

export function ReceptionsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const { data, error, loading } = useReceptions();

  const filtered = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase();
    return (data ?? []).filter((reception) =>
      `${reception.receptionNumber} ${reception.customerName}`
        .toLocaleLowerCase()
        .includes(normalizedQuery),
    );
  }, [data, query]);
  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const visibleItems = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/receptions">مدیریت پذیرش</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">پذیرش‌های باز</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>

          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              render={<Link href="/receptions/new" />}
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
            >
              <Plus className="size-4" />
              پذیرش جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجوی پذیرش"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس شماره پذیرش یا مشتری..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 px-2 text-start">شماره پذیرش</TableHead>
                  <TableHead className="h-9 px-2 text-start">نام مشتری</TableHead>
                  <TableHead className="h-9 px-2 text-start">شناسه تعرفه</TableHead>
                  <TableHead className="h-9 px-2 text-start">تاریخ پذیرش</TableHead>
                  <TableHead className="h-9 px-2 text-start">کد پذیرشگر</TableHead>
                  <TableHead className="h-9 px-2 text-start">مبلغ کل</TableHead>
                  <TableHead className="h-9 px-2 text-start">پرداخت‌شده</TableHead>
                  <TableHead className="h-9 px-2 text-start">مانده</TableHead>
                  <TableHead className="h-9 px-2 text-start">وضعیت</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell colSpan={9} className="py-8 text-center text-sm text-destructive" role="alert">
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell colSpan={9} className="py-8 text-center text-sm text-slate-500">
                      در حال دریافت پذیرش‌های باز...
                    </TableCell>
                  </TableRow>
                ) : visibleItems.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={9} className="py-12 text-center text-sm text-slate-500">
                      {query ? "پذیرشی با این مشخصات پیدا نشد." : "پذیرش بازی برای نمایش وجود ندارد."}
                    </TableCell>
                  </TableRow>
                ) : (
                  visibleItems.map((reception) => (
                    <TableRow key={reception.id} className="text-xs">
                      <TableCell className="px-2 py-3 font-medium" dir="ltr">
                        {reception.receptionNumber}
                      </TableCell>
                      <TableCell className="px-2 py-3">{reception.customerName}</TableCell>
                      <TableCell className="px-2 py-3" dir="ltr">
                        {reception.serviceTariffId ?? "—"}
                      </TableCell>
                      <TableCell className="px-2 py-3">{formatDate(reception.receptionDate)}</TableCell>
                      <TableCell className="px-2 py-3" dir="ltr">{reception.receptionistId}</TableCell>
                      <TableCell className="px-2 py-3">{formatAmount(reception.totalAmount)}</TableCell>
                      <TableCell className="px-2 py-3">{formatAmount(reception.paidAmount)}</TableCell>
                      <TableCell className="px-2 py-3">{formatAmount(reception.remainingAmount)}</TableCell>
                      <TableCell className="px-2 py-3">
                        <span className="rounded-full bg-emerald-50 px-2 py-1 text-[11px] text-emerald-700">
                          باز
                        </span>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          {!error && !loading && totalPages > 1 && (
            <Pagination className="mt-auto pt-5">
              <PaginationContent>
                <PaginationItem>
                  <PaginationPrevious
                    href="#"
                    aria-label="صفحه قبل"
                    onClick={(event) => {
                      event.preventDefault();
                      setPage((current) => Math.max(1, current - 1));
                    }}
                    className={page === 1 ? "pointer-events-none opacity-50" : ""}
                  />
                </PaginationItem>
                {Array.from({ length: totalPages }, (_, index) => index + 1)
                  .filter((pageNumber) =>
                    pageNumber === 1 || pageNumber === totalPages || Math.abs(pageNumber - page) <= 1,
                  )
                  .map((pageNumber, index, pages) => (
                    <PaginationItem key={pageNumber}>
                      {index > 0 && pageNumber - pages[index - 1] > 1 && <PaginationEllipsis />}
                      <PaginationLink
                        href="#"
                        isActive={pageNumber === page}
                        onClick={(event) => {
                          event.preventDefault();
                          setPage(pageNumber);
                        }}
                      >
                        {new Intl.NumberFormat("fa-IR").format(pageNumber)}
                      </PaginationLink>
                    </PaginationItem>
                  ))}
                <PaginationItem>
                  <PaginationNext
                    href="#"
                    aria-label="صفحه بعد"
                    onClick={(event) => {
                      event.preventDefault();
                      setPage((current) => Math.min(totalPages, current + 1));
                    }}
                    className={page === totalPages ? "pointer-events-none opacity-50" : ""}
                  />
                </PaginationItem>
              </PaginationContent>
            </Pagination>
          )}
        </div>
      </main>
    </div>
  );
}
