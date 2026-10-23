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

import { useSalons } from "./use-salons";

export default function SalonsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const { data, error, loading } = useSalons(query, page);
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);
  const salons = data?.list ?? [];

  return (
    <div className="min-h-screen bg-muted/30">
      <div className="bg-black px-6 pb-24 pt-4">
        <Breadcrumb>
          <BreadcrumbList className="text-white/70">
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
      </div>

      <main className="-mt-16 px-6">
        <div className="rounded-xl bg-background p-4 shadow-sm">
          <div className="mb-4 flex items-center justify-between gap-4">
            <Button className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              <Plus className="size-4" />
              سالن جدید
            </Button>

            <div className="relative w-full max-w-sm">
              <Search className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام سالن..."
                className="ps-9"
              />
            </div>
          </div>

          <div className="rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="text-start">نام سالن</TableHead>
                  <TableHead className="text-start">وضعیت</TableHead>
                  <TableHead className="w-12" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell
                      colSpan={3}
                      className="py-8 text-center text-destructive"
                      role="alert"
                    >
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell colSpan={3} className="py-8 text-center">
                      در حال دریافت سالن‌ها...
                    </TableCell>
                  </TableRow>
                ) : salons.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={3} className="py-8 text-center">
                      سالنی یافت نشد.
                    </TableCell>
                  </TableRow>
                ) : (
                  salons.map((salon) => (
                    <TableRow key={salon.id}>
                      <TableCell>{salon.name}</TableCell>
                      <TableCell>
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
                                className="size-8"
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
                    {index > 0 &&
                      pageNumber - pages[index - 1] > 1 && (
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
        </div>
      </main>
    </div>
  );
}
