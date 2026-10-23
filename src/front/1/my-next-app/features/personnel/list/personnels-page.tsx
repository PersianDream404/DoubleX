"use client";

import { useState } from "react";
import { Search } from "lucide-react";

import { Input } from "@/components/ui/input";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { usePersonnels } from "./use-personnels";

export function PersonnelsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const { data, error, loading } = usePersonnels(query, page);
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);
  const personnels = data?.list ?? [];

  return (
    <div className="min-h-screen bg-muted/30">
      <div className="bg-black px-6 pb-24 pt-4">
        <Breadcrumb>
          <BreadcrumbList className="text-white/70">
            <BreadcrumbItem>
              <BreadcrumbLink href="/staff">مدیریت پرسنل</BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbPage className="text-white">
                فهرست پرسنل
              </BreadcrumbPage>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>
      </div>

      <main className="-mt-16 px-4 pb-8 sm:px-6">
        <section
          aria-label="فهرست پرسنل"
          className="rounded-xl bg-background p-4 shadow-sm"
        >
          <div className="mb-4 flex justify-start">
            <div className="relative w-full max-w-sm">
              <Search
                aria-hidden="true"
                className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground"
              />
              <Input
                aria-label="جستجو بر اساس نام پرسنل"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام پرسنل..."
                className="ps-9"
              />
            </div>
          </div>

          <div className="rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="text-start">نام و نام خانوادگی</TableHead>
                  <TableHead className="text-start">کد ملی</TableHead>
                  <TableHead className="text-start">عنوان شغلی</TableHead>
                  <TableHead className="text-start">شماره کارت</TableHead>
                  <TableHead className="text-start">تاریخ تولد</TableHead>
                  <TableHead className="text-start">شماره همراه</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell
                      colSpan={6}
                      className="py-8 text-center text-destructive"
                      role="alert"
                    >
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell
                      colSpan={6}
                      className="py-8 text-center"
                      role="status"
                    >
                      در حال دریافت فهرست پرسنل...
                    </TableCell>
                  </TableRow>
                ) : personnels.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="py-8 text-center">
                      پرسنلی یافت نشد.
                    </TableCell>
                  </TableRow>
                ) : (
                  personnels.map((personnel) => (
                    <TableRow key={personnel.id}>
                      <TableCell>{personnel.fullName}</TableCell>
                      <TableCell>{personnel.nationalCode}</TableCell>
                      <TableCell>{personnel.jobTitle}</TableCell>
                      <TableCell>{personnel.cardNumber}</TableCell>
                      <TableCell>
                        {new Date(`${personnel.birthDate}T00:00:00`).toLocaleDateString(
                          "fa-IR",
                        )}
                      </TableCell>
                      <TableCell>{personnel.mobilePhone}</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          <Pagination className="mt-4">
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious
                  href="#"
                  text="قبلی"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((currentPage) => Math.max(1, currentPage - 1));
                  }}
                />
              </PaginationItem>

              {Array.from({ length: totalPages }, (_, index) => index + 1)
                .filter(
                  (pageNumber) =>
                    pageNumber === 1 ||
                    pageNumber === totalPages ||
                    Math.abs(pageNumber - page) <= 1,
                )
                .map((pageNumber, index, pages) => (
                  <PaginationItem key={pageNumber}>
                    {index > 0 && pageNumber - pages[index - 1] > 1 && (
                      <PaginationEllipsis />
                    )}
                    <PaginationLink
                      href="#"
                      isActive={pageNumber === page}
                      onClick={(event) => {
                        event.preventDefault();
                        setPage(pageNumber);
                      }}
                    >
                      {pageNumber.toLocaleString("fa-IR")}
                    </PaginationLink>
                  </PaginationItem>
                ))}

              <PaginationItem>
                <PaginationNext
                  href="#"
                  text="بعدی"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((currentPage) =>
                      Math.min(totalPages, currentPage + 1),
                    );
                  }}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </section>
      </main>
    </div>
  );
}
