"use client";

import { useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
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
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
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
import { useSalons } from "@/features/salons/list/use-salons";

export default function SalonsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const { data, error, loading } = useSalons(query, page);
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);
  const salons = data?.list ?? [];

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[84%] max-w-[1200px] pt-2">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href="/salon">مدیریت سالن‌ها</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">
                  فهرست سالن‌ها
                </BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>

          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              <Plus className="size-4" />
              سالن جدید
            </Button>

            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام سالن..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[84%] max-w-[1200px] pb-8">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-3 pb-3 pt-1 shadow-sm">
          <div className="overflow-hidden rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 text-start">نام سالن</TableHead>
                  <TableHead className="h-9 w-32 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 w-14" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell
                      colSpan={3}
                      className="py-8 text-center text-sm text-destructive"
                      role="alert"
                    >
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell colSpan={3} className="py-8 text-center text-sm">
                      در حال دریافت سالن‌ها...
                    </TableCell>
                  </TableRow>
                ) : salons.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={3} className="py-8 text-center text-sm">
                      سالنی یافت نشد.
                    </TableCell>
                  </TableRow>
                ) : (
                  salons.map((salon) => (
                    <TableRow key={salon.id} className="h-9">
                      <TableCell className="py-2 text-sm">{salon.name}</TableCell>
                      <TableCell className="py-2">
                        <Badge
                          className={
                            salon.status
                              ? "bg-emerald-600 text-white hover:bg-emerald-600"
                              : "bg-muted text-muted-foreground hover:bg-muted"
                          }
                        >
                          {salon.status ? "فعال" : "غیرفعال"}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={
                              <Button
                                variant="ghost"
                                size="icon"
                                className="size-7 text-fuchsia-700 hover:text-fuchsia-800"
                              />
                            }
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem>مشاهده</DropdownMenuItem>
                            <DropdownMenuItem>ویرایش</DropdownMenuItem>
                            <DropdownMenuItem className="text-destructive">
                              حذف
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          <Pagination className="mt-auto pt-5" dir="ltr">
            <PaginationContent className="gap-2">
              <PaginationItem>
                <PaginationPrevious
                  href="#"
                  text=""
                  aria-label="صفحه قبلی"
                  className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((currentPage) => Math.max(1, currentPage - 1));
                  }}
                />
              </PaginationItem>

              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(
                  new Set([1, page, totalPages].filter((pageNumber) => pageNumber > 0)),
                )
                  .sort((first, second) => first - second)
                  .map((pageNumber, index, pages) => (
                    <span key={pageNumber} className="flex items-center">
                      {index > 0 && pageNumber - pages[index - 1] > 1 && (
                        <PaginationEllipsis className="size-6 text-muted-foreground" />
                      )}
                      <PaginationLink
                        href="#"
                        isActive={pageNumber === page}
                        size="icon-sm"
                        className={
                          pageNumber === page
                            ? "size-6 rounded-full border-0 bg-fuchsia-600 text-white hover:bg-fuchsia-700 hover:text-white"
                            : "size-6 rounded-full border-0 text-foreground hover:bg-fuchsia-50"
                        }
                        onClick={(event) => {
                          event.preventDefault();
                          setPage(pageNumber);
                        }}
                      >
                        {pageNumber.toLocaleString("fa-IR")}
                      </PaginationLink>
                    </span>
                  ))}
              </PaginationItem>

              <PaginationItem>
                <PaginationNext
                  href="#"
                  text=""
                  aria-label="صفحه بعدی"
                  className="size-7 rounded-full border border-slate-300 bg-white p-0 text-slate-500 hover:bg-slate-50"
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
        </div>
      </main>
    </div>
  );
}
