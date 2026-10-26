"use client";

import { useEffect, useMemo, useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
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
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { SalonService } from "@/features/salons/list/salon.service";
import type { GetSelectListSalonResponse } from "@/features/salons/list/salon.types";
import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceCategoryDetailsDialog } from "./service-category-details-dialog";
import { ServiceCategoryFormDialog } from "./service-category-form-dialog";
import { ServiceCatalogService } from "../list/service.service";
import type { ServiceCategoryDto } from "../list/service.types";
import { useServiceCategories } from "./use-service-categories";

const PAGE_SIZE = 10;

export function ServiceCategoriesPage() {
  const [salons, setSalons] = useState<GetSelectListSalonResponse[]>([]);
  const [salonId, setSalonId] = useState(0);
  const [salonsLoading, setSalonsLoading] = useState(true);
  const [salonsError, setSalonsError] = useState<string | null>(null);
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [statusTarget, setStatusTarget] = useState<ServiceCategoryDto | null>(null);
  const [statusError, setStatusError] = useState<string | null>(null);
  const [changingStatus, setChangingStatus] = useState(false);

  useEffect(() => {
    const controller = new AbortController();
    Promise.resolve()
      .then(async () => {
        const api = new SalonService();
        const first = await api.getSelectList(
          { PageNumber: 1, PageSize: 100 },
          { signal: controller.signal },
        );
        if (!first.data) throw new Error(first.message ?? "فهرست سالن‌ها دریافت نشد.");
        const items = [...first.data.list];
        for (let currentPage = 2; currentPage <= first.data.pagination.totalPages; currentPage += 1) {
          const response = await api.getSelectList(
            { PageNumber: currentPage, PageSize: 100 },
            { signal: controller.signal },
          );
          if (!response.data) throw new Error(response.message ?? "فهرست سالن‌ها کامل دریافت نشد.");
          items.push(...response.data.list);
        }
        if (controller.signal.aborted) return;
        const activeSalons = items.filter((item) => item.status);
        setSalons(activeSalons);
        if (activeSalons.length === 1) setSalonId(activeSalons[0].id);
        if (activeSalons.length === 0) setSalonsError("سالن فعالی برای مدیریت دسته‌بندی‌ها وجود ندارد.");
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) setSalonsError(handleApiError(reason));
      })
      .finally(() => {
        if (!controller.signal.aborted) setSalonsLoading(false);
      });
    return () => controller.abort();
  }, []);

  const { data, error, loading } = useServiceCategories(salonId, refreshKey);
  const categories = useMemo(() => {
    const normalized = query.trim().toLocaleLowerCase();
    return (data ?? []).filter((category) =>
      !normalized ||
      category.name.toLocaleLowerCase().includes(normalized) ||
      (category.description ?? "").toLocaleLowerCase().includes(normalized),
    );
  }, [data, query]);
  const totalPages = Math.max(1, Math.ceil(categories.length / PAGE_SIZE));
  const visibleCategories = categories.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  function openCreate() {
    setEditingId(null);
    setFormOpen(true);
  }

  function openEdit(id: number) {
    setDetailsId(null);
    setEditingId(id);
    setFormOpen(true);
  }

  async function confirmDelete() {
    if (!deleteTarget || deleting) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      const response = await new ServiceCatalogService().deleteCategory(deleteTarget.id, salonId);
      if (response.statusCode >= 400) {
        throw new Error(response.message ?? "حذف دسته‌بندی انجام نشد.");
      }
      setDeleteTarget(null);
      setDetailsId(null);
      setRefreshKey((current) => current + 1);
    } catch (reason: unknown) {
      setDeleteError(handleApiError(reason));
    } finally {
      setDeleting(false);
    }
  }

  async function confirmStatusChange() {
    if (!statusTarget || changingStatus) return;
    setChangingStatus(true);
    setStatusError(null);
    try {
      const service = new ServiceCatalogService();
      const response = statusTarget.isActive
        ? await service.deactivateCategory(statusTarget.id, salonId)
        : await service.updateCategory(statusTarget.id, {
          salonId,
          name: statusTarget.name,
          description: statusTarget.description,
          isActive: true,
        });
      if (response.statusCode >= 400) {
        throw new Error(response.message ?? "تغییر وضعیت دسته‌بندی انجام نشد.");
      }
      setStatusTarget(null);
      setRefreshKey((current) => current + 1);
    } catch (reason: unknown) {
      setStatusError(handleApiError(reason));
    } finally {
      setChangingStatus(false);
    }
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/services">مدیریت خدمات</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">دسته‌بندی خدمات</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              onClick={openCreate}
              disabled={!salonId}
            >
              <Plus className="size-4" />
              دسته‌بندی جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجو بر اساس نام دسته‌بندی"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام دسته‌بندی..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 px-2 py-2">
            <label className="flex items-center gap-2 text-xs text-slate-600">
              سالن
              <select
                aria-label="انتخاب سالن برای دسته‌بندی‌ها"
                value={salonId || ""}
                disabled={salonsLoading || salons.length === 0}
                onChange={(event) => {
                  setSalonId(Number(event.target.value));
                  setPage(1);
                }}
                className="h-8 min-w-40 rounded-md border border-slate-200 bg-white px-2 text-sm"
              >
                <option value="" disabled>{salonsLoading ? "در حال دریافت..." : "انتخاب سالن"}</option>
                {salons.map((salon) => <option key={salon.id} value={salon.id}>{salon.name}</option>)}
              </select>
            </label>
            {salonId > 0 && <span className="text-xs text-slate-500">{categories.length.toLocaleString("fa-IR")} دسته‌بندی</span>}
          </div>
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 min-w-40 px-2 text-start">نام دسته‌بندی</TableHead>
                  <TableHead className="h-9 min-w-48 px-2 text-start">توضیحات</TableHead>
                  <TableHead className="h-9 min-w-24 px-2 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {salonsError ? (
                  <TableRow><TableCell colSpan={4} role="alert" className="py-8 text-center text-sm text-destructive">{salonsError}</TableCell></TableRow>
                ) : salonsLoading ? (
                  <TableRow><TableCell colSpan={4} role="status" className="py-8 text-center text-sm">در حال دریافت سالن‌ها...</TableCell></TableRow>
                ) : !salonId ? (
                  <TableRow><TableCell colSpan={4} className="py-8 text-center text-sm">برای مشاهده دسته‌بندی‌ها، سالن را انتخاب کنید.</TableCell></TableRow>
                ) : error ? (
                  <TableRow><TableCell colSpan={4} role="alert" className="py-8 text-center text-sm text-destructive">{error}</TableCell></TableRow>
                ) : loading ? (
                  <TableRow><TableCell colSpan={4} role="status" className="py-8 text-center text-sm">در حال دریافت دسته‌بندی‌ها...</TableCell></TableRow>
                ) : visibleCategories.length === 0 ? (
                  <TableRow><TableCell colSpan={4} className="py-8 text-center text-sm">دسته‌بندی‌ای یافت نشد.</TableCell></TableRow>
                ) : (
                  visibleCategories.map((category) => (
                    <TableRow key={category.id} className="h-9">
                      <TableCell className="px-2 py-2 text-sm">
                        <button
                          type="button"
                          className="rounded-sm text-start hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                          onClick={() => setDetailsId(category.id)}
                        >
                          {category.name}
                        </button>
                      </TableCell>
                      <TableCell className="max-w-xs truncate px-2 py-2 text-sm">{category.description || "—"}</TableCell>
                      <TableCell className="px-2 py-2">
                        <Badge className={category.isActive ? "bg-emerald-100 text-emerald-700 hover:bg-emerald-100" : "bg-slate-100 text-slate-600 hover:bg-slate-100"}>
                          {category.isActive ? "فعال" : "غیرفعال"}
                        </Badge>
                      </TableCell>
                      <TableCell className="px-2 py-2">
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={<Button variant="ghost" size="icon" aria-label={`عملیات دسته‌بندی ${category.name}`} className="size-7 text-fuchsia-700 hover:text-fuchsia-800" />}
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem onClick={() => setDetailsId(category.id)}>مشاهده</DropdownMenuItem>
                            <DropdownMenuItem onClick={() => openEdit(category.id)}>ویرایش</DropdownMenuItem>
                            <DropdownMenuItem onClick={() => {
                              setStatusError(null);
                              setStatusTarget(category);
                            }}>
                              {category.isActive ? "غیرفعال کردن" : "فعال کردن"}
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              className="text-destructive focus:text-destructive"
                              onClick={() => {
                                setDeleteError(null);
                                setDeleteTarget({ id: category.id, name: category.name });
                              }}
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
                  className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((current) => Math.max(1, current - 1));
                  }}
                />
              </PaginationItem>
              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(new Set([1, page, totalPages])).sort((a, b) => a - b).map((pageNumber, index, pages) => (
                  <span key={pageNumber} className="flex items-center">
                    {index > 0 && pageNumber - pages[index - 1] > 1 && <PaginationEllipsis className="size-6 text-muted-foreground" />}
                    <PaginationLink
                      href="#"
                      isActive={pageNumber === page}
                      size="icon-sm"
                      className={pageNumber === page ? "size-6 rounded-full border-0 bg-fuchsia-600 text-white hover:bg-fuchsia-700 hover:text-white" : "size-6 rounded-full border-0 text-foreground hover:bg-fuchsia-50"}
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
                  className="size-7 rounded-full border border-slate-300 bg-white p-0 text-slate-600 hover:bg-slate-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((current) => Math.min(totalPages, current + 1));
                  }}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      </main>

      <ServiceCategoryFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        onSaved={() => setRefreshKey((current) => current + 1)}
        categoryId={editingId}
        salonId={salonId}
      />
      <ServiceCategoryDetailsDialog
        open={detailsId !== null}
        onOpenChange={(open) => !open && setDetailsId(null)}
        categoryId={detailsId}
        onEdit={openEdit}
      />
      <Dialog open={statusTarget !== null} onOpenChange={(open) => {
        if (!open && !changingStatus) {
          setStatusTarget(null);
          setStatusError(null);
        }
      }}>
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">
              {statusTarget?.isActive ? "غیرفعال‌سازی دسته‌بندی" : "فعال‌سازی دسته‌بندی"}
            </DialogTitle>
            <DialogDescription className="text-right leading-6">
              دسته‌بندی «{statusTarget?.name}» {statusTarget?.isActive ? "غیرفعال" : "فعال"} شود؟
            </DialogDescription>
          </div>
          {statusError && <p role="alert" className="text-sm text-destructive">{statusError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" disabled={changingStatus} onClick={() => void confirmStatusChange()}>
              {changingStatus ? "در حال ثبت..." : "تأیید"}
            </Button>
            <Button type="button" variant="outline" disabled={changingStatus} onClick={() => setStatusTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      <Dialog open={deleteTarget !== null} onOpenChange={(open) => {
        if (!open && !deleting) {
          setDeleteTarget(null);
          setDeleteError(null);
        }
      }}>
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">حذف دسته‌بندی</DialogTitle>
            <DialogDescription className="text-right leading-6">
              دسته‌بندی «{deleteTarget?.name}» حذف شود؟ اگر خدمتی به آن متصل باشد، حذف انجام نمی‌شود.
            </DialogDescription>
          </div>
          {deleteError && <p role="alert" className="text-sm text-destructive">{deleteError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" variant="destructive" disabled={deleting} onClick={() => void confirmDelete()}>
              {deleting ? "در حال حذف..." : "حذف"}
            </Button>
            <Button type="button" variant="outline" disabled={deleting} onClick={() => setDeleteTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
