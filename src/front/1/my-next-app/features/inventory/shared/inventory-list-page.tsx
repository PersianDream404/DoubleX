"use client";

import type { ReactNode } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";

export interface InventoryListItem {
  id: number;
  name: string;
  isActive: boolean;
}

export interface InventoryColumn<TItem> {
  key: string;
  title: string;
  render: (item: TItem) => ReactNode;
}

interface InventoryListPageProps<TItem extends InventoryListItem> {
  title: string;
  breadcrumbHref: string;
  searchPlaceholder: string;
  query: string;
  onQueryChange: (value: string) => void;
  page: number;
  onPageChange: (value: number) => void;
  totalPages: number;
  items: TItem[];
  columns: InventoryColumn<TItem>[];
  loading: boolean;
  error: string | null;
  onCreate: () => void;
  onView: (item: TItem) => void;
  onEdit: (item: TItem) => void;
  onSetActive: (item: TItem) => void;
  onDelete: (item: TItem) => void;
  onIncreaseStock?: (item: TItem) => void;
  onWithdrawStock?: (item: TItem) => void;
}

export function InventoryListPage<TItem extends InventoryListItem>({
  title,
  breadcrumbHref,
  searchPlaceholder,
  query,
  onQueryChange,
  page,
  onPageChange,
  totalPages,
  items,
  columns,
  loading,
  error,
  onCreate,
  onView,
  onEdit,
  onSetActive,
  onDelete,
  onIncreaseStock,
  onWithdrawStock,
}: InventoryListPageProps<TItem>) {
  const columnCount = columns.length + 2;

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem>
                <BreadcrumbLink href={breadcrumbHref}>مدیریت انبار</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">{title}</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>

          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              onClick={onCreate}
            >
              <Plus className="size-4" />
              مورد جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label={searchPlaceholder}
                value={query}
                onChange={(event) => onQueryChange(event.target.value)}
                placeholder={searchPlaceholder}
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
                  {columns.map((column) => (
                    <TableHead key={column.key} className="h-9 px-2 text-start">
                      {column.title}
                    </TableHead>
                  ))}
                  <TableHead className="h-9 w-28 px-2 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell colSpan={columnCount} className="py-8 text-center text-sm text-destructive" role="alert">
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell colSpan={columnCount} className="py-8 text-center text-sm" role="status">
                      در حال دریافت اطلاعات...
                    </TableCell>
                  </TableRow>
                ) : items.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={columnCount} className="py-8 text-center text-sm">
                      موردی یافت نشد.
                    </TableCell>
                  </TableRow>
                ) : (
                  items.map((item) => (
                    <TableRow key={item.id} className="h-9">
                      {columns.map((column, index) => (
                        <TableCell key={column.key} className="px-2 py-2 text-sm">
                          {index === 0 ? (
                            <button
                              type="button"
                              className="rounded-sm text-start hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                              onClick={() => onView(item)}
                            >
                              {column.render(item)}
                            </button>
                          ) : (
                            column.render(item)
                          )}
                        </TableCell>
                      ))}
                      <TableCell className="px-2 py-2">
                        <Badge
                          className={
                            item.isActive
                              ? "bg-emerald-100 text-emerald-700 hover:bg-emerald-100"
                              : "bg-slate-100 text-slate-600 hover:bg-slate-100"
                          }
                        >
                          {item.isActive ? "فعال" : "غیرفعال"}
                        </Badge>
                      </TableCell>
                      <TableCell className="px-2 py-2">
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={
                              <Button
                                variant="ghost"
                                size="icon"
                                aria-label={`عملیات ${item.name}`}
                                className="size-7 text-fuchsia-700 hover:text-fuchsia-800"
                              />
                            }
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem onClick={() => onView(item)}>مشاهده</DropdownMenuItem>
                            <DropdownMenuItem onClick={() => onEdit(item)}>ویرایش</DropdownMenuItem>
                            {onIncreaseStock && item.isActive && (
                              <DropdownMenuItem onClick={() => onIncreaseStock(item)}>
                                افزایش موجودی
                              </DropdownMenuItem>
                            )}
                            {onWithdrawStock && item.isActive && (
                              <DropdownMenuItem onClick={() => onWithdrawStock(item)}>
                                برداشت از موجودی
                              </DropdownMenuItem>
                            )}
                            <DropdownMenuItem onClick={() => onSetActive(item)}>
                              {item.isActive ? "غیرفعال کردن" : "فعال کردن"}
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              className="text-destructive focus:text-destructive"
                              onClick={() => onDelete(item)}
                            >
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
                  aria-disabled={page <= 1}
                  className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50"
                  onClick={(event) => {
                    event.preventDefault();
                    onPageChange(Math.max(1, page - 1));
                  }}
                />
              </PaginationItem>
              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(new Set([1, page, totalPages].filter((value) => value > 0)))
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
                          onPageChange(pageNumber);
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
                  aria-disabled={page >= totalPages}
                  className="size-7 rounded-full border border-slate-300 bg-white p-0 text-slate-500 hover:bg-slate-50"
                  onClick={(event) => {
                    event.preventDefault();
                    onPageChange(Math.min(totalPages, page + 1));
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
